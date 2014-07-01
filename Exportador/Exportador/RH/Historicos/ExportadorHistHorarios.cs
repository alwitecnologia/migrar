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

namespace Exportador.RH.Historicos
{
    public class ExportadorHistHorarios: IExportador
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
        public ExportadorHistHorarios()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorHistHorarios(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorHistHorarios(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryHistHorarios = @"with HistHorario as (
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
                      ,histHorario.datalt as DtMudanca
                      ,histHorario.codesc as CodHorario
                      from vetorh.r038hes histHorario
                      inner join vetorh.r034fun funcionario on funcionario.numemp=histHorario.numemp
                          and funcionario.tipcol=histHorario.tipcol
                          and funcionario.numcad=histHorario.numcad
                      inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
                      where secao.taborg=5
                    )

                    select Chapa
	                    ,DtMudanca
	                    ,CodHorario
	                    ,ROW_NUMBER() OVER (PARTITION BY CHAPA	ORDER BY DtMudanca) AS IndiceInicioHorario
                    from HistHorario
                    order by Chapa,DtMudanca
                        ";

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

            List<Horarios> horarios = new List<Horarios>();

            error = buscarHistoricoHorarios(horarios);

            FileHelperEngine engine = new FileHelperEngine(typeof(Horarios), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, horarios);
        }

        private bool buscarHistoricoHorarios(List<Horarios> horarios)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryHistHorarios.Replace("{schemaName}", dbName));

            IDataReader drHistHorarios = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drHistHorarios.Read())
            {
                Horarios histHorarios = new Horarios();

                try
                {
                    processedRecords++;

                    histHorarios.Chapa = drHistHorarios["Chapa"].ToString();
                    histHorarios.CodHorario = drHistHorarios["CodHorario"].ToString();
                    histHorarios.IndiceInicioHorario = Convert.ToInt32(drHistHorarios["IndiceInicioHorario"]);

                    if (drHistHorarios["DtMudanca"] != DBNull.Value)
                        histHorarios.DtMudanca = Convert.ToDateTime(drHistHorarios["DtMudanca"]);

                    horarios.Add(histHorarios);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a alteração: Chapa {0}, DtMudanca {1}. Motivo:{2}", histHorarios.Chapa, Convert.ToDateTime(histHorarios.DtMudanca).ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;
        }
    }
}
