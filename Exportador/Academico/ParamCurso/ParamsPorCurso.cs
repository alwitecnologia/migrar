using System;
using FileHelpers;
using FileHelpers.Converters;
namespace Exportador.Academico.ParamCurso
{
    [DelimitedRecord(";")]
    public sealed class ParamsPorCurso
    {

        public Int32 CodColigada;

        public String CodPerLet;

        public String CodCurso;

        public String CodHabilitacao;

        public String CodGrade;

        public String Turno;

        public Int32 CodFilial;

        public Int32 CodTipoCurso;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtNumAutomatica;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DiInicioMatricula;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtFimMatricula;

        public String HrInicioMatricula;

        public String HorFimMatricula;

        public Double PontuacaMinima;

        public Int32 MaximoAulas;

        public String PlanoPagto;

        public String PlanoPagtoServico;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtInicioAlteracaoPrograma;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtFimAlteracaoPrograma;

        public String HrInicioAlteracaoPrograma;

        public String HrFimAlteracaoPrograma;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtInicioAutEspecial;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtFimAutEspecial;

        public String HrInicioAutEspecial;

        public String HrFimAutEspecial;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtLimiteTrancamento;

        public Int32 CodColCxa;

        public String CodCxa;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtCompetenciaInicial;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtCompetenciaFinal;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtCompetenciaInicialMov;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtCompetenciaFinalMov;

    }
}
