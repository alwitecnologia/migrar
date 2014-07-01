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
    public class ExportadorAgencias : IExportador
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
        public ExportadorAgencias()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorAgencias(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorAgencias(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryAgencias = @"select
	                                        case
		                                        when agencia.codban = 1242 
		                                        then '/@33@/'
		                                        else '/@' + CAST(agencia.codban AS VARCHAR(4)) + '@/'	
	                                        end as NUMBANCO,
	                                        '/@' + CAST(agencia.codage AS VARCHAR(6)) + '@/' as NUMAGENCIA,
	                                        '/@' + CAST(agencia.nomage AS VARCHAR(20)) + '@/' as NOME,
	                                        '/@@/' AS PRACA,
	                                        '/@@/' AS CODCOMPENSACAO,
	                                        '/@@/' AS RUA,
	                                        '/@@/' AS NUMERO,
	                                        '/@@/' AS COMPLEMENTO,
	                                        '/@@/' AS BAIRRO,
	                                        '/@' + CAST(agencia.codest AS VARCHAR(20)) + '@/' AS ESTADO,
	                                        '/@' + CAST(cidade.nomcid AS VARCHAR(32)) + '@/' AS CIDADE,
	                                        '/@@/' AS CEP,
	                                        '/@@/' AS PAIS,
	                                        '/@1@/' AS TIPOAGENCIA,
	                                        '/@' + CAST(digage AS VARCHAR(2)) + '@/' AS DIGAG,
	                                        '/@@/' AS TELEFONE
                                        from vetorh.r012age agencia
		                                        join vetorh.r074CID cidade on agencia.codcid = cidade.codcid";

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

            List<Agencias> agencias = new List<Agencias>();

            error = buscarAgencias(agencias);

            FileHelperEngine engine = new FileHelperEngine(typeof(Agencias), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, agencias);
        }

        private bool buscarAgencias(List<Agencias> fichas)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryAgencias.Replace("{schemaName}", dbName));

            _bgWorker.ReportProgress(0, "Executando consulta de Agencias...");

            IDataReader drAgencias = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            _bgWorker.ReportProgress(0, "Montando objetos...");

            while (drAgencias.Read())
            {
                Agencias agencias = new Agencias();

                try
                {
                    processedRecords++;

                    agencias.NUMBANCO = drAgencias["NUMBANCO"].ToString();
                    agencias.NUMAGENCIA = drAgencias["NUMAGENCIA"].ToString();

                    agencias.NOME = drAgencias["NOME"].ToString();
                    agencias.PRACA = drAgencias["PRACA"].ToString();
                    agencias.CODCOMPENSACAO = drAgencias["CODCOMPENSACAO"].ToString();
                    agencias.RUA = drAgencias["RUA"].ToString();
                    agencias.NUMERO = drAgencias["NUMERO"].ToString();
                    agencias.COMPLEMENTO = drAgencias["COMPLEMENTO"].ToString();
                    agencias.BAIRRO = drAgencias["BAIRRO"].ToString();
                    agencias.ESTADO = drAgencias["ESTADO"].ToString();
                    agencias.CIDADE = drAgencias["CIDADE"].ToString();
                    agencias.CEP = drAgencias["CEP"].ToString();
                    agencias.PAIS = drAgencias["PAIS"].ToString();
                    agencias.TIPOAGENCIA = drAgencias["TIPOAGENCIA"].ToString();
                    agencias.DIGAG = drAgencias["DIGAG"].ToString();
                    agencias.TELEFONE = drAgencias["TELEFONE"].ToString();

                    fichas.Add(agencias);
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
