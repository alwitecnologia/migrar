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

namespace Exportador.Academico.MatrizCurricular.Periodo
{
    public class ExportadorPeriodo: IExportador
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
        public ExportadorPeriodo()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorPeriodo(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorPeriodo(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryTodosPeriodos = @"select
                                                    id_curso as CODCURSO
                                                    ,id as CODGRADE
                                                    ,nr_fases as nSemestres
                                                from grade";

        private string _queryCount = @"SELECT COUNT(*) CT FROM (
                                                select
                                                    id_curso as CODCURSO
                                                    ,id as CODGRADE
                                                    ,nr_fases as nSemestres
                                                from grade
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

        public void Exportar()
        {
            error = false;

            List<Periodo> periodos = new List<Periodo>();

            periodos = buscarPeriodos();

            removerPeriodosSemGrade(periodos);

            FileHelperEngine engine = new FileHelperEngine(typeof(Periodo), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, periodos);
        }

        private void removerPeriodosSemGrade(List<Periodo> periodos)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            List<string> gradesRM = new List<string>();

            using (DbCommand command = database.GetSqlStringCommand("SELECT DISTINCT CODGRADE FROM SGRADE"))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    gradesRM.Add(reader.GetString("CODGRADE"));
                }
            }

            List<Periodo> lPeriodo = new List<Periodo>();

            foreach (var codGrade in gradesRM)
            {
                lPeriodo.AddRange(periodos.Where(g => g.CodGrade == codGrade));
            }

            periodos.Clear();

            periodos.AddRange(lPeriodo);
        }

        private List<Periodo> buscarPeriodos()
        {
            List<Periodo> lDocs = new List<Periodo>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCount))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodosPeriodos))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        lDocs.AddRange(ConverterPeriodo(reader));
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o periodo: Motivo:{0}", ex.Message));
                    }
                }
            }

            return lDocs;
        }

        private List<Periodo> ConverterPeriodo(IDataReader drPeriodo)
        {
            string codCurso = (drPeriodo["CODCURSO"] == DBNull.Value) ? String.Empty : drPeriodo["CODCURSO"].ToString();
            string codGrade = (drPeriodo["CODGRADE"] == DBNull.Value) ? String.Empty : drPeriodo["CODGRADE"].ToString();

            int nSemestres = (drPeriodo["nSemestres"] == DBNull.Value) ? 0 : Convert.ToInt32(drPeriodo["nSemestres"]);

            List<Periodo> lPeriodos = new List<Periodo>();

            for (int i = 0; i < nSemestres; i++)
            {
                Periodo p = new Periodo() { 
                    CodCurso = codCurso
                    ,CodHabilitacao = codCurso
                    ,CodGrade = codGrade
                    ,CodColigada = 1
                    ,CodPeriodo = i+1
                    ,Descricao = String.Format("Semestre {0}",(i+1).ToString())
                };

                lPeriodos.Add(p);
            }

            return lPeriodos;
        }


        private double getCount()
        {
            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryTodosPeriodos);

            IDataReader drCount = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
        }
    }
}
