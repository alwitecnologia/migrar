using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Data.Common;
using System.Data;
using Exportador;
using Exportador.Interface;

namespace Exportador.BackOffice.PlanoContas
{
    public class ExportadorPlanoContas: IExportador
    {
        private BackgroundWorker _bgWorker;
        private string _filename;
        private int _recordsToReturn;
        private int _pageSize;
        private bool _debugMode;

        private string _contadorPlanoContas = @"
            select count(*)  
            from e045pla plano
            inner join e070emp empresa on empresa.codemp=plano.codemp
            where empresa.sigemp='UNIPLAC'";

        private string _queryPlanoContas = @"
            select 
                plano.clacta as codContabil
                ,plano.ctared as codReduzido
                ,plano.descta
                ,plano.anasin
                ,plano.natcta as devCred
                ,'C' as contabil
                ,case when coalesce(rateios.nRateios,0)>0 then 'S' else 'N' end as distrGer
                ,*
            from e045pla as plano
            left join (
                        select codemp,ctared,COUNT(*) nRateios
                        from e045rat
                        group by codemp,ctared
                        ) as rateios on rateios.codemp=plano.codemp
	                        and rateios.ctared=plano.ctared
            inner join e070emp empresa on empresa.codemp=plano.codemp
            where empresa.sigemp='UNIPLAC'
            order by plano.nivcta,plano.clacta
        ";

        private string _queryPlanoContasPaginado = @"
        with planoContas as (
            select ROW_NUMBER() OVER ( ORDER BY clacta ) AS RowNum
	            ,plano.clacta as codContabil
	            ,plano.ctared as codReduzido
	            ,plano.descta
	            ,plano.anasin
	            ,plano.natcta as devCred
	            ,'C' as contabil
	            ,case when coalesce(rateios.nRateios,0)>0 then 'S' else 'N' end as distrGer
            from e045pla as plano
            left join (
			            select codemp,ctared,COUNT(*) nRateios
			            from e045rat
			            group by codemp,ctared
			            ) as rateios on rateios.codemp=plano.codemp
				            and rateios.ctared=plano.ctared
            inner join e070emp empresa on empresa.codemp=plano.codemp
            where empresa.sigemp='UNIPLAC'
            order by plano.nivcta,plano.clacta
        )
        select * from planoContas
        WHERE RowNum >= @minimo AND RowNum <= @maximo 
        ORDER BY RowNum";
        

        #region Properties

        /// <summary>
        /// Número de registros a ser retornado por iteração.
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public int RecordsCount
        {
            get { return _recordsToReturn; }
            set { _recordsToReturn = value; }
        }

        public bool DebugMode
        {
            get { return _debugMode; }
            set { _debugMode = value; }
        }

        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            try
            {
                List<PlanoContas> planoContas = ExportarPlanoContas();

                FileHelperEngine engine = new FileHelperEngine(typeof(PlanoContas));

                engine.WriteFile(_filename, planoContas);

            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, string.Format("Erro! {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace));
            }
        }

        private List<PlanoContas> ExportarPlanoContas()
        {
            List<PlanoContas> listaItensArquivo = new List<PlanoContas>();

            if (_bgWorker != null)
                _bgWorker.ReportProgress(0, "Contando registros do plano de contas a processar...");

            int recordsToProcess;

            if (_recordsToReturn == 0)
            {
                recordsToProcess = ContadorRegistros();
            }
            else
            {
                recordsToProcess = _recordsToReturn;
            }

            if (_bgWorker != null)
                _bgWorker.ReportProgress(0, String.Format("{0} registros a processar...", recordsToProcess));

            if (_recordsToReturn > 0)
            {
                if (_bgWorker != null)
                    _bgWorker.ReportProgress(0, "Gerando arquivo do plano de contas...");

                int processedRecords = 0;
                while (processedRecords < recordsToProcess && processedRecords < _recordsToReturn)
                {

                    DataTable dtPlanoContas = BuscarPlanoContas(processedRecords + 1, processedRecords + (_pageSize < _recordsToReturn ? _pageSize : _recordsToReturn));

                    listaItensArquivo.AddRange(retornaPlanoContas(dtPlanoContas));

                    processedRecords = listaItensArquivo.Count;

                    double progressPercentage = (double)processedRecords / (double)recordsToProcess * 100;

                    if (_bgWorker != null)
                        _bgWorker.ReportProgress((int)progressPercentage, String.Format("{0} registros processados. {1}% do total.", processedRecords, progressPercentage));

                }
            }
            else
            {
                DataTable dtPlanoContas = BuscarPlanoContas();

                listaItensArquivo.AddRange(retornaPlanoContas(dtPlanoContas));
            }
            return listaItensArquivo;
        }

        private DataTable BuscarPlanoContas()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_queryPlanoContas);

            return database.ExecuteDataSet(command).Tables[0];
        }

        private List<PlanoContas> retornaPlanoContas(DataTable dtPlanoContas)
        {
            List<PlanoContas> plano = new List<PlanoContas>();

            foreach (DataRow row in dtPlanoContas.Rows)
            {
                PlanoContas conta = new PlanoContas();

                conta.CodContabil = retornaCodigoComMascara(row.Field<string>("codContabil").ToString());
                conta.CodReduzido = row.Field<Int32>("codReduzido").ToString();
                conta.Descricao = row.Field<string>("descta").ToString();
                conta.AnaSin = row.Field<string>("anasin").ToString();
                conta.DevCred = row.Field<string>("devCred").ToString();
                conta.Contabil = row.Field<string>("contabil").ToString();
                conta.DistrGer = row.Field<string>("distrGer").ToString();

                plano.Add(conta);
            }

            return plano;
        }

        private string retornaCodigoComMascara(string codigo)
        {
            char[] _codigo = codigo.ToCharArray();

            //1.2.33.44.55.6666

            string codRetornar = "";

            //1
            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = _codigo[0].ToString();
            else
                return codRetornar;

            //2
            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + "." + _codigo[1].ToString();
            else
                return codRetornar;

            //33
            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + "." + string.Join("", _codigo[2], _codigo[3]);
            else
                return codRetornar;

            //44
            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + "." + string.Join("", _codigo[4], _codigo[5]);
            else
                return codRetornar;

            //55
            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + "." + string.Join("", _codigo[6], _codigo[7]);
            else
                return codRetornar;

            //6666
            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + "." + string.Join("", _codigo[8], _codigo[9], _codigo[10], _codigo[11]);
            else
                return codRetornar;

            if (codRetornar.Replace(".", "").Length < codigo.Length)
                throw new BusinessException(string.Format("Código não mapeado: {0}", codigo));

            _bgWorker.ReportProgress(0, string.Format("Código inicial: {0}. Código formatado: : {1}", codigo, codRetornar));

            return codRetornar;
        }

        private DataTable BuscarPlanoContas(int minimo, int maximo)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_queryPlanoContasPaginado);

            database.AddInParameter(command, "minimo", DbType.Int32, minimo);
            database.AddInParameter(command, "maximo", DbType.Int32, maximo);

            return database.ExecuteDataSet(command).Tables[0];
        }

        private int ContadorRegistros()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_contadorPlanoContas);

            return Convert.ToInt32(database.ExecuteScalar(command));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorPlanoContas()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorPlanoContas(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorPlanoContas(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

    }
}
