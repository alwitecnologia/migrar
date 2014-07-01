using System;
using FileHelpers;
namespace Exportador.RH.Horario
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class DiaSemBatida
    {

        [FieldFixedLength(2)]
        public String TipoRegistro="05";

        [FieldFixedLength(10)]
        public String CodHorario;

        [FieldFixedLength(4)]
        public Int32 IndiceDia;

    }
}