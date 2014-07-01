using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Ferias
{
    [DelimitedRecord(";")]
    public sealed class VerbasFerias
    {
        public String CHAPA;

        public String FIMPERAQUIS;

        public String DATAPGTO;

        public String CodEvento;

        public String HORA;

        public String REF;

        public String VALOR;

        public String ALTERADOMANUAL;

    }
}
