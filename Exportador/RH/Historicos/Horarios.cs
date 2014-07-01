using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.RH.Historicos
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class Horarios
    {

        [FieldFixedLength(16, ";")]
        public String Chapa;

        [FieldFixedLength(8, ";")]
        public DateTime DtMudanca;

        [FieldFixedLength(10, ";")]
        public String CodHorario;

        [FieldFixedLength(2, ";")]
        public Int32 IndiceInicioHorario;

    }
}