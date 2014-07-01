using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Exportador.BackOffice.ClienteFornecedor
{
    //Ultima contagem: 133 campos!
    [IgnoreEmptyLines()]
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class ClienteFornecedor
    {

        [FieldFixedLength(1)]
        public String TipoDaLinha = "C";

        [FieldFixedLength(5)]
        public Int32 Coligada;

        [FieldFixedLength(25)]
        public String Codigo;

        [FieldFixedLength(60)]
        public String NomeFantasia;

        [FieldFixedLength(60)]
        public String Nome;

        [FieldFixedLength(20)]
        public String CNPJCPF;

        [FieldFixedLength(20)]
        public String IE;

        /// <summary>
        ///1 - Cliente
        ///2 - Fornecedor
        /// </summary>
        [FieldFixedLength(5)]
        public Int32 ClienteOuFornecedor;

        [FieldFixedLength(40)]
        public String NaoUtilizado1;

        [FieldFixedLength(8)]
        public String Numero;

        [FieldFixedLength(20)]
        public String Complemento;

        [FieldFixedLength(20)]
        public String NaoUtilizado2;

        [FieldFixedLength(32)]
        public String NaoUtilizado3;

        [FieldFixedLength(2)]
        public String Estado;

        [FieldFixedLength(9)]
        public String CEP;

        [FieldFixedLength(15)]
        public String Telefone;

        [FieldFixedLength(40)]
        public String NãoUtilizado4;

        [FieldFixedLength(8)]
        public String NumeroDePagamento;

        [FieldFixedLength(20)]
        public String ComplementoDePagamento;

        [FieldFixedLength(20)]
        public String NaoUtilizado5;

        [FieldFixedLength(32)]
        public String CidadeDePagamento;

        [FieldFixedLength(2)]
        public String EstadoDePagamento;

        [FieldFixedLength(9)]
        public String CEPDePagamento;

        [FieldFixedLength(15)]
        public String TelefoneDePagamento;

        [FieldFixedLength(40)]
        public String NaoUtilizado6;

        [FieldFixedLength(8)]
        public String NumeroDeEntrega;

        [FieldFixedLength(20)]
        public String ComplementoDeEntrega;

        [FieldFixedLength(20)]
        public String NaoUtilizado7;

        [FieldFixedLength(32)]
        public String CidadeDeEntrega;

        [FieldFixedLength(2)]
        public String EstadoDeEntrega;

        [FieldFixedLength(9)]
        public String CEPDeEntrega;

        [FieldFixedLength(15)]
        public String TelefoneDeEntrega;

        [FieldFixedLength(15)]
        public String Fax;

        [FieldFixedLength(15)]
        public String Celular;

        [FieldFixedLength(60)]
        public String Email;

        [FieldFixedLength(40)]
        public String Contato;

        [FieldFixedLength(25)]
        public String TipoDeClienteFornecedor;

        [FieldFixedLength(5)]
        public Int32 Ativo;

        [FieldFixedLength(10)]
        public String LimiteDeCredito;

        [FieldFixedLength(10)]
        public String NaoUtilizado8;

        [FieldFixedLength(10)]
        public String ValorDoUltimoLancamento;

        [FieldFixedLength(5)]
        public String NaoUtilizado9;

        [FieldFixedLength(10)]
        public String NaoUtilizado10;

        [FieldFixedLength(10)]
        public String NaoUtilizado11;

        [FieldFixedLength(10)]
        public String NaoUtilizado12;

        [FieldFixedLength(10)]
        public String NaoUtilizado13;

        [FieldFixedLength(10)]
        public String NaoUtilizado14;

        [FieldFixedLength(40)]
        public String CampoLivre;

        [FieldFixedLength(40)]
        public String CampoAlfa1;

        [FieldFixedLength(40)]
        public String CampoAlfa2;

        [FieldFixedLength(40)]
        public String CampoAlfa3;

        [FieldFixedLength(10)]
        public String ValorOpcional1;

        [FieldFixedLength(10)]
        public String ValorOpcional2;

        [FieldFixedLength(10)]
        public String ValorOpcional3;

        [FieldFixedLength(10)]
        public String DataOpcional1;

        [FieldFixedLength(10)]
        public String DataOpcional2;

        [FieldFixedLength(10)]
        public String DataOpcional3;

        [FieldFixedLength(15)]
        public String NaoUtilizado15;

        [FieldFixedLength(1)]
        public String NaoUtilizado16;

        [FieldFixedLength(10)]
        public String DataDeInicioDasAtividades;

        [FieldFixedLength(10)]
        public String Patrimonio;

        [FieldFixedLength(10)]
        public String NumeroDeFuncionarios;

        [FieldFixedLength(94)]
        public String NaoUtilizado17;

        [FieldFixedLength(5)]
        public String ColigadaDoTipoDeClienteFornecedor;

        [FieldFixedLength(5)]
        public String FaxDedicado;

        [FieldFixedLength(5)]
        public String NaoUtilizado18;

        [FieldFixedLength(20)]
        public String InscricaoMunicipal;

        [FieldFixedLength(1)]
        public String PessoaFisicaOuJuridica;

        [FieldFixedLength(40)]
        public String ContatoDePagamento;

        [FieldFixedLength(40)]
        public String ContatoDeEntrega;

        [FieldFixedLength(20)]
        public String Pais;

        [FieldFixedLength(20)]
        public String PaisDePagamento;

        [FieldFixedLength(20)]
        public String PaisDeEntrega;

        [FieldFixedLength(15)]
        public String FaxDoEnderecoDeEntrega;

        [FieldFixedLength(60)]
        public String EmailDoEnderecoDeEntrega;

        [FieldFixedLength(15)]
        public String FaxDoEnderecoDePagamento;

        [FieldFixedLength(60)]
        public String EmailDoEnderecoDePagamento;

        [FieldFixedLength(20)]
        public String CarteiraDeIDentidade;

        [FieldFixedLength(10)]
        public String EmissorDaCarteiraDeIdentidade;

        [FieldFixedLength(2)]
        public String EstadoEmissorDaCarteiraDeIdentidade;

        [FieldFixedLength(20)]
        public String CodigoDoMunicipio;

        [FieldFixedLength(5)]
        public String FornecedorDeAtivoImobilizado;

        [FieldFixedLength(1)]
        public String NaoUtilizado19;

        [FieldFixedLength(3)]
        public String CodigoDoCargo;

        [FieldFixedLength(1)]
        public String VinculoEmpregaticio;

        [FieldFixedLength(10)]
        public String ValorDoFretePorFornecedor;

        [FieldFixedLength(5)]
        public String TipoDeTomador;

        [FieldFixedLength(5)]
        public Int32 ContribuinteISS;

        [FieldFixedLength(10)]
        public Int32 NumeroDeDependentes;

        [FieldFixedLength(60)]
        public String EmpresaQueAPessoaTrabalha;

        [FieldFixedLength(1)]
        public String EstadoCivil;

        [FieldFixedLength(1)]
        public String ProdutorRural;

        [FieldFixedLength(14)]
        public String InscricaoNoSuframa;

        [FieldFixedLength(1)]
        public Int32 ContribuinteICMS;

        [FieldFixedLength(5)]
        public String OrgaoPublico;

        [FieldFixedLength(15)]
        public String TelefoneComercial;

        [FieldFixedLength(10)]
        public String CaixaPostal;

        [FieldFixedLength(10)]
        public String CaixaPostalDePagamento;

        [FieldFixedLength(10)]
        public String CaixaPostalDeEntrega;

        [FieldFixedLength(5)]
        public String CategoriaDoAutonomo;

        [FieldFixedLength(10)]
        public String CodigoBrasileiroDeOcupacaoDoAutonomo;

        [FieldFixedLength(11)]
        public String NumeroContribuinteIndividualDoAutonomo;

        [FieldFixedLength(10)]
        public String NaoUtilizado20;

        [FieldFixedLength(10)]
        public String ValorDeOutrasDeducoesParaCalculoDeIRRF;

        [FieldFixedLength(10)]
        public String CodigoDaReceita;

        [FieldFixedLength(1)]
        public String NaoUtilizado21;

        [FieldFixedLength(20)]
        public String CEI;

        [FieldFixedLength(5)]
        public String OptantePeloSimples;

        [FieldFixedLength(5)]
        public String TipoDeRua;

        [FieldFixedLength(5)]
        public String TipoDeBairro;

        [FieldFixedLength(1)]
        public String RegimeDeISS;

        [FieldFixedLength(5)]
        public String RetencaoDeISS;

        [FieldFixedLength(10)]
        public String DataDeNascimento;

        [FieldFixedLength(1)]
        public String DesativarDadosBancarios;

        [FieldFixedLength(20)]
        public String InscricaoEstadualSTEmMG;

        [FieldFixedLength(30)]
        public String Bairro;

        [FieldFixedLength(30)]
        public String BairroDeEntrega;

        [FieldFixedLength(30)]
        public String BairroDePagamento;

        [FieldFixedLength(2)]
        public String RamoDeAtividade;

        [FieldFixedLength(100)]
        public String Rua;

        [FieldFixedLength(100)]
        public String RuaDePagamento;

        [FieldFixedLength(100)]
        public String RuaDeEntrega;

        [FieldFixedLength(5)]
        public String CodigoDePagamentoGPS;

        [FieldFixedLength(1)]
        public String Nacionalidade;

        [FieldFixedLength(20)]
        public String CodigoDoMunicipioDePagamento;

        [FieldFixedLength(20)]
        public String CodigoDoMunicipioDeEntrega;

        [FieldFixedLength(3)]
        public String IDPais;

        [FieldFixedLength(3)]
        public String IDPaisPagamento;

        [FieldFixedLength(3)]
        public String IDPaisEntrega;

        [FieldFixedLength(5)]
        public String TipoRuaPgto;

        [FieldFixedLength(5)]
        public String TipoBairroPgto;

        [FieldFixedLength(5)]
        public String TipoRuaEntrega;

        [FieldFixedLength(5)]
        public String TipoBairroEntrega;

        [FieldIgnored()]
        public String CodigoAcademico;

    }
}
