using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.RH.Secao
{

    //[FixedLengthRecord(FixedMode.ExactLength)]
    [DelimitedRecord(";")]
    public sealed class Secao
    {

        //[FieldFixedLength(35,";")]
        public String Codigo;

        //[FieldFixedLength(60,";")]
        public String Descricao;

        //[FieldFixedLength(20,";")]
        public String CNPJ;

       // [FieldFixedLength(20,";")]
        public String CNPJAnterior;

        //[FieldFixedLength(20,";")]
        public String CEI;

       // [FieldFixedLength(3,";")]
        public String FPAS;

        //[FieldFixedLength(12,";")]
        public String SAT;

       // [FieldFixedLength(7,";")]
        public String CNAE;

       // [FieldFixedLength(15, ";")]
        public String InscricaoEstadual;

       // [FieldFixedLength(15, ";")]
        public String InscricaoMunicipal;

       // [FieldFixedLength(30, ";")]
        public String Rua;

      //  [FieldFixedLength(8, ";")]
        public String Numero;

      //  [FieldFixedLength(30, ";")]
        public String Complemento;

       // [FieldFixedLength(30, ";")]
        public String Bairro;

       // [FieldFixedLength(2, ";")]
        public String Estado;

       // [FieldFixedLength(32, ";")]
        public String Cidade;

      //  [FieldFixedLength(9, ";")]
        public String CEP;

       // [FieldFixedLength(16, ";")]
        public String Pais;

      //  [FieldFixedLength(15, ";")]
        public String Telefone;

        //[FieldFixedLength(14, ";")]
        public String CodCaixaEconFederal;

      //  [FieldFixedLength(4, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]        
        public Int32? NumNaoEmpregadosProprietarios;

       // [FieldFixedLength(2, ";")]
        public String PrefixoInscricaoRAIS;

       // [FieldFixedLength(2, ";")]
        public String CodCategoriaFGTS;

        //[FieldFixedLength(15, ";")]
        public String IntegContabil;

      //  [FieldFixedLength(15, ";")]
        public String IntegGerencial;

       // [FieldFixedLength(7, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodFilialContabil;

    //    [FieldFixedLength(25, ";")]
        public String CodCentroCusto;

     //   [FieldFixedLength(4, ";")]
        public String CodTerceirosINSS;

      //  [FieldFixedLength(6, ";")]
        [FileHelpers.FieldConverter(typeof(DoubleNullableConverter))]
        public Double? PercentualTerceiros;

     //   [FieldFixedLength(6, ";")]
        [FileHelpers.FieldConverter(typeof(DoubleNullableConverter))]
        public Double? PercentualAcidTrabalho;

      //  [FieldFixedLength(15, ";")]
        [FileHelpers.FieldConverter(typeof(DoubleNullableConverter))]
        public Double? FaturamentoBruto;

       // [FieldFixedLength(15, ";")]
        [FileHelpers.FieldConverter(typeof(DoubleNullableConverter))]
        public Double? ValorFrete;

      //  [FieldFixedLength(37, ";")]
        public String InfoGRPS_1;

       // [FieldFixedLength(37, ";")]
        public String InfoGRPS_2;

       // [FieldFixedLength(37, ";")]
        public String InfoGRPS_3;

      ///  [FieldFixedLength(6, ";")]
        public String CodUnidadeEntrega;

    //    [FieldFixedLength(20, ";")]
        public String PessoaContato;

     //   [FieldFixedLength(4, ";")]
        public String Ramal;

      //  [FieldFixedLength(6, ";")]
        public String CodDepartamentoVTTicket;

      //  [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Inicio1oPeriodoRecolhimentoProprio;

     //   [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Fim1oPeriodoRecolhimentoProprio;

   //     [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? EventosPagosAte5oDiaUtil;

     //   [FieldFixedLength(2, ";")]
        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Inicio2oPeriodoRecolhimentoProprio;

     //   [FieldFixedLength(2, ";")]
        public String Fim2oPeriodoRecolhimentoProprio;

    //    [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? EventosPagosAte5oDiaUtil_2;

     //   [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Inicio1oPeriodoRecCentralizado;

     //   [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Fim1oPeriodoRecCentralizado;

    //    [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? RecCentrDeEventosAte5oDiaUtil;

     //   [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Inicio2oPeriodoRecCentralizado;

      //  [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? Fim2oPeriodoRecCentralizado;

      //  [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? RecCentrEventosAte5oDiaUtil_2;

      //  [FieldFixedLength(35, ";")]
        public String CodSecaoCentralizador;

      //  [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? ContribuicaoSESI_SENAI;

     //   [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? DistribuidorPetrolifero;

      //  [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? PessoaFisica;

      //  [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? inidicativoSecaoDesativada;

      //  [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? IndicativoIdentiticativoIdentificacaoCNPJ;

     //   [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? IndicativoAlteracaoEndereco;

     //   [FieldFixedLength(48, ";")]
        public String EnderecoPgto_VTTicket;

    //    [FieldFixedLength(10, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? IdentificacaoPedido_VTTicket;

    //    [FieldFixedLength(10, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? IdentificacaoPersonalizado_VTTicket;

      //  [FieldFixedLength(6, ";")]
        public String CodCliente_VTTicket;

      //  [FieldFixedLength(10, ";")]
        public String CodLocal_VTTicket;

      //  [FieldFixedLength(35, ";")]
        public String CodSecaoFaturamento_VTTicket;

      //  [FieldFixedLength(35, ";")]
        public String CodSecaoPedido_VTTicket;

     //   [FieldFixedLength(35, ";")]
        public String CodSecaoCobranca_VTTicket;

      //  [FieldFixedLength(35, ";")]
        public String CodSecaoEntrega_VTTicket;

     //   [FieldFixedLength(25, ";")]
        public String CodCentroCusto_VTTTicket;

      //  [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CausaMudancaCNPJ;

      //  [FieldFixedLength(7, ";")]
        public String CodMunicipio;

     //   [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? MesDataBase;

      //  [FieldFixedLength(4, ";")]
        public String NaturezaJuridica;

     //   [FieldFixedLength(16, ";")]
        public String CodCalendario;

     //   [FieldFixedLength(2, ";")]
        public String PrefixoInscricaoFGTS;

     //   [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? PrimeiraDeclaracaoCAGED;

     //   [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? AlteracaoParaCAGED;

     //   [FieldFixedLength(7,";")]
        //[FileHelpers.FieldConverter(typeof(NullableConverter))]
        [FieldConverter(typeof(IntConverter))]
        public Int32? CodIdentificadorFilial;

      //  [FieldFixedLength(25, ";")]
        public String CodIdentificadorDepartamento;

      //  [FieldFixedLength(10, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? LimiteFuncionarios;

      //  [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? OptaPeloSimples;

      //  [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? AlterouCNAE;

      //  [FieldFixedLength(16, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? ChapaFuncResponsavel;

     //   [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PercentualGRPS15AnosAposentadoria;

       // [FieldFixedLength(2, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PercentualGRPS20AnosAposentadoria;

       // [FieldFixedLength(5, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PercentualGRPS25AnosAposentadoria;

      //  [FieldFixedLength(4, ";")]
        public String DDD;

      //  [FieldFixedLength(5, ";")]
        public String CodPgtoGPS;

     //   [FieldFixedLength(5, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PorteDaEmpresa;

       // [FieldFixedLength(15, ";")]
        [FileHelpers.FieldConverter(typeof(DoubleNullableConverter))]
        public Double? PercentualIsencaoFilantropia;

      //  [FieldFixedLength(5, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? ParticipaPAT;

     //   [FieldFixedLength(25, ";")]
        public String CodDepartamentoContabil;

     //   [FieldFixedLength(1, ";")]
        [FileHelpers.FieldConverter(typeof(BooleanNullableConverter))]
        public bool? IsentaContribuicaoSocial;

     //   [FieldFixedLength(10, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? VinculosParticipaPATAcima5SalMin;

    //    [FieldFixedLength(10, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? VinculosParticipaPATAte5SalMin;

   //     [FieldFixedLength(5, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PorcentagemAdministracaoCozinha;

    //    [FieldFixedLength(5, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PorcentagemAlimentacaoConvenio;

    //    [FieldFixedLength(5, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PorcentagemCestaAlimento;

    //    [FieldFixedLength(5, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PorcentagemRefeicaoConvenio;

   //     [FieldFixedLength(5, ";")]
        public String PorcentagemRefeicaoTransportada;

     //   [FieldFixedLength(5, ";")]
        public String PorcentagemServicosProprio;

    //    [FieldFixedLength(5, ";")]
        [FileHelpers.FieldConverter(typeof(Int32NullableConverter))]
        public Int32? IndicadorEncerramentoAtividades;

     //   [FieldFixedLength(2, ";")]
        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime? DataEncerramentoAtividades;

    //    [FieldFixedLength(5, ";")]
        public String Brancos;

     //   [FieldFixedLength(45, ";")]
        public String EMail;

     //   [FieldFixedLength(40, ";")]
        public String Localidade;

      //  [FieldFixedLength(15, ";")]
        [FileHelpers.FieldConverter(typeof(DoubleNullableConverter))]
        public Double? CapitalSocialEmpresa;

    //    [FieldFixedLength(15, ";")]
        [FileHelpers.FieldConverter(typeof(DoubleNullableConverter))]
        public Double? CapitalSocialEstabelecimento;

     //   [FieldFixedLength(4, ";")]
        [FileHelpers.FieldConverter(typeof(DoubleNullableConverter))]
        public Double? CodigoPgtoGPSSomenteTerceiros;

    }
}