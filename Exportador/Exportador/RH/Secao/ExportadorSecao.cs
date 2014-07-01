using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using Exportador;
using Exportador.Interface;
using Exportador.Helpers;

namespace Exportador.RH.Secao
{
    public class ExportadorSecao: IExportador
    {
        #region Private Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsToReturn;
        private bool _debugMode;

        #endregion

        #region Queries

        private string _querySecoes = @"
                                select * from (
	                                SELECT '0' AS Codigo
		                                ,'UNIPLAC' AS Descricao
		                                ,1 as CodIdentificadorFilial
		                                ,'01' as CodIdentificadorDepartamento
		                                ,'84.953.579/0001-05' as CNPJ

		                                union

	                                SELECT '01' AS Codigo
		                                ,'UNIPLAC' AS Descricao
		                                ,1 as CodIdentificadorFilial
		                                ,'01' as CodIdentificadorDepartamento
		                                ,'84.953.579/0001-05' as CNPJ

		                                union

	                                select 
		                                '01.'+arvore.codloc as Codigo
		                                ,locais.nomloc as Descricao
		                                ,1 as CodIdentificadorFilial
		                                ,'01' as CodIdentificadorDepartamento
		                                ,'84.953.579/0001-05' as CNPJ
	                                from {schemaName}.r016ore organograma
	                                inner join {schemaName}.r016hie arvore on arvore.taborg=organograma.taborg
	                                inner join {schemaName}.r016orn locais on locais.taborg=arvore.taborg
		                                and locais.numloc=arvore.numloc
	                                where organograma.taborg=5
                                        and arvore.datini<=GETDATE()
                                        and arvore.datfim>=GETDATE()
                                ) as secoes
                                order by Codigo
                                    ";

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
        public ExportadorSecao()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorSecao(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorSecao(string filename, BackgroundWorker bgWorker)
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
            ExportarSecoes();
            /*
            try
            {
                
            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, e.StackTrace);
            }
             * */
        }

        private void ExportarSecoes()
        {
            List<Secao> secoes = new List<Secao>();

            secoes.AddRange(buscarSecoes());

            FileHelperEngine engine = new FileHelperEngine(typeof(Secao), Encoding.Default);

            //engine.BeforeWriteRecord += new BeforeWriteRecordHandler(BeforeWriteEvent);

            engine.WriteFile(_filename, secoes);
        }

        private List<Secao> buscarSecoes()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_querySecoes.Replace("{schemaName}",dbName));

            IDataReader drSecoes = database.ExecuteReader(command);

            List<Secao> lSecoes = new List<Secao>();

            while (drSecoes.Read())
            {
                Secao secao = new Secao();

                secao.Codigo = drSecoes["Codigo"].ToString();
                secao.Descricao = drSecoes["Descricao"].ToString().RemoveSpecialChars();
                secao.CodIdentificadorFilial = Convert.ToInt32(drSecoes["CodIdentificadorFilial"]);
                secao.CodIdentificadorDepartamento = drSecoes["CodIdentificadorDepartamento"].ToString();
                secao.CNPJ = drSecoes["CNPJ"].ToString();

                lSecoes.Add(secao);
            }
            
            return lSecoes;
        }

        #endregion

        private void BeforeWriteEvent(EngineBase engine, BeforeWriteRecordEventArgs e)
        {
            Secao secao = (Secao)e.Record;

            Type type = typeof(Secao);

            foreach (FieldInfo field in type.GetFields())
            {
                string name = field.Name;

                if (field.GetValue(e.Record) != null)
                {
                    string objValue = field.GetValue(e.Record).ToString();

                    object[] attributes = field.GetCustomAttributes(false);

                    FieldFixedLengthAttribute attribute = (FieldFixedLengthAttribute)attributes.FirstOrDefault(item => item is FieldFixedLengthAttribute);

                    if (attribute != null)
                    {
                        objValue = objValue + (new string(' ', attribute.Length - objValue.Length - 1)) + ";";
                    }

                    field.SetValue(e.Record, objValue);
                }
            }
        }
    }
}
