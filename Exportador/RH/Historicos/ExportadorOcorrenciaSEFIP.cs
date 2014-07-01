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
using Exportador.RH.Funcionario;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Configuration;

namespace Exportador.RH.Historicos
{
    public class ExportadorOcorrenciaSEFIP: IExportador
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
        public ExportadorOcorrenciaSEFIP()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorOcorrenciaSEFIP(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorOcorrenciaSEFIP(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryHistOcorrencias = @"select
	                                            chapa.Chapa as Chapa    
                                                ,funcionario.datadm as DataAdmissao
                                                from vetorh.r034fun as funcionario
                                                inner join dbo.vw_totvs_chapafuncionario chapa on funcionario.numcad = chapa.numcad
																						and funcionario.tipcol = chapa.tipcol
                                                inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
                                                where chapa.numcpf <> '0'    ";

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

            List<OcorrenciaSEFIP> ocorrencias = new List<OcorrenciaSEFIP>();

            error = buscarOcorrencias(ocorrencias);

            FileHelperEngine engine = new FileHelperEngine(typeof(OcorrenciaSEFIP), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, ocorrencias);
        }

        private bool buscarOcorrencias(List<OcorrenciaSEFIP> ocorrencias)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryHistOcorrencias.Replace("{schemaName}", dbName));

            IDataReader drOcorrencias = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drOcorrencias.Read())
            {
                OcorrenciaSEFIP ocorrencia = new OcorrenciaSEFIP();

                try
                {
                    processedRecords++;

                    ocorrencia.Chapa = drOcorrencias["Chapa"].ToString().PadLeft(5, '0');
                    ocorrencia.DtMudanca = Convert.ToDateTime(drOcorrencias["DataAdmissao"]);

                    ocorrencia.CodOcorrencia = Ocorrencia.NuncaExpostoAAgenteNocivo;//"0"; //Deixado fixo "Nunca exposto a agentes"


                    ocorrencias.Add(ocorrencia);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a movimentação de categoria SEFIP: Chapa {0}, DtMudanca {1}. Motivo:{2}", ocorrencia.Chapa, ocorrencia.DtMudanca.ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }
    
    }
}
