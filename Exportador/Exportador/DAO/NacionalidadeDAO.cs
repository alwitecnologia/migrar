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
    public class NacionalidadeDAO
    {
        private string _buscarNacionalidadePorDescricao = @"select CODCLIENTE as CODIGO
	                                                            ,DESCRICAO
                                                            from PCODNACAO 
                                                            where DESCRICAO=@Descricao";

        private string _buscarTodas = @"select CODCLIENTE as CODIGO
	                                                            ,DESCRICAO
                                                            from PCODNACAO";

        public Nacionalidade buscarNacionalidadePorDescricao(string descricao)
        {
            try
            {
                Nacionalidade nac = null;

                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                DbCommand command = database.GetSqlStringCommand(_buscarNacionalidadePorDescricao);

                database.AddInParameter(command, "@descricao", DbType.String, descricao);

                IDataReader drNacionalidade = database.ExecuteReader(command);

                while (drNacionalidade.Read()) 
                {
                    nac = mapearNacionalidade(drNacionalidade);
                }

                if (nac == null)
                    throw new BusinessException(string.Format("Não foi possível retornar a nacionalidade '{0}'.", descricao));

                return nac;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar a nacionalidade '{0}', motivo:{1}", descricao, e.Message));
            }
        }

        private Nacionalidade mapearNacionalidade(IDataRecord drNacionalidade)
        {
            Nacionalidade nac = new Nacionalidade();

            nac.Codigo = (DBNull.Value == drNacionalidade["CODIGO"]) ? String.Empty : (string)drNacionalidade["CODIGO"];
            nac.Descricao = (DBNull.Value == drNacionalidade["DESCRICAO"]) ? String.Empty : (string)drNacionalidade["DESCRICAO"];

            return nac;
        }


        public List<Nacionalidade> buscarTodas()
        {
            List<Nacionalidade> nacionalidades = new List<Nacionalidade>();

            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                using (DbCommand command = database.GetSqlStringCommand(_buscarTodas))
                {
                    IDataReader drNac = database.ExecuteReader(command);

                    while (drNac.Read())
                    {
                        Nacionalidade nac = mapearNacionalidade(drNac);

                        nacionalidades.Add(nac);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar todas as profissões. Motivo:{0}", e.Message));
            }

            return nacionalidades;
        }
    }
}
