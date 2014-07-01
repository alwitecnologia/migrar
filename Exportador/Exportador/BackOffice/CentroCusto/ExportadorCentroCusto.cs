using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Data.Common;
using Exportador.Interface;
using FileHelpers;
using System.Reflection;
using Exportador;

namespace Exportador.BackOffice.CentroCusto
{
    public class ExportadorCentroCusto: IExportador
    {
        #region Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _recordsToReturn;
        private int _pageSize;
        private bool _debugMode;

        private string _queryCentroCustoTodos = @"select 1 as codColigada
            ,codccu as CodReduzido
            ,desccu as nomeCentro
            ,claccu as CodCentroCusto
            ,mskccu as mascaraCodigo
            from e044ccu";
        
        private string _contadorCentroCusto = "select count(*) from e044ccu";

        private string _queryCentroCustoPaginado = @"
        with centroCusto as (
            select ROW_NUMBER() OVER ( ORDER BY codccu ) AS RowNum,
            1 as codColigada
            ,codccu as CodReduzido
            ,desccu as nomeCentro
            ,claccu as CodCentroCusto
            ,mskccu as mascaraCodigo
            from e044ccu
	        ) 
        select * from centroCusto
        WHERE RowNum >= @minimo AND RowNum <= @maximo ORDER BY RowNum";

        #endregion

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

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorCentroCusto()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorCentroCusto(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorCentroCusto(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
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
                List<CentroCusto> centros = ExportarCentroCusto();

                FileHelperEngine engine = new FileHelperEngine(typeof(CentroCusto));

                //engine.BeforeWriteRecord += new BeforeWriteRecordHandler(BeforeWriteEvent);

                engine.WriteFile(_filename, centros);
                
            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, string.Format("Erro! {0} {1} {2}",e.Message,System.Environment.NewLine,e.StackTrace));
            }
        }

        #endregion

        private List<CentroCusto> ExportarCentroCusto()
        {
            List<CentroCusto> listaItensArquivo = new List<CentroCusto>();

            if (_bgWorker != null)
                _bgWorker.ReportProgress(0, "Contando registros de centros de custo a processar...");

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
                _bgWorker.ReportProgress(0, String.Format("{0} registros a processar.", recordsToProcess));

            if (_recordsToReturn > 0)
            {
                if (_bgWorker != null)
                    _bgWorker.ReportProgress(0, "Gerando arquivo de centros de custo...");

                int processedRecords = 0;
                while (processedRecords < recordsToProcess && processedRecords < _recordsToReturn)
                {

                    DataTable dtLancamentos = BuscarCentrosCusto(processedRecords + 1, processedRecords + (_pageSize < _recordsToReturn ? _pageSize : _recordsToReturn));

                    listaItensArquivo.AddRange(retornaCentrosCusto(dtLancamentos));

                    processedRecords = listaItensArquivo.Count;

                    double progressPercentage = (double)processedRecords / (double)recordsToProcess * 100;

                    if (_bgWorker != null)
                        _bgWorker.ReportProgress((int)progressPercentage, String.Format("{0} registros processados. {1}% do total.", processedRecords, progressPercentage));

                }
            }
            else
            {
                DataTable dtLancamentos = BuscarCentrosCusto();

                listaItensArquivo.AddRange(retornaCentrosCusto(dtLancamentos));
            }
            return listaItensArquivo;
        }

        private DataTable BuscarCentrosCusto(int minimo, int maximo)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_queryCentroCustoPaginado);

            database.AddInParameter(command, "minimo", DbType.Int32, minimo);
            database.AddInParameter(command, "maximo", DbType.Int32, maximo);

            return database.ExecuteDataSet(command).Tables[0];
        }

        private DataTable BuscarCentrosCusto()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_queryCentroCustoTodos);

            return database.ExecuteDataSet(command).Tables[0];
        }

        private List<CentroCusto> retornaCentrosCusto(DataTable dtCentros)
        {
            List<CentroCusto> centros = new List<CentroCusto>();

            foreach (DataRow row in dtCentros.Rows)
            {
                CentroCusto centro = new CentroCusto();

                /*
            1 as codColigada
            ,codccu as CodReduzido
            ,desccu as nomeCentro
            ,claccu as CodCentroCusto
            ,mskccu as mascaraCodigo
                */


                centro.CodColigada = Convert.ToInt32(row.Field<Int32>("codColigada"));
                centro.CodReduzido = row.Field<string>("CodReduzido").ToString();
                centro.Nome = row.Field<string>("nomeCentro").ToString();
                centro.CodCentroCusto = retornaCodigoComMascara(row.Field<string>("CodCentroCusto").ToString(), row.Field<string>("mascaraCodigo").ToString());

                centros.Add(centro);
            }

            return centros;
        }

        private string retornaCodigoComMascara(string codigo, string mascara)
        {
            char[] _codigo = codigo.ToCharArray();

            string codRetornar = "";

            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = (string.IsNullOrEmpty(_codigo[0].ToString()) ? "" : _codigo[0].ToString());
            else
                return codRetornar;

            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + (string.IsNullOrEmpty(_codigo[1].ToString()) ? "" : "." + _codigo[1].ToString());
            else
                return codRetornar;

            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + (string.IsNullOrEmpty(string.Join("", _codigo[2], _codigo[3])) ? "" : "." + string.Join("", _codigo[2], _codigo[3]));
            else
                return codRetornar;

            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + (string.IsNullOrEmpty(string.Join("", _codigo[4], _codigo[5], _codigo[6])) ? "" : "." + string.Join("", _codigo[4], _codigo[5], _codigo[6]));
            else
                return codRetornar;

            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + (string.IsNullOrEmpty(string.Join("", _codigo[7], _codigo[8], _codigo[9])) ? "" : "." + string.Join("", _codigo[7], _codigo[8], _codigo[9]));
            else
                return codRetornar;

            if (codRetornar.Replace(".", "").Length < codigo.Length)
                codRetornar = codRetornar + (string.IsNullOrEmpty(string.Join("", _codigo[10], _codigo[11], _codigo[12])) ? "" : "." + string.Join("", _codigo[10], _codigo[11], _codigo[12]));
            else
                return codRetornar;

            _bgWorker.ReportProgress(0,string.Format("Código inicial: {0}. Código formatado de acordo com a máscara '{1}': {2}",codigo,mascara,codRetornar));

            return codRetornar;
        }

        private int ContadorRegistros()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            DbCommand command = database.GetSqlStringCommand(_contadorCentroCusto);

            return Convert.ToInt32(database.ExecuteScalar(command));
        }

        private void BeforeWriteEvent(EngineBase engine, BeforeWriteRecordEventArgs e)
        {
            CentroCusto centro = (CentroCusto)e.Record;

            Type type = typeof(CentroCusto);

            foreach (FieldInfo field in type.GetFields())
            {
                string name = field.Name;

                string objValue = field.GetValue(e.Record).ToString();

                object[] attributes = field.GetCustomAttributes(false);

                FieldFixedLengthAttribute attribute = (FieldFixedLengthAttribute)attributes.FirstOrDefault(item => item is FieldFixedLengthAttribute);

                if (attribute != null)
                {
                    objValue = objValue + (new string(' ', attribute.Length - objValue.Length - 1)) + ";";
                }

                field.SetValue(centro, objValue);
            }
        }
    }
}
