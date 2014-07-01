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
    public class OcupacaoDAO
    {
        private string _queryBuscaCodOcupacao = "select codcliente from ETABOCUP where descricao=@descricao";
        private string _queryBuscaTodas = "select codcliente,descricao from ETABOCUP";

        public string buscarCodOcupacao(string descOcupacao)
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");                

                using(DbCommand command = database.GetSqlStringCommand(_queryBuscaCodOcupacao))
	            {
                    database.AddInParameter(command, "@descricao", DbType.String, descOcupacao);

                    object value = database.ExecuteScalar(command);

                    return value == null ? String.Empty : value.ToString();
	            }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar a ocupação '{0}', motivo:{1}", descOcupacao, e.Message));
            } 
        }

        public List<Ocupacao> buscarTodas() 
        {
            List<Ocupacao> ocupacoes = new List<Ocupacao>();

            //try
            //{
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                using (DbCommand command = database.GetSqlStringCommand(_queryBuscaTodas))
                {
                    DataSet ds = database.ExecuteDataSet(command);

                    foreach (DataRow row in ds.Tables["table"].Rows)
                    {
                        Ocupacao ocupa = new Ocupacao();
                        ocupa.CodCliente = (DBNull.Value == row["CODCLIENTE"]) ? String.Empty : (string)row["CODCLIENTE"];
                        ocupa.Descricao = (DBNull.Value == row["DESCRICAO"]) ? String.Empty : (string)row["DESCRICAO"];

                        ocupacoes.Add(ocupa);
                    }
                }
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(string.Format("Não foi possível retornar todas as ocupações. Motivo:{0}",e.Message));
            //}

            return ocupacoes;
        }
    }
}
