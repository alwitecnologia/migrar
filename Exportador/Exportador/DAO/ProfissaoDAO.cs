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
    public class ProfissaoDAO
    {
        private string _buscarCodProfissao = "select codcliente from eprofiss where descricao=@descricao";
        private string _queryBuscaTodas = "select codcliente,descricao from eprofiss";

        public Int32? buscarCodProfissao(string descricao)
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                DbCommand command = database.GetSqlStringCommand(_buscarCodProfissao);

                database.AddInParameter(command, "@descricao", DbType.String, descricao);

                return (Int32?)database.ExecuteScalar(command);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar a profissão '{0}', motivo:{1}", descricao,e.Message));
            }
        }

        public List<Profissao> buscarTodas()
        {
            List<Profissao> profissoes = new List<Profissao>();

            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                using (DbCommand command = database.GetSqlStringCommand(_queryBuscaTodas))
                {
                    DataSet ds = database.ExecuteDataSet(command);

                    foreach (DataRow row in ds.Tables["Table"].Rows)
                    {
                        Profissao prof = new Profissao();
                        prof.CodCliente = (DBNull.Value == row["CODCLIENTE"]) ? null : (Int32?)row["CODCLIENTE"];
                        prof.Descricao = (DBNull.Value == row["DESCRICAO"]) ? String.Empty : (string)row["DESCRICAO"];

                        profissoes.Add(prof);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar todas as profissões. Motivo:{0}", e.Message));
            }

            return profissoes;
        }
    }
}
