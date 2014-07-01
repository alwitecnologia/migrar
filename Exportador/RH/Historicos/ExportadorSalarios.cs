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
    public class ExportadorSalarios: IExportador
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
        public ExportadorSalarios()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorSalarios(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorSalarios(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion


        #region Queries

        private string _queryHistSalarios = @"select 

	                                                chapa.Chapa as Chapa
	                                                ,histSal.datalt as DtMudanca
	                                                ,histSal.codmot as CodMotivo
	                                                ,histSal.seqalt as NumSalario
	                                                ,histSal.valsal as ValorSalario
                             ,CAST(escala.hormes/60 AS VARCHAR(50)) +':'+ REPLICATE('0', 2 - LEN(CAST(escala.hormes%60 AS VARCHAR(50))))+ RTrim(CAST(escala.hormes%60 AS VARCHAR(50))) as Jornada

                                                from vetorh.r038hsa as histSal
                                                inner join dbo.vw_totvs_chapafuncionario chapa on histSal.numcad = chapa.numcad
																							  and histSal.tipcol = chapa.tipcol
                                                inner join vetorh.r034fun funcionario on funcionario.numemp=histSal.numemp
                                                    and funcionario.tipcol=histSal.tipcol
                                                    and funcionario.numcad=histSal.numcad
                                                
                                                inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
                                                left join vetorh.r006esc escala on escala.codesc=funcionario.codesc
                                                where chapa.numcpf <> '0'    
                                                order by funcionario.numcad,datalt desc";

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

            List<Salarios> periodos = new List<Salarios>();

            error = buscarHistoricoSalarios(periodos);

            FileHelperEngine engine = new FileHelperEngine(typeof(Salarios), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, periodos);
        }

        private bool buscarHistoricoSalarios(List<Salarios> periodos)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryHistSalarios.Replace("{schemaName}", dbName));

            IDataReader drHistSalarios = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drHistSalarios.Read())
            {
                Salarios histSalario = new Salarios();

                try
                {
                    processedRecords++;

                   // histSalario.Chapa = drHistSalarios["Chapa"].ToString();
                    histSalario.Chapa = drHistSalarios["Chapa"].ToString();
                    histSalario.Chapa = histSalario.Chapa.PadLeft(5, '0');

                    histSalario.CodMotivo = drHistSalarios["CodMotivo"].ToString();
                    histSalario.Jornada = drHistSalarios["Jornada"].ToString();

                    if (drHistSalarios["DtMudanca"] != DBNull.Value)
                    {
                        histSalario.DtMudanca = Convert.ToDateTime(drHistSalarios["DtMudanca"]);
                        histSalario.DtProcessamento = Convert.ToDateTime(drHistSalarios["DtMudanca"]);
                    }

                    if (drHistSalarios["NumSalario"] != DBNull.Value)
                        histSalario.NumSalario = Convert.ToInt32(drHistSalarios["NumSalario"]);

                    if (drHistSalarios["ValorSalario"] != DBNull.Value)
                        histSalario.ValorSalario = Convert.ToDouble(drHistSalarios["ValorSalario"]);

                    periodos.Add(histSalario);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a alteração: Chapa {0}, DtMudanca {1}. Motivo:{2}", histSalario.Chapa, Convert.ToDateTime(histSalario.DtMudanca).ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;
        }
    }
}
