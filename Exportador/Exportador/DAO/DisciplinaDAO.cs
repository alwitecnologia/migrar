using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exportador.Academico.Disciplina;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using Exportador.Helpers;

namespace Exportador.DAO
{
    public class DisciplinaDAO
    {
        private string _buscarTodasDisciplinas = @"SELECT ND.ID AS CODDISC
                                                    ,ND.NOME AS NOMEDISC
                                                    FROM NOME_DISCIPLINA ND";

        private Disciplina ConverterDisciplina(IDataReader reader)
        {
            Disciplina disc = new Disciplina();

            string tipoCurso = reader.GetString("TIPOCURSO");
            string nomeCurso = reader.GetString("NOMECURSO");

            if ((!String.IsNullOrEmpty(tipoCurso)) && (!String.IsNullOrEmpty(nomeCurso)))
                disc.CodTipoCurso = (new CursoDAO()).buscarTipoCurso(tipoCurso, nomeCurso);

            disc.CodDisc = (reader["CODDISC"] == DBNull.Value) ? String.Empty : reader["CODDISC"].ToString();
            disc.Nome = (reader["NOMEDISC"] == DBNull.Value) ? String.Empty : reader["NOMEDISC"].ToString();

            disc.CodColigada = 1;
            disc.CursoLivre = "N";
            disc.TipoAula = "M";
            disc.TipoNota = "C";

            return disc;
        }
        
        /// <summary>
        /// Retorna todas as disciplinas cadastradas no sistema de origem.
        /// </summary>
        /// <returns></returns>
        public List<Disciplina> BuscarDisciplinasOrigem()
        {
            List<Disciplina> lDisc = new List<Disciplina>();

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("SICA");

            using (DbCommand command = database.GetSqlStringCommand(_buscarTodasDisciplinas))
            {
                var reader = database.ExecuteReader(command);

                while (reader.Read())
                {
                    lDisc.Add(ConverterDisciplina(reader));
                }
            }

            return lDisc;
        }
    }
}
