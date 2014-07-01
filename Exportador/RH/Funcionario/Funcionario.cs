using FileHelpers;
using System;
using FileHelpers.Converters;

namespace Exportador.RH.Funcionario
{
    //[FixedLengthRecord(FixedMode.ExactLength,";")]
    [DelimitedRecord(";")]
    public sealed class Funcionario
    {
        //[FieldFixedLength(16,";")]
        public String Chapa;

        //[FieldFixedLength(120,";")]
        public String Nome;

        //[FieldFixedLength(40, ";")]
        public String Apelido;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? Nascimento;

        //[FieldFixedLength(1, ";")]
        public String EstadoCivil;

        //[FieldFixedLength(1, ";")]
        public String Sexo;

        //[FieldFixedLength(2, ";")]
        public String Nacionalidade;

        //[FieldFixedLength(1, ";")]
        public String GrauInstrucao;

        //[FieldFixedLength(30, ";")]
        public String Rua;

        //[FieldFixedLength(8, ";")]
        public String Numero;

        //[FieldFixedLength(16, ";")]
        public String Complemento;

        //[FieldFixedLength(20, ";")]
        public String Bairro;

        //[FieldFixedLength(2, ";")]
        public String Estado;

        //[FieldFixedLength(32, ";")]
        public String Cidade;

        //[FieldFixedLength(9, ";")]
        public String CEP;

        //[FieldFixedLength(16, ";")]
        public String Pais;

        //[FieldFixedLength(15, ";")]
        public String RegistroProfissional;

        //[FieldFixedLength(12, ";")]
        public String CPF;

        //[FieldFixedLength(15, ";")]
        public String Telefone1;

        //[FieldFixedLength(15, ";")]
        public String Telefone2;

        //[FieldFixedLength(15, ";")]
        public String CarteiraIdentidade;

        //[FieldFixedLength(2, ";")]
        public String UFCartIdentidade;

        //[FieldFixedLength(15, ";")]
        public String OrgaoEmissorCartIdentidade;
        
        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? EmissaoCartIdentidade;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? VencCartIdentidade;

        //[FieldFixedLength(14, ";")]
        public String TituloEleitor;

        //[FieldFixedLength(6, ";")]
        public String ZonaVotacao;

        //[FieldFixedLength(6, ";")]
        public String SecaoVotacao;

        //[FieldFixedLength(10, ";")]
        public String CarteiraTrabalho;

        //[FieldFixedLength(5, ";")]
        public String SerieCarteiraTrabalho;

        //[FieldFixedLength(2, ";")]
        public String UFCarteiraTrabalho;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? EmissaoCarteiraTrabalho;

        //[FieldFixedLength(10, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? VencCarteiraTrabalho;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? NIT_TipoCarteiraTrabalho;

        //[FieldFixedLength(15, ";")]
        public String CarteiraMotorista;

        //[FieldFixedLength(5, ";")]
        public String TipoCarteiraMotorista;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? VencCartMotorista;

        //[FieldFixedLength(40, ";")]
        public String CertifReservista;

        //[FieldFixedLength(10, ";")]
        public String CategMilitar;

        //[FieldFixedLength(32, ";")]
        public String Naturalidade;

        //[FieldFixedLength(2, ";")]
        public String EstadoNatal;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? ChegadaAoBrasil;

        //[FieldFixedLength(15, ";")]
        public String CartaModelo19;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? ConjugeBrasil;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? Naturalizado;
                
        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? FilhosNoBrasil;

        //[FieldFixedLength(2, ";")]
        public Int32 NumFilhosNoBrasil;

        //[FieldFixedLength(15, ";")]
        public String NumRegistroGeral;

        //[FieldFixedLength(15, ";")]
        public String NumDecreto;

        //[FieldFixedLength(10, ";")]
        public String TipoVisto;

        //[FieldFixedLength(60, ";")]
        public String eMail;

        //[FieldFixedLength(10, ";")]
        public String SenhaEMail;

        //[FieldFixedLength(10, ";")]
        public Int32 FichaRegistro;

        //[FieldFixedLength(1, ";")]
        public String TipoRecebimento;

        //[FieldFixedLength(1, ";")]
        public String Situacao;

        //[FieldFixedLength(1, ";")]
        public String TipoFuncionario;

        //[FieldFixedLength(35, ";")]
        public String CodSecao;

        //[FieldFixedLength(10, ";")]
        public String CodFuncao;

        //[FieldFixedLength(10, ";")]
        public String CodSindicato;

        //[FieldFixedLength(6, ";")]
        public String Jornada;

        //[FieldFixedLength(10, ";")]
        public String CodHorario;

        //[FieldFixedLength(10, ";")]
        public Int32 nDependentesIRRF;

        //[FieldFixedLength(2, ";")]
        public Int32 nDependentesSalarioFamilia;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? DataBase;

        //[FieldFixedLength(15, ";")]
        [FieldConverter(ConverterKind.Double, ",")]
        public Double Salario;

        //[FieldFixedLength(1, ";")]
        public String SituacaoFGTS;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? DataOpcaoFGTS;

        //[FieldFixedLength(11, ";")]
        public String NumContaFGTS;//aqui

        //[FieldFixedLength(15, ";")]
        [FieldConverter(ConverterKind.Double, ",")]
        public Double SaldoFGTSBanco;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime DataSaldoFGTS;

        //[FieldFixedLength(11, ";")]
        public String PIS_PASEP;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime DataCadastrosPIS;

        //[FieldFixedLength(3, ";")]
        public String CodBancoPIS;

        //[FieldFixedLength(1, ";")]
        public String ContribuicaoSindical;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? Aposentado;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? MaisDe65Anos;

        //[FieldFixedLength(15, ";")]
        [FieldConverter(ConverterKind.Double, ",")]
        public Double AjudaCusto;

        //[FieldFixedLength(6, ";")]
        [FieldConverter(ConverterKind.Double, ",")]
        public Double PercentualAdiantamento;

        //[FieldFixedLength(10, ";")]
        public String Arredondamento;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? DataAdmissao;

        //[FieldFixedLength(1, ";")]
        public String TipoAdmissao;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? DataTransferencia;

        //[FieldFixedLength(2, ";")]
        public String MotivoAdmissao;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? ContratoPrazoDeterminado;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? FimPrazoContrato;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(typeof(DateTimeNullableConverter), "ddMMyyyy")]
        public DateTime? DataDemissao;

        //[FieldFixedLength(1, ";")]
        public String TipoDemissao;

        //[FieldFixedLength(2, ";")]
        public String MotivoDemissao;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DataDesligamento;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime UltimaMovimentacao;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DataPagtoRescisao;

        //[FieldFixedLength(2, ";")]
        public String CodSaqueFGTS;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? AvisoPrevio;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DataAvisoPrevio;

        //[FieldFixedLength(3, ";")]
        public Int32 NumDiasAviso;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DataVencFerias;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime InicioProgFerias;

        //[FieldFixedLength(8, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime FimProgFerias;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? QuerAbono;

        
        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? Quer1aParcela13o;

        //[FieldFixedLength(3, ";")]
        public Int32 NDiasAdiantaFerias;

        //[FieldFixedLength(4, ";")]
        public String EventoAdiantaFerias;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? FeriasColetivasGlobais;

        //[FieldFixedLength(7, ";")]
        public Double NDiasFerias;

        //[FieldFixedLength(6, ";")]
        public Double NDiasAbono;

        //[FieldFixedLength(6, ";")]
        public Double SaldoFerias;

        //[FieldFixedLength(6, ";")]
        public Double SaldoFeriasAnterior;

        //[FieldFixedLength(6, ";")]
        public Double SaldoFeriasAuxiliar;

        //[FieldFixedLength(80, ";")]
        public String ObsFerias;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime DataPagtoFerias;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime DataAvisoFerias;

        //[FieldFixedLength(6, ";")]
        public Double NDiasLicencaRemunerada1;

        //[FieldFixedLength(6, ";")]
        public Double NDiasLicencaRemunerada2;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(5, ";")]
        public DateTime DataInicioLicenca;

        //[FieldFixedLength(15, ";")]
        public Double MediaSalMaternidade;

        //[FieldFixedLength(1, ";")]
        public String SituacaoRAIS;

        //[FieldFixedLength(3, ";")]
        public String CodBancoPgto;

        //[FieldFixedLength(6, ";")]
        public String CodAgenciaPgto;

        //[FieldFixedLength(15, ";")]
        public String ContaPgto;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MembroSindical;

        //[FieldFixedLength(1, ";")]
        public String VinculoRAIS;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? UsaValeTransporte;

        //[FieldFixedLength(2, ";")]
        public Int32 DaisUteisMes;

        //[FieldFixedLength(2, ";")]
        public Int32 DiasUteisMeioExpediente;

        //[FieldFixedLength(2, ";")]
        public Int32 DiasUteisProximoMes;

        //[FieldFixedLength(2, ";")]
        public Int32 DiasUteisProximoMesMeioExpediente;

        //[FieldFixedLength(2, ";")]
        public Int32 DiasUteisRestantes;

        //[FieldFixedLength(2, ";")]
        public Int32 DiasUteisRestatesMeio;

        //[FieldFixedLength(10, ";")]
        public String MudouEndereco;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MudouCartTrabalho;

        //[FieldFixedLength(10, ";")]
        public String AntigaCartTrabalho;

        //[FieldFixedLength(5, ";")]
        public String AntigaSerieCartTrabalho;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MudouNome;

        //[FieldFixedLength(45, ";")]
        public String AntigoNome;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MudouPIS;

        //[FieldFixedLength(11, ";")]
        public String AntigoPIS;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MudouChapa;

        //[FieldFixedLength(16, ";")]
        public String AntigaChapa;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MudouDataAdmissao;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime AntigaDataAdmissao;

        //[FieldFixedLength(1, ";")]
        public String AntigoVinculo;

        //[FieldFixedLength(1, ";")]
        public String AntigoTipoFuncionario;

        //[FieldFixedLength(1, ";")]
        public String AntigoTipoAdmissao;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MudouDataOpcao;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime AntigaDataOpcao;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MudouSecao;

        //[FieldFixedLength(35, ";")]
        public String AntigaSecao;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MudouDataNascimento;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime AntigaDataNascimento;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? FaltaAlterarFGTS;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? DeduzirIRRFMais65;

        //[FieldFixedLength(11, ";")]
        public String PISparaFGTS;

        //[FieldFixedLength(3, ";")]
        public String CodBancoFGTS;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? DescontaAvisoPrevio;

        //[FieldFixedLength(7, ";")]
        public Int32 CodFilial;

        //[FieldFixedLength(5, ";")]
        public String IndiceInicioHorario;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? UsaSalarioComposto;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? MembroCIPA;

        //[FieldFixedLength(5, ";")]
        public String OperacaoBancaria;

        //[FieldFixedLength(2, ";")]
        public Int32 NVezesDescontoEmprestimoFerias;

        //[FieldFixedLength(10, ";")]
        public String DataInicioDescontoEmprestimo;

        //[FieldFixedLength(10, ";")]
        public String GrupoSalarial;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? Atual;

        //[FieldFixedLength(8, ";")]
        public DateTime PrevisaoDisponibilidade;

        //[FieldFixedLength(2, ";")]
        public Int32 CodOcorrencia;

        //[FieldFixedLength(2, ";")]
        public Int32 CodCategoria;

        //[FieldFixedLength(8, ";")]
        public Int32 ClasseContribuicaoINSS;

        //[FieldFixedLength(4, ";")]
        public String CodEquipe;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? Supervisor;

        //[FieldFixedLength(22, ";")]
        public String IntegContabil;

        //[FieldFixedLength(22, ";")]
        public String IntegGerencial;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? UsaControleSaldoVerbas;

        //[FieldFixedLength(11, ";")]
        public String CodContribuinteIndividual;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1,";")]
        public bool? MudouCodContribuinteIndividual;

        //[FieldFixedLength(11, ";")]
        public Int32 AntigoCodContribuinteIndividual;

        //[FieldFixedLength(2, ";")]
        public Int32 PeriodoRescisao;

        //[FieldFixedLength(1, ";")]
        public Int32 CorRaca;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? DeficienteFisico;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? FGTSMesAnteriorSeraRecolhidoNaGRFC;

        //[FieldFixedLength(10, ";")]
        public String CodNivelTabelaSalarial;

        //[FieldFixedLength(15, ";")]
        public String NumDiasFeriasParajornadaReduzida;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "2")]
        public bool? AlvaraJudicialFuncMenor16;

        //[FieldFixedLength(1, ";")]
        public Int32 SituacaoINSS;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime DataAposentadoria;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? QuerAdiantamentoFerias;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime DataProximoPeriodoAquisitivoFerias;

        //[FieldFixedLength(5, ";")]
        public Int32 ColigadaFornecedor;

        //[FieldFixedLength(25, ";")]
        public String CodFornecedor;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? DeficienteAuditivo;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? DeficienteFala;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? DeficienteMental;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(1, ";")]
        public bool? DeficienteVisual;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(40, ";")]
        public String Localidade;

        //[FieldFixedLength(20, ";")]
        public String CodMunicipio;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(2, ";")]
        public bool? PosicaoAbono;

        //[FieldFixedLength(2, ";")]
        public Int32 NDiasFeriasCorridos;

        //[FieldFixedLength(20, ";")]
        public String PaisOrigem;

        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        //[FieldFixedLength(2, ";")]
        public bool? Fumante;

        //[FieldFixedLength(15, ";")]
        public String NumPassaporte;

        //[FieldFixedLength(15, ";")]
        public String Telefone3;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime DataEmissaoPassaporte;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        //[FieldFixedLength(8, ";")]
        public DateTime DataValidadePassaporte;

        //[FieldFixedLength(1, ";")]
        public Int32 TipoAposentadoria;

        //[FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "S", "N")]
        public bool? ReposicaoVaga;

        //[FieldFixedLength(15, ";")]
        public Double SaldoFGTSReal;

        //[FieldFixedLength(5, ";")]
        public Int32 BR_PDH;

        [FieldIgnored()]
        public Int32 Id;
   }

    public sealed class SituacaoFGTS
    {
        public static readonly string Optante = "1";
        public static readonly string NaoOptante = "2";
    }

    public sealed class ContribuicaoSindical
    {
        public static readonly string Descontou = "J";
        public static readonly string Liberal = "L";
        public static readonly string NaoDescontou = "N";
    }

    public sealed class EstadoCivil 
    {
        public static readonly string Casado = "C";
        public static readonly string Desquitado = "D";
        public static readonly string Divorciado = "I";
        public static readonly string Solteiro = "S";
        public static readonly string Viuvo = "V";
    }

    public sealed class Nacionalidade 
    {
        public static readonly int Brasileiro = 10;
        public static readonly int NaturalBrasileiro = 20;
        public static readonly int Argentino = 21;
        public static readonly int Boliviano = 22;
        public static readonly int Chileno = 23;
        public static readonly int Paraguaio = 24;
        public static readonly int Uruguaio = 25;
        public static readonly int Alemao = 30;
        public static readonly int Belga = 31;
        public static readonly int Britanico = 32;
        public static readonly int Canadense = 34;
        public static readonly int Espanhol = 35;
        public static readonly int NorteAmericano = 36;
        public static readonly int Frances = 37;
        public static readonly int Suico = 38;
        public static readonly int Italiano = 39;
        public static readonly int Japones = 41;
        public static readonly int Chines = 42;
        public static readonly int Coreano = 43;
        public static readonly int Portugues = 45;
        public static readonly int LatinoAmericano = 48;
        public static readonly int Asiatico = 49;
        public static readonly int Outros = 50;
    }

    public sealed class GrauInstrucao 
    {
        public static readonly string Analfabeto = "1";
        public static readonly string PrimarioIncompleto = "2";
        public static readonly string PrimarioCompleto = "3";
        public static readonly string GinasialIncompleto = "4";
        public static readonly string GinasialCompleto = "5";
        public static readonly string ColegialIncompleto = "6";
        public static readonly string ColegialCompleto = "7";
        public static readonly string SuperiorIncompleto = "8";
        public static readonly string SuperiorCompleto = "9";
        public static readonly string PosGradIncompleto = "A";
        public static readonly string PosGradCompleto = "B";
        public static readonly string MestradoIncompleto = "C";
        public static readonly string MestradoCompleto = "D";
        public static readonly string DoutoradoIncompleto = "E";
        public static readonly string DoutoradoCompleto = "F";
        public static readonly string PosDoutoradoIncompleto = "G";
        public static readonly string PosDoutoradoCompleto = "H";

    }

    public sealed class TipoRecebimento 
    {
        public static readonly string Diarista = "D";
        public static readonly string Horista = "H";
        public static readonly string Mensalista = "M";
        public static readonly string Outros = "O";
        public static readonly string Quinzenalista = "Q";
        public static readonly string Semanalista = "S";
        public static readonly string Tarefeiro = "T";
        public static readonly string ProfessorHorista = "P";
    }

    public sealed class SituacaoFuncionario
    {
        public static readonly string Ativo = "A";
        public static readonly string Demitido = "D";
        public static readonly string LicencaMaternidade = "E";
        public static readonly string Ferias = "F";
        public static readonly string AposInvalidez = "I";
        public static readonly string LicencaSemVencimento = "L";
        public static readonly string ServMilitar = "M";
        public static readonly string AfastPrevidencia = "P";
        public static readonly string LicencaRemunerada = "R";
        public static readonly string AfastAcidTrabalho = "T";
        public static readonly string Outros = "U";
        public static readonly string AvisoPrevio = "V";
        public static readonly string DemissaoNoMes = "X";
        public static readonly string AdmissaoProxMes = "Z";
    }

    public sealed class TipoFuncionario 
    {
        public static readonly string Autonomo = "A";
        public static readonly string Conselheiro = "C";
        public static readonly string Diretor = "D";
        public static readonly string EstatuarioCedido = "E";
        public static readonly string Cedido = "I";
        public static readonly string Misto = "M";
        public static readonly string Normal = "N";
        public static readonly string Comissionista = "O";
        public static readonly string Temporario = "P";
        public static readonly string Rural = "R";
        public static readonly string Pensionista = "S";
        public static readonly string Estagiario = "T";
        public static readonly string Expatriado = "X";
        public static readonly string Aprendiz = "Z";
    }

    public sealed class TipoAdmissao
    {
        public static readonly string EntTransfComOnus = "E";
        public static readonly string Reintegracao = "I";
        public static readonly string OutrosCasos = "O";
        public static readonly string PrimeiroEmprego = "P";
        public static readonly string EntTransfSemOnus = "T";
        public static readonly string Reemprego = "R";
    }

    public sealed class MotivoAdmissao
    {
        public static readonly string AumentoQuadro = "1";
        public static readonly string MudancaEstrategias = "2";
        public static readonly string RemanejamentoVagas = "3";
        public static readonly string Transferencia = "4";
        public static readonly string Substituicao = "5";
    }

    public sealed class TipoDemissao
    { 
        public static readonly string InicDoEmpregadorComJustaCausa = "1";
        public static readonly string InicDoEmpregadorSemJustaCausa = "2";
        public static readonly string InicDoEmpregadoComJustaCausa = "3";
        public static readonly string InicDoEmpregadoSemJustaCausa = "4";
        public static readonly string CessaoOutraAutoridadeSemOnus = "5";
        public static readonly string TransferenciaMesmaEmpresa = "6";
        public static readonly string ReformaOuTransfParaReserva = "7";
        public static readonly string Falecimento = "8";
        public static readonly string OutrosCasos = "9";
        public static readonly string AposentadoriaInvalidezAcidTrabalho = "A";
        public static readonly string CulpaReceproca = "C";
        public static readonly string AposentadoriaInvalidezDoenca = "D";
        public static readonly string FalecimentoPorAcidTrabalho = "F";
        public static readonly string AposPorIdadeComRescisaoDeContrato = "I";
        public static readonly string AposPorIdadeSemRescisaoDeContrato = "J";
        public static readonly string MudancaDeRegimeTrabalhista = "M";
        public static readonly string AposentadoriaInvalidezOutros = "O";
        public static readonly string FalecimentoPorDoencaProfissional = "P";
        public static readonly string AposTempoServicoComRescDeContrato = "R";
        public static readonly string AposTempoServicoSemRescDeContrato = "S";
        public static readonly string TerminoDeContratoDeTrabalho = "T";
        public static readonly string AposentadoriaCompulsoria = "U";
    }

    /* public sealed class CodSaqueFGTS
    { 
        public static readonly string SemDireitoASaque = "00";
        public static readonly string DespedidaRescisaoExoneracaoSemJustaCausa = "01";
        public static readonly string RescisaoPorCulpaReciprocaOuForcaMaior = "02";
        public static readonly string RescisaoPorExtincaoDaEmpresa = "03";
        public static readonly string ExtincaoNormalDoContratoATermo = "04";
        public static readonly string Aposentadoria = "05";
        public static readonly string SuspensaoDoTrabalhoAvulso = "06";
        public static readonly string RescisaoDeNaoOptanteComPagtoIndenizacao = "10";
        public static readonly string Falecimento = "23";
        public static readonly string RescisaoDeNaoOptanteSemPagtoIndenizacao = "26";
        public static readonly string EfetivacaoDepositoArt73OuPagtoArt6 = "27";
        public static readonly string PermContaVincPor3AnosSemCredito = "87";
        public static readonly string DeterminacaoJudicial = "88";
    } */

    public sealed class SituacaoRAIS
    { 
        public static readonly string AtivNormalCRemunLicRemunCDirInteg = "1";
        public static readonly string AfastadoPorAcidDeTrabalhoPorPeriodoSuperiorA15Dias = "2";
        public static readonly string AfastadoPorPrestacaoDeServicoMilitar = "3";
        public static readonly string AfastadoPorMotivoDeLicencaGestante = "4";
        public static readonly string AfastadoPorDoencaPorPeriodoSuperiorA15Dias = "5";
        public static readonly string ForaAtivComVagaQuadroEmpSemBenefLegisl = "6";
        public static readonly string AfastadoPorMaisDeUmMotivo = "7";
        public static readonly string NaoSaiNaRAIS = "N";
    }

    public sealed class VinculoRAIS
    {
        public static readonly string Aprendiz = "0";
        public static readonly string ContrTrabExprOuTacitoPPrazoIndeterminado = "1";
        public static readonly string EstatutarioDaUniaoEstadoMunicEMilitar = "2";
        public static readonly string TrabalhadorAvulso = "3";
        public static readonly string TrabTemporarioRegidoPLei6019de03_01_74 = "4";
        public static readonly string ContrTrabP_CLTPorTempoDeterminadoOuObraCerta = "5";
        public static readonly string FuncPorEstatutoTrabRuralLei5889_08_03_73 = "6";
        public static readonly string DiretorSemVinculoEmpregatPQAEmpresaOptouFGTS = "7";
        public static readonly string ServPublicosNaoEfetivosDemissiveisAdNutum = "8";
    }

    public sealed class Ocorrencia
    {
        public static readonly int NuncaExpostoAAgenteNocivo = 0;
        public static readonly int NaoExposicaoAAgenteNocivo_1Vinculo = 1;
        public static readonly int ExposicaoAAgenteNocivoAposAos15AnosDeServico_1vinculo = 2;
        public static readonly int ExposicaoAAgenteNocivoAposAos20AnosDeServico_1Vinculo = 3;
        public static readonly int ExposicaoAAgenteNocivoAposAos25AnosDeServico_1Vinculo = 4;
        public static readonly int ExposicaoAAgenteNocivoAposAos15AnosDeServico_1Vinculo = 5;
        public static readonly int NaoExposicaoAAgenteNocivo_MaisDe1Vinculo = 6;
        public static readonly int ExposicaoAAgenteNocivoAposAos20AnosDeServico_MaisDe1Vinculo = 7;
        public static readonly int ExposicaoAAgenteNocivoAposAos25AnosDeServico_MaisDe1Vinculo = 8;
    }

    public sealed class Categoria
    { 
        public static readonly int Empregado = 1;
        public static readonly int TrabalhadorAvulso = 2;
        public static readonly int EmpAfastadoPPrestarSerMilitarObrigatorio = 3;
        public static readonly int EmpSobContratoDeTrabPorPrazoDetLeiN960198 = 4;
        public static readonly int DiretorNaoEmpregadoComFGTS_Empresario = 5;
        public static readonly int TrabalhadorDe14a18AnosComContratoDeAprendizagem = 7;
        public static readonly int DiretorNaoEmpregadoSemFGTS_Empresario = 11;
        public static readonly int AgentePublico = 12;
        public static readonly int TrabAutonomoOuEquiparadoCContribSobreRemuneracao = 13;
        public static readonly int TrabAutonomoOuEuiparadoCContribASobreSalarioBase = 14;
        public static readonly int TransportadorAutonomoCContribSobreRemuneracao = 15;
        public static readonly int TransportadorAutonomoCContribSobreSalarioBase = 16;
        public static readonly int CooperadoDaCooperativaDeTrabalho = 17;
    }

    public sealed class CorRaca 
    { 
        public static readonly int Indigena = 1;
        public static readonly int Branca = 2;
        public static readonly int Preta = 4;
        public static readonly int Amarela = 6;
        public static readonly int Parda = 8;

    }



}