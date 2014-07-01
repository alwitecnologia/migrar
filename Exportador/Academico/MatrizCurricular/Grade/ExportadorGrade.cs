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


namespace Exportador.Academico.MatrizCurricular.Grade
{
    public class ExportadorGrade : IExportador
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
        public ExportadorGrade()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorGrade(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorGrade(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryTodasAsGrades = @"SELECT DISTINCT
                                                    GRADE.ID_CURSO AS CODCURSO
                                                    ,GRADE.ID AS CODGRADE
                                                    ,IIF(CURSO.NOME_OFICIAL IS NULL,CURSO.NOME,CURSO.NOME_OFICIAL) as NOMECURSO
                                                    ,GRADE,GRADE AS SEQ_GRADE
                                                    ,GRADE.GRADE_INATIVA AS STATUS
                                                FROM GRADE
                                                INNER JOIN CURSO ON CURSO.ID=GRADE.ID_CURSO
                                                INNER JOIN MATRICULA_CURSO MC ON MC.ID_GRADE=GRADE.ID
                                                ORDER BY grade.ID_CURSO,GRADE.GRADE";

        private string _queryCountGrades = @"SELECT COUNT(*) CT FROM (
                                                SELECT DISTINCT
                                                    GRADE.ID_CURSO AS CODCURSO
                                                    ,GRADE.ID AS CODGRADE
                                                    ,IIF(CURSO.NOME_OFICIAL IS NULL,CURSO.NOME,CURSO.NOME_OFICIAL) as NOMECURSO
                                                    ,GRADE,GRADE AS SEQ_GRADE
                                                    ,GRADE.GRADE_INATIVA AS STATUS
                                                FROM GRADE
                                                INNER JOIN CURSO ON CURSO.ID=GRADE.ID_CURSO
                                                INNER JOIN MATRICULA_CURSO MC ON MC.ID_GRADE=GRADE.ID
                                                ORDER BY grade.ID_CURSO,GRADE.GRADE) AS CT";

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

        public void Exportar()
        {
            error = false;

            List<Grade> grades = new List<Grade>();

            grades = buscarGrades();

            excluirCursosNaoCadastrados(grades);

            FileHelperEngine engine = new FileHelperEngine(typeof(Grade), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, grades);
        }

        private void excluirCursosNaoCadastrados(List<Grade> grades)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            List<string> cursosRM = new List<string>();

            using (DbCommand command = database.GetSqlStringCommand("SELECT DISTINCT CODCURSO FROM SCURSO"))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    cursosRM.Add(reader.GetString("CODCURSO"));
                }
            }

            List<Grade> lGrades = new List<Grade>();

            foreach (var codCurso in cursosRM)
            {
                lGrades.AddRange(grades.Where(g => g.CodCurso == codCurso));
            }

            grades.Clear();

            grades.AddRange(lGrades);
        }

        private List<Grade> buscarGrades()
        {
            List<Grade> lGrade = new List<Grade>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountGrades))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodasAsGrades))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        lGrade.Add(ConverterGrade(reader));
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        string codGrade = (reader["CODGRADE"] == DBNull.Value) ? String.Empty : reader["CODGRADE"].ToString();

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a Grade: Código {0},Motivo:{1}", codGrade, ex.Message));
                    }
                }
            }

            return lGrade;
        }

        private Grade ConverterGrade(IDataReader drGrade)
        {
            Grade g = new Grade();

            g.CodCurso = (drGrade["CODCURSO"] == DBNull.Value) ? String.Empty : drGrade["CODCURSO"].ToString();
            g.CodHabilitacao = (drGrade["CODCURSO"] == DBNull.Value) ? String.Empty : drGrade["CODCURSO"].ToString();
            g.CodGrade = (drGrade["CODGRADE"] == DBNull.Value) ? String.Empty : drGrade["CODGRADE"].ToString();

            string nomeCurso = (drGrade["NOMECURSO"] == DBNull.Value) ? String.Empty : drGrade["NOMECURSO"].ToString();
            string seqGrade = (drGrade["SEQ_GRADE"] == DBNull.Value) ? String.Empty : drGrade["SEQ_GRADE"].ToString();
            g.Descricao = String.Format("{0} - Grade {1}", ((nomeCurso.Length > 50) ? nomeCurso.Substring(0, 50) : nomeCurso), seqGrade);

            g.Status = ((bool)(DBHelper.GetNullableBoolean(drGrade["STATUS"]))) ? "1" : "0";
            
            g.CodColigada = 1;
            g.ControleVagas = "1";
            g.Regime = "C";
            g.TipoAtividadeCurricular = "C";
            g.TipoEletiva = "C";
            g.TipoOptativa = "C";

            return g;
        }


        private double getCount()
        {
            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryTodasAsGrades);

            IDataReader drCount = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
        }

    }
}
