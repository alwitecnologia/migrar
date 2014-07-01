using System;
using FileHelpers;

namespace Exportador.RH.Historicos
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class Funcoes
    {
        [FieldFixedLength(16,";")]
        public String Chapa;

        [FieldFixedLength(10, ";")]
        public DateTime DtMudanca;

        [FieldFixedLength(2, ";")]
        public String CodMotivoMudanca;

        [FieldFixedLength(10, ";")]
        public String CodFuncao;

        [FieldFixedLength(10, ";")]
        public String NivelSalarial;

        [FieldFixedLength(10, ";")]
        public String FaixaSalarial;
    }
}