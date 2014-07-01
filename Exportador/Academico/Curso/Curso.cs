using System;
using FileHelpers;
namespace Exportador.Academico.Curso
{
    [DelimitedRecord(";")]
    public sealed class Curso
    {

        public Int32 CodColigada;

        public Int32 CodTipoCurso;

        public String CodCurso;

        public String Nome;

        public String Descricao;

        public String Complemento;

        public String CodCursoINEP;

        public String Decreto;

        public String RegContrato;

        public String MascaraRA;

        public String Habilitacao;

        public String CodCapes;

        public String CursoPresDist;

        public String CodModalidadeCurso;

        public String Escola;

        public String Area;

        public String MascaraTurma;

    }

    public static class TipoCurso 
    { 
        public static int EnsinoSuperiorGraduacaoLagesSC = 1;
        public static int PosGraduacaoMestrado = 2;
        public static int Extensao = 3;
        public static int EnsinoSuperiorGraduacaoSaoJoaquimSC = 7;
    }
}