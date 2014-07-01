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
    public class ExportadorFuncoes: IExportador
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
        public ExportadorFuncoes()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorFuncoes(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorFuncoes(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryHistFuncoes = @"select 
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
	                                            ,histFuncoes.datalt as DtMudanca
	                                            ,histFuncoes.codmot as CodMotivoMudanca
	                                            ,histFuncoes.codcar as CodFuncao
                                            from {schemaName}.r038hca histFuncoes
                                            inner join {schemaName}.r034fun funcionario on funcionario.numemp=histFuncoes.numemp
                                                and funcionario.tipcol=histFuncoes.tipcol
                                                and funcionario.numcad=histFuncoes.numcad
                                            left join {schemaName}.r016hie secao on secao.numloc=funcionario.numloc
                                            where secao.taborg=5";

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

            List<Funcoes> funcoes = new List<Funcoes>();

            error = buscarHistoricoFuncoes(funcoes);

            FileHelperEngine engine = new FileHelperEngine(typeof(Funcoes), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, funcoes);
        }

        private bool buscarHistoricoFuncoes(List<Funcoes> funcoes)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryHistFuncoes.Replace("{schemaName}", dbName));

            IDataReader drFuncoes = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drFuncoes.Read())
            {
                Funcoes histFuncao = new Funcoes();

                try
                {
                    processedRecords++;

                    histFuncao.Chapa = drFuncoes["Chapa"].ToString();
                    histFuncao.DtMudanca = Convert.ToDateTime(drFuncoes["DtMudanca"]);
                    histFuncao.CodMotivoMudanca = drFuncoes["CodMotivoMudanca"].ToString();
                    histFuncao.CodFuncao = drFuncoes["CodFuncao"].ToString();
                    
                    funcoes.Add(histFuncao);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o movimento de função: Chapa {0}, DtMudanca {1}. Motivo:{2}", histFuncao.Chapa, histFuncao.DtMudanca.ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;
        }
    }
}
