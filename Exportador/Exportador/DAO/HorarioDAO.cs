using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;
using System.Data.Common;
using Exportador.RH.Horario;
using FileHelpers;

namespace Exportador.Helpers
{
    public class HorarioDAO
    {
        private static string _queryInserirHorario = @"INSERT INTO [AHORARIO] ([CODCOLIGADA], [CODIGO], [DESCRICAO], [DATABASEHOR], [TIPOHOR]
                                            , [INATIVO], [HORNOTURNO], [CONSFERIADO], [HORARIOJOR], [CONSFERDIAANT], [RECCREATEDBY]
                                            , [RECCREATEDON], [RECMODIFIEDBY], [RECMODIFIEDON]) 
                                            VALUES (@p1, @p2, @p3, @p4, @p5
                                                    , @p6, @p7, @p8, @p9, @p10, @RECCREATEDBY
                                            , CONVERT ( DATETIME, CONVERT ( VARCHAR, GETDATE(), 120) ), @RECMODIFIEDBY
                                            , CONVERT ( DATETIME, CONVERT ( VARCHAR, GETDATE(), 120) ))";


        
        
        
        private static string _queryInserirBatida = @"INSERT INTO ABATHOR (CODCOLIGADA, CODHORARIO, INDICE, BATIDA
                                                    ,TIPO, FIM, NATUREZA, INICIO, TEMPOMINREF,INDLIMREF, ATRASOMINREF, EXTRAMINREF
                                                    ,RECMODIFIEDBY, RECMODIFIEDON, RECCREATEDBY, RECCREATEDON)
                                                    VALUES (@CODCOLIGADA, @CODHORARIO, @INDICE, @BATIDA, @TIPO,
                                                    @FIM, @NATUREZA, @INICIO, @TEMPOMINREF, @INDLIMREF,@ATRASOMINREF, @EXTRAMINREF,
                                                    @RECMODIFIEDBY, CONVERT ( DATETIME, CONVERT ( VARCHAR, GETDATE(), 120) )
                                                    , @RECCREATEDBY, CONVERT ( DATETIME, CONVERT ( VARCHAR, GETDATE(), 120) ))";

        private static string _queryCountIndice = @"SELECT COUNT(*) FROM AINDHOR 
                                                    WHERE CODCOLIGADA=@CODCOLIGADA
                                                        AND CODHORARIO=@CODHORARIO
                                                        AND INDINICIOHOR=@INDICE";

        private static string _queryInserirIndice = @"INSERT INTO AINDHOR(CODCOLIGADA, CODHORARIO, INDINICIOHOR, DESCRICAO
                                                    ,RECMODIFIEDBY, RECMODIFIEDON, RECCREATEDBY, RECCREATEDON)
                                                    VALUES(@CODCOLIGADA, @CODHORARIO, @INDICE, @LETRA,@RECMODIFIEDBY
                                                    , CONVERT ( DATETIME, CONVERT ( VARCHAR, GETDATE(), 120) )
                                                    , @RECCREATEDBY, CONVERT ( DATETIME, CONVERT ( VARCHAR, GETDATE(), 120) ))
";

        private static string _queryInserirJornada = @"INSERT INTO AJORHOR(CODCOLIGADA, CODHORARIO, INDINICIO, BATINICIO
                                                        ,INDFIM, BATFIM, HORASFALTA, HORASEXTRAS,RECMODIFIEDBY, RECMODIFIEDON
                                                        , RECCREATEDBY, RECCREATEDON)
                                                    VALUES (@CODCOLIGADA, @CODHORARIO, @INDINICIO, @BATINICIO, @INDFIM
                                                        , @BATFIM, @HORASFALTA, @HORASEXTRAS,@RECMODIFIEDBY
                                                        , CONVERT ( DATETIME, CONVERT ( VARCHAR, GETDATE(), 120) ), @RECCREATEDBY,
                                                         CONVERT ( DATETIME, CONVERT ( VARCHAR, GETDATE(), 120) ))";


        public static void CadastrarHorario(IdentificacaoHorario horario)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            DbCommand command = database.GetSqlStringCommand(_queryInserirHorario);

            command.Connection = database.CreateConnection();

            database.AddInParameter(command, "@p1", DbType.Int32, horario.CodColigada);
            database.AddInParameter(command, "@p2", DbType.String, horario.CodHorario);
            database.AddInParameter(command, "@p3", DbType.String, horario.DescricaoHorario);
            database.AddInParameter(command, "@p4", DbType.DateTime, Convert.ToDateTime(horario.DataBaseHorario));
            database.AddInParameter(command, "@p5", DbType.Int32, Convert.ToInt32(horario.TipoHorario));
            database.AddInParameter(command, "@p6", DbType.Int32, horario.Inativo);
            database.AddInParameter(command, "@p7", DbType.Int32, horario.HorNoturno);
            database.AddInParameter(command, "@p8", DbType.Int32, horario.ConsFeriado);
            database.AddInParameter(command, "@p9", DbType.Int32, horario.HorarioJor);
            database.AddInParameter(command, "@p10", DbType.Int32, horario.ConsFerDiaAnt);
            database.AddInParameter(command, "@RECCREATEDBY", DbType.String, horario.RecCreatedBy);
            database.AddInParameter(command, "@RECMODIFIEDBY", DbType.String, horario.RecModifiedBy);

            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            command.ExecuteNonQuery();

            command.Connection.Close();
        }

        private static void CadastrarIndiceHorario(BatidasNormais batida) 
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            DbCommand command = database.GetSqlStringCommand(_queryCountIndice);

            database.AddInParameter(command, "@CODCOLIGADA", DbType.Int32, batida.CodColigada);
            database.AddInParameter(command, "@CODHORARIO", DbType.String, batida.CodHorario);
            database.AddInParameter(command, "@INDICE", DbType.Int32, batida.IndiceDia);

            int count = Convert.ToInt32(database.ExecuteScalar(command));

            if (count == 0)
            {
                command = database.GetSqlStringCommand(_queryInserirIndice);                

                database.AddInParameter(command, "@CODCOLIGADA", DbType.Int32, batida.CodColigada);
                database.AddInParameter(command, "@CODHORARIO", DbType.String, batida.CodHorario);
                database.AddInParameter(command, "@INDICE", DbType.Int32, batida.IndiceDia);
                database.AddInParameter(command, "@LETRA", DbType.String, batida.LetraDia);

                database.AddInParameter(command, "@RECCREATEDBY", DbType.String, batida.RecCreatedBy);
                database.AddInParameter(command, "@RECMODIFIEDBY", DbType.String, batida.RecModifiedBy);

                command.Connection = database.CreateConnection();

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                command.ExecuteNonQuery();

                command.Connection.Close();

            }
        }

        public static void CadastrarBatida(BatidasNormais batida) 
        {
            CadastrarIndiceHorario(batida);
            CadastrarJornada(batida);

            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            DbCommand command = database.GetSqlStringCommand(_queryInserirBatida);
   
            database.AddInParameter(command, "@CODCOLIGADA", DbType.Int32, batida.CodColigada);
            database.AddInParameter(command, "@CODHORARIO", DbType.String, batida.CodHorario);
            database.AddInParameter(command, "@INDICE", DbType.Int32, batida.IndiceDia);

            database.AddInParameter(command, "@RECCREATEDBY", DbType.String, batida.RecCreatedBy);
            database.AddInParameter(command, "@RECMODIFIEDBY", DbType.String, batida.RecModifiedBy);

            database.AddInParameter(command, "@TIPO", DbType.Int32, batida.TipoBatida);
            database.AddInParameter(command, "@INICIO", DbType.Int32, batida.Inicio);
            database.AddInParameter(command, "@FIM", DbType.Int32, batida.Fim);
            database.AddInParameter(command, "@TEMPOMINREF", DbType.Int32, batida.TempoMinRef);
            database.AddInParameter(command, "@INDLIMREF", DbType.Int32, batida.IndLimRef);
            database.AddInParameter(command, "@EXTRAMINREF", DbType.Int32, batida.ExtraMinRef);
            database.AddInParameter(command, "@ATRASOMINREF", DbType.Int32, batida.AtrasoMinRef);

            database.AddInParameter(command, "@BATIDA", DbType.Int32, batida.HorarioBatida1);
            database.AddInParameter(command, "@NATUREZA", DbType.Int32, batida.NaturezaBatida1);

            executeCommand(database, command);

            if (batida.HorarioBatida2 != 0)
            {

                command.Parameters.RemoveAt("@BATIDA");
                command.Parameters.RemoveAt("@NATUREZA");

                database.AddInParameter(command, "@BATIDA", DbType.Int32, batida.HorarioBatida2);
                database.AddInParameter(command, "@NATUREZA", DbType.Int32, batida.NaturezaBatida2);

                executeCommand(database, command);
            }

            if (batida.HorarioBatida3 != 0)
            {
                command.Parameters.RemoveAt("@BATIDA");
                command.Parameters.RemoveAt("@NATUREZA");

                database.AddInParameter(command, "@BATIDA", DbType.Int32, batida.HorarioBatida3);
                database.AddInParameter(command, "@NATUREZA", DbType.Int32, batida.NaturezaBatida3);

                executeCommand(database, command);
            }

            if (batida.HorarioBatida4 != 0)
            {
                command.Parameters.RemoveAt("@BATIDA");
                command.Parameters.RemoveAt("@NATUREZA");

                database.AddInParameter(command, "@BATIDA", DbType.Int32, batida.HorarioBatida4);
                database.AddInParameter(command, "@NATUREZA", DbType.Int32, batida.NaturezaBatida4);

                executeCommand(database, command);
            }

            if (batida.HorarioBatida5 != 0)
            {
                command.Parameters.RemoveAt("@BATIDA");
                command.Parameters.RemoveAt("@NATUREZA");

                database.AddInParameter(command, "@BATIDA", DbType.Int32, batida.HorarioBatida5);
                database.AddInParameter(command, "@NATUREZA", DbType.Int32, batida.NaturezaBatida5);

                executeCommand(database, command);
            }

            if (batida.HorarioBatida6 != 0)
            {
                command.Parameters.RemoveAt("@BATIDA");
                command.Parameters.RemoveAt("@NATUREZA");

                database.AddInParameter(command, "@BATIDA", DbType.Int32, batida.HorarioBatida6);
                database.AddInParameter(command, "@NATUREZA", DbType.Int32, batida.NaturezaBatida6);

                executeCommand(database, command);
            }

        }

        private static void CadastrarJornada(BatidasNormais batida) 
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>("RM");

            DbCommand command = database.GetSqlStringCommand(_queryInserirJornada);

            database.AddInParameter(command, "@CODCOLIGADA", DbType.Int32, batida.CodColigada);
            database.AddInParameter(command, "@CODHORARIO", DbType.String, batida.CodHorario);
            
            database.AddInParameter(command, "@INDINICIO", DbType.Int32, batida.IndiceDia);
            database.AddInParameter(command, "@INDFIM", DbType.Int32, batida.IndiceDia);

            database.AddInParameter(command, "@BATINICIO", DbType.Int32, batida.HorarioBatida1);

            if (batida.NaturezaBatida6 != 0)
            {
                database.AddInParameter(command, "@BATFIM", DbType.Int32, batida.HorarioBatida6);
            }
            else
            {
                if (batida.NaturezaBatida4 != 0)
                {
                    database.AddInParameter(command, "@BATFIM", DbType.Int32, batida.HorarioBatida4);
                }
                else
                {
                    if (batida.NaturezaBatida2 != 0)
                        database.AddInParameter(command, "@BATFIM", DbType.Int32, batida.HorarioBatida2);
                }
            }
            
            database.AddInParameter(command, "@RECCREATEDBY", DbType.String, batida.RecCreatedBy);
            database.AddInParameter(command, "@RECMODIFIEDBY", DbType.String, batida.RecModifiedBy);
            
            database.AddInParameter(command, "@HORASFALTA", DbType.Int32, 0);
            database.AddInParameter(command, "@HORASEXTRAS", DbType.Int32, 0);

            executeCommand(database, command);

        }

        private static void executeCommand(Database database, DbCommand command)
        {
            command.Connection = database.CreateConnection();

            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            command.ExecuteNonQuery();

            command.Connection.Close();
        }
    }
}
