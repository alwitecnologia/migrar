using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.RH.Historicos
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class Afastamento
    {

        [FieldFixedLength(16,";")]
        public String Chapa;

        [FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DataInicioAfastamento;

        [FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DataFinalAfastamento;

        [FieldFixedLength(1, ";")]
        public String CodTipoAfastamento;

        [FieldFixedLength(2, ";")]
        public String CodMotivoAfastamento;

        [FieldFixedLength(50, ";")]
        public String Observacao;

        [FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? EstornaTempoServico;

        [FieldFixedLength(25, ";")]
        public String CodTomador;

        [FieldFixedLength(1, ";")]
        public Int32 TipoTomador;

        [FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DataRequerimento;

        [FieldFixedLength(20, ";")]
        public String CEI;

    }

    public sealed class TipoAfastamento 
    {
        public static readonly string LicencaMaternidade = "E";
        public static readonly string AposInvalodez = "I";
        public static readonly string LicencaSemVencimento = "L";
        public static readonly string ServMilitar = "M";
        public static readonly string AfPrevidencia = "P";
        public static readonly string LicencaRemunerada = "R";
        public static readonly string AfastamentoAcidenteTrabalho = "T";
        public static readonly string Outros = "U";
    }
}