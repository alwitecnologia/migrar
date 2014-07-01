using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exportador.Helpers
{
    public class ConnectionStringHelper
    {
        public static string GetDatabaseName(string connString) 
        {
            string databaseName="";

            string[] parameters = connString.Split(';');
            foreach (string param in parameters)
            {
                if (param.Contains("Initial Catalog") || param.Contains("Database"))
                {
                    databaseName = param.Substring(param.IndexOf("=")+1);
                }
            }

            return databaseName;
        }
    }
}
