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

namespace Exportador.Academico.Turma.Turma
{
    public class ExportadorTurma: IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool error;
        private bool _debugMode;
        private List<Curso.Curso> lCursos = new List<Curso.Curso>();

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
        public ExportadorTurma()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorTurma(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorTurma(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryTodasTurmas = @"SELECT DISTINCT
                                                T.ID_CURSO AS COD_CURSO
                                                ,T.ID_GRADE AS COD_GRADE
                                                ,T.ANO
                                                ,T.SEMESTRE
                                                ,T.ID AS COD_TURMA
                                            FROM TURMA T";

        private string _queryTodosCursosImportaddos = "SELECT DISTINCT CODCURSO,CODTIPOCURSO FROM SCURSO";

        private string _queryHorarioTurmas = "SELECT DISTINCT ID_TURMA,TURNO,AULA FROM HORARIO";

        private string _queryCountTodasTurmas = @"SELECT COUNT(*) CT FROM (
                                            SELECT DISTINCT
                                                T.ID_CURSO AS COD_CURSO
                                                ,T.ID_GRADE AS COD_GRADE
                                                ,T.ANO
                                                ,T.SEMESTRE
                                                ,T.ID AS COD_TURMA
                                            FROM TURMA T
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

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryTodasTurmas);

            IDataReader drCount = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
        }

        public void Exportar()
        {
            error = false;

            lCursos = buscarCursos();
            
            List<Turma> turmas = new List<Turma>();

            turmas = buscarTurmas();

            excluirTurmasSemCurso(turmas);

            buscarTiposCurso(turmas);

            FileHelperEngine engine = new FileHelperEngine(typeof(Turma), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, turmas);
        }

        private void buscarTiposCurso(List<Turma> turmas)
        {
            foreach (Turma t in turmas)
            {
                t.CodTipoCurso = lCursos.Single(c => c.CodCurso == t.CodCurso).CodTipoCurso;
            }
        }

        private void excluirTurmasSemCurso(List<Turma> todasTurmas)
        {
            List<Turma> turmasComCurso = new List<Turma>();

            foreach (Curso.Curso c in lCursos)
            {                
                turmasComCurso.AddRange(
                        todasTurmas.Where(t => t.CodCurso == c.CodCurso).ToList()                        
                        );
            }

            todasTurmas.Clear();

            todasTurmas.AddRange(turmasComCurso);
        }

        private List<Curso.Curso> buscarCursos()
        {
            return DBHelper.GetAll("RM", _queryTodosCursosImportaddos, String.Empty, ConverterCurso);
        }

        private List<Turma> buscarTurmas()
        {
            object[] parms = new object[] { _bgWorker };

            return DBHelper.GetAll("SICA", _queryTodasTurmas,_queryCountTodasTurmas, ConverterTurma,parms);
            
            #region

            /*






            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            //_bgWorker.ReportProgress(0, "Gerando contadores...");

            //double totalRecords;

            //using (DbCommand command = database.GetSqlStringCommand(_queryCountTodasTurmas))
            //{
            //    totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            //}

            //_bgWorker.ReportProgress(0, "Buscando registros...");

            List<Turma> lTurmas = new List<Turma>();

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodasTurmas))
            {
                var reader = database.ExecuteReader(command);

                double totalRecords;

                totalRecords = DBHelper.RowCount(reader);

                while (reader.Read())
                {
                    try
                    {
                        Turma t = new Turma();

                        t = ConverterHabilitacaoAluno(reader);

                        lTurmas.Add(t);
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a disciplina da grade: Motivo:{0}", ex.Message));
                    }
                }
            }

            return lTurmas;

                

                 */

            #endregion
        }

        private Turma ConverterTurma(IDataReader drTurmas)
        {
            Turma t = new Turma();

            t.CodCurso = drTurmas.GetString("COD_CURSO");
            t.CodHabilitacao = t.CodCurso;
            t.CodGrade = drTurmas.GetString("COD_GRADE");

            /*
            string turno = drTurmas.GetString("TURNO");
            string aula = drTurmas.GetString("AULA");

            t.Turno = String.Format("T{0}A{1}", turno, aula);

            string tipoCurso = drTurmas.GetString("TIPO_CURSO");
            string nomeCurso = drTurmas.GetString("NOME_CURSO");

            t.CodTipoCurso = (new CursoDAO()).buscarTipoCurso(tipoCurso, nomeCurso);
            */

            string ano = drTurmas.GetString("ANO");
            string semestre = drTurmas.GetString("SEMESTRE");
            
            t.CodPerLet = String.Format("{0}/{1}", ano, semestre);
            t.CodTurma = drTurmas.GetString("COD_TURMA");

            t.CodColigada = 1;
            t.CodFilial = 1;

            return t;
        }

        private Curso.Curso ConverterCurso(IDataReader drCurso)
        {
            Curso.Curso c = new Curso.Curso();

            c.CodTipoCurso = (int)drCurso.GetNullableInt32("CODTIPOCURSO");
            c.CodCurso = drCurso.GetString("CODCURSO");

            return c;
        } 
    }
}
