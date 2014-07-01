using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exportador.Academico.Turno;
using Exportador.Helpers;

namespace Exportador.DAO
{
    public class TurnoDAO
    {
        #region Queries

        private string _queryRetornaTodos = @"SELECT [CODCOLIGADA]
                                                ,[CODTIPOCURSO]
                                                ,[CODFILIAL]
                                                ,[NOME]            
                                                ,[CODTURNO]
                                                ,[HORINI]
                                                ,[HORFIM]
                                                ,[TIPO]
                                            FROM [STURNO]";

        #endregion

        public List<Turno> RetornaTodos() 
        {
            return DBHelper.GetAll("RM", _queryRetornaTodos, String.Empty, TurnoConverter);
        }

        private Turno TurnoConverter(IDataReader reader)
        {
            Turno t = new Turno();

            t.CodColigada = (int)reader.GetNullableInt32("CODCOLIGADA");
            t.CodTipoCurso = (int)reader.GetNullableInt32("CODTIPOCURSO");
            t.CodFilial = (int)reader.GetNullableInt32("CODFILIAL");
            t.Nome = reader.GetString("NOME");
            t.CodTurno = reader.GetString("CODTURNO");
            t.HoraInicio = reader.GetString("HORINI");
            t.HoraFim = reader.GetString("HORFIM");
            t.Tipo = reader.GetString("TIPO");
            
            return t;
        }
    }
}
