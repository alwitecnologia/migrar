using System;
using FileHelpers;
using FileHelpers.Converters;
namespace Exportador.Academico.Matricula.HabilitacaoAluno
{
    [DelimitedRecord(";")]
    public sealed class HabilitacaoAluno
    {

        public Int32 CodColigada;

        public String CodCurso;

        public String CodHabilitacao;

        public String CodGrade;

        public String Turno;

        public Int32 CodFilial;

        public Int32 CodTipoCurso;

        public String RA;

        public String Ingresso;

        public String Instituicao;

        public String Status;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtIngresso;

        public String PontosVestibular;

        public String ClassificacaoVestibular;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? MediaVestibular;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtColacaoGrau;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtEmissaoDiploma;

        public String RegistroConclusao;

        public String LivroRegistro;

        public String PaginaRegistro;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtConclusaoCurso;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? CR;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? MediaGlobal;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtProvao;

        public String ProcessoRegistro;

        public String InstituicaoDiploma;

        public String RealizouProvao;

        public String CodCursoTransf;

        public String CodHabilitacaoTransf;

        public String CodGradeTransf;

        public String TurnoTransf;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodTipoCursoTransf;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodFilialTransf;

        public String MotivoTransf;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? IndiceCarencia;

        public String Observacao;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodInstituicao;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodInstituicaoDiploma;

        public String Campus;

        public String LocalizacaoFisica;


    }
}