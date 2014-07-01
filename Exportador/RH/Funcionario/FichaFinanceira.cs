using System;
using FileHelpers;
using FileHelpers.Converters;
namespace Exportador.RH.Funcionario
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class FichaFinanceira
    {

        [FieldFixedLength(16,";")]
        public String Chapa;

        [FieldFixedLength(4, ";")]
        public Int32 AnoCompetencia;

        [FieldFixedLength(2, ";")]
        public Int32 MesCompetencia;

        [FieldFixedLength(2, ";")]
        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? NumPeriodo;

        [FieldFixedLength(4, ";")]
        public String CodEvento;

        [FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter))]
        public DateTime? DtPagto;

        [FieldFixedLength(6, ";")]
        public String Hora;

        [FieldFixedLength(6, ";")]
        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? Referencia;

        [FieldFixedLength(15, ";")]
        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? Valor;

        [FieldFixedLength(15, ";")]
        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? ValorOriginal;

    }
}