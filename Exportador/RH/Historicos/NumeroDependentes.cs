using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Historicos
{
    [DelimitedRecord(";")]
    public sealed class NumeroDependentes
    {
        public String Chapa;

        public String Dataadm;

        public String IncideSalFamilia;

        public String IncideIRRF;
    }
}
