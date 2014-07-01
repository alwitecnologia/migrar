using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FileHelpers;

namespace Exportador
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationSingleton.Instance.ConfigureContainer();
            Application.Run(new MainForm());
        }
    }
}
