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

namespace Exportador.RH.Funcionario
{
    public class ExportadorValoresPagosPensaoDependentes : IExportador
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

        private string _queryAquisicaoFerias = @"select distinct
    chapa.Chapa 'CHAPA', 
    dep.coddep 'NDEPENDENTE', 
     Coalesce (Year (histpagval.perref), '') 'ANOCOMPETENCIA',
     Month (histpagval.perref) as 'MESCOMPETENCIA',
     Month (histpagval.datpag) 'MESCAIXA',
     dep.grapar as 'TIPOMOVIMENTACAOPENSAO',
    CASE 
			when MONTH(histpagval.datpag) = '11' and MONTH(histpagval.perref) = '11' then '30'
			when MONTH(histpagval.datpag) = '12' and MONTH(histpagval.perref) = '12' then '35'
			else '20'
	end as 'NUMERODOPERIODO',
    CAST (histpag.valpen AS MONEY) as 'VALOR',
    CAST (histpag.valpen AS MONEY) as 'VALORORIGINAL',
    0 'INDICATIVOALTERACAOMANUAL'
from
vw_totvs_chapafuncionario chapa
INNER JOIN vetorh.r036dep dep 
           on dep.numcad=chapa.numcad
           and dep.tipcol=chapa.tipcol 
           and dep.penjud = 's'
INNER JOIN vetorh.r036pjp histpag
           on histpag.tipcol = dep.tipcol
           and histpag.numcad = dep.numcad
           and histpag.coddep = dep.coddep
inner JOIN vetorh.r044cal histpagval
          on histpagval.codcal = histpag.codcal          
WHERE dep.grapar in (1,2) and Chapa.numcpf <> '0'  ";
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarValoresPagosPensaoDependentes();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorValoresPagosPensaoDependentes()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorValoresPagosPensaoDependentes(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorValoresPagosPensaoDependentes(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarValoresPagosPensaoDependentes()
        {
            List<ValoresPagosPensaoDependentes> aquisicao = new List<ValoresPagosPensaoDependentes>();

            aquisicao.AddRange(buscarValoresPagosPensaoDependentes());

            FileHelperEngine engine = new FileHelperEngine(typeof(ValoresPagosPensaoDependentes), Encoding.UTF8);

            engine.WriteFile(_filename, aquisicao);
        }

        private List<ValoresPagosPensaoDependentes> buscarValoresPagosPensaoDependentes()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryAquisicaoFerias.Replace("{schemaName}", dbName));

            IDataReader drAquisicaoFerias = database.ExecuteReader(command);

            List<ValoresPagosPensaoDependentes> lpensao = new List<ValoresPagosPensaoDependentes>();

            while (drAquisicaoFerias.Read())
            {
                ValoresPagosPensaoDependentes pensao = new ValoresPagosPensaoDependentes();

                pensao.CHAPA = drAquisicaoFerias["CHAPA"].ToString();
                pensao.NDEPENDENTE = drAquisicaoFerias["NDEPENDENTE"].ToString();
                pensao.ANOCOMPETENCIA = drAquisicaoFerias["ANOCOMPETENCIA"].ToString();
                pensao.MESCOMPETENCIA = drAquisicaoFerias["MESCOMPETENCIA"].ToString();
                pensao.MESCAIXA = drAquisicaoFerias["MESCAIXA"].ToString();
                pensao.TIPOMOVIMENTACAOPENSAO = drAquisicaoFerias["TIPOMOVIMENTACAOPENSAO"].ToString();
                pensao.NUMEROPERIODO = drAquisicaoFerias["NUMERODOPERIODO"].ToString();

                pensao.VALOR = drAquisicaoFerias["VALOR"].ToString();
                pensao.VALORORIGINAL = drAquisicaoFerias["VALORORIGINAL"].ToString();
                pensao.INDICATIVOALTERACAOMANUAL = drAquisicaoFerias["INDICATIVOALTERACAOMANUAL"].ToString();

                lpensao.Add(pensao);
            }

            return lpensao;
        }
    }
}
