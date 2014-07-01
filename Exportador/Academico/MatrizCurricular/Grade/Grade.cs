using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.Academico.MatrizCurricular.Grade
{
    [DelimitedRecord(";")]
    public sealed class Grade
    {

        public Int32 CodColigada;

        public String CodCurso;

        public String CodHabilitacao;

        public String CodGrade;

        public String Descricao;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtInicio;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtFim;

        public Int32 CargaHoraria;

        public String ControleVagas;

        public String Status;

        public String CodCursoProx;

        public String CodHabilitacaoProx;

        public String CodGradeProx;

        public Int32 MaxCredPeriodo;

        public Int32 MinCredPeriodo;

        public String Regime;

        public String TipoAtividadeCurricular;

        public String TipoEletiva;

        public String TipoOptativa;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtDOU;

        public Double TotalCreditos;

    }
}