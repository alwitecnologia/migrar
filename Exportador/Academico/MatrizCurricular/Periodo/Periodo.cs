using System;
using FileHelpers;
using FileHelpers.Converters;
namespace Exportador.Academico.MatrizCurricular.Periodo
{
    [DelimitedRecord(";")]
    public sealed class Periodo
    {

        public Int32 CodColigada;

        public String CodCurso;

        public String CodHabilitacao;

        public String CodGrade;

        public Int32 CodPeriodo;

        public String Descricao;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? ValorEletiva;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? ValorOptativa;

    }
}