using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Exportador.Academico.Pessoa;
using Exportador.BackOffice.ClienteFornecedor;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.DAO
{
    public class ClienteDAO
    {

        #region Queries

        private string _buscarTodosSapiens = @"select codcli,usu_coduni,cgccpf,nomcli
                                        from e085cli";

        private string _buscarTodosRM = @"select CODCFO,CGCCFO,CIDENTIDADE,NOME from FCFO";

        #endregion
                
        public List<ClienteFornecedor> buscarTodosSapiens()
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("Sapiens");

                DbCommand command = database.GetSqlStringCommand(_buscarTodosSapiens);

                IDataReader drClientes = database.ExecuteReader(command);

                List<ClienteFornecedor> clientes = new List<ClienteFornecedor>();

                while (drClientes.Read())
                {
                    clientes.Add(mapearClienteSapiens(drClientes));
                }

                return clientes;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar os clientes Sapiens, motivo:{0}", e.Message));
            }
        }

        private ClienteFornecedor mapearClienteSapiens(IDataReader drClientes)
        {
            ClienteFornecedor cliente = new ClienteFornecedor();

            cliente.Codigo = (DBNull.Value == drClientes["codcli"]) ? String.Empty : drClientes["codcli"].ToString();
            cliente.CodigoAcademico = (DBNull.Value == drClientes["usu_coduni"]) ? String.Empty : drClientes["usu_coduni"].ToString();
            cliente.CNPJCPF = (DBNull.Value == drClientes["cgccpf"]) ? String.Empty : drClientes["cgccpf"].ToString().PadLeft(11, '0');
            cliente.Nome = (DBNull.Value == drClientes["nomcli"]) ? String.Empty : drClientes["nomcli"].ToString();

            return cliente;
        }

        public List<ClienteFornecedor> buscarTodosRM()
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                DbCommand command = database.GetSqlStringCommand(_buscarTodosRM);

                IDataReader drClientes = database.ExecuteReader(command);

                List<ClienteFornecedor> clientes = new List<ClienteFornecedor>();

                while (drClientes.Read())
                {
                    clientes.Add(mapearClienteRM(drClientes));
                }

                return clientes;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar os clientes RM, motivo:{0}", e.Message));
            }
        }

        private ClienteFornecedor mapearClienteRM(IDataReader drClientes)
        {
            ClienteFornecedor cliente = new ClienteFornecedor();

            cliente.Codigo = (DBNull.Value == drClientes["CODCFO"]) ? String.Empty : drClientes["CODCFO"].ToString();
            cliente.CNPJCPF = (DBNull.Value == drClientes["CGCCFO"]) ? String.Empty : drClientes["CGCCFO"].ToString().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty).PadLeft(11, '0');
            cliente.Nome = (DBNull.Value == drClientes["NOME"]) ? String.Empty : drClientes["NOME"].ToString();
            cliente.CarteiraDeIDentidade = (DBNull.Value == drClientes["CIDENTIDADE"]) ? String.Empty : drClientes["CIDENTIDADE"].ToString().Replace(".",String.Empty).Replace("-",String.Empty);

            cliente.CodigoAcademico = String.Empty;

            return cliente;
        }
    }
}
