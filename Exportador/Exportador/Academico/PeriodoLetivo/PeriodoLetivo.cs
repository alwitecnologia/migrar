using System;
using FileHelpers;
using FileHelpers.Converters;
namespace Exportador.Academico.PeriodoLetivo
{
    [DelimitedRecord(";")]
    public sealed class PeriodoLetivo
    {

        public Int32 CodColigada;

        public Int32 CodFilial;

        public Int32 CodTipoCurso;

        public String CodPeriodoLetivo;

        public String Descricao;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? DiasLetivos;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CargaHoraria;

        public String Observacao;

        [FieldConverter(ConverterKind.Boolean, "S", "N")]
        public bool Encerrado;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtInicio;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtPrevista;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtFim;

        public String Calendario;

        public String CodPeriodoLetivoAnterior;

        [FieldConverter(typeof(BooleanNullableConverter), "S", "N")]
        public bool? EncerradoPgto;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtCompetenciaInicial;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? dtCompetenciaFinal;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtCompetencialMov;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtCompetenciaFinalMov;


    }
}