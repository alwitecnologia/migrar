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
using System.IO;

namespace Exportador.RH.Ferias
{
    public class ExportadorPeriodosGozo : IExportador
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

        private string _queryPeriodosGozo = @"select
	                                chapa.Chapa as Chapa,
	                                REPLACE(CONVERT(char,periodo.fimper,103),'/','') as 'FimPeriodoAquisitivo',
                                    REPLACE(CONVERT(char,recibo.datpag,103),'/','') as 'DataPgto',    
                                    REPLACE(CONVERT(char,recibo.inifer,103),'/','') as 'DataInicio',
                                    REPLACE(CONVERT(char, dateadd (day, recibo.diafer, recibo.inifer),103),'/','') as DataFim,     
                                    REPLACE(CONVERT(char,recibo.inifer,103),'/','') as DataAviso,
                                    REPLACE(recibo.diaabo, '.',',') as NdiasAbono,
                                    case when recibo.opc13s = 'N'
                                    then '0' else '1' end as Pag13Salario,
                                    case when recibo.licrem IS NULL or recibo.licrem = '0'
                                    then '0' end as NDiasLicRem1,    
                                    '0' as NDiasLicRem2,
                                    case when recibo.tipfer = 'C'
		                                then '1'
                                    else '0' end as FeriasColetivas,
    
                                    case 
		                                when periodo.qtdfal <= '5' then '0' 
		                                when periodo.qtdfal >= '6' and periodo.qtdfal <= '14' then '6' 
		                                when periodo.qtdfal >= '15' and periodo.qtdfal <= '23' then '12' 
		                                when periodo.qtdfal >= '24' and periodo.qtdfal <= '32' then '18' 
		                                when periodo.qtdfal > '32' then '30' 
                                    end as FeriasPerdidas,
                                    --periodo.qtdfal as NdiasFaltas,
                                    '' as Observacao,
                                    'F' as SituacaoFerias,
                                    '' as DataInicioDeEmprestimo,
                                    '0' as NumeroVezesEmprestimo,
                                    REPLACE(periodo.qtdfal,'.',',') as Faltas,
                                    '0' as NDiasAntecipados,
                                    '' as FimPerAquisAntec,
                                    '' as DataPgtoAntec,
                                    '0' as PeriodoAntecipado,
                                    '0' as NroDiasFeriado
    
                                from vetorh.r040per as periodo
	                                         inner join dbo.vw_totvs_chapafuncionario chapa on periodo.tipcol = chapa.tipcol
													                                  and periodo.numcad = chapa.numcad
                                             right join vetorh.r040fem recibo on  recibo.numemp = periodo.numemp	      
	                                                                             and recibo.tipcol = periodo.tipcol	
	                                                                             and recibo.numcad = periodo.numcad
	                                                                             and recibo.iniper = periodo.iniper  
                                     where chapa.numcpf <> '0'    ";
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarPeriodosGozo();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorPeriodosGozo()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorPeriodosGozo(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorPeriodosGozo(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarPeriodosGozo()
        {
            List<PeriodosGozo> periodos = new List<PeriodosGozo>();

            periodos.AddRange(buscarPeriodosGozo());

            FileHelperEngine engine = new FileHelperEngine(typeof(PeriodosGozo), Encoding.UTF8);

            engine.WriteFile(_filename, periodos);
            
        }

        private List<PeriodosGozo> buscarPeriodosGozo()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryPeriodosGozo.Replace("{schemaName}", dbName));

            IDataReader periodos = database.ExecuteReader(command);

            List<PeriodosGozo> lperiodos = new List<PeriodosGozo>();

            while (periodos.Read())
            {
                PeriodosGozo lnperiodos = new PeriodosGozo();

                lnperiodos.Chapa = periodos["Chapa"].ToString();
                lnperiodos.FimPeriodoAquisitivo = periodos["FimPeriodoAquisitivo"].ToString();
                lnperiodos.DataPgto = periodos["DataPgto"].ToString();
                lnperiodos.DataInicio = periodos["DataInicio"].ToString();
                lnperiodos.DataFim = periodos["DataFim"].ToString();
                lnperiodos.DataAviso = periodos["DataAviso"].ToString();
                lnperiodos.NdiasAbono = periodos["NdiasAbono"].ToString();
                lnperiodos.Pag13Salario = periodos["Pag13Salario"].ToString();
                lnperiodos.NdiasLicRem1 = periodos["NdiasLicRem1"].ToString();
                lnperiodos.NdiasLicRem2 = periodos["NdiasLicRem2"].ToString();
                lnperiodos.FeriasColetivas = periodos["FeriasColetivas"].ToString();
                lnperiodos.FeriasPerdidas = periodos["FeriasPerdidas"].ToString();
                lnperiodos.Observacao = periodos["Observacao"].ToString();
                lnperiodos.SituacaoFerias = periodos["SituacaoFerias"].ToString();
                lnperiodos.DataInicioDeEmprestimo = periodos["DataInicioDeEmprestimo"].ToString();
                lnperiodos.NumeroVezesEmprestimo = periodos["NumeroVezesEmprestimo"].ToString();
                lnperiodos.Faltas = periodos["Faltas"].ToString();
                lnperiodos.NDiasAntecipados = periodos["NDiasAntecipados"].ToString();
                lnperiodos.FimPerAquisAntec = periodos["FimPerAquisAntec"].ToString();
                lnperiodos.DataPgtoAntec = periodos["DataPgtoAntec"].ToString();
                lnperiodos.PeriodoAntecipado = periodos["PeriodoAntecipado"].ToString();
                lnperiodos.NroDiasFeriado = periodos["NroDiasFeriado"].ToString();

                lnperiodos.NroDiasFeriado = "";

                lnperiodos.EndLine = String.Empty;

                lperiodos.Add(lnperiodos);
            }

            return lperiodos;
        }
    }
}
