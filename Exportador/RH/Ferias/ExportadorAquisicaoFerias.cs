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
    public class ExportadorAquisicaoFerias: IExportador
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
                                                   chapa.Chapa as Chapa,
                                                   REPLACE(CONVERT(char,periodo.fimper,103),'/','') as 'FimPeriodoAquisitivo',
                                                   REPLACE(CONVERT(char,periodo.iniper,103),'/','') as 'InicioPeriodoAquisitivo',
	                                               REPLACE(periodo.qtdsld,'.',',') as Saldo,
                                                   case when periodo.fimper >= GETDATE()
			                                            then '1'
	                                               else '0' end as PeriodoAberto,
	                                               case when recibo.inifer IS NOT NULL and periodo.fimper >= GETDATE()
			                                            then '1'
	                                               else '0' end as PeriodoPerdido,
	                                               '' as MotivoPerda,
	                                               REPLACE(periodo.qtdfal,'.',',') as Faltas,
	                                               '0,00' as Bonus
                                                from vetorh.r040per as periodo
                                                        inner join vetorh.r034fun funcionario on periodo.numemp = funcionario.numemp
	                                                                                         and periodo.tipcol = funcionario.tipcol
	                                                                                         and periodo.numcad = funcionario.numcad
	                                                    inner join dbo.vw_totvs_chapafuncionario chapa on periodo.tipcol = chapa.tipcol
														                                     and periodo.numcad = chapa.numcad 
                                                        left join vetorh.r040fem recibo on  recibo.numemp = periodo.numemp	      
	                                                                                         and recibo.tipcol = periodo.tipcol	
	                                                                                         and recibo.numcad = periodo.numcad
	                                                                                         and recibo.iniper = periodo.iniper    
                                                        inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
				                                    where chapa.numcpf <> '0'     
                                                    order by PeriodoAberto desc";
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarAquisicaoFerias();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorAquisicaoFerias()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorAquisicaoFerias(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorAquisicaoFerias(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarAquisicaoFerias()
        {
            List<AquisicaoFerias> aquisicao = new List<AquisicaoFerias>();

            aquisicao.AddRange(buscarAquisicaoFerias());

            FileHelperEngine engine = new FileHelperEngine(typeof(AquisicaoFerias), Encoding.UTF8);

            engine.WriteFile(_filename, aquisicao);
        }

        private List<AquisicaoFerias> buscarAquisicaoFerias()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryAquisicaoFerias.Replace("{schemaName}", dbName));

            IDataReader drAquisicaoFerias = database.ExecuteReader(command);

            List<AquisicaoFerias> laquisicao = new List<AquisicaoFerias>();

            while (drAquisicaoFerias.Read())
            {
                AquisicaoFerias aquisicao = new AquisicaoFerias();

                aquisicao.Chapa = drAquisicaoFerias["Chapa"].ToString();
                aquisicao.FimPeriodoAquisitivo = drAquisicaoFerias["FimPeriodoAquisitivo"].ToString();
                aquisicao.InicioPeriodoAquisitivo = drAquisicaoFerias["InicioPeriodoAquisitivo"].ToString();
                aquisicao.Saldo = drAquisicaoFerias["Saldo"].ToString();
                aquisicao.PeriodoAberto = drAquisicaoFerias["PeriodoAberto"].ToString();
                aquisicao.PeriodoPerdido = drAquisicaoFerias["PeriodoPerdido"].ToString();
                aquisicao.MotivoPerda = drAquisicaoFerias["MotivoPerda"].ToString();
                aquisicao.Faltas = drAquisicaoFerias["Faltas"].ToString();
                aquisicao.Bonus = drAquisicaoFerias["Bonus"].ToString();

                laquisicao.Add(aquisicao);
            }

            return laquisicao;
        }
    }
}
