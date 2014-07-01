using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Exportador.Interface;
using Exportador.Helpers;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using Exportador.DAO;


namespace Exportador.Academico.MatrizAplicada
{
    public class ExportadorHabilitacaoFilial: IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool error;
        private bool _debugMode;

        #endregion

        #region Properties

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

        public int RecordsCount
        {
            get
            {
                return _recordsToReturn;
            }
            set
            {
                _recordsToReturn = value;
            }
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
        public ExportadorHabilitacaoFilial()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorHabilitacaoFilial(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorHabilitacaoFilial(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryBuscarTudo = @"select
                                                t.id_curso
                                                ,t.id_grade
                                                ,g.grade_inativa
                                                ,min(ah.entrada) as entrada
                                                ,max(ah.saida) as saida
                                            from horario h
                                            inner join turma t on h.id_turma=t.id
                                            inner join aula_horario ah on h.turno=ah.turno
                                                and h.aula=ah.aula
                                            inner join grade g on t.id_grade=g.id
                                            group by t.id_curso
                                                ,t.id_grade
                                                ,g.grade_inativa";


        private string _queryCountTudo = @"SELECT COUNT(*) CT FROM (
                                            select
                                                t.id_curso
                                                ,t.id_grade
                                                ,g.grade_inativa
                                                ,min(ah.entrada) as entrada
                                                ,max(ah.saida) as saida
                                            from horario h
                                            inner join turma t on h.id_turma=t.id
                                            inner join aula_horario ah on h.turno=ah.turno
                                                and h.aula=ah.aula
                                            inner join grade g on t.id_grade=g.id
                                            group by t.id_curso
                                                ,t.id_grade
                                                ,g.grade_inativa
                                                            ) AS CT";

        #endregion

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (error)
            {
                MessageBox.Show("Houveram erros no processo.", "Erro.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Processo concluído com sucesso.", "Sucesso.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private double getCount()
        {
            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryCountTudo);

            IDataReader drCount = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
        }

        public void Exportar()
        {
            error = false;

            List<HabilitacaoFilial> habFiliais = new List<HabilitacaoFilial>();

            habFiliais = buscarTudo();



            FileHelperEngine engine = new FileHelperEngine(typeof(HabilitacaoFilial), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, habFiliais);
        }


        /// <summary>
        /// Busca as disciplinas obrigatórias da grade.
        /// </summary>
        /// <returns></returns>
        private List<HabilitacaoFilial> buscarTudo()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            _bgWorker.ReportProgress(0, "Gerando contadores...");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountTudo))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            List<HabilitacaoFilial> lHabsFiliais = new List<HabilitacaoFilial>();

            _bgWorker.ReportProgress(0, "Buscando registros...");

            List<Curso.Curso> cursosRM = (new CursoDAO()).buscarTodosDestino();

            List<Turno.Turno> turnosRM = (new TurnoDAO()).RetornaTodos();

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryBuscarTudo))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        HabilitacaoFilial habFilial = new HabilitacaoFilial();

                        habFilial = ConverterHabilitacaoFilial(reader);

                        validarCamposObrigatorios(habFilial);

                        //Busca do tipo curso...
                        if (cursosRM.Exists(c => c.CodCurso == habFilial.CodCurso))
                            habFilial.CodTipoCurso = cursosRM.Where(c => c.CodCurso == habFilial.CodCurso).First().CodTipoCurso;
                        else
                            throw new BusinessException(String.Format("Não foi possível efetuar a exportação. Motivo: Curso código {0} não foi encontrado.", habFilial.CodCurso));
                                               
                        //Busca do turno...
                        TimeSpan horIni = (TimeSpan)reader["entrada"];
                        TimeSpan horFim = (TimeSpan)reader["saida"];

                        string strHorIni = String.Format("{0}:{1}", (horIni.Hours.ToString()).PadLeft(2, '0'), (horIni.Minutes.ToString()).PadLeft(2, '0'));
                        string strHorFim = String.Format("{0}:{1}", (horFim.Hours.ToString()).PadLeft(2, '0'), (horFim.Minutes.ToString()).PadLeft(2, '0'));

                        habFilial.Turno = turnosRM.Where(t => t.HoraInicio == strHorIni && t.HoraFim == strHorFim && t.CodTipoCurso == habFilial.CodTipoCurso).First().Nome;

                        lHabsFiliais.Add(habFilial);

                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível efetuar a exportação. Motivo:{0}", ex.Message));
                    }
                }
            }

            return lHabsFiliais;
        }

        private void validarCamposObrigatorios(HabilitacaoFilial habFilial)
        {
            if (String.IsNullOrEmpty(habFilial.CodCurso))
                throw new BusinessException("Campo CodCurso não encontrado");

            if (String.IsNullOrEmpty(habFilial.CodHabilitacao))
                throw new BusinessException("Campo CodHabilitacao não encontrado");

            if (String.IsNullOrEmpty(habFilial.CodGrade))
                throw new BusinessException("Campo CodGrade não encontrado");
        }

        private HabilitacaoFilial ConverterHabilitacaoFilial(IDataRecord record)
        {
            HabilitacaoFilial habFilial = new HabilitacaoFilial();

            habFilial.CodCurso = record.GetString("id_curso");
            habFilial.CodHabilitacao = habFilial.CodCurso;
            habFilial.CodGrade = record.GetString("id_grade");
            habFilial.Ativo = record.GetString("grade_inativa") == "0" ? "S":"N";

            habFilial.CodColigada = 1;
            habFilial.CodFilial = 1;

            return habFilial;
        }
    }
}
