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
    public class ExportadorNumeroDependentes : IExportador
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

        private string _queryNumeroDependentes = @"select 
                                                chapa.Chapa as Chapa
                                                , REPLACE(CONVERT(char,funcionario.datadm,103),'/','') as 'Datadm'
                                                ,SUM(case when FLOOR((CAST (GetDate() AS INTEGER) - CAST(dependente.datnas AS INTEGER)) / 365.25) < 14 then 1 else 0 end) 
                                                AS IncideSalFamilia
                                                ,SUM(case when FLOOR((CAST (GetDate() AS INTEGER) - CAST(dependente.datnas AS INTEGER)) / 365.25) < 21 then 1 else 0 end) 
                                                AS IncideIRRF
                                                from vetorh.r036dep as dependente
                                                inner join dbo.vw_totvs_chapafuncionario chapa on dependente.numcad = chapa.numcad
												                                                and dependente.tipcol = chapa.tipcol 
                                                left join vetorh.r034fun as funcionario on 
                                                dependente.numemp = funcionario.numemp
                                                and dependente.tipcol = funcionario.tipcol
                                                and dependente.numcad = funcionario.numcad
where chapa.numcpf <> '0'    
                                                GROUP BY chapa.Chapa, funcionario.datadm";
        #endregion

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {
            ExportarNumeroDependentes();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorNumeroDependentes()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorNumeroDependentes(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorNumeroDependentes(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void ExportarNumeroDependentes()
        {
            List<NumeroDependentes> aquisicao = new List<NumeroDependentes>();

            aquisicao.AddRange(buscarNumeroDependentes());

            FileHelperEngine engine = new FileHelperEngine(typeof(NumeroDependentes), Encoding.UTF8);

            engine.WriteFile(_filename, aquisicao);
        }

        private List<NumeroDependentes> buscarNumeroDependentes()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryNumeroDependentes.Replace("{schemaName}", dbName));

            IDataReader drNumeroDependentes = database.ExecuteReader(command);

            List<NumeroDependentes> ldependentes = new List<NumeroDependentes>();

            while (drNumeroDependentes.Read())
            {
                NumeroDependentes numeroDependentes = new NumeroDependentes();

                numeroDependentes.Chapa = drNumeroDependentes["Chapa"].ToString();
                numeroDependentes.Dataadm = drNumeroDependentes["Datadm"].ToString();
                numeroDependentes.IncideSalFamilia = drNumeroDependentes["IncideSalFamilia"].ToString();
                numeroDependentes.IncideIRRF = drNumeroDependentes["IncideIRRF"].ToString();

                ldependentes.Add(numeroDependentes);
            }

            return ldependentes;
        }
    }
}
