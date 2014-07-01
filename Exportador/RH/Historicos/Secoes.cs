using System;
using FileHelpers;

namespace Exportador.RH.Historicos
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class Secoes
    {
        [FieldFixedLength(16,";")]
        public String Chapa;

        [FieldFixedLength(8, ";")]
        public DateTime DtMudanca;

        [FieldFixedLength(2, ";")]
        public String CodMotivoMudanca;

        [FieldFixedLength(35, ";")]
        public String CodSecao;
    }
}