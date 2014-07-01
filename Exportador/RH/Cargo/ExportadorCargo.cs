using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Data.Common;
using System.Reflection;
using Exportador.RH.Funcionario;
using Exportador.Interface;
using Exportador.Helpers;
using FileHelpers;
using System.Configuration;

namespace Exportador.RH.Cargo
{
    public class ExportadorCargo: IExportador
    {

        #region Fields

        private int _pageSize;
        private int _recordsToReturn;
        private  string _filename;
        private BackgroundWorker _bgWorker;
        private bool _debugMode;

        #endregion

        #region Queries

        private string _queryCargos = "SELECT siscar as Codigo,dessis as Nome FROM {schemaName}.r024sis";

        #endregion

        #region Properties

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

        #region Interface Methods

        public void ValidarCamposObrigatorios()
        {
            throw new NotImplementedException();
        }

        public void Exportar()
        {

                List<Cargo> listaItensArquivo = ExportarCargos();

                FileHelperEngine engine = new FileHelperEngine(typeof(Cargo));

                //engine.BeforeWriteRecord += new BeforeWriteRecordHandler(BeforeWriteEvent);

                engine.WriteFile(_filename, listaItensArquivo);

        }

        private List<Cargo> ExportarCargos()
        {
            List<Cargo> listaItensArquivo = new List<Cargo>();
            DataTable dtCargo = BuscarCargos();
            foreach (DataRow dr in dtCargo.Rows)
            {
                Cargo itemArquivo = new Cargo();

                // codcar, titred, titcar
                itemArquivo.Codigo = dr["Codigo"].ToString();
                itemArquivo.Nome = dr["Nome"].ToString().RemoveSpecialChars();
                itemArquivo.Descricao = "/@" + dr["Nome"].ToString().RemoveSpecialChars() + "@/";
                
                listaItensArquivo.Add(itemArquivo);
            }

            return listaItensArquivo;
        }

        private DataTable BuscarCargos()
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

            string dbName = ConfigurationManager.AppSettings["SchemaName"];

            DbCommand command = database.GetSqlStringCommand(_queryCargos.Replace("{schemaName}", dbName));

            return database.ExecuteDataSet(command).Tables[0];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        public ExportadorCargo()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorCargo(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorCargo(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        private void BeforeWriteEvent(EngineBase engine, BeforeWriteRecordEventArgs e)
        {
            Cargo cargo = (Cargo)e.Record;

            Type type = typeof(Cargo);

            foreach (FieldInfo field in type.GetFields())
            {
                string name = field.Name;

                string objValue = field.GetValue(e.Record).ToString();

                object[] attributes = field.GetCustomAttributes(false);

                FieldFixedLengthAttribute attribute = (FieldFixedLengthAttribute)attributes.FirstOrDefault(item => item is FieldFixedLengthAttribute);

                if (attribute != null)
                {
                    if (field.Name == "Descricao")
                    {
                        objValue = string.Format("/@{0}{1}@/{2}", objValue, (new string(' ', attribute.Length - objValue.Length - 5)),";");
                    }
                    else
                    {
                        objValue = objValue + (new string(' ', attribute.Length - objValue.Length - 1)) + ";";
                    }
                }

                field.SetValue(cargo, objValue);
            }
	    }
    }
}
