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
    public class ExportadorSituacoes: IExportador
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
        public ExportadorSituacoes()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorSituacoes(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorSituacoes(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries
        private string _querySituacoes1 = @"select 
			chapa.Chapa as Chapa
            ,DATEADD(minute,afastamentos.horafa,afastamentos.datafa) as DataMudanca              
            ,case motivo.codsit
				when '2' then '02'
				when '3' then '03'
				when '4' then '04'
				when '5' then '05'
				when '6' then '06'
				when '7' then '07'
				when '8' then '08'
				when '9' then '09'
				when '11' then '10'
				when '12' then '11'
				when '13' then '12'
				when '208' then '13'
				when '17' then '07'
				when '52' then '02'
				when '53' then '03'
				when '54' then '04'
				when '55' then '05'
				when '56' then '06'
				when '58' then '08'
				when '59' then '09'
				when '60' then '09'
				when '62' then '11'
				when '63' then '12'
			end as CodMotivoAfastamento                          
            ,case motivo.codsit
				when '2' then 'F'
				when '3' then 'P'
				when '4' then 'T'
				when '5' then 'M'
				when '6' then 'E'
				when '7' then 'D'
				when '8' then 'L'
				when '9' then 'R'
				when '11' then 'U'
				when '12' then 'F'
				when '13' then 'V'
				when '208' then 'L'
				when '17' then 'D'
				when '52' then 'F'
				when '53' then 'P'
				when '54' then 'T'
				when '55' then 'M'
				when '56' then 'E'
				when '58' then 'L'
				when '59' then 'R'
				when '60' then 'R'
				when '62' then 'F'
				when '63' then 'V'
			end as CodSituacao	                        
        from vetorh.r038afa afastamentos
        inner join dbo.vw_totvs_chapafuncionario chapa on afastamentos.numcad = chapa.numcad
													and afastamentos.tipcol = chapa.tipcol
        inner join vetorh.r034fun funcionario on funcionario.tipcol=afastamentos.tipcol
            and funcionario.numcad=afastamentos.numcad
        inner join vetorh.r010sit motivo on motivo.codsit=afastamentos.sitafa
        left join vetorh.r016hie secao on secao.numloc=funcionario.numloc        
        where chapa.numcpf <> '0' and motivo.codsit in ('2','3','4','5','6','7','8','9','11','12','13','208','17','52','53','54','55','56','58','59','60','62','63')
order by chapa.Chapa";

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

            List<Situacoes> lSituacoes = new List<Situacoes>();

            error = buscarSituacoes(lSituacoes);

            FileHelperEngine engine = new FileHelperEngine(typeof(Situacoes), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, lSituacoes);
        }

        private bool buscarSituacoes(List<Situacoes> lSituacoes)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            error = buscarSituacoes(lSituacoes, database, _querySituacoes1.Replace("{schemaName}", dbName));

            return error;

        }

        private bool buscarSituacoes(List<Situacoes> lSituacoes, Database database, string _query)
        {
            DbCommand command = database.GetSqlStringCommand(_query);

            command.CommandTimeout = 500;

            IDataReader drSituacoes = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drSituacoes.Read())
            {
                Situacoes altSituacao = new Situacoes();

                try
                {
                    processedRecords++;

                    altSituacao.Chapa = drSituacoes["Chapa"].ToString().PadLeft(5, '0');
                    altSituacao.CodMotivoMudanca = drSituacoes["CodMotivoAfastamento"].ToString();
                    altSituacao.DtMudanca = Convert.ToDateTime(drSituacoes["DataMudanca"]);
                    altSituacao.NovaSituacao = drSituacoes["CodSituacao"].ToString();

                    lSituacoes.Add(altSituacao);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a alteração de situação: Chapa {0}, DtMudanca {1}. Motivo:{2}", altSituacao.Chapa, altSituacao.DtMudanca.ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }
            return error;
        }
    }
}
