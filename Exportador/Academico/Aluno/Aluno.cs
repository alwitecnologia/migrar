using FileHelpers;
using System;
using FileHelpers.Converters;

namespace Exportador.Academico.Aluno
{

    [DelimitedRecord(";")]
    public sealed class Aluno
    {
        public String Nome;

        public String Sobrenome;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtNascimento;

        public String CPF;

        public String CartIdentidade;

        public String UFCartIdent;

        public String CarteiraTrab;

        public String SerieCartTrab;

        public String UFCartTrab;

        public String EmpresaNome;

        public String EmpresaRua;

        public String EmpresaNumero;

        public String EmpresaComplemento;

        public String EmpresaBairro;

        public String EmpresaCep;

        public String EmpresaCidade;

        public String EmpresaUF;

        public String EmpresaTelefone;

        public String EmpresaHorario;

        public String TipoCertidao;

        public String CertNumero;

        public String CertCartorio;

        public String CertComarca;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? CertData;

        public String CertFolha;

        public String CertLivro;

        public String CertDistrito;

        public String CertUF;

        public String NomePai;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtNascimentoPai;

        public String CpfPai;

        public String RgPai;

        [FieldConverter(typeof(BooleanNullableConverter), "S","N")]
        public bool? PaiVivo;

        public String NomeMae;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtNascimentoMae;

        public String CpfMae;

        public String RgMae;

        [FieldConverter(typeof(BooleanNullableConverter), "S", "N")]
        public bool? MaeViva;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodColigada;

        public String RA;

        public String TipoAluno;

        public String InstDestino;

        public String InstOrigem;

        public String CodCurHist;

        public String CodSerieHist;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodColCFO;

        public String CodCfo;

        public String CodParentCfo;

        public String NomeRespAcad;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtNascimentoRespAcad;

        public String CpfRespAcad;

        public String RgRespAcad;

        public String CodParentRaca;

        public String ObsHist;

        public String Identificador2;

        public String Identificador3;

        public String AnoIngresso;

        public String Anotacoes;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodTipoCurso;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodInstOrigem;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodInstDestino;

        public String Naturalidade;

        public String EstadoNatal;

        public String NaturalidadePai;

        public String EstadoNatalPai;

        public String NaturalidadeMae;

        public String EstadoNatalMae;

        public String NaturalidadeAcad;

        public String EstadoNatalAcad;

        public String CodSistec;

        [FieldIgnored()]
        public Int32 IdAluno;
    }
}