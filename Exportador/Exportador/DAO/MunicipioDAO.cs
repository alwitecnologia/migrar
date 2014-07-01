using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Data.Common;
using Exportador;
using Exportador.VO;

namespace Exportador.Helpers
{
    public class MunicipioDAO
    {
        private string _buscarCodMunicipioNomeUF = "SELECT CODMUNICIPIO FROM GMUNICIPIO WHERE NOMEMUNICIPIO=@NOME AND CODETDMUNICIPIO=@UF";

        private string _buscarTodos = "SELECT CODMUNICIPIO,CODETDMUNICIPIO,NOMEMUNICIPIO FROM GMUNICIPIO";

        public string BuscarCodMunicipio(string nomeMunicipio,string uf)
        {
            try
            {
                Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

                DbCommand command = database.GetSqlStringCommand(_buscarCodMunicipioNomeUF);

                database.AddInParameter(command, "@NOME", DbType.String, nomeMunicipio);
                database.AddInParameter(command, "@UF", DbType.String, uf);

                return database.ExecuteScalar(command).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Município não encontrado! Nome:{0}, UF:{1}",nomeMunicipio,uf));
            }
        }

        private Municipio Converter(IDataReader drProfessor)
        {
            Municipio m = new Municipio();

            m.CodMunicipio = (drProfessor["CODMUNICIPIO"] == DBNull.Value) ? String.Empty : drProfessor["CODMUNICIPIO"].ToString();
            m.Nome = (drProfessor["NOMEMUNICIPIO"] == DBNull.Value) ? String.Empty : drProfessor["NOMEMUNICIPIO"].ToString();
            m.CodEstado = (drProfessor["CODETDMUNICIPIO"] == DBNull.Value) ? String.Empty : drProfessor["CODETDMUNICIPIO"].ToString();

            return m;
        }

        public List<Municipio> buscarTodos()
        {
            return DBHelper.GetAll<Municipio>("RM", _buscarTodos,String.Empty, Converter);
        }
    }
}
