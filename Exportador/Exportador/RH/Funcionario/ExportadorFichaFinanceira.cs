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

namespace Exportador.RH.Funcionario
{
    public class ExportadorFichaFinanceira: IExportador
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
        public ExportadorFichaFinanceira()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorFichaFinanceira(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorFichaFinanceira(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryFichas = @"select * from (
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
	,DATEPART(YEAR,calculo.perref) as AnoCompetencia
	,DATEPART(MONTH,calculo.perref) as MesCompetencia
	,fichaFinanc.codeve as CodEvento
	,calculo.datpag as DtPagto
	,fichaFinanc.refeve as Referencia
	,fichaFinanc.valeve as Valor
	
	,fichaFinanc.codcal
from vetorh.r046ver as fichaFinanc
inner join vetorh.r034fun funcionario on funcionario.numemp=fichaFinanc.numemp
    and funcionario.tipcol=fichaFinanc.tipcol
    and funcionario.numcad=fichaFinanc.numcad
inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
inner join vetorh.r044cal calculo on calculo.numemp=fichaFinanc.numemp
	and calculo.codcal=fichaFinanc.codcal
where secao.taborg=5
) as fichas
where fichas.Chapa=480
and AnoCompetencia=2013
and MesCompetencia=9";

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

            List<FichaFinanceira> fichas = new List<FichaFinanceira>();

            error = buscarFichasFinanceiras(fichas);

            FileHelperEngine engine = new FileHelperEngine(typeof(FichaFinanceira), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, fichas);
        }

        private bool buscarFichasFinanceiras(List<FichaFinanceira> fichas)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryFichas.Replace("{schemaName}", dbName));

            _bgWorker.ReportProgress(0, "Executando consulta...");

            IDataReader drFichas = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            _bgWorker.ReportProgress(0, "Montando objetos...");

            while (drFichas.Read())
            {
                FichaFinanceira ficha = new FichaFinanceira();

                try
                {
                    processedRecords++;

                    ficha.Chapa = drFichas["Chapa"].ToString();
                    ficha.AnoCompetencia = Convert.ToInt32(drFichas["AnoCompetencia"]);
                    ficha.MesCompetencia = Convert.ToInt32(drFichas["MesCompetencia"]);
                    ficha.CodEvento = drFichas["CodEvento"].ToString();
                    ficha.DtPagto = Convert.ToDateTime(drFichas["DtPagto"]);
                    ficha.Referencia = Convert.ToDouble(drFichas["Referencia"]);
                    ficha.Valor = Convert.ToDouble(drFichas["Valor"]);
                    
                    ficha.NumPeriodo = 1;//deixado fixo pois não foi encontrado no sistema atual.
                    ficha.ValorOriginal = Convert.ToDouble(drFichas["Valor"]);//mesmo campo valor, pois não foi encontrado no sistema atual.

                    fichas.Add(ficha);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a ficha financeira: Chapa {0}, Competência {1}/{2}. Motivo:{3}", ficha.Chapa,ficha.MesCompetencia.ToString(), ficha.AnoCompetencia.ToString(), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }
    }
}
