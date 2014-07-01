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

namespace Exportador.RH.Historicos
{
    public class ExportadorDadosBancarios : IExportador
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

        private string _queryAquisicaoFerias = @"select 
 chapa.Chapa as 'CHAPA'
 ,case 
		when funcionario.codban = 1242 
		then '33'
		else CAST(funcionario.codban AS VARCHAR(4))	
	end as BANCOPAGAMENTO
     ,funcionario.codage as 'AGENCIAPAGAMENTO'
     ,cast(funcionario.conban as varchar) + cast(funcionario.digban as varchar) as 'CONTAPAGAMENTO'
     ,'' as 'OPERACAOBANCARIA' ,
     '01112013' as 'DATAMUDANCA'
 from 
     vetorh.r034fun funcionario inner join vw_totvs_chapafuncionario chapa on funcionario.numcad = chapa.numcad
							                                             and funcionario.tipcol = chapa.tipcol
	 inner join vetorh.r012age agencia on funcionario.codban = agencia.codban and funcionario.codage = agencia.codage				                                             
where chapa.numcpf <> '0' and (funcionario.codban <> 0 and funcionario.codage <> 0 and funcionario.conban <> 0)
     
     			                                             ";
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarDadosBancarios();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorDadosBancarios()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorDadosBancarios(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorDadosBancarios(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarDadosBancarios()
        {
            List<DadosBancarios> aquisicao = new List<DadosBancarios>();

            aquisicao.AddRange(buscarDadosBancarios());

            FileHelperEngine engine = new FileHelperEngine(typeof(DadosBancarios), Encoding.UTF8);

            engine.WriteFile(_filename, aquisicao);
        }

        private List<DadosBancarios> buscarDadosBancarios()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryAquisicaoFerias.Replace("{schemaName}", dbName));

            IDataReader drDadosBancarios = database.ExecuteReader(command);

            List<DadosBancarios> ldados = new List<DadosBancarios>();

            while (drDadosBancarios.Read())
            {
                DadosBancarios dados = new DadosBancarios();

                dados.CHAPA = drDadosBancarios["CHAPA"].ToString().PadLeft(5, '0');
                dados.BANCOPAGAMENTO = drDadosBancarios["BANCOPAGAMENTO"].ToString();
                dados.AGENCIAPAGAMENTO = drDadosBancarios["AGENCIAPAGAMENTO"].ToString();
                dados.CONTAPAGAMENTO = drDadosBancarios["CONTAPAGAMENTO"].ToString();
                dados.OPERACAOBANCARIA = drDadosBancarios["OPERACAOBANCARIA"].ToString();
                dados.DATAMUDANCA = drDadosBancarios["DATAMUDANCA"].ToString();

                ldados.Add(dados);
            }

            return ldados;
        }
    }
}
