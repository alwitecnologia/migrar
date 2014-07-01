using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.DAO
{
    public class FuncionarioDAO
    {
        private static string _buscarCodPessoa = "select CODPESSOA from PFUNC where CHAPA=@CHAPA";

        private static string _existePessoa = "select cast(case when count(*)=0 then 0 else 1 end as bit) from PFUNC where CHAPA=@CHAPA";

        private static string _buscaChapa = "select CHAPA from PFUNC funcionario join PPESSOA pessoa on funcionario.CODPESSOA = pessoa.CODIGO where CPF=@CPF";

        private static bool ExistePessoa(string chapa)
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                DbCommand command = database.GetSqlStringCommand(_existePessoa);

                database.AddInParameter(command, "@CHAPA", DbType.String, chapa);

                return Convert.ToBoolean(database.ExecuteScalar(command));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Houve um erro ao verificar existência da pessoa chapa {0}. {1}", chapa, e.Message));
            }
        }

        public static string BuscarCodPessoa(string chapa)
        {
            try
            {
                if (ExistePessoa(chapa))
                {
                    Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                    DbCommand command = database.GetSqlStringCommand(_buscarCodPessoa);

                    database.AddInParameter(command, "@CHAPA", DbType.String, chapa);

                    object codPessoa = database.ExecuteScalar(command);

                    return database.ExecuteScalar(command).ToString();
                }
                else
                {
                    throw new BusinessException("Funcionário e/ou pessoa não existe.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Houve um erro ao procurar o funcionário chapa {0}. {1}", chapa,e.Message));
            }
        }

        public static string buscarChapa(string cpf, string nome)
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                DbCommand command = database.GetSqlStringCommand(_buscaChapa);

                database.AddInParameter(command, "@CPF", DbType.String, cpf);

                object chapa = database.ExecuteScalar(command);

                if (chapa == null)
                    return String.Empty;
                else
                    return chapa.ToString();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Houve um erro ao encontrar a chapa do funcionario CPF: {0}. {1}", cpf, nome,e.Message));
            }
        }
    }
}
