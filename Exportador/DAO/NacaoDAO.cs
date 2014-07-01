using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Exportador.Academico.Pessoa;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.DAO
{
    public class NacaoDAO
    {
        private string _buscarTodas = @"select IDPAIS,CODPAIS,DESCRICAO
                                        from GPAIS";


        public List<Pais> buscarTodas()
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                DbCommand command = database.GetSqlStringCommand(_buscarTodas);

                IDataReader drNacoes = database.ExecuteReader(command);

                List<Pais> nacoes = new List<Pais>();

                while (drNacoes.Read())
                {
                    nacoes.Add(mapearNacao(drNacoes));
                }

                return nacoes;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar as nações, motivo:{0}", e.Message));
            }
        }

        private Pais mapearNacao(IDataRecord drNacao)
        {
            Pais n = new Pais();

            n.Id = (DBNull.Value == drNacao["IDPAIS"]) ? null : (int?)drNacao["IDPAIS"];
            n.Codigo = (DBNull.Value == drNacao["CODPAIS"]) ? String.Empty : (string)drNacao["CODPAIS"];
            n.Descricao = (DBNull.Value == drNacao["DESCRICAO"]) ? String.Empty : (string)drNacao["DESCRICAO"];

            return n;
        }

    }
}
