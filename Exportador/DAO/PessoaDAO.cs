using Exportador.Academico.PessoaFiltro;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Exportador.DAO
{
    class PessoaDAO
    {
        public static string _verificaCPF = @"select cast(case when count(*)=0 then 0 else 1 end as bit) from vetorh.r034fun where numcpf = @CPF ";

        /// <summary>
        /// Método que verifica se a pessoa possui cadastro na base do sistema de RH de origem.
        /// </summary>
        /// <param name="cpf">CPF da pessoa.</param>
        /// <returns></returns>
        public static bool existePessoa(string cpf)
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("VetoRH");

                DbCommand command = database.GetSqlStringCommand(_verificaCPF);

                database.AddInParameter(command, "@cpf", DbType.String, cpf);

                return Convert.ToBoolean(database.ExecuteScalar(command));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Houve um erro ao verificar existência da pessoa cpf {0}, erro: {1}", cpf, e.Message));
            }
        }
    }
}
