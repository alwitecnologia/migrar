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
    public class ExportadorPeriodoFichaFinanceira : IExportador
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

        private string _queryPeriodoFichaFinanceira = @"select * from (
select distinct
		chapa.Chapa as Chapa
		,YEAR(calculo.perref) as AnoCompetencia
		,MONTH(calculo.perref) as MesCompetencia
		, CASE 
			when MONTH(calculo.datpag) = '11' and MONTH(calculo.perref) = '11' then '30'
			when MONTH(calculo.datpag) = '12' and MONTH(calculo.perref) = '12' then '35'
			else '20'
		end as NumeroPeriodo
		,MONTH(calculo.datpag) as MesCaixaComum
		,'0,00' as BaseINSS
		,'0,00' as BaseINSSdoOutroEmprego
		,'0,00' as INSS
		,'0,00' as INSSdeFerias
		,'0,00' as INSSdeOutroEmprego
		,'0,00' as BaseINSSdo13
		,'0,00' as BaseINSS13doOutroEmprego
		,'0,00' as INSSdo13
		,'0,00' as BaseSalarioFamilia
		,'0,00' as SalarioFamilia
		,'0,00' as BaseDeValeTransporte
		,'0,00' as ValeTransporteEntregue
		,'0,00' as ValeTransporteDescontado
		,'0,00' as BaseIRRF
		,'0,00' as IRRF
		,'0,00' as INSSCaixa
		,'0,00' as INSSCalculadoPeloUsuario
		,'0,00' as DedutivelEmIRRF
		,'0' as BaseIRRFFerias
		,'0,00' as IRFFParticipacao
		,'0,00' as IRRFFerias
		,'0,00' as IndicativoDeINSSComCPMF
		,'0,00' as BaseDeFGTS
		,'0,00' as BaseDeFGTSDe13Salario
		,'0,00' as SalarioPago
		,'FOLHA MENSAL\FÉRIAS\RESCISÃO NOR' as  DescricaoDoPeriodo
		,'0,00' as BaseDeIRRFde13Salario
		,'0,00' as IRRFSob13
		,'0,00' as INSSDeFeruasComCPMF
		,'0,00' as INSSDeDiferencaSalarial
		,'0,00' as INSSDeDiferencaSalarialDe13
		,'0,00' as INSSDeDiferencaSalarialDeFerias
		,'0,00' as BaseFGTSDifSalarial
		,'0,00' as a1
		,'0,00' as a2
		,'0,00' as a3
		,'0,00' as a4
		,'0,00' as a5
		,'0,00' as a6
from vetorh.r046ver as fichaFinanc
inner join vetorh.r034fun funcionario on funcionario.numemp=fichaFinanc.numemp
    and funcionario.tipcol=fichaFinanc.tipcol
    and funcionario.numcad=fichaFinanc.numcad
inner join dbo.vw_totvs_chapafuncionario chapa on fichaFinanc.numcad = chapa.numcad
												and fichaFinanc.tipcol = chapa.tipcol    
inner join vetorh.r016hie secao on secao.numloc=funcionario.numloc
inner join vetorh.r044cal calculo on calculo.numemp=fichaFinanc.numemp
	and calculo.codcal=fichaFinanc.codcal
inner join vetorh.r008evc evento on fichaFinanc.codeve = evento.codeve
where chapa.numcpf <> '0'    
) as fichas
where AnoCompetencia >= 2012 ";

        // where secao.taborg=5 and chapa.numcad = 359
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarPeriodoFichaFinanceira();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorPeriodoFichaFinanceira()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorPeriodoFichaFinanceira(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorPeriodoFichaFinanceira(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarPeriodoFichaFinanceira()
        {
            List<PeriodoFichaFinanceira> periodo = new List<PeriodoFichaFinanceira>();

            periodo.AddRange(buscarPeriodoFichaFinanceira());

            FileHelperEngine engine = new FileHelperEngine(typeof(PeriodoFichaFinanceira), Encoding.UTF8);

            engine.WriteFile(_filename, periodo);
        }

        private List<PeriodoFichaFinanceira> buscarPeriodoFichaFinanceira()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryPeriodoFichaFinanceira.Replace("{schemaName}", dbName));

            IDataReader drPeriodoFerias = database.ExecuteReader(command);

            List<PeriodoFichaFinanceira> lperiodo = new List<PeriodoFichaFinanceira>();

            while (drPeriodoFerias.Read())
            {
                PeriodoFichaFinanceira periodo = new PeriodoFichaFinanceira();

                periodo.Chapa = drPeriodoFerias["Chapa"].ToString();
                periodo.AnoCompetencia = drPeriodoFerias["AnoCompetencia"].ToString();
                periodo.MesCompetencia = drPeriodoFerias["MesCompetencia"].ToString();
                periodo.NumeroPeriodo = drPeriodoFerias["NumeroPeriodo"].ToString();
                periodo.MesCaixaComum = drPeriodoFerias["MesCaixaComum"].ToString();
                periodo.BaseINSS = drPeriodoFerias["BaseINSS"].ToString();
                periodo.BaseINSSdoOutroEmprego = drPeriodoFerias["BaseINSSdoOutroEmprego"].ToString();
                periodo.INSS = drPeriodoFerias["INSS"].ToString();
                periodo.INSSdeFerias = drPeriodoFerias["INSSdeFerias"].ToString();
                periodo.INSSdeOutroEmprego = drPeriodoFerias["INSSdeOutroEmprego"].ToString();
                periodo.BaseINSSdo13 = drPeriodoFerias["BaseINSSdo13"].ToString();
                periodo.BaseINSS13doOutroEmprego = drPeriodoFerias["BaseINSS13doOutroEmprego"].ToString();
                periodo.INSSdo13 = drPeriodoFerias["INSSdo13"].ToString();
                periodo.BaseSalarioFamilia = drPeriodoFerias["BaseSalarioFamilia"].ToString();
                periodo.SalarioFamilia = drPeriodoFerias["SalarioFamilia"].ToString();
                periodo.BaseDeValeTransporte = drPeriodoFerias["BaseDeValeTransporte"].ToString();
                periodo.ValeTransporteEntregue = drPeriodoFerias["ValeTransporteEntregue"].ToString();
                periodo.ValeTransporteDescontado = drPeriodoFerias["ValeTransporteDescontado"].ToString();
                periodo.BaseIRRF = drPeriodoFerias["BaseIRRF"].ToString();
                periodo.IRRF = drPeriodoFerias["IRRF"].ToString();
                periodo.INSSCaixa = drPeriodoFerias["INSSCaixa"].ToString();
                periodo.INSSCalculadoPeloUsuario = drPeriodoFerias["INSSCalculadoPeloUsuario"].ToString();
                periodo.DedutivelEmIRRF = drPeriodoFerias["DedutivelEmIRRF"].ToString();
                periodo.BaseIRRFFerias = drPeriodoFerias["BaseIRRFFerias"].ToString();
                periodo.IRFFParticipacao = drPeriodoFerias["IRFFParticipacao"].ToString();
                periodo.IRRFFerias = drPeriodoFerias["IRRFFerias"].ToString();
                periodo.IndicativoDeINSSComCPMF = drPeriodoFerias["IndicativoDeINSSComCPMF"].ToString();
                periodo.BaseDeFGTS = drPeriodoFerias["BaseDeFGTS"].ToString();
                periodo.BaseDeFGTSDe13Salario = drPeriodoFerias["BaseDeFGTSDe13Salario"].ToString();

                periodo.BaseDeFGTSDe13Salario = drPeriodoFerias["BaseDeFGTSDe13Salario"].ToString();
                periodo.SalarioPago = drPeriodoFerias["SalarioPago"].ToString();
                periodo.DescricaoDoPeriodo = drPeriodoFerias["DescricaoDoPeriodo"].ToString();
                periodo.BaseDeIRRFde13Salario = drPeriodoFerias["BaseDeIRRFde13Salario"].ToString();
                periodo.IRRFSob13 = drPeriodoFerias["IRRFSob13"].ToString();
                periodo.INSSDeFeruasComCPMF = drPeriodoFerias["INSSDeFeruasComCPMF"].ToString();
                periodo.INSSDeDiferencaSalarial = drPeriodoFerias["INSSDeDiferencaSalarial"].ToString();
                periodo.INSSDeDiferencaSalarialDe13 = drPeriodoFerias["INSSDeDiferencaSalarialDe13"].ToString();
                periodo.INSSDeDiferencaSalarialDeFerias = drPeriodoFerias["INSSDeDiferencaSalarialDeFerias"].ToString();

                periodo.BaseFGTSDifSalarial = drPeriodoFerias["BaseFGTSDifSalarial"].ToString();
                periodo.a1 = drPeriodoFerias["a1"].ToString();
                periodo.a2 = drPeriodoFerias["a2"].ToString();
                periodo.a3 = drPeriodoFerias["a3"].ToString();
                periodo.a4 = drPeriodoFerias["a4"].ToString();
                periodo.a5 = drPeriodoFerias["a5"].ToString();
                periodo.a6 = drPeriodoFerias["a6"].ToString();

                lperiodo.Add(periodo);
            }

            return lperiodo;
        }
    }
}
