using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.Academico.PessoaFiltro
{
    [DelimitedRecord(";")]
    public sealed class PessoaFiltro
    {
        public String Codigo;

        public String Nome;

        public String Apelido;

        
        public String DtNascimento;

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

        
        public String DtEmissaoIdent;

        public String TituloEleitor;

        public String ZonaTitEleitor;

        public String SecaoTitEleitor;

        
        public String DtTitEleitor;

        public String EstEleit;

        public String CarteiraTrab;

        public String SerieCartTrab;

        public String UFCartTrab;

        
        public String DtCartTrab;

        
        public String NIT;

        public String CartMotorista;

        public String TipoCartHabilit;

        
        public String DtVencHabilit;

        public String SitMilitar;

        public String CertifReserv;

        public String CategMilitar;

        public String CSM;

        
        public String DtExpCml;

        public String Exped;

        public String RM;

        public String NPassaporte;

        public String PaisOrigem;

        public String DtEmissPassaporte;

        
        public String DtValPassaporte;

        public String CorRaca;

        
        public String DeficienteFisico;

        
        public String DeficienteAuditivo;

        
        public String DeficienteFala;

        
        public String DeficienteVisual;

        
        public String DeficienteMental;

        public String RecursoRealizacaoTrab;

        public String RecursoAcessibilidade;

        public String Profissao;

        public String Empresa;

        public String Ocupacao;

        public String TipoSang;

        
        public String Aluno;

        
        public String Professor;

        
        public String UsuarioBiblios;

        
        public String Funcionario;

        
        public String ExFuncionario;

        
        public String Candidato;
    }
}