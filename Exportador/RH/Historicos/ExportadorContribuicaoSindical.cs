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

namespace Exportador.RH.Historicos
{
    public class ExportadorContribuicaoSindical: IExportador
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
        public ExportadorContribuicaoSindical()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorContribuicaoSindical(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorContribuicaoSindical(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryHistContribuicoes = @"select 
	                            chapa.Chapa as Chapa
	                            ,DATEADD(YEAR,contSindical.anobas-1900,0) as DtContribuicao
	                            ,REPLICATE('0', 4 - LEN(contSindical.codsin ))+RTrim(contSindical.codsin)
	                            as CodSindicato
	                            ,contSindical.valcon as Valor
                            from vetorh.r053csc as contSindical
                            inner join dbo.vw_totvs_chapafuncionario chapa on contSindical.numcad = chapa.numcad
																			and contSindical.tipcol = chapa.tipcol
                            inner join vetorh.r034fun funcionario on funcionario.numemp=contSindical.numemp
                                and funcionario.tipcol=contSindical.tipcol
                                and funcionario.numcad=contSindical.numcad
                            
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

            List<ContribuicaoSindical> contribuicoes = new List<ContribuicaoSindical>();

            error = buscarHistoricoContribuicoes(contribuicoes);

            FileHelperEngine engine = new FileHelperEngine(typeof(ContribuicaoSindical), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, contribuicoes);
        }

        private bool buscarHistoricoContribuicoes(List<ContribuicaoSindical> contribuicoes)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryHistContribuicoes.Replace("{schemaName}", dbName));

            IDataReader drContribuicao = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drContribuicao.Read())
            {
                ContribuicaoSindical contr = new ContribuicaoSindical();

                try
                {
                    processedRecords++;

                    //contr.Chapa = drContribuicao["Chapa"].ToString();
                    contr.Chapa = drContribuicao["Chapa"].ToString();
                    contr.Chapa = contr.Chapa.PadLeft(5, '0');

                    contr.CodSindicato = drContribuicao["CodSindicato"].ToString();

                    contr.Valor = Convert.ToDouble(drContribuicao["Valor"]);

                    contr.DtContribuicao = Convert.ToDateTime(drContribuicao["DtContribuicao"]);

                    contribuicoes.Add(contr);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a alteração: Chapa {0}, DtContribuição {1}. Motivo:{2}", contr.Chapa, Convert.ToDateTime(contr.DtContribuicao).ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }
    }
}
