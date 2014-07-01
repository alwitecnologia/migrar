using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.Unity;

namespace Exportador.Log
{
    public sealed class Logger
    {
        private static Logger _instance;

        private Logger()
        {
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Logger();

                return _instance;
            }
        }

        /// <summary>
        /// Log the messsage in the log file.
        /// </summary>
        /// <param name="path">Folder where the log files will be created.</param>
        /// <param name="message">The message to log.</param>
        public void Trace(string path, string message)
        {
            int sequentialFile = 1;

            string fileName = String.Format("logExport_{0}_{1}.txt", DateTime.Now.ToString("yyyy_MM_dd"), sequentialFile.ToString().PadLeft(3, '0'));

            string fullFilename = Path.Combine(path, fileName);

            if (File.Exists(fullFilename))
            {
                while (File.ReadAllBytes(fullFilename).Length > 5242880)
                {
                    sequentialFile++;

                    fileName = String.Format("logExport_{0}_{1}.txt", DateTime.Now.ToString("yyyy_MM_dd"), sequentialFile.ToString().PadLeft(3, '0'));

                    fullFilename = Path.Combine(path, fileName);
                }
            }

            File.AppendAllText(fullFilename, message + System.Environment.NewLine);




        }
    }
}
