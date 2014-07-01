using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exportador.Academico.MatrizCurricular.Grade;
using Exportador.Helpers;

namespace Exportador.DAO
{
    public class GradeDAO
    {

        #region Queries

            private string _queryGradesDestino = @"SELECT [CODCOLIGADA]
                                                        ,[CODCURSO]
                                                        ,[CODHABILITACAO]
                                                        ,[CODGRADE]
                                                        ,[DESCRICAO]      
                                                        ,[DTINICIO]
                                                        ,[DTFIM]   
                                                        ,[CARGAHORARIA]      
                                                        ,[CONTROLEVAGAS]      
                                                        ,[STATUS]
                                                        ,[CODCURSOPROX]
                                                        ,[CODHABILITACAOPROX]
                                                        ,[CODGRADEPROX]
                                                        ,[MAXCREDPERIODO]
                                                        ,[MINCREDPERIODO]
                                                        ,[REGIME]
                                                        ,[TIPOATIVIDADECURRICULAR]
                                                        ,[TIPOELETIVA]
                                                        ,[TIPOOPTATIVA]
                                                        ,[DTDOU]
                                                        ,[TOTALCREDITOS]
                                                FROM [SGRADE]";


        #endregion

        /// <summary>
        /// Retorna todas as grades importadas para o sistema de destino.
        /// </summary>
        /// <returns></returns>
        public List<Grade> buscarTodasDestino()
        {
            return DBHelper.GetAll("RM", _queryGradesDestino, String.Empty, GradeConverter);
        }

        private Grade GradeConverter(IDataReader drGrade)
        {
            Grade g = new Grade();

            g.CodColigada = (int)drGrade.GetNullableInt32("CODCOLIGADA");
            g.CodCurso = drGrade.GetString("CODCURSO");
            g.CodHabilitacao = drGrade.GetString("CODHABILITACAO");
            g.CodGrade = drGrade.GetString("CODGRADE");
            g.Descricao = drGrade.GetString("DESCRICAO");
            g.DtInicio = drGrade.GetNullableDateTime("DTINICIO");
            g.DtFim = drGrade.GetNullableDateTime("DTFIM");
            g.CargaHoraria = (int)drGrade.GetNullableInt32("CARGAHORARIA");
            g.ControleVagas = drGrade.GetString("CONTROLEVAGAS");
            g.Status = drGrade.GetString("STATUS");
            g.CodCursoProx = drGrade.GetString("CODCURSOPROX");
            g.CodHabilitacaoProx = drGrade.GetString("CODHABILITACAOPROX");
            g.CodGradeProx = drGrade.GetString("CODGRADEPROX");
            g.MaxCredPeriodo = (int)drGrade.GetNullableInt32("MAXCREDPERIODO");
            g.MinCredPeriodo = (int)drGrade.GetNullableInt32("MINCREDPERIODO");
            g.Regime = drGrade.GetString("REGIME");
            g.TipoAtividadeCurricular = drGrade.GetString("TIPOATIVIDADECURRICULAR");
            g.TipoEletiva = drGrade.GetString("TIPOELETIVA");
            g.TipoOptativa = drGrade.GetString("TIPOOPTATIVA");
            g.DtDOU = drGrade.GetNullableDateTime("DTDOU");
            g.TotalCreditos = (double)drGrade.GetNullableDouble("TOTALCREDITOS");

            return g;
        }

    }
}
