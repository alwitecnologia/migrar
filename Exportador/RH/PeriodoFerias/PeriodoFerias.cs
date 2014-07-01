using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.RH.PeriodoFerias
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class PeriodoFerias
    {

        [FieldFixedLength(8,";")]
        public String ChapaFunc;

        [FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? DtVencimento;

        [FieldFixedLength(8, ";")]
        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? NumeroPeriodo;

        [FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? DtPagamento;

        [FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? DtAviso;

        [FieldFixedLength(80, ";")]
        public String Observacao;

        [FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? BaseINSS_Mes;

        [FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? BaseINSS_ProxMes;

    }
}