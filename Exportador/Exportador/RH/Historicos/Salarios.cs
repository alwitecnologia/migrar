using System;
using FileHelpers;
using FileHelpers.Converters;
namespace Exportador.RH.Historicos 
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class Salarios
    {

        [FieldFixedLength(16)]
        public String Chapa;

        [FieldFixedLength(14)]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyyhhmmss")]
        public DateTime? DtMudanca;

        [FieldFixedLength(2)]
        public String CodMotivo;

        [FieldFixedLength(2)]
        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? NumSalario;

        [FieldFixedLength(15)]
        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? ValorSalario;

        [FieldFixedLength(6)]
        public String Jornada;

        [FieldFixedLength(14)]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyyhhmmss")]
        public DateTime? DtProcessamento;

        [FieldFixedLength(4)]
        public String CodEvento;

    }
}