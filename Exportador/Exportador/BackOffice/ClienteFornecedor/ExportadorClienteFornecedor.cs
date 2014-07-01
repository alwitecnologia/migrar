using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Data.Common;
using Exportador;
using Exportador.Interface;
using Exportador.Helpers;
using FileHelpers;
using Exportador.VO;
using System.IO;

namespace Exportador.BackOffice.ClienteFornecedor
{
    public class ExportadorClienteFornecedor : IExportador
    {
        #region Private Fields

        private string _filename;
        private BackgroundWorker _bgWorker;
        private int _pageSize;
        private int _recordsCount;
        private bool _debugMode;
        private List<Municipio> municipios;
        private MunicipioDAO municipioDAO = new MunicipioDAO();

        #endregion

        #region Public Properties

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public int RecordsCount
        {
            get { return _recordsCount; }
            set { _recordsCount = value; }
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
        public ExportadorClienteFornecedor()
        {

        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        public ExportadorClienteFornecedor(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Gerenciador da exportacao dos arquivos.
        /// </summary>
        /// <param name="modelo">Modelo de exportação a ser utilizado.</param>
        /// <param name="filename">Local onde os arquivos serão salvos.</param>
        /// <param name="bgWorker">Mecanismo de fundo, responsável pelo processamento no plano de fundo.</param>
        public ExportadorClienteFornecedor(string filename, BackgroundWorker bgWorker)
        {
            this._filename = filename;
            this._bgWorker = bgWorker;
        }

        #endregion

        #region Queries

        private string _retornaCodClientePorRA = "select codcli from [e085cli] where usu_coduni={0}";

        private string _buscarTodosFornecedores = @"SELECT forn.*, pai.nompai 
                                                    FROM [e095for] forn 
                                                    LEFT JOIN [e006pai] pai ON pai.codpai = forn.codpai 
                                                    WHERE forn.codfor not in (2, 3, 2141, 2988)  
                                                    ORDER BY forn.codfor";

        private string _countTodosFornecedores = @"SELECT COUNT(*) FROM (

                                                        SELECT forn.*, pai.nompai 
                                                        FROM [e095for] forn 
                                                        LEFT JOIN [e006pai] pai ON pai.codpai = forn.codpai 
                                                        WHERE forn.codfor not in (2, 3, 2141, 2988) 

                                                    ) AS CT";

        private string _buscarTodosClientes = @"SELECT cli.*, pai.nompai 
                                                FROM [e085cli] cli 
                                                LEFT JOIN [e006pai] pai ON pai.codpai = cli.codpai 
                                                WHERE cli.codcli not in (2,3) 
                                                ORDER BY cli.codcli";

        private string _countTodosClientes = @"SELECT COUNT(*) FROM (

                                                    SELECT cli.*, pai.nompai 
                                                    FROM [e085cli] cli 
                                                    LEFT JOIN [e006pai] pai ON pai.codpai = cli.codpai 
                                                    WHERE cli.codcli not in (2,3) 

                                                    ) AS CT";


        #endregion

        public void Exportar()
        {
            try
            {
                
                _bgWorker.ReportProgress(0, "Buscando municípios...");

                municipios = municipioDAO.buscarTodos();

                _bgWorker.ReportProgress(0, "Exportando fornecedores...");

                List<ClienteFornecedor> fornecedores = BuscarFornecedores();

                FileHelperEngine engine = new FileHelperEngine(typeof(ClienteFornecedor), Encoding.Unicode);

                _filename = Path.Combine(Path.GetDirectoryName(_filename), "fornecedores.txt");
                                        
                engine.WriteFile(_filename, fornecedores);

                _bgWorker.ReportProgress(0, "Exportando clientes...");

                List<ClienteFornecedor> clientes = BuscarClientes();

                _filename = Path.Combine(Path.GetDirectoryName(_filename), "clientes.txt");

                engine.WriteFile(_filename, clientes);

            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, e.Message + System.Environment.NewLine + e.StackTrace);
            }
        }

        private ClienteFornecedor ConverterFornecedores(IDataRecord drForn)
        {
            ClienteFornecedor forn = new ClienteFornecedor();

            forn.Codigo = (drForn["codfor"] == DBNull.Value) ? String.Empty : drForn["codfor"].ToString();
            forn.NomeFantasia = (drForn["apefor"] == DBNull.Value) ? String.Empty : drForn["apefor"].ToString();
            forn.Nome = (drForn["nomfor"] == DBNull.Value) ? String.Empty : drForn["nomfor"].ToString();
            forn.CNPJCPF = (drForn["cgccpf"] == DBNull.Value) ? String.Empty : drForn["cgccpf"].ToString();
            forn.IE = (drForn["insest"] == DBNull.Value) ? String.Empty : drForn["insest"].ToString();
            forn.Numero = (drForn["nenfor"] == DBNull.Value) ? String.Empty : drForn["nenfor"].ToString();
            forn.Complemento = (drForn["cplend"] == DBNull.Value) ? String.Empty : drForn["cplend"].ToString();
            forn.Estado = (drForn["sigufs"] == DBNull.Value) ? String.Empty : drForn["sigufs"].ToString();
            forn.CEP = (drForn["cepfor"] == DBNull.Value) ? String.Empty : drForn["cepfor"].ToString();
            forn.Telefone = (drForn["fonfor"] == DBNull.Value) ? String.Empty : drForn["fonfor"].ToString();
            forn.Fax = (drForn["faxfor"] == DBNull.Value) ? String.Empty : drForn["faxfor"].ToString();
            forn.Email = (drForn["intnet"] == DBNull.Value) ? String.Empty : drForn["intnet"].ToString();
            forn.Contato = (drForn["nomven"] == DBNull.Value) ? String.Empty : drForn["nomven"].ToString();
            forn.Ativo = (((drForn["sitfor"] == DBNull.Value) ? String.Empty : drForn["sitfor"].ToString()) == "A") ? 1 : 0;
            forn.InscricaoMunicipal = (drForn["insmun"] == DBNull.Value) ? String.Empty : drForn["insmun"].ToString();
            forn.PessoaFisicaOuJuridica = (drForn["tipfor"] == DBNull.Value) ? String.Empty : drForn["tipfor"].ToString();
            forn.Pais = (drForn["nompai"] == DBNull.Value) ? String.Empty : drForn["nompai"].ToString();
            forn.CarteiraDeIDentidade = (drForn["numrge"] == DBNull.Value) ? String.Empty : drForn["numrge"].ToString();
            forn.ContribuinteISS = (((drForn["triiss"] == DBNull.Value) ? String.Empty : drForn["triiss"].ToString()) == "S") ? 1 : 0;
            forn.NumeroDeDependentes = (drForn["qtddep"] == DBNull.Value) ? 0 : Convert.ToInt32(drForn["qtddep"]);
            forn.InscricaoNoSuframa = (drForn["codsuf"] == DBNull.Value) ? String.Empty : drForn["codsuf"].ToString();
            forn.ContribuinteICMS = ((bool)DBHelper.GetNullableBoolean(drForn["triicm"])) ? 1 : 0;
            forn.CaixaPostal = (drForn["cxapst"] == DBNull.Value) ? String.Empty : drForn["cxapst"].ToString();
            forn.Bairro = (drForn["baifor"] == DBNull.Value) ? String.Empty : drForn["baifor"].ToString();
            forn.Rua = (drForn["endfor"] == DBNull.Value) ? String.Empty : drForn["endfor"].ToString();

            string nomeMunicipio = (drForn["cidfor"] == DBNull.Value) ? String.Empty : drForn["cidfor"].ToString();
            string uf = (drForn["sigufs"] == DBNull.Value) ? String.Empty : drForn["sigufs"].ToString();

            var municipio = municipios.FirstOrDefault(x => ((x.CodEstado.RemoveSpecialChars().Equals(uf)) || (x.Nome.RemoveSpecialChars().Equals(nomeMunicipio))));
            forn.CodigoDoMunicipio = municipio.CodMunicipio;

            forn.Coligada = 1;
            forn.ClienteOuFornecedor = 2; //Fornecedor

            forn.OptantePeloSimples = "00000";
            if (string.IsNullOrEmpty(forn.NomeFantasia.Trim()))
                forn.NomeFantasia = forn.Nome;

            if (forn.PessoaFisicaOuJuridica == "J")
                forn.CNPJCPF = forn.CNPJCPF.PadLeft(14, '0'); //cnpj
            else
                forn.CNPJCPF = forn.CNPJCPF.PadLeft(11, '0'); //cpf

            if (forn.Estado == "." || forn.Estado.Trim() == string.Empty)
                forn.Estado = string.Empty;

            if (Convert.ToInt64(forn.CNPJCPF) == 0)
                forn.CNPJCPF = string.Empty;

            if (forn.IE.ToUpper().Equals("ISENTO") && forn.Estado == string.Empty)
                forn.IE = string.Empty;

            return forn;
        }

        private ClienteFornecedor ConverterCliente(IDataRecord drCliente)
        {
            ClienteFornecedor cliente = new ClienteFornecedor();

            cliente.Codigo = (drCliente["codcli"] == DBNull.Value) ? String.Empty : drCliente["codcli"].ToString();
            cliente.CNPJCPF = (drCliente["cgccpf"] == DBNull.Value) ? String.Empty : drCliente["cgccpf"].ToString();
            cliente.NomeFantasia = (drCliente["apecli"] == DBNull.Value) ? String.Empty : drCliente["apecli"].ToString();
            cliente.Nome = (drCliente["nomcli"] == DBNull.Value) ? String.Empty : drCliente["nomcli"].ToString();
            cliente.IE = (drCliente["insest"] == DBNull.Value) ? String.Empty : drCliente["insest"].ToString();
            cliente.Numero = (drCliente["nencli"] == DBNull.Value) ? String.Empty : drCliente["nencli"].ToString();
            cliente.Complemento = (drCliente["cplend"] == DBNull.Value) ? String.Empty : drCliente["cplend"].ToString();
            cliente.Estado = (drCliente["sigufs"] == DBNull.Value) ? String.Empty : drCliente["sigufs"].ToString();
            cliente.CEP = (drCliente["cepcli"] == DBNull.Value) ? String.Empty : drCliente["cepcli"].ToString();
            cliente.Telefone = (drCliente["foncli"] == DBNull.Value) ? String.Empty : drCliente["foncli"].ToString();
            cliente.Fax = (drCliente["faxcli"] == DBNull.Value) ? String.Empty : drCliente["faxcli"].ToString();
            cliente.Email = (drCliente["intnet"] == DBNull.Value) ? String.Empty : drCliente["intnet"].ToString();

            string sitCliente = (drCliente["sitcli"] == DBNull.Value) ? String.Empty : drCliente["sitcli"].ToString();
            cliente.Ativo = (sitCliente == "A") ? 1 : 0;

            cliente.InscricaoMunicipal = (drCliente["insmun"] == DBNull.Value) ? String.Empty : drCliente["insmun"].ToString();
            cliente.PessoaFisicaOuJuridica = (drCliente["tipcli"] == DBNull.Value) ? String.Empty : drCliente["tipcli"].ToString();
            cliente.Pais = (drCliente["nompai"] == DBNull.Value) ? String.Empty : drCliente["nompai"].ToString();
            cliente.InscricaoNoSuframa = (drCliente["codsuf"] == DBNull.Value) ? String.Empty : drCliente["codsuf"].ToString();
            
            string contribICMS = (drCliente["triicm"] == DBNull.Value) ? String.Empty : drCliente["triicm"].ToString();
            cliente.ContribuinteICMS = ((bool)DBHelper.GetNullableBoolean(contribICMS)) ? 1 : 0;

            cliente.CaixaPostal = (drCliente["cxapst"] == DBNull.Value) ? String.Empty : drCliente["cxapst"].ToString();
            cliente.Bairro = (drCliente["baicli"] == DBNull.Value) ? String.Empty : drCliente["baicli"].ToString();
            cliente.Rua = (drCliente["endcli"] == DBNull.Value) ? String.Empty : drCliente["endcli"].ToString();
            cliente.RuaDeEntrega = (drCliente["endent"] == DBNull.Value) ? String.Empty : drCliente["endent"].ToString();
            cliente.BairroDeEntrega = (drCliente["baient"] == DBNull.Value) ? String.Empty : drCliente["baient"].ToString();
            cliente.CidadeDeEntrega = (drCliente["cident"] == DBNull.Value) ? String.Empty : drCliente["cident"].ToString();
            cliente.EstadoDeEntrega = (drCliente["estent"] == DBNull.Value) ? String.Empty : drCliente["estent"].ToString();
            cliente.CEPDeEntrega = (drCliente["cepent"] == DBNull.Value) ? String.Empty : drCliente["cepent"].ToString();
            cliente.NumeroDeEntrega = (drCliente["nenent"] == DBNull.Value) ? String.Empty : drCliente["nenent"].ToString();
            cliente.ComplementoDeEntrega = (drCliente["cplent"] == DBNull.Value) ? String.Empty : drCliente["cplent"].ToString();

            cliente.ClienteOuFornecedor = 1; //Cliente 
            cliente.Coligada = 1;
            cliente.OptantePeloSimples = "00000";

            string nomeMunicipio = (drCliente["cidcli"] == DBNull.Value) ? String.Empty : drCliente["cidcli"].ToString();
            string uf = (drCliente["sigufs"] == DBNull.Value) ? String.Empty : drCliente["sigufs"].ToString();

            var municipio = municipios.FirstOrDefault(x => ((x.CodEstado.RemoveSpecialChars().Equals(uf)) || (x.Nome.RemoveSpecialChars().Equals(nomeMunicipio))));

            cliente.CodigoDoMunicipio = municipio.CodMunicipio;

            if (string.IsNullOrEmpty(cliente.NomeFantasia.Trim()))
                cliente.NomeFantasia = cliente.Nome;

            if (cliente.PessoaFisicaOuJuridica == "J")
                cliente.CNPJCPF = cliente.CNPJCPF.PadLeft(14, '0'); //cnpj
            else
                cliente.CNPJCPF = cliente.CNPJCPF.PadLeft(11, '0'); //cpf

            if (cliente.Estado == "." || cliente.Estado.Trim() == string.Empty)
                cliente.Estado = string.Empty;

            if (Convert.ToInt64(cliente.CNPJCPF) == 0)
                cliente.CNPJCPF = string.Empty;

            if (cliente.IE.ToUpper().Equals("ISENTO") && cliente.Estado == string.Empty)
                cliente.IE = string.Empty;
            
            return cliente;
        }




        private List<ClienteFornecedor> BuscarFornecedores()
        {
            List<ClienteFornecedor> lForn = new List<ClienteFornecedor>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_countTodosFornecedores))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }
            
            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_buscarTodosFornecedores))
            {
                var reader = database.ExecuteReader(command);                

                while (reader.Read())
                {
                    try
                    {
                        lForn.Add(ConverterFornecedores(reader));
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        string codForn = (reader["codfor"] == DBNull.Value) ? String.Empty : reader["codfor"].ToString();

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o Fornecedor: Código {0},Motivo:{1}", codForn, ex.Message));
                    }
                }
            }

            return lForn;
        }

        private List<ClienteFornecedor> BuscarClientes()
        {
            List<ClienteFornecedor> lCli = new List<ClienteFornecedor>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

            double totalRecords;

            using (DbCommand command = database.GetSqlStringCommand(_countTodosClientes))
            {
                totalRecords = Convert.ToDouble(database.ExecuteScalar(command));
            }

            double processedRecords = 0;

            using (DbCommand command = database.GetSqlStringCommand(_buscarTodosClientes))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    try
                    {
                        lCli.Add(ConverterCliente(reader));
                        processedRecords++;

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        string codCli = (reader["codfor"] == DBNull.Value) ? String.Empty : reader["codcli"].ToString();

                        _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar o Cliente: Código {0},Motivo:{1}", codCli, ex.Message));
                    }
                }
            }

            return lCli;
        }



    }
}
