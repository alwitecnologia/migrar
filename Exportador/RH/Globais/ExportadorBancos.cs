using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Exportador.Helpers;
using Exportador.Interface;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Configuration;

namespace Exportador.RH.Globais
{
    public class ExportadorBancos : IExportador
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
        public ExportadorBancos()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorBancos(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorBancos(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryBancos = @"select
	                                        case 
		                                        when codban = 1242 
		                                        then '/@33@/'
		                                        else '/@' + CAST(codban AS VARCHAR(4)) + '@/'	
	                                        end as NUMBANCO,	
	                                        '/@' + CAST(nomban AS VARCHAR(40)) + '@/' AS NOME,
	                                        '/@' + CAST(nomban AS VARCHAR(20)) + '@/' AS NOMEREDUZIDO,
	                                        '/@@/' AS MASCCONTA,
	                                        case 
		                                        when codban = 1242 
		                                        then '/@33@/'
		                                        else '/@' + CAST(codban AS VARCHAR(4)) + '@/'	
	                                        end AS NUMEROOFICIAL,
	                                        '/@@/' AS DIGBANCO
                                        FROM vetorh.r012ban";

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

            List<Bancos> bancos = new List<Bancos>();

            error = buscarBancos(bancos);

            FileHelperEngine engine = new FileHelperEngine(typeof(Bancos), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, bancos);
        }

        private bool buscarBancos(List<Bancos> fichas)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryBancos.Replace("{schemaName}", dbName));

            _bgWorker.ReportProgress(0, "Executando consulta de bancos...");

            IDataReader drBancos = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            _bgWorker.ReportProgress(0, "Montando objetos...");

            while (drBancos.Read())
            {
                Bancos bancos = new Bancos();

                try
                {
                    processedRecords++;

                    bancos.NUMBANCO = drBancos["NUMBANCO"].ToString();
                    bancos.NOME = drBancos["NOME"].ToString();

                    bancos.NOMEREDUZIDO = drBancos["NOMEREDUZIDO"].ToString();
                    bancos.MASCCONTA = drBancos["MASCCONTA"].ToString();
                    bancos.NUMEROOFICIAL = drBancos["NUMEROOFICIAL"].ToString();
                    bancos.DIGBANCO = drBancos["DIGBANCO"].ToString();

                    fichas.Add(bancos);
                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar os bancos - Erro: {0}", ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }
    }
}
