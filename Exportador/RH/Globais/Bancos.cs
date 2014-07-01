using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Globais
{
    [DelimitedRecord(";")]
    public sealed class Bancos
    {
        public String NUMBANCO;

        public String NOME;

        public String NOMEREDUZIDO;

        public String MASCCONTA;

        public String NUMEROOFICIAL;

        public String DIGBANCO;
    }
}