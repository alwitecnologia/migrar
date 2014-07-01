using FileHelpers;
using System;
using FileHelpers.Converters;
using System.Collections.Generic;

namespace Exportador.Academico.Pessoa
{

    [DelimitedRecord(";")]
    public sealed class Pessoa
    {
        public Int32 Codigo;

        public String Nome;

        public String Apelido;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtNascimento;

        public String EstadoCivil;

        public String Sexo;

        public String Naturalidade;

        public String EstadoNatal;

        public String Nacionalidade;

        public String GrauInstrucao;

        public String Rua;

        public String Numero;

        public String Complemento;

        public String Bairro;

        public String Estado;

        public String Cidade;

        public String CEP;

        public String Pais;

        public String RegProfissional;

        public String CPF;

        public String Telefone1;

        public String Telefone2;

        public String Telefone3;

        public String Fax;

        public String EMail;

        public String CartIdentidade;

        public String UFCartIdent;

        public String OrgEmissorIdent;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtEmissaoIdent;

        public String TituloEleitor;

        public String ZonaTitEleitor;

        public String SecaoTitEleitor;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtTitEleitor;

        public String EstEleit;

        public String CarteiraTrab;

        public String SerieCartTrab;

        public String UFCartTrab;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtCartTrab;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? NIT;

        public String CartMotorista;

        public String TipoCartHabilit;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtVencHabilit;

        public String SitMilitar;

        public String CertifReserv;

        public String CategMilitar;

        public String CSM;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtExpCml;

        public String Exped;

        public String RM;

        public String NPassaporte;

        public String PaisOrigem;

        public String DtEmissPassaporte;

        [FieldConverter(typeof(DateTimeNullableConverter), "yyyy-MM-dd")]
        public DateTime? DtValPassaporte;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CorRaca;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? DeficienteFisico;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? DeficienteAuditivo;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool DeficienteFala;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? DeficienteVisual;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? DeficienteMental;

        public String RecursoRealizacaoTrab;

        public String RecursoAcessibilidade;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Profissao;

        public String Empresa;

        public String Ocupacao;

        public String TipoSang;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? Aluno;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? Professor;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool UsuarioBiblios;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? Funcionario;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? ExFuncionario;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? Candidato;

    }
}