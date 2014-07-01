using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exportador.Academico.Pessoa;
using Exportador.Academico.Professor;

namespace ConsoleApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ExportadorPessoa exp = new ExportadorPessoa();
            exp.Exportar();


        }
    }
}
