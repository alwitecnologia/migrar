using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.Academico.DocumentoAluno
{
    [DelimitedRecord(";")]
    public sealed class DocumentoAluno
    {
        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodColigada;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodFilial;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodTipoCurso;

        public String CodPerLet;

        public String RA;

        public String DescDocumento;

        public String Observacao;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtEntrega;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtPrevista;

        public String CodUsuario;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Quantidade;
    }
}