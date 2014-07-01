using System;
using FileHelpers;

namespace Exportador.RH.Dependente
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class Dependente
    {

        [FieldFixedLength(16,";")]
        public String Chapa;

        [FieldFixedLength(2, ";")]
        public Int32 NumDependente;

        [FieldFixedLength(120, ";")]
        public String Nome;

        [FieldFixedLength(12, ";")]
        public String CPF;

        [FieldFixedLength(8, ";")]
        public DateTime DtNasc;

        [FieldFixedLength(1, ";")]
        public String Sexo;

        [FieldFixedLength(1, ";")]
        public String EstadoCivil;

        [FieldFixedLength(32, ";")]
        public String LocalNascimento;

        [FieldFixedLength(40, ";")]
        public String NomeCartorio;

        [FieldFixedLength(10, ";")]
        public String NumRegistro;

        [FieldFixedLength(8, ";")]
        public String NumLivroRegistro;

        [FieldFixedLength(5, ";")]
        public String NumFolhaRegistro;

        [FieldFixedLength(1, ";")]
        public Int32 IncideIRRF;

        [FieldFixedLength(1, ";")]
        public Int32 IncideINSS;

        [FieldFixedLength(1, ";")]
        public Int32 incideAssisMedica;

        [FieldFixedLength(1, ";")]
        public Int32 IncidePensao;

        [FieldFixedLength(20, ";")]
        public String IncidenciasDefiniveis;

        [FieldFixedLength(1, ";")]
        public String GrauParentesco;

        [FieldFixedLength(1, ";")]
        public String PossuiCartaoVacina;

        [FieldFixedLength(8, ";")]
        public DateTime DtEntregaCertNascimento;

        [FieldFixedLength(1, ";")]
        public String ApresentouComprFreqEscolar;

        [FieldFixedLength(1, ";")]
        public String UniversitarioEscolaTecnica2Grau;

        [FieldFixedLength(1, ";")]
        public String IncideSalFamilia;

        [FieldFixedLength(6, ";")]
        public String AgPagtoPensao;

        [FieldFixedLength(3, ";")]
        public String BancoPagtoPensao;

        [FieldFixedLength(1, ";")]
        public String CalculoPensaoSobreBruto;

        [FieldFixedLength(15, ";")]
        public String ContaPagtoPensao;

        [FieldFixedLength(8, ";")]
        public String FormulaAdicionalPensao;

        [FieldFixedLength(8, ";")]
        public String FornulaCalculo;

        [FieldFixedLength(9, ";")]
        public String PercentualPensao;

        [FieldFixedLength(2, ";")]
        public String TipoPensao;

        [FieldFixedLength(8, ";")]
        public DateTime DtInicioDescontoPensao;

        [FieldFixedLength(45, ";")]
        public String NomeResponsavel;

        [FieldFixedLength(80, ";")]
        public String Observacao;

        [FieldFixedLength(5, ";")]
        public Int32 ColigadaFornecedor;

        [FieldFixedLength(25, ";")]
        public String CodFornecedor;

        [FieldFixedLength(5, ";")]
        public String OperacaoBancaria;

    }
}