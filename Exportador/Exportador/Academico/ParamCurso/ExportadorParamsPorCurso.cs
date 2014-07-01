using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exportador.DAO;
using Exportador.Interface;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using Exportador.Helpers;
using System.Data;
using System.Windows.Forms;

namespace Exportador.Academico.ParamCurso
{
    public class ExportadorParamsPorCurso : IExportador
    {

        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsCount;
        private bool _debugMode;
        private bool _error = false;
        private List<Curso.Curso> cursos = new List<Curso.Curso>();
        private List<Turno.Turno> turnos = new List<Turno.Turno>();

        #endregion

        #region Properties

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public int RecordsCount
        {
            get { return _recordsCount; }
            set { _recordsCount = value; }
        }

        public bool DebugMode
        {
            get { return _debugMode; }
            set { _debugMode = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorParamsPorCurso()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorParamsPorCurso(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorParamsPorCurso(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryBuscarTudo = @"select
                                                mc.ano
                                                ,mc.semestre
                                                ,mc.id_curso
                                                ,mc.id_grade
                                                ,min(ah.entrada) as entrada
                                                ,max(ah.saida) as saida
                                            from matricula_curso mc
                                            inner join turma t on mc.id_curso=t.id_curso
                                                and mc.id_grade=t.id_grade
                                                and mc.ano=t.ano
                                                and mc.semestre=t.semestre
                                            inner join horario h on t.id=h.id_turma
                                                and t.ano=h.ano
                                                and t.semestre=h.semestre
                                            inner join aula_horario ah on h.turno=h.turno
                                                and h.aula=ah.aula
                                            group by mc.ano
                                                ,mc.semestre
                                                ,mc.id_curso
                                                ,mc.id_grade";

        private string _countParams = @"SELECT COUNT(*)
                                            FROM (

                                            select
                                                mc.ano
                                                ,mc.semestre
                                                ,mc.id_curso
                                                ,mc.id_grade
                                                ,min(ah.entrada) as entrada
                                                ,max(ah.saida) as saida
                                            from matricula_curso mc
                                            inner join turma t on mc.id_curso=t.id_curso
                                                and mc.id_grade=t.id_grade
                                                and mc.ano=t.ano
                                                and mc.semestre=t.semestre
                                            inner join horario h on t.id=h.id_turma
                                                and t.ano=h.ano
                                                and t.semestre=h.semestre
                                            inner join aula_horario ah on h.turno=h.turno
                                                and h.aula=ah.aula
                                            group by mc.ano
                                                ,mc.semestre
                                                ,mc.id_curso
                                                ,mc.id_grade

                                            ) AS CT";

        #endregion

        public void Exportar()
        {
            try
            {
                _bgWorker.ReportProgress(0, "Buscando cursos já cadastrados...");

                cursos = (new CursoDAO()).buscarTodosDestino();

                _bgWorker.ReportProgress(0, "Buscando turnos já cadastrados...");

                turnos = (new TurnoDAO()).RetornaTodos();

                _bgWorker.ReportProgress(60, "Buscando parametrizações no sistema origem...");

                List<ParamsPorCurso> paramsCursos = BuscarParamsPorCurso();

                _bgWorker.ReportProgress(80, "Buscando disciplinas no sistema origem...");

                FileHelperEngine engine = new FileHelperEngine(typeof(ParamsPorCurso), Encoding.Unicode);

                _bgWorker.RunWorkerCompleted += workerCompleted;

                engine.WriteFile(_filename, paramsCursos);

                _bgWorker.ReportProgress(100);
            }
            catch (Exception e)
            {
                _error = true;

                _bgWorker.ReportProgress(0, e.Message + System.Environment.NewLine + e.StackTrace);
            }
        }

        /// <summary>
        /// Retorna todas as parametrizações dos cursos no sistema de origem.
        /// </summary>
        /// <returns></returns>
        private List<ParamsPorCurso> BuscarParamsPorCurso()
        {
            List<ParamsPorCurso> lDisc = new List<ParamsPorCurso>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            _bgWorker.ReportProgress(0, "Gerando contadores...");

            double totalRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_countParams))
            {
                totalRecords = (int)database.ExecuteScalar(command);
            }

            using (DbCommand command = database.GetSqlStringCommand(_queryBuscarTudo))
            {
                _bgWorker.ReportProgress(0, "Executando consulta na base de origem...");

                var reader = database.ExecuteReader(command);                

                double processedRecords = 0;

                _bgWorker.ReportProgress(0, "Gerando registros de destino...");

                while (reader.Read())
                {
                    try
                    {
                        lDisc.Add(ConverterParamCurso(reader));

                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        string msg = String.Format("Não foi possível exportar o registro. Motivo:{0}", ex.Message);

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), msg);
                    }
                }
            }

            return lDisc;
        }

        private ParamsPorCurso ConverterParamCurso(IDataReader reader)
        {
            ParamsPorCurso paramCurso = new ParamsPorCurso();

            //mc.ano
            //,mc.semestre
            //,mc.id_curso
            //,mc.id_grade
            //,min(ah.entrada) as entrada
            //,max(ah.saida) as saida

            int? ano = reader.GetNullableInt32("ano");
            int? semestre = reader.GetNullableInt32("semestre");
            string codCurso = reader.GetString("id_curso");
            string codGrade = reader.GetString("id_grade");

            int codTipoCurso;

            if (cursos.Exists(c => c.CodCurso == codCurso))
                codTipoCurso = cursos.Where(c => c.CodCurso == codCurso).First().CodTipoCurso;
            else
                throw new BusinessException(String.Format("Curso código {0} não encontrado.", codCurso));

            TimeSpan tsHorIni = (TimeSpan)reader["entrada"];
            TimeSpan tsHorFim = (TimeSpan)reader["saida"];

            string horIni = String.Format("{0}:{1}", tsHorIni.Hours.ToString().PadLeft(2, '0'), tsHorIni.Minutes.ToString().PadLeft(2, '0'));
            string horFim = String.Format("{0}:{1}", tsHorFim.Hours.ToString().PadLeft(2, '0'), tsHorFim.Minutes.ToString().PadLeft(2, '0'));


            paramCurso.CodPerLet = String.Format("{0}-{1}/{2}", (codTipoCurso == 1 ? "SUP" :
                                               codTipoCurso == 2 ? "PGM" :
                                               codTipoCurso == 3 ? "EXT" :
                                               codTipoCurso == 7 ? "EXT" : String.Empty), ano.ToString(), semestre.ToString());
            paramCurso.CodCurso = codCurso;
            paramCurso.CodTipoCurso = codTipoCurso;
            paramCurso.CodHabilitacao = codCurso;
            paramCurso.CodGrade = codGrade;

            if (turnos.Exists(t => t.HoraInicio == horIni && t.HoraFim == horFim && t.CodTipoCurso == codTipoCurso))
                paramCurso.Turno = turnos.Where(t => t.HoraInicio == horIni && t.HoraFim == horFim && t.CodTipoCurso == codTipoCurso).First().Nome;
            else
                throw new BusinessException("Turno não encontrado.");

            paramCurso.CodColigada = 1;
            paramCurso.CodFilial = 1;

            return paramCurso;
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_error)
            {
                MessageBox.Show("Houveram erros no processo.", "Erro.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Processo concluído com sucesso.", "Sucesso.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

}