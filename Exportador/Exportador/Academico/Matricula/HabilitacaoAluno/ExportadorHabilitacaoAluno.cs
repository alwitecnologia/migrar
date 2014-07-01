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


namespace Exportador.Academico.Matricula.HabilitacaoAluno
{
    public class ExportadorHabilitacaoAluno : IExportador
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
        public ExportadorHabilitacaoAluno()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorHabilitacaoAluno(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorHabilitacaoAluno(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryTodasHabilitacoesAlunos = @"select distinct
                                                            mc.id_curso as codcurso
                                                            ,mc.id_grade as codgrade
                                                            ,horario.turno
                                                            ,horario.aula
                                                            ,curso.nome as nome_curso
                                                            ,nc.nome as nivel_curso
                                                            ,u_aluno.matricula as RA
                                                            ,fi.nome as ingresso
                                                            ,smc.nome as status
                                                            ,mc.data_cadastro
                                                        from matricula_curso mc
                                                        inner join curso on curso.id=mc.id_curso
                                                        inner join nivel_curso nc on nc.id=curso.id_nivel_curso
                                                        inner join usuario u_aluno on u_aluno.id=mc.id_aluno
                                                        inner join forma_ingresso fi on fi.id=mc.id_forma_ingresso
                                                        inner join situacao_matricula_curso smc on smc.id=mc.id_situacao_matricula_curso
                                                        inner join matricula_disciplina md on md.id_matricula_curso=mc.id
                                                        inner join horario on horario.id_grupo=md.id_grupo
                                                        order by mc.ano_entrada
                                                            ,mc.semestre_entrada
                                                            ,u_aluno.matricula";


        private string _queryCountTodasHabilitacoesAlunos = @"SELECT COUNT(*) CT FROM (
                                                        select distinct
                                                            mc.id_curso as codcurso
                                                            ,mc.id_grade as codgrade
                                                            ,horario.turno
                                                            ,horario.aula
                                                            ,curso.nome as nome_curso
                                                            ,nc.nome as nivel_curso
                                                            ,u_aluno.matricula as RA
                                                            ,fi.nome as ingresso
                                                            ,smc.nome as status
                                                            ,mc.data_cadastro
                                                        from matricula_curso mc
                                                        inner join curso on curso.id=mc.id_curso
                                                        inner join nivel_curso nc on nc.id=curso.id_nivel_curso
                                                        inner join usuario u_aluno on u_aluno.id=mc.id_aluno
                                                        inner join forma_ingresso fi on fi.id=mc.id_forma_ingresso
                                                        inner join situacao_matricula_curso smc on smc.id=mc.id_situacao_matricula_curso
                                                        inner join matricula_disciplina md on md.id_matricula_curso=mc.id
                                                        inner join horario on horario.id_grupo=md.id_grupo
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

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryTodasHabilitacoesAlunos);

            IDataReader drCount = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
        }

        public void Exportar()
        {
            error = false;

            List<HabilitacaoAluno> habAlunos = new List<HabilitacaoAluno>();

            habAlunos = buscarHabilitacoesAlunos();

            FileHelperEngine engine = new FileHelperEngine(typeof(HabilitacaoAluno), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, habAlunos);
        }

        private List<HabilitacaoAluno> buscarHabilitacoesAlunos()
        {
            List<HabilitacaoAluno> lHabsAlunos = new List<HabilitacaoAluno>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            _bgWorker.ReportProgress(0, "Gerando contadores...");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountTodasHabilitacoesAlunos))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            _bgWorker.ReportProgress(0, "Buscando registros...");

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodasHabilitacoesAlunos))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        HabilitacaoAluno habAluno = new HabilitacaoAluno();

                        habAluno = ConverterHabilitacaoAluno(reader);

                        lHabsAlunos.Add(habAluno);
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a disciplina da grade: Motivo:{0}", ex.Message));
                    }
                }
            }

            return lHabsAlunos;
        }

        private HabilitacaoAluno ConverterHabilitacaoAluno(IDataReader drHabilitacaoAluno)
        {
            HabilitacaoAluno habAluno = new HabilitacaoAluno();

            habAluno.CodCurso = drHabilitacaoAluno.GetString("codcurso");
            habAluno.CodHabilitacao = habAluno.CodCurso;
            habAluno.CodGrade = drHabilitacaoAluno.GetString("CODGRADE");

            string turno = drHabilitacaoAluno.GetString("turno");
            string aula = drHabilitacaoAluno.GetString("aula");

            habAluno.Turno = String.Format("T{0}A{1}", turno, aula);

            string nomeCurso = drHabilitacaoAluno.GetString("nome_curso");
            string nivelCurso = drHabilitacaoAluno.GetString("nivel_curso");

            habAluno.CodTipoCurso = (new CursoDAO()).buscarTipoCurso(nivelCurso, nomeCurso);

            habAluno.RA = drHabilitacaoAluno.GetString("RA");
            habAluno.Ingresso = drHabilitacaoAluno.GetString("ingresso");
            habAluno.Status = drHabilitacaoAluno.GetString("status");
            habAluno.DtIngresso = drHabilitacaoAluno.GetNullableDateTime("data_cadastro");

            habAluno.CodColigada = 1;
            habAluno.CodFilial = 1;

            return habAluno;
        }



    }
}