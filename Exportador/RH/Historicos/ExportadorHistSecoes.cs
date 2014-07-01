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
    public class ExportadorHistSecoes: IExportador
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
        public ExportadorHistSecoes()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorHistSecoes(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorHistSecoes(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryHistSecoes = @"select 

                                                  chapa.Chapa  as Chapa
	                                            ,histSecoes.datalt as DtMudanca
	                                            ,histSecoes.motalt as CodMotivoMudanca
	                                            ,case when len(secaoHist.codloc) = 15 and secao.taborg = '5'
													  then '01.' + secaoHist.codloc
													  else '01.01.99.99.99.999'
												end as CodSecao
                                            from vetorh.r038hlo histSecoes
                                            inner join dbo.vw_totvs_chapafuncionario chapa on histSecoes.numcad = chapa.numcad
																							and histSecoes.tipcol = chapa.tipcol
                                              inner join vetorh.r034fun funcionario on funcionario.numemp=histSecoes.numemp
                                                  and funcionario.tipcol=histSecoes.tipcol
                                                  and funcionario.numcad=histSecoes.numcad
                                              inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
                                              inner join vetorh.r016hie secaoHist on secaoHist.numloc=histSecoes.numloc
                                              where chapa.numcpf <> '0' ";

        #endregion //and len(secaoHist.codloc) = 15

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

            List<Secoes> secoes = new List<Secoes>();

            error = buscarHistoricoSecoes(secoes);

            FileHelperEngine engine = new FileHelperEngine(typeof(Secoes), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, secoes);
        }

        private bool buscarHistoricoSecoes(List<Secoes> secoes)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryHistSecoes.Replace("{schemaName}", dbName));

            IDataReader drHistHorarios = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drHistHorarios.Read())
            {
                Secoes histSecoes = new Secoes();

                try
                {
                    processedRecords++;

                    histSecoes.Chapa = drHistHorarios["Chapa"].ToString().PadLeft(5, '0');
                    histSecoes.CodMotivoMudanca = "1"; //Fixo "Admissão"
                    histSecoes.CodSecao = drHistHorarios["CodSecao"].ToString();

                    if (drHistHorarios["DtMudanca"] != DBNull.Value)
                        histSecoes.DtMudanca = Convert.ToDateTime(drHistHorarios["DtMudanca"]);

                    secoes.Add(histSecoes);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a alteração: Chapa {0}, DtMudanca {1}. Motivo:{2}", histSecoes.Chapa, Convert.ToDateTime(histSecoes.DtMudanca).ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }
    }
}
