using System;
using FileHelpers;
namespace Exportador.RH.Historicos
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class Situacoes
    {

        [FieldFixedLength(16,";")]
        public String Chapa;

        [FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DtMudanca;

        [FieldFixedLength(2, ";")]
        public String CodMotivoMudanca;

        [FieldFixedLength(1, "")]
        public String NovaSituacao;

    }
}