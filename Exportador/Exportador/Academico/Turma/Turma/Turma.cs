using System;
using FileHelpers;
using FileHelpers.Converters;
namespace Exportador.Academico.Turma.Turma 
{
    [DelimitedRecord(";")]
    public sealed class Turma
    {
        public Int32 CodColigada;

        public String CodCurso;

        public String CodHabilitacao;

        public String CodGrade;

        public String Turno;

        public Int32 CodFilial;

        public Int32 CodTipoCurso;

        public String CodPerLet;

        public String CodTurma;

        public String CodDepartamento;

        public String CodPredio;

        public String CodSala;

        public String CodCCusto;

        public String NomeRed;

        public String Nome;

        public Int32 MaxAlunos;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtInicial;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtFinal;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? AlunosLabore;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtAlunosLabore;

        public String CodTurmaProx;

        public String CodCampus;

        public String CodBloco;
    }
}