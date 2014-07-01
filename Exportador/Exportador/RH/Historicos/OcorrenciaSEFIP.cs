using System;
using FileHelpers;

namespace Exportador.RH.Historicos
{
    [DelimitedRecord(";")]
    public sealed class OcorrenciaSEFIP
    {

        public String Chapa;

        public DateTime DtMudanca;

        public Int32 CodOcorrencia;

    }
}