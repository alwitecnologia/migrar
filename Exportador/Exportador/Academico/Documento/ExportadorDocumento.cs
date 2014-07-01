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

namespace Exportador.Academico.Documento
{
    public class ExportadorDocumento : IExportador
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
        public ExportadorDocumento()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorDocumento(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorDocumento(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryTodosDocumentos = @"select id,nome from documento";

        private string _queryCountDocs = @"SELECT COUNT(*) CT FROM (
                                                select id,nome from documento
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

            List<Documento> docs = new List<Documento>();

            docs = buscarDocs();

            FileHelperEngine engine = new FileHelperEngine(typeof(Documento), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, docs);
        }

        private List<Documento> buscarDocs()
        {
            List<Documento> lDocs = new List<Documento>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_queryCountDocs))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_queryTodosDocumentos))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        lDocs.Add(ConverterDocumento(reader));
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        string codDoc = (reader["id"] == DBNull.Value) ? String.Empty : reader["id"].ToString();

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o Documento: Código {0},Motivo:{1}", codDoc, ex.Message));
                    }
                }
            }

            return lDocs;
        }

        private Documento ConverterDocumento(IDataReader drDoc)
        {
            Documento d = new Documento();

            d.Codigo = (drDoc["ID"] == DBNull.Value) ? 0 : Convert.ToInt32(drDoc["ID"]);
            
            string descDoc = (drDoc["NOME"] == DBNull.Value) ? String.Empty : drDoc["NOME"].ToString();
            d.Descricao = (descDoc.Length > 57) ? String.Concat(descDoc.Substring(0,57),"...") : descDoc;

            return d;
        }


        private double getCount()
        {
            Database sica = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            DbCommand sicaCmd = sica.GetSqlStringCommand(_queryTodosDocumentos);

            IDataReader drCount = sica.ExecuteReader(sicaCmd);

            return Convert.ToDouble(sica.ExecuteScalar(sicaCmd));
        }

    }
}
