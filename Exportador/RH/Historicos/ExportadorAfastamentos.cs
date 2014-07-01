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
    public class ExportadorAfastamentos: IExportador
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
        public ExportadorAfastamentos()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorAfastamentos(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorAfastamentos(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _queryAfastamentos = @"
                                           select 
			chapa.Chapa as Chapa
            ,DATEADD(minute,afastamentos.horafa,afastamentos.datafa) as DataInicioAfastamento
            ,DATEADD(minute,afastamentos.horter,afastamentos.datter) as DataFinalAfastamento                 
            ,case motivo.codsit
				when '3' then '03'
				when '4' then '04'
				when '5' then '05'
				when '6' then '06'
				when '8' then '08'
				when '11' then '10'
				when '208' then '13'
				when '53' then '03'
				when '54' then '04'
				when '55' then '05'
				when '56' then '06'
				when '58' then '08'
			end as CodMotivoAfastamento                          
            ,case motivo.codsit
				when '3' then 'P'
				when '4' then 'T'
				when '5' then 'M'
				when '6' then 'E'
				when '8' then 'L'
				when '11' then 'U'
				when '208' then 'L'
				when '53' then 'P'
				when '54' then 'T'
				when '55' then 'M'
				when '56' then 'E'
				when '58' then 'L'
			end as CodSituacao	                        
        from vetorh.r038afa afastamentos
        inner join dbo.vw_totvs_chapafuncionario chapa on afastamentos.numcad = chapa.numcad
													and afastamentos.tipcol = chapa.tipcol
        inner join vetorh.r034fun funcionario on funcionario.tipcol=afastamentos.tipcol
            and funcionario.numcad=afastamentos.numcad
        inner join vetorh.r010sit motivo on motivo.codsit=afastamentos.sitafa
        left join vetorh.r016hie secao on secao.numloc=funcionario.numloc        
        where chapa.numcpf <> '0' and motivo.codsit in ('3','4','5','6','8','11','208','53','54','55','56','58')
        order by chapa.Chapa 
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

            List<Afastamento> lAfastamentos = new List<Afastamento>();

            //funcionarios.AddRange(buscarFuncionarios(error));

            error = buscarAfastamentos(lAfastamentos);

            FileHelperEngine engine = new FileHelperEngine(typeof(Afastamento), Encoding.Unicode);

            _bgWorker.RunWorkerCompleted += workerCompleted;

            engine.WriteFile(_filename, lAfastamentos);
        }

        private bool buscarAfastamentos(List<Afastamento> lAfastamentos)
        {
            bool error = false;

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryAfastamentos.Replace("{schemaName}", dbName));

            IDataReader drAfastamento = database.ExecuteReader(command);

            double totalRecords = database.ExecuteReader(command).RowCount();

            double processedRecords = 0;

            while (drAfastamento.Read())
            {
                Afastamento afast = new Afastamento();

                try
                {
                    processedRecords++;

                    //afast.Chapa = drAfastamento["Chapa"].ToString();
                    afast.Chapa = drAfastamento["Chapa"].ToString();
                    afast.Chapa = afast.Chapa.PadLeft(5, '0');

                    afast.DataInicioAfastamento = Convert.ToDateTime(drAfastamento["DataInicioAfastamento"]);
                    afast.DataFinalAfastamento = Convert.ToDateTime(drAfastamento["DataFinalAfastamento"]);
                    afast.CodTipoAfastamento = drAfastamento["CodSituacao"].ToString();
                    afast.CodMotivoAfastamento = drAfastamento["CodMotivoAfastamento"].ToString();

                    lAfastamentos.Add(afast);

                }
                catch (Exception ex)
                {
                    error = true;

                    _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o afastamento: Chapa {0}, DtInicioAfastamento {1}. Motivo:{2}", afast.Chapa, afast.DataInicioAfastamento.ToString("ddMMyyyy hh:mm"), ex.Message));
                }

                _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
            }

            return error;

        }

        private string BuscarTipoAfastamento(string tipoAfastamento, string motivoAfastamento)
        {
            if (tipoAfastamento == "Licença Maternidade (e Paternidade até 2005)" || motivoAfastamento == "Lic.Maternidade")
                return TipoAfastamento.LicencaMaternidade;

            if (motivoAfastamento == "AUX.MAT.INSS")
                return TipoAfastamento.LicencaMaternidade;

            if (motivoAfastamento == "Licença Rem. p/ Empresa")
                return TipoAfastamento.LicencaRemunerada;

            if (motivoAfastamento == "Acidente Trabalho")
                return TipoAfastamento.AfastamentoAcidenteTrabalho;

            return TipoAfastamento.Outros;
        }

    }
}
