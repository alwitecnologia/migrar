using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.Academico.MatrizAplicada
{
    [DelimitedRecord(";")]
    public sealed class HabilitacaoFilial
    {

        public Int32 CodColigada;

        public Int32 CodFilial;

        public Int32 CodTipoCurso;

        public String CodCurso;

        public String CodHabilitacao;

        public String CodGrade;

        public String Turno;

        public String CodCCusto;

        public String Ativo;

        public String CodDepartamento;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtAutorizacao;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtDOUAutorizacao;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtResolucao;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtReconhecimento;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtDOUReconhecimento;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtInicioCurso;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtFimCurso;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtInicioSuspensao;

        public String PortAutorizacao;

        public String portReconhecimento;

        public String NumResolucao;

        public String EmailCoordenacao;

        public String DecretoCurso;

        public String DecretoHabilitacao;

        public String DescricaoCurso;

        public String DescricaoHabilitacao;


    }
}