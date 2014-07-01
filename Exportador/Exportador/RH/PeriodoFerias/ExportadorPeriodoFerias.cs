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

namespace Exportador.RH.PeriodoFerias
{
    public class ExportadorPeriodoFerias: IExportador
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
        public ExportadorPeriodoFerias()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorPeriodoFerias(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorPeriodoFerias(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryPeriodosFerias = @"WITH PeriodoFerias as (
                                                    select 
      case when funcionario.tipcol=2 
		and funcionario.usu_terati='N'
		and LEN(case when funcionario.codcha='' then funcionario.numcad else funcionario.codcha end)<5
		then 
              '6'+REPLICATE('0', 4 - LEN(
                                          case when funcionario.codcha='' then funcionario.numcad else funcionario.codcha end
                                          ))+ 
                                          RTrim(case when funcionario.codcha='' then funcionario.numcad else funcionario.codcha end)
      else 
              CAST(funcionario.numcad as varchar(50))
      end
      as Chapa
	                                                    ,perFerias.iniper as DtVencimento
	                                                    ,reciboFeriasMestre.datpag as DtPagto
                                                    from vetorh.r040per perFerias
                                                    inner join vetorh.r034fun funcionario on funcionario.numemp=perFerias.numemp
                                                        and funcionario.tipcol=perFerias.tipcol
                                                        and funcionario.numcad=perFerias.numcad
                                                    inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
                                                    inner join vetorh.r040fem reciboFeriasMestre on reciboFeriasMestre.numemp=perFerias.numemp
	                                                    and reciboFeriasMestre.tipcol=perFerias.tipcol
	                                                    and reciboFeriasMestre.numcad=perFerias.numcad
	                                                    and reciboFeriasMestre.iniper=perFerias.iniper
                                                    where secao.taborg=5

                                                    )

                                                    select 
	                                                    Chapa 
	                                                    ,ROW_NUMBER() OVER (PARTITION BY CHAPA	ORDER BY DtVencimento) AS NumeroPeriodo
	                                                    ,DtVencimento
	                                                    ,DtPagto
                                                    from PeriodoFerias";

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

            List<PeriodoFerias> periodos = new List<PeriodoFerias>();

            error = buscarPeriodosFerias(periodos);

            FileHelperEngine engine = new FileHelperEngine(typeof(PeriodoFerias), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, periodos);
        }

        private bool buscarPeriodosFerias(List<PeriodoFerias> periodos)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryPeriodosFerias.Replace("{schemaName}", dbName));

            IDataReader drPeriodos = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drPeriodos.Read())
            {
                PeriodoFerias pFerias = new PeriodoFerias();

                try
                {
                    processedRecords++;

                    pFerias.ChapaFunc = drPeriodos["Chapa"].ToString();
                    pFerias.NumeroPeriodo = Convert.ToInt32(drPeriodos["NumeroPeriodo"]);

                    if (drPeriodos["DtVencimento"] != DBNull.Value)
                        pFerias.DtVencimento = Convert.ToDateTime(drPeriodos["DtVencimento"]);

                    if (drPeriodos["DtPagto"] != DBNull.Value)
                        pFerias.DtPagamento = Convert.ToDateTime(drPeriodos["DtPagto"]);

                    periodos.Add(pFerias);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o período de férias: Chapa {0}, DtMudanca {1}. Motivo:{2}", pFerias.ChapaFunc, Convert.ToDateTime(pFerias.DtVencimento).ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;
        }
    }
}
