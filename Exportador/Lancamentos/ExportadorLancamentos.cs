using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Data.Common;
using Exportador.BackOffice.ClienteFornecedor;
using Exportador;
using Exportador.Lancamentos;
using System.ComponentModel;
using FileHelpers.DataLink;
using System.Configuration;
using Exportador;
using Exportador.Interface;
using FileHelpers;

namespace Exportador.Lancamentos
{
    public enum ModeloExportacao
    { 
        Lancamentos
        ,Parametros
    }

    public enum TipoConta 
    { 
        AReceber = 1
        ,APagar = 2
    }

    public enum TabelaPagar 
    {
        codemp=1,   codfil=2,   numtit=3,   codtpt=4,   seqmov=5,   codtns=6,   datmov=7,   obsmcp=8,   numdoc=9,   vctpro=10,
        projrs=11,  vlrabe=12,  codfrj=13,  datpgt=14,  codfpg=15,  diaatr=16,  diajrs=17,  datlib=18,  vlrmov=19,  vlrdsc=20,
        vlrode=21,  vlrjrs=22,  vlrmul=23,  vlrenc=24,  vlrcor=25,  vlroac=26,  vlrpis=27,  vlrbpr=28,  vlrcof=29,  vlrbcr=30, 
        vlrpit=31,  vlrbpt=32,  vlropt=33,  vlrcrt=34,  vlrbct=35,  vlroct=36,  vlrcsl=37,  vlrbcl=38,  vlrocl=39,  vlrour=40,
        vlrbor=41,  vlroor=42,  vlrliq=43,  perjrs=44,  ultpgt=45,  cjmant=46,  jrscal=47,  codpor=48,  codcrt=49,  porant=50,
        crtant=51,  numprj=52,  codfpj=53,  ctafin=54,  ctared=55,  codccu=56,  empcco=57,  numcco=58,  datcco=59,  seqcco=60,
        filrlc=61,  numrlc=62,  tptrlc=63,  forrlc=64,  seqmcp=65,  indvcr=66,  lctfin=67,  lotbai=68,  numlot=69,  usuger=70,
        datger=71,  horger=72,  indexp=73,  filfix=74,  numfix=75,  intimp=76,  vlrirf=77,  titban=78,  sittit=79,  codmoe=80,
        datppt=81,  datemi=82,  codbar=83
    }

    public enum TabelaReceber 
    {
        codemp=1         ,codfil=2        ,numtit=3        ,codtpt=4        ,seqmov=5    ,codtns=6    ,datmov=7    ,obsmcr=8        ,numdoc=9        ,vctpro=10
        ,projrs=11       ,vlrabe=12       ,codfrj=13       ,cotfrj=14       ,datpgt=15   ,codfpg=16   ,cotmcr=17   ,diaatr=18       ,diajrs=19       ,recjoa=20
        ,recjod=21       ,datlib=22       ,vlrmov=23       ,vlrdsc=24       ,vlrode=25   ,vlrjrs=26   ,vlrmul=27   ,vlrenc=28       ,vlrcor=29       ,vlroac=30
        ,vlrpis=31       ,vlrbpr=32       ,vlrcof=33       ,vlrbcr=34       ,vlrpit=35   ,vlrbpt=36   ,vlropt=37   ,vlrcrt=38       ,vlrbct=39       ,vlroct=40
        ,vlrcsl=41       ,vlrbcl=42       ,vlrocl=43       ,vlrour=44       ,vlrbor=45   ,vlroor=46   ,vlrliq=47   ,vlrbco=48       ,vlrcom=49       ,perjrs=50
        ,ultpgt=51       ,cjmant=52       ,jrscal=53       ,jrspro=54       ,codpor=55   ,codcrt=56   ,porant=57   ,crtant=58       ,numprj=59       ,codfpj=60
        ,ctafin=61       ,ctared=62       ,codccu=63       ,empcco=64       ,numcco=65   ,datcco=66   ,seqcco=67   ,filrlc=68       ,numrlc=69       ,tptrlc=70
        ,forrlc=71       ,seqrlc=72       ,seqmcp=73       ,indvcr=74       ,lctfin=75   ,tipcof=76   ,lotbai=77   ,lotbfi=78       ,numlot=79       ,usuger=80
        ,datger=81       ,horger=82       ,indexp=83       ,filfix=84       ,numfix=85   ,recmoa=86   ,intimp=87   ,usu_codeve=88   ,usu_ctafin=89   ,usu_ctactb=90
        ,usu_ctaccu=91   ,usu_nroche=92   ,usu_datapr=93   ,usu_opesic=94   ,intdif=95   ,vlrirf=96   ,vlrbir=97   ,vlroir=98       ,titban=99       ,sittit=100
        ,codmoe=101      ,datppt=102      ,datemi=103      ,codcli=104      ,codbar=105
    }

    public class ExportadorLancamentos: IExportador
    {
        #region Queries

        private string _queryContadorRegistrosContasAReceber = @"
        SELECT COUNT(*)
        FROM [VW_TOTVS_LancamentosAReceber]
        ";

        private string _queryLancamentosAReceberPaginado =
        @"
            exec [SP_TOTVS_LancamentosAReceber] {0},{1}
        ";
//        @"
//        with aReceber as (
//	        SELECT ROW_NUMBER() OVER ( ORDER BY datmov ) AS RowNum
//	            ,[codemp]       ,[codfil]       ,[numtit]       ,[codtpt]       ,[seqmov]   ,[codtns]   ,[datmov]   ,[obsmcr]       ,[numdoc]       ,[vctpro]
//                ,[projrs]       ,[vlrabe]       ,[codfrj]       ,[cotfrj]       ,[datpgt]   ,[codfpg]   ,[cotmcr]   ,[diaatr]       ,[diajrs]       ,[recjoa]
//                ,[recjod]       ,[datlib]       ,[vlrmov]       ,[vlrdsc]       ,[vlrode]   ,[vlrjrs]   ,[vlrmul]   ,[vlrenc]       ,[vlrcor]       ,[vlroac]
//                ,[vlrpis]       ,[vlrbpr]       ,[vlrcof]       ,[vlrbcr]       ,[vlrpit]   ,[vlrbpt]   ,[vlropt]   ,[vlrcrt]       ,[vlrbct]       ,[vlroct]
//                ,[vlrcsl]       ,[vlrbcl]       ,[vlrocl]       ,[vlrour]       ,[vlrbor]   ,[vlroor]   ,[vlrliq]   ,[vlrbco]       ,[vlrcom]       ,[perjrs]
//                ,[ultpgt]       ,[cjmant]       ,[jrscal]       ,[jrspro]       ,[codpor]   ,[codcrt]   ,[porant]   ,[crtant]       ,[numprj]       ,[codfpj]
//                ,[ctafin]       ,[ctared]       ,[codccu]       ,[empcco]       ,[numcco]   ,[datcco]   ,[seqcco]   ,[filrlc]       ,[numrlc]       ,[tptrlc]
//                ,[forrlc]       ,[seqrlc]       ,[seqmcp]       ,[indvcr]       ,[lctfin]   ,[tipcof]   ,[lotbai]   ,[lotbfi]       ,[numlot]       ,[usuger]
//                ,[datger]       ,[horger]       ,[indexp]       ,[filfix]       ,[numfix]   ,[recmoa]   ,[intimp]   ,[usu_codeve]   ,[usu_ctafin]   ,[usu_ctactb]
//                ,[usu_ctaccu]   ,[usu_nroche]   ,[usu_datapr]   ,[usu_opesic]   ,[intdif]   ,[vlrirf]   ,[vlrbir]   ,[vlroir]       ,[titban]       ,[sittit]
//                ,[codmoe]       ,[datppt]       ,[datemi]       ,[codcli]       ,[codbar]
//            FROM [dbo].[VW_TOTVS_LancamentosAReceber]
//
//	        ) 
//        select * from aReceber	
//        WHERE RowNum >= {0} AND RowNum <= {1} ORDER BY RowNum";

        private string _queryLancamentosAReceberTotal = @" 
        with aReceber as (
	        SELECT ROW_NUMBER() OVER ( ORDER BY datmov ) AS RowNum
	        ,* FROM [VW_TOTVS_LancamentosAReceber]
	        ) 
        select * from aReceber";

        private string _queryContadorRegistrosContasAPagar = @"
        SELECT COUNT(*)
        FROM [VW_TOTVS_LancamentosAPagar]
        ";

        private string _queryLancamentosAPagarPaginado = @"
        with aPagar as (
	        SELECT ROW_NUMBER() OVER ( ORDER BY datmov ) AS RowNum
                ,[codemp],[codfil],[numtit],[codtpt],[seqmov],[codtns],[datmov],[obsmcp],[numdoc],[vctpro]
                ,[projrs],[vlrabe],[codfrj],[datpgt],[codfpg],[diaatr],[diajrs],[datlib],[vlrmov],[vlrdsc]
                ,[vlrode],[vlrjrs],[vlrmul],[vlrenc],[vlrcor],[vlroac],[vlrpis],[vlrbpr],[vlrcof],[vlrbcr]
                ,[vlrpit],[vlrbpt],[vlropt],[vlrcrt],[vlrbct],[vlroct],[vlrcsl],[vlrbcl],[vlrocl],[vlrour]
                ,[vlrbor],[vlroor],[vlrliq],[perjrs],[ultpgt],[cjmant],[jrscal],[codpor],[codcrt],[porant]
                ,[crtant],[numprj],[codfpj],[ctafin],[ctared],[codccu],[empcco],[numcco],[datcco],[seqcco]
                ,[filrlc],[numrlc],[tptrlc],[forrlc],[seqmcp],[indvcr],[lctfin],[lotbai],[numlot],[usuger]
                ,[datger],[horger],[indexp],[filfix],[numfix],[intimp],[vlrirf],[titban],[sittit],[codmoe]
                ,[datppt],[datemi],[codbar]
            FROM [dbo].[VW_TOTVS_LancamentosAPagar]
	        ) 
        select * from aPagar	
        WHERE RowNum >= {0} AND RowNum <= {1} ORDER BY RowNum";

        private string _queryLancamentosAPagarTotal = @" 
        with aPagar as (
	        SELECT ROW_NUMBER() OVER ( ORDER BY datmov ) AS RowNum
                ,[codemp]      ,[codfil]      ,[numtit]      ,[codtpt]
                ,[seqmov]      ,[codtns]      ,[datmov]      ,[obsmcp]
                ,[numdoc]      ,[vctpro]      ,[projrs]      ,[vlrabe]
                ,[codfrj]      ,[datpgt]      ,[codfpg]      ,[diaatr]
                ,[diajrs]      ,[datlib]      ,[vlrmov]      ,[vlrdsc]
                ,[vlrode]      ,[vlrjrs]      ,[vlrmul]      ,[vlrenc]
                ,[vlrcor]      ,[vlroac]      ,[vlrpis]      ,[vlrbpr]
                ,[vlrcof]      ,[vlrbcr]      ,[vlrpit]      ,[vlrbpt]
                ,[vlropt]      ,[vlrcrt]      ,[vlrbct]      ,[vlroct]
                ,[vlrcsl]      ,[vlrbcl]      ,[vlrocl]      ,[vlrour]
                ,[vlrbor]      ,[vlroor]      ,[vlrliq]      ,[perjrs]
                ,[ultpgt]      ,[cjmant]      ,[jrscal]      ,[codpor]
                ,[codcrt]      ,[porant]      ,[crtant]      ,[numprj]
                ,[codfpj]      ,[ctafin]      ,[ctared]      ,[codccu]
                ,[empcco]      ,[numcco]      ,[datcco]      ,[seqcco]
                ,[filrlc]      ,[numrlc]      ,[tptrlc]      ,[forrlc]
                ,[seqmcp]      ,[indvcr]      ,[lctfin]      ,[lotbai]
                ,[numlot]      ,[usuger]      ,[datger]      ,[horger]
                ,[indexp]      ,[filfix]      ,[numfix]      ,[intimp]
                ,[vlrirf]      ,[titban]      ,[sittit]      ,[codmoe]
                ,[datppt]      ,[datemi]      ,[codbar]
            FROM [dbo].[VW_TOTVS_LancamentosAPagar]
	        ) 
        select * from aPagar";


        private string _queryContaContabil = @"
        select * from e045pla
        where ctared={0}
        ";
            
        #endregion

        #region Private Fields
        
        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool _debugMode;

        #endregion

        #region Public Properties

        /// <summary>
        /// Número de registros totais a ser retornados. 
        /// 0 para todos.
        /// </summary>
        public int RecordsCount
        {
            get { return _recordsToReturn; }
            set { _recordsToReturn = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
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
        public ExportadorLancamentos()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorLancamentos(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorLancamentos(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Interface Methods

        public void Exportar()
        {
            try
            {
                ExportarLancamentos();
            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, e.StackTrace);
            }
        }

        public void ValidarCamposObrigatorios()
        {
            
        }
        #endregion

        /// <summary>
        /// Busca o contador de registros de lançamentos a receber.
        /// </summary>
        /// <returns></returns>
        private int ContadorLancamentosAReceber()
        {
            //Database database = ApplicationSingleton.Instance.Container.Resolve<Database>();
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_queryContadorRegistrosContasAReceber);

            int count = Convert.ToInt32(database.ExecuteScalar(command));

            return count;
        }

        /// <summary>
        /// Busca o contador de registros de lançamentos a pagar.
        /// </summary>
        /// <returns></returns>
        private int ContadorLancamentosAPagar()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_queryContadorRegistrosContasAPagar);

            int count = Convert.ToInt32(database.ExecuteScalar(command));

            return count;
        }

        /// <summary>
        /// Busca todos os lançamentos de contas a receber.
        /// </summary>
        /// <returns></returns>
        private IDataReader BuscarLancamentosContasAReceber()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_queryLancamentosAReceberTotal);

            return database.ExecuteReader(command);
        }

        /// <summary>
        /// Busca todos os lançamentos de contas a pagar.
        /// </summary>
        /// <returns></returns>
        private IDataReader BuscarLancamentosContasAPagar()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_queryLancamentosAPagarTotal);

            return database.ExecuteReader(command);
        }

        /// <summary>
        /// Busca os lançamentos de contas a receber, baseado na paginação.
        /// </summary>
        /// <param name="minimo">Número da linha de início da paginação.</param>
        /// <param name="maximo">Numero da linha de fim da paginação.</param>
        /// <returns></returns>
        private IDataReader BuscarLancamentosContasAReceber(int minimo, int maximo)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            //DbCommand command = database.GetSqlStringCommand(String.Format(_queryLancamentosAReceberPaginado,minimo,maximo));
            DbCommand command = database.GetStoredProcCommand("SP_TOTVS_LancamentosAReceber");
            database.AddInParameter(command, "minimo", DbType.Int32, minimo);
            database.AddInParameter(command, "maximo", DbType.Int32, maximo);

            return database.ExecuteReader(command);
        }

        /// <summary>
        /// Busca os lançamentos de contas a pagar, baseado na paginação.
        /// </summary>
        /// <param name="minimo">Número da linha de início da paginação.</param>
        /// <param name="maximo">Numero da linha de fim da paginação.</param>
        /// <returns></returns>
        private IDataReader BuscarLancamentosContasAPagar(int minimo, int maximo)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(String.Format(_queryLancamentosAPagarPaginado, minimo, maximo));

            return database.ExecuteReader(command);
        }

        private DataTable BuscarRateioLancamentoContaAReceber(string numeroTitulo)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            StringBuilder sbSql = new StringBuilder("SELECT * FROM [e301rat] ");
            sbSql.Append("WHERE numtit = '").Append(numeroTitulo).Append("'");
            sbSql.Append(" ORDER BY seqrat");

            DbCommand command = database.GetSqlStringCommand(sbSql.ToString());
            return database.ExecuteDataSet(command).Tables[0];
        }

        private List<ItemArquivoLancamentoContabilizacao> gerarContabilizacoesAReceber(List<ItemArquivoLancamento> lancamentos)
        {
            //List<ItemArquivoLancamento> aReceber = lancamentos.Where(l => l.APagarOuAReceber == (int)TipoConta.AReceber).ToList
            //aqui
            List<ItemArquivoLancamentoContabilizacao> contabilizacoes = new List<ItemArquivoLancamentoContabilizacao>();

            _bgWorker.ReportProgress(0, "Buscando contabilizações...");

            foreach (ItemArquivoLancamento lancamento in lancamentos)
            {               
                contabilizacoes.Add(retornaContabilizacao(lancamento.ContaContabil));
            }

            _bgWorker.ReportProgress(0, string.Format("{0} contabilizações retornadas!",contabilizacoes.Count));

            return contabilizacoes;
        }

        /// <summary>
        /// Retorna dados da conta contábil com base no seu código.
        /// </summary>
        /// <param name="codContaContabil"></param>
        /// <returns></returns>
        private ItemArquivoLancamentoContabilizacao retornaContabilizacao(int codContaContabil)
        {
            ItemArquivoLancamentoContabilizacao cont = new ItemArquivoLancamentoContabilizacao();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(String.Format(_queryContaContabil,codContaContabil));

            //return database.ExecuteReader(command);

            return cont;            
        }

        private List<ItemArquivoLancamento> gerarLancamentosAReceber()
        {
            List<ItemArquivoLancamento> listaItensArquivo = new List<ItemArquivoLancamento>();

            if (_bgWorker != null)
                _bgWorker.ReportProgress(0, "Contando registros de lançamentos a receber...");

            int recordsToProcess;

            if (_recordsToReturn == 0)
            {
                recordsToProcess = ContadorLancamentosAReceber();
            }
            else
            {
                recordsToProcess = _recordsToReturn;
            }

            if (_bgWorker != null)
                _bgWorker.ReportProgress(0, String.Format("{0} registros a processar.", recordsToProcess));

            if (_recordsToReturn > 0)
            {
                if (_bgWorker != null)
                    _bgWorker.ReportProgress(0,"Gerando arquivo de lançamentos a receber...");

                int processedRecords = 0;
                while (processedRecords < recordsToProcess && processedRecords < _recordsToReturn)
                {

                    IDataReader drLancamentos = BuscarLancamentosContasAReceber(processedRecords + 1, processedRecords + (_pageSize < _recordsToReturn ? _pageSize : _recordsToReturn));

                    listaItensArquivo.AddRange(retornaLancamentosAReceber(drLancamentos));

                    processedRecords = listaItensArquivo.Count;

                    double progressPercentage = (double)processedRecords / (double)recordsToProcess * 100;

                    if (_bgWorker != null)
                        _bgWorker.ReportProgress((int)progressPercentage, String.Format("{0} registros processados. {1}% do total.", processedRecords, progressPercentage));

                }
            }
            else
            {
                IDataReader drLancamentos = BuscarLancamentosContasAReceber();

                listaItensArquivo.AddRange(retornaLancamentosAReceber(drLancamentos));
            }
            return listaItensArquivo;
        }

        private List<ItemArquivoLancamento> gerarLancamentosAPagar()
        {
            List<ItemArquivoLancamento> listaItensArquivo = new List<ItemArquivoLancamento>();

            if (_bgWorker != null)
                _bgWorker.ReportProgress(0, "Contando registros de lançamentos a pagar...");

            int recordsToProcess = ContadorLancamentosAPagar();

            if (_bgWorker != null)
                _bgWorker.ReportProgress(0, String.Format("{0} registros encontrados...", recordsToProcess));

            if (_recordsToReturn > 0)
            {
                int processedRecords = 0;
                while (processedRecords < recordsToProcess && processedRecords < _recordsToReturn)
                {
                    IDataReader drLancamentos = BuscarLancamentosContasAPagar(processedRecords + 1, processedRecords + (_pageSize < _recordsToReturn ? _pageSize : _recordsToReturn));

                    listaItensArquivo.AddRange(retornaLancamentosAPagar(drLancamentos));

                    processedRecords = listaItensArquivo.Count;

                    double progressPercentage = (double)processedRecords / (double)recordsToProcess * 100;

                    if (_bgWorker != null)
                        _bgWorker.ReportProgress((int)progressPercentage, String.Format("{0} registros processados. {1}%", processedRecords, progressPercentage));

                }
            }
            else
            {
                IDataReader drLancamentos = BuscarLancamentosContasAPagar();
                listaItensArquivo.AddRange(retornaLancamentosAPagar(drLancamentos));
            }
            return listaItensArquivo;
        }

        private List<ItemArquivoLancamento> retornaLancamentosAReceber(IDataReader drLancamentos)
        {
            List<ItemArquivoLancamento> listaItensArquivo = new List<ItemArquivoLancamento>();

            while (drLancamentos.Read())
            {
                ItemArquivoLancamento itemArquivo = new ItemArquivoLancamento();
                itemArquivo.CodigoColigadaCliFor = 1;
                itemArquivo.CodigoDaFilial = (short)drLancamentos["codfil"];
                //dr.Field<short>("codfil");
                itemArquivo.CodigoDoTipoDeDocumento = drLancamentos["codtpt"].ToString();
                //dr.Field<string>("codtpt");
                itemArquivo.NumeroDoDocumento = drLancamentos["numdoc"].ToString();
                //dr.Field<string>("numdoc");
                itemArquivo.APagarOuAReceber = (int)TipoConta.AReceber;
                itemArquivo.CodigoDoCentroDeCusto = drLancamentos["codccu"].ToString();
                //dr.Field<string>("codccu");
                itemArquivo.DataDeVencimento = ((DateTime)drLancamentos["vctpro"]).ToString("ddMMyy");
                //dr.Field<DateTime>("vctpro").ToString("ddMMyy");
                itemArquivo.DataDeEmissao = ((DateTime)drLancamentos["datemi"]).ToString("ddMMyy");
                //dr.Field<DateTime>("datemi").ToString("ddMMyy");
                itemArquivo.DataDePrevisaoDeBaixa = ((DateTime)drLancamentos["datppt"]).ToString("ddMMyy");
                //dr.Field<DateTime>("datppt").ToString("ddMMyy");
                itemArquivo.CodigoDaMoeda = drLancamentos["codmoe"].ToString();
                //dr.Field<string>("codmoe");
                itemArquivo.CodigoDaContaCaixa = ((int)drLancamentos["ctafin"]).ToString();
                //dr.Field<int>("ctafin").ToString();
                itemArquivo.ValorOriginal = (decimal)drLancamentos["vlrmov"];
                //dr.Field<decimal>("vlrmov");
                itemArquivo.ValorDosJuros = (decimal)drLancamentos["vlrjrs"];
                //dr.Field<decimal>("vlrjrs");
                itemArquivo.ValorDoDesconto = (decimal)drLancamentos["vlrdsc"];
                //dr.Field<decimal>("vlrdsc");
                itemArquivo.NossoNumeroCNAB = drLancamentos["titban"].ToString();
                //dr.Field<string>("titban");
                itemArquivo.ValorDaMulta = (decimal)drLancamentos["vlrmul"];
                //dr.Field<decimal>("vlrmul");
                itemArquivo.DataDePagamento = ((DateTime)drLancamentos["datpgt"]).ToString("ddMMyy");
                //dr.Field<DateTime>("datpgt").ToString("ddMMyy");
                itemArquivo.CodigoDeBarras = drLancamentos["codbar"].ToString();
                //dr.Field<string>("codbar");
                itemArquivo.StatusDoLancamento = drLancamentos["sittit"].ToString() == "CA" ? 2 : 0;
                //dr.Field<string>("sittit") == "CA" ? 2 : 0;

                itemArquivo.TipoContabil = "0";
                itemArquivo.AceiteCNAB = "0";
                itemArquivo.Reembolsavel = "0";
                itemArquivo.TipoDeJurosDoDia = "0";
                itemArquivo.CodColigadaContaCaixa = "0001";
                itemArquivo.CodigoColigadaDoExtrato = "0001";

                //Informação não é utilizada na geração dos arquivos de lançamentos...
                itemArquivo.ContaContabil = Convert.ToInt32(drLancamentos["ctared"]);

                listaItensArquivo.Add(itemArquivo);

            }

            return listaItensArquivo;
        }

        protected void CarregarLanctoReceber(object registro, object[] campos) 
        {
            ItemArquivoLancamento lancto = (ItemArquivoLancamento)registro;

            lancto.CodigoColigadaCliFor = 1;
            lancto.APagarOuAReceber = (int)TipoConta.AReceber;
            //itemArquivo.APagarOuAReceber = (int)TipoConta.AReceber;
            
            lancto.CodigoDaFilial = Convert.ToInt32(campos[(int)TabelaReceber.codfil]);
            lancto.CodigoDoTipoDeDocumento = campos[(int)TabelaReceber.codtpt].ToString();
            //itemArquivo.CodigoDoTipoDeDocumento = drLancamentos["codtpt"].ToString();

            lancto.NumeroDoDocumento = campos[(int)TabelaReceber.numdoc].ToString();
            //itemArquivo.NumeroDoDocumento = drLancamentos["numdoc"].ToString();

            lancto.CodigoDoCentroDeCusto = campos[(int)TabelaReceber.codccu].ToString();
            //itemArquivo.CodigoDoCentroDeCusto = drLancamentos["codccu"].ToString();

            lancto.DataDeVencimento = ((DateTime)campos[(int)TabelaReceber.vctpro]).ToString("ddMMyy");
            //itemArquivo.DataDeVencimento = ((DateTime)drLancamentos["vctpro"]).ToString("ddMMyy");

            lancto.DataDeEmissao = ((DateTime)campos[(int)TabelaReceber.datemi]).ToString("ddMMyy");
            //itemArquivo.DataDeEmissao = ((DateTime)drLancamentos["datemi"]).ToString("ddMMyy");

            lancto.DataDePrevisaoDeBaixa = ((DateTime)campos[(int)TabelaReceber.datppt]).ToString("ddMMyy");
            //itemArquivo.DataDePrevisaoDeBaixa = ((DateTime)drLancamentos["datppt"]).ToString("ddMMyy");

            lancto.CodigoDaMoeda = campos[(int)TabelaReceber.codmoe].ToString();
            //itemArquivo.CodigoDaMoeda = drLancamentos["codmoe"].ToString();

            lancto.CodigoDaContaCaixa = ((int)campos[(int)TabelaReceber.ctafin]).ToString();
            //itemArquivo.CodigoDaContaCaixa = ((int)drLancamentos["ctafin"]).ToString();

            lancto.ValorOriginal = (decimal)campos[(int)TabelaReceber.vlrmov];
            //itemArquivo.ValorOriginal = (decimal)drLancamentos["vlrmov"];

            lancto.ValorDosJuros = (decimal)campos[(int)TabelaReceber.vlrjrs];
            //itemArquivo.ValorDosJuros = (decimal)drLancamentos["vlrjrs"];

            lancto.ValorDoDesconto = (decimal)campos[(int)TabelaReceber.vlrdsc];
            //itemArquivo.ValorDoDesconto = (decimal)drLancamentos["vlrdsc"];

            lancto.NossoNumeroCNAB = campos[(int)TabelaReceber.titban].ToString();
            //itemArquivo.NossoNumeroCNAB = drLancamentos["titban"].ToString();

            lancto.ValorDaMulta = (decimal)campos[(int)TabelaReceber.vlrmul];
            //itemArquivo.ValorDaMulta = (decimal)drLancamentos["vlrmul"];

            lancto.DataDePagamento = ((DateTime)campos[(int)TabelaReceber.datpgt]).ToString("ddMMyy");
            //itemArquivo.DataDePagamento = ((DateTime)drLancamentos["datpgt"]).ToString("ddMMyy");

            lancto.CodigoDeBarras = campos[(int)TabelaReceber.codbar].ToString();
            //itemArquivo.CodigoDeBarras = drLancamentos["codbar"].ToString();

            lancto.StatusDoLancamento = campos[(int)TabelaReceber.sittit].ToString() == "CA" ? 2 : 0;
            //itemArquivo.StatusDoLancamento = drLancamentos["sittit"].ToString() == "CA" ? 2 : 0;

            lancto.TipoContabil = "0";
            lancto.AceiteCNAB = "0";
            lancto.Reembolsavel = "0";
            lancto.TipoDeJurosDoDia = "0";
            lancto.CodColigadaContaCaixa = "0001";
            lancto.CodigoColigadaDoExtrato = "0001";

            ////Informação não é utilizada na geração dos arquivos de lançamentos...
            lancto.ContaContabil = Convert.ToInt32(campos[(int)TabelaReceber.ctared]);
            //itemArquivo.ContaContabil = Convert.ToInt32(drLancamentos["ctared"]);
        }

        protected void CarregarLactoPagar(object registro, object[] campos) 
        {
            ItemArquivoLancamento lancto = (ItemArquivoLancamento)registro;

            lancto.CodigoColigadaCliFor = 1;
            lancto.APagarOuAReceber = (int)TipoConta.APagar;
            lancto.TipoContabil = "0";
            lancto.AceiteCNAB = "0";
            lancto.Reembolsavel = "0";
            lancto.TipoDeJurosDoDia = "0";
            lancto.CodColigadaContaCaixa = "0001";
            lancto.CodigoColigadaDoExtrato = "0001";

            //itemArquivo.CodigoDaFilial = (short)drLancamentos["codfil"];
            

            //itemArquivo.CodigoDoTipoDeDocumento = drLancamentos["codtpt"].ToString();
            //itemArquivo.NumeroDoDocumento = drLancamentos["numdoc"].ToString();
            
            //itemArquivo.CodigoDoCentroDeCusto = drLancamentos["codccu"].ToString();
            //itemArquivo.DataDeVencimento = ((DateTime)drLancamentos["vctpro"]).ToString("ddMMyy");
            //itemArquivo.DataDeEmissao = ((DateTime)drLancamentos["datemi"]).ToString("ddMMyy");
            //itemArquivo.DataDePrevisaoDeBaixa = ((DateTime)drLancamentos["datppt"]).ToString("ddMMyy");
            //itemArquivo.CodigoDaMoeda = drLancamentos["codmoe"].ToString();
            //itemArquivo.CodigoDaContaCaixa = ((int)drLancamentos["ctafin"]).ToString();
            //itemArquivo.ValorOriginal = (decimal)drLancamentos["vlrmov"];
            //itemArquivo.ValorDosJuros = (decimal)drLancamentos["vlrjrs"];
            //itemArquivo.ValorDoDesconto = (decimal)drLancamentos["vlrdsc"];
            //itemArquivo.NossoNumeroCNAB = drLancamentos["titban"].ToString();
            //itemArquivo.ValorDaMulta = (decimal)drLancamentos["vlrmul"];
            //itemArquivo.DataDePagamento = ((DateTime)drLancamentos["datpgt"]).ToString("ddMMyy");
            //itemArquivo.CodigoDeBarras = drLancamentos["codbar"].ToString();
            //itemArquivo.StatusDoLancamento = drLancamentos["sittit"].ToString() == "CA" ? 2 : 0;



            //Informação não é utilizada na geração dos arquivos de lançamentos...
            //itemArquivo.ContaContabil = Convert.ToInt32(drLancamentos["ctared"]);
        }

        private IEnumerable<ItemArquivoLancamento> retornaLancamentosAPagar(IDataReader drLancamentos)
        {
            List<ItemArquivoLancamento> listaItensArquivo = new List<ItemArquivoLancamento>();

            while (drLancamentos.Read())
            {
                ItemArquivoLancamento itemArquivo = new ItemArquivoLancamento();
                itemArquivo.CodigoColigadaCliFor = 1;
                itemArquivo.CodigoDaFilial = (short)drLancamentos["codfil"];
                //dr.Field<short>("codfil");
                itemArquivo.CodigoDoTipoDeDocumento = drLancamentos["codtpt"].ToString();
                //dr.Field<string>("codtpt");
                itemArquivo.NumeroDoDocumento = drLancamentos["numdoc"].ToString();
                //dr.Field<string>("numdoc");
                itemArquivo.APagarOuAReceber = (int)TipoConta.APagar;

                itemArquivo.CodigoDoCentroDeCusto = drLancamentos["codccu"].ToString();
                //dr.Field<string>("codccu");
                itemArquivo.DataDeVencimento = ((DateTime)drLancamentos["vctpro"]).ToString("ddMMyy");
                //dr.Field<DateTime>("vctpro").ToString("ddMMyy");
                itemArquivo.DataDeEmissao = ((DateTime)drLancamentos["datemi"]).ToString("ddMMyy");
                //dr.Field<DateTime>("datemi").ToString("ddMMyy");
                itemArquivo.DataDePrevisaoDeBaixa = ((DateTime)drLancamentos["datppt"]).ToString("ddMMyy");
                //dr.Field<DateTime>("datppt").ToString("ddMMyy");
                itemArquivo.CodigoDaMoeda = drLancamentos["codmoe"].ToString();
                //dr.Field<string>("codmoe");
                itemArquivo.CodigoDaContaCaixa = ((int)drLancamentos["ctafin"]).ToString();
                //dr.Field<int>("ctafin").ToString();
                itemArquivo.ValorOriginal = (decimal)drLancamentos["vlrmov"];
                //dr.Field<decimal>("vlrmov");
                itemArquivo.ValorDosJuros = (decimal)drLancamentos["vlrjrs"];
                //dr.Field<decimal>("vlrjrs");
                itemArquivo.ValorDoDesconto = (decimal)drLancamentos["vlrdsc"];
                //dr.Field<decimal>("vlrdsc");
                itemArquivo.NossoNumeroCNAB = drLancamentos["titban"].ToString();
                //dr.Field<string>("titban");
                itemArquivo.ValorDaMulta = (decimal)drLancamentos["vlrmul"];
                //dr.Field<decimal>("vlrmul");
                itemArquivo.DataDePagamento = ((DateTime)drLancamentos["datpgt"]).ToString("ddMMyy");
                //dr.Field<DateTime>("datpgt").ToString("ddMMyy");
                itemArquivo.CodigoDeBarras = drLancamentos["codbar"].ToString();
                //dr.Field<string>("codbar");
                itemArquivo.StatusDoLancamento = drLancamentos["sittit"].ToString() == "CA" ? 2 : 0;
                //dr.Field<string>("sittit") == "CA" ? 2 : 0;

                itemArquivo.TipoContabil = "0";
                itemArquivo.AceiteCNAB = "0";
                itemArquivo.Reembolsavel = "0";
                itemArquivo.TipoDeJurosDoDia = "0";
                itemArquivo.CodColigadaContaCaixa = "0001";
                itemArquivo.CodigoColigadaDoExtrato = "0001";

                //Informação não é utilizada na geração dos arquivos de lançamentos...
                itemArquivo.ContaContabil = Convert.ToInt32(drLancamentos["ctared"]);

                listaItensArquivo.Add(itemArquivo);

            }

            return listaItensArquivo;
        }

        private void ValidarCamposObrigatorios(List<ItemArquivoLancamento> lancamentos)
        {
            foreach (ItemArquivoLancamento lancto in lancamentos)
            {
                if (String.IsNullOrEmpty(lancto.TipoDaLinha))
                    _bgWorker.ReportProgress(0, "TipoDaLinha não informado.");

                
            }
        }

        /// <summary>
        /// Exporta os lançamentos de contas a receber e a pagar.
        /// </summary>
        public void ExportarLancamentos()
        {
            List<ItemArquivoLancamento> lancamentos = new List<ItemArquivoLancamento>();

            lancamentos.AddRange(gerarLancamentosAReceber());

            //lancamentos.AddRange(gerarLancamentosAPagar());

            //List<ItemArquivoLancamentoContabilizacao> contabilizacoes = gerarContabilizacoesAReceber(lancamentos);


            ValidarCamposObrigatorios(lancamentos);

            //MultiRecordEngine engine = new MultiRecordEngine(
            //        new Type[]{typeof(ItemArquivoLancamento)
            //          , typeof(ItemArquivoLancamentoRateioCentroCusto)
            //          , typeof(ItemArquivoLancamentoContabilizacao)});
            //engine.WriteFile(filename, listaItensArquivo);

            FileHelperEngine engine = new FileHelperEngine(typeof(ItemArquivoLancamento),Encoding.Unicode);
            engine.WriteFile(_filename, lancamentos);

        }

        #region Deprecated methods

        /*

        private DataTable BuscarLancamentosContasAPagar()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            StringBuilder sbSql = new StringBuilder("SELECT * FROM [e501mcp] ");
            sbSql.Append("ORDER BY datmov");

            DbCommand command = database.GetSqlStringCommand(sbSql.ToString());
            return database.ExecuteDataSet(command).Tables[0];
        }

        private DataTable BuscarRateioLancamentoContaAPagar(string numeroTitulo)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            StringBuilder sbSql = new StringBuilder("SELECT * FROM [e501rat]");
            sbSql.Append("WHERE numtit = '").Append(numeroTitulo).Append("'");
            sbSql.Append(" ORDER BY sqrat");

            DbCommand command = database.GetSqlStringCommand(sbSql.ToString());
            return database.ExecuteDataSet(command).Tables[0];
        }

        */

        #endregion
    }



}
