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

namespace Exportador.RH.Funcao
{
    public class ExportadorFuncao: IExportador
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

        private string _queryFuncoes = @"
                                        select 
	                                        codcar as Codigo
	                                        ,titred as Nome
	                                        ,titcar as Descricao
	                                        ,codcbo as CBO
	                                        ,case when siscar=0 then null else siscar end as CodCargo
	                                        ,case when codcb2 IN ('0','1','') then null else codcb2 end as CBO2002
                                        from {schemaName}.r024car
                                        ";
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarFuncoes();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorFuncao()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorFuncao(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorFuncao(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarFuncoes()
        {
            List<Funcao> funcoes = new List<Funcao>();

            funcoes.AddRange(buscarFuncoes());

            FileHelperEngine engine = new FileHelperEngine(typeof(Funcao), Encoding.UTF8);

            engine.WriteFile(_filename, funcoes);
        }

        private List<Funcao> buscarFuncoes()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConnectionStringHelper.GetDatabaseName(database.ConnectionString);

            DbCommand command = database.GetSqlStringCommand(_queryFuncoes.Replace("{schemaName}", dbName));

            IDataReader drFuncoes = database.ExecuteReader(command);

            List<Funcao> lFuncoes = new List<Funcao>();

            while (drFuncoes.Read())
            {
                Funcao funcao = new Funcao();

                funcao.Codigo = drFuncoes["Codigo"].ToString();
                funcao.Nome = drFuncoes["Nome"].ToString().RemoveSpecialChars();
                funcao.Descricao = "/@"+drFuncoes["Descricao"].ToString().RemoveSpecialChars()+"@/";
                funcao.CBO = drFuncoes["CBO"].ToString();
                funcao.CodCargo = drFuncoes["CodCargo"].ToString();
                funcao.CBO2002 = drFuncoes["CBO2002"].ToString();

                lFuncoes.Add(funcao);
            }

            return lFuncoes;
        }
    }
}
