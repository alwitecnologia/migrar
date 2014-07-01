using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Exportador.Helpers;
using Exportador.Interface;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Configuration;

namespace Exportador.RH.Ferias
{
    public class ExportadorReciboFerias: IExportador
    {
        #region Private Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool _debugMode;

        #endregion

        #region Public Properties

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

        #region Queries

        private string _queryReciboFerias = @"select
	                                            chapa.Chapa as 'CHAPA',
	                                            REPLACE(CONVERT(char,periodo.fimper,103),'/','') as 'FIMPERAQUIS',
	                                            REPLACE(CONVERT(char,recibo.datpag,103),'/','') as 'DATAPAGTO',
	                                            '0' as 'INSS1',
	                                            '0' AS 'INSS2',
	                                            '0' AS 'IRRF',
	                                            '0' AS 'BASEINSS1',
	                                            '0' AS 'BASEINSS2' ,
	                                            '0' AS 'BASEIRRF',
	                                            '0' AS 'LIQUIDO',
	                                            '' AS 'OBSERVACAO',
	                                            '0' AS 'EXECID',
	                                            '0' AS 'PENSAO',
	                                            '0' AS 'BASEPENSAO',
	                                            '0' AS 'VALORESFORCADOS',
	                                            '0' AS 'MEDIAPERAQATUAL',
	                                            '0' AS 'MEDIAPROXPERAQ',
	                                            '0' AS 'SALARIO'
                                            from 
	                                            vetorh.r040per periodo 
		                                            inner join dbo.vw_totvs_chapafuncionario chapa 
				                                            on chapa.numcad = periodo.numcad
				                                            and chapa.tipcol = periodo.tipcol
		                                            inner join vetorh.r040fem recibo
				                                            on recibo.numemp = periodo.numemp
				                                            and recibo.tipcol = periodo.tipcol
				                                            and recibo.numcad = periodo.numcad
				                                            and recibo.iniper = periodo.iniper
                                                where chapa.numcpf <> '0'    
";
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarReciboFerias();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorReciboFerias()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorReciboFerias(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorReciboFerias(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarReciboFerias()
        {
            List<ReciboFerias> aquisicao = new List<ReciboFerias>();

            aquisicao.AddRange(buscarReciboFerias());

            FileHelperEngine engine = new FileHelperEngine(typeof(ReciboFerias), Encoding.UTF8);

            engine.WriteFile(_filename, aquisicao);
        }

        private List<ReciboFerias> buscarReciboFerias()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryReciboFerias.Replace("{schemaName}", dbName));

            IDataReader drAquisicaoFerias = database.ExecuteReader(command);

            List<ReciboFerias> lrecibo = new List<ReciboFerias>();

            while (drAquisicaoFerias.Read())
            {
                ReciboFerias recibo = new ReciboFerias();

                recibo.CHAPA = drAquisicaoFerias["CHAPA"].ToString();
                recibo.FIMPERAQUIS = drAquisicaoFerias["FIMPERAQUIS"].ToString();
                recibo.DATAPAGTO = drAquisicaoFerias["DATAPAGTO"].ToString();
                recibo.INSS1 = drAquisicaoFerias["INSS1"].ToString();
                recibo.INSS2 = drAquisicaoFerias["INSS2"].ToString();
                recibo.IRRF = drAquisicaoFerias["IRRF"].ToString();
                recibo.BASEINSS1 = drAquisicaoFerias["BASEINSS1"].ToString();
                recibo.BASEINSS2 = drAquisicaoFerias["BASEINSS2"].ToString();
                recibo.BASEIRRF = drAquisicaoFerias["BASEIRRF"].ToString();
                recibo.LIQUIDO = drAquisicaoFerias["LIQUIDO"].ToString();
                recibo.OBSERVACAO = drAquisicaoFerias["OBSERVACAO"].ToString();
                recibo.EXECID = drAquisicaoFerias["EXECID"].ToString();
                recibo.PENSAO = drAquisicaoFerias["PENSAO"].ToString();
                recibo.BASEPENSAO = drAquisicaoFerias["BASEPENSAO"].ToString();
                recibo.VALORESFORCADOS = drAquisicaoFerias["VALORESFORCADOS"].ToString();
                recibo.MEDIAPERAQATUAL = drAquisicaoFerias["MEDIAPERAQATUAL"].ToString();
                recibo.MEDIAPROXPERAQ = drAquisicaoFerias["MEDIAPROXPERAQ"].ToString();
                recibo.SALARIO = drAquisicaoFerias["SALARIO"].ToString();

                lrecibo.Add(recibo);
            }

            return lrecibo;
        }
    }
}
