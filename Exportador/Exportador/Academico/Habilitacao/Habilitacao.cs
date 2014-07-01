using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.Academico.Habilitacao
{
    [DelimitedRecord(";")]
    public sealed class Habilitacao
    {
        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodColigada;

        public String CodCurso;

        public String CodHabilitacao;

        public String Nome;

        public String Descricao;

        public String Complemento;

        public String Complemento2;

        public String CodCursoHist;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodSerieHist;

        public String TextoConclusao;

        public String Decreto;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? Integralizacao;

        public String CodHabinep;

        [FieldConverter(typeof(DateTimeNullableConverter))]
        public DateTime? DtProvao;

        public String Juramento;

    }
}