using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity;

namespace Exportador.Helpers
{
    public delegate T ConverterDelegate<T>(IDataReader reader);

    public static class DBHelper
    {
        public static object DefaultDbNull(this object value, object defaultValue)
        {
            if (value == Convert.DBNull)
                return defaultValue;
            return value;
        }

        public static String GetString(this IDataRecord row, string columnName)
        {
            try
            {
                return (row[columnName] == DBNull.Value) ? String.Empty : row[columnName].ToString().Replace(";",",");
            }
            catch (IndexOutOfRangeException ex)
            {
                return String.Empty;
            }            
        }

        public static Int32? GetNullableInt32(this IDataRecord row, string columnName)
        {
            try
            {
                return (row[columnName] == DBNull.Value) ? (Int32?)null : Convert.ToInt32(row[columnName]);
            }
            catch (IndexOutOfRangeException ex)
            {
                return (Int32?)null;
            }   
        }

        public static Double? GetNullableDouble(this IDataRecord row, string columnName)
        {
            try
            {
                return (row[columnName] == DBNull.Value) ? (Double?)null : Convert.ToDouble(row[columnName]);
            }
            catch (IndexOutOfRangeException ex)
            {
                return (Double?)null;
            }   
        }

        public static DateTime? GetNullableDateTime(this IDataRecord value, string columnName)
        {
            try
            {
                //1/1/1753 12:00:00 AM e 31/12/9999 11:59:59 PM.
                DateTime min = new DateTime(1753, 1, 1, 12, 00, 00);
                DateTime max = new DateTime(9999, 12, 31, 23, 59, 59);

                DateTime dtDefault = new DateTime(1900, 1, 1);

                if (DBNull.Value == value[columnName])
                    return dtDefault;

                if ((((DateTime)value[columnName]) < min) || (((DateTime)value[columnName]) > max))
                    return dtDefault;

                return (DateTime)value[columnName];
            }
            catch (IndexOutOfRangeException ex)
            {
                return (DateTime?)null;
            } 
        }

        public static bool? GetNullableBoolean(object value)
        {
            if (value == null)
                return null;

            if (value.ToString().RemoveSpecialChars() == "TRUE")
                return true;

            if (value.ToString().RemoveSpecialChars() == "S")
                return true;

            if (value.ToString().RemoveSpecialChars() == "Y")
                return true;

            if (value.ToString().RemoveSpecialChars() == "N")
                return false;

            if (value.ToString().RemoveSpecialChars() == "FALSE")
                return false;

            if (Convert.ToInt32(value) == 1)
                return true;

            if (Convert.ToInt32(value) == 0)
                return false;

            if (Convert.ToBoolean(value))
                return true;
            else
                return false;
        }

        public static int RowCount(this IDataReader dr)
        {
            DataTable tb = new DataTable();
            tb.Load(dr);

            //int rowCount = 0;

            //while (dr.Read())
            //{
            //    rowCount++;
            //}

            return tb.Rows.Count;
        }

        public static DbConnection CreateAndOpenConnection(this Database db)
        {
            DbConnection conn = db.CreateConnection();

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            return conn;
        }

        public static List<T> GetAll<T>(string dbName, string sqlStatement, string recordsCountStatement, ConverterDelegate<T> convert, object[] parms = null)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>(dbName);
            
            BackgroundWorker _bgWorker = null;

            if (parms != null)
            {
                foreach (object item in parms)
                {
                    if (item.GetType() == typeof(BackgroundWorker))
                        _bgWorker = (BackgroundWorker)item;
                }
            }

            if (_bgWorker != null)
                _bgWorker.ReportProgress(0, "Gerando contadores...");
            
            double totalRecords = 0;

            if ((_bgWorker != null) || (!String.IsNullOrEmpty(recordsCountStatement)))
            {
                using (DbCommand countCmd = database.GetSqlStringCommand(recordsCountStatement))
                {
                    totalRecords = Convert.ToDouble(database.ExecuteScalar(countCmd));
                }
            }

            int processedRecords = 0;            

            using (DbCommand command = database.GetSqlStringCommand(sqlStatement))
            {
                var list = new List<T>();
                var reader = database.ExecuteReader(command);

                if (_bgWorker != null)
                    _bgWorker.ReportProgress(0, "Buscando registros...");

                while (reader.Read())
                {
                    try
                    {
                        var obj = convert(reader);
                        list.Add(obj);

                        processedRecords++;

                        if ((_bgWorker != null) || (totalRecords > 0))
                            _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100));
                    }
                    catch (Exception ex)
                    {
                        if ((_bgWorker != null) || (totalRecords > 0))
                            _bgWorker.ReportProgress(Convert.ToInt32(processedRecords / totalRecords * 100), String.Format("Não foi possível exportar a disciplina da grade: Motivo:{0}", ex.Message));
                    }
                }

                return list;
            }
        }

        public static T Get<T>(string dbName, string sqlStatement, ConverterDelegate<T> convert, object[] parms = null)
        {
            Database database = ApplicationSingleton.Instance.Container.Resolve<Database>(dbName);

            using (DbCommand command = database.GetSqlStringCommand(sqlStatement))
            {
                T t = default(T);

                var reader = command.ExecuteReader();

                if (reader.Read())
                    t = convert(reader);

                return t;
            }
        }
    }
}
