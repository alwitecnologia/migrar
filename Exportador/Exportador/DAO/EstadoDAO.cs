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
    public class EstadoDAO
    {
        private string _buscarTodas = @"SELECT 
	                                        GPAIS.IDPAIS
	                                        ,GPAIS.CODPAIS AS CODPAIS
	                                        ,GPAIS.DESCRICAO AS DESCPAIS
	                                        ,GETD.CODETD AS CODESTADO
	                                        ,GETD.NOME AS DESCESTADO
                                        from GETD
                                        left join GPAIS on GPAIS.IDPAIS=GETD.IDPAIS";


        public List<Estado> buscarTodas()
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                DbCommand command = database.GetSqlStringCommand(_buscarTodas);

                IDataReader drEstados = database.ExecuteReader(command);

                List<Estado> estados = new List<Estado>();

                while (drEstados.Read())
                {
                    estados.Add(mapearEstado(drEstados));
                }

                return estados;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível retornar as nações, motivo:{0}", e.Message));
            }
        }

        private Estado mapearEstado(IDataReader drEstados)
        {
            Estado e = new Estado();

            e.Pais = new Pais();

            e.Pais.Id = (DBNull.Value == drEstados["IDPAIS"]) ? null : (int?)Convert.ToInt16(drEstados["IDPAIS"]);
            e.Pais.Codigo = (DBNull.Value == drEstados["CODPAIS"]) ? String.Empty : (string)drEstados["CODPAIS"];
            e.Pais.Descricao = (DBNull.Value == drEstados["DESCPAIS"]) ? String.Empty : (string)drEstados["DESCPAIS"];

            e.Descricao = (DBNull.Value == drEstados["DESCESTADO"]) ? String.Empty : (string)drEstados["DESCESTADO"];
            e.Codigo = (DBNull.Value == drEstados["CODESTADO"]) ? String.Empty : (string)drEstados["CODESTADO"];

            return e;
        }

    }
}
