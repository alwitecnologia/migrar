using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Exportador.Lancamentos
{
    /// <summary>
    /// Classe criada com base no arquivo de layout "\\Layouts\LayoutImportacaoLancamento.docx"
    /// </summary>
    [IgnoreEmptyLines()]
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class ItemArquivoLancamento
    {

        [FieldFixedLength(1)]
        public String TipoDaLinha = "L";

        [FieldFixedLength(3)]
        public String CodigoDoBancoCNAB;

        [FieldFixedLength(4)]
        public Int32 CodigoDaFilial;

        [FieldFixedLength(25)]
        public String CodigoDoCliForCNPJCPF;

        [FieldFixedLength(10)]
        public String CodigoDoTipoDeDocumento;

        [FieldFixedLength(40)]
        public String NumeroDoDocumento;

        [FieldFixedLength(3)]
        public String NaoUtilizado1;

        [FieldFixedLength(1)]
        public Int32 StatusDoFaturamento;

        [FieldFixedLength(10)]
        public String NaoUtilizado2;

        [FieldFixedLength(10)]
        public String NaoUtilizado3;

        [FieldFixedLength(1)]
        public Int32 APagarOuAReceber;

        [FieldFixedLength(25)]
        public String CodigoDaTabelaOpcional1;

        [FieldFixedLength(25)]
        public String CodigoDaTabelaOpcional2;

        [FieldFixedLength(25)]
        public String CodigoDaTabelaOpcional3;

        [FieldFixedLength(25)]
        public String CodigoDaTabelaOpcional4;

        [FieldFixedLength(25)]
        public String CodigoDaTabelaOpcional5;

        [FieldFixedLength(25)]
        public String CodigoDoDepartamento;

        [FieldFixedLength(25)]
        public String CodigoDoCentroDeCusto;

        [FieldFixedLength(6)]
        public string DataDeVencimento;

        [FieldFixedLength(6)]
        public String DataDeEmissao;

        [FieldFixedLength(6)]
        public String NaoUtilizado4;

        [FieldFixedLength(6)]
        public String DataDePrevisaoDeBaixa;

        [FieldFixedLength(6)]
        public String DataDeContabilizacao;

        [FieldFixedLength(6)]
        public String DataDeCancelamento;

        [FieldFixedLength(6)]
        public String NaoUtilizado5;

        [FieldFixedLength(6)]
        public String NaoUtilizado6;

        [FieldFixedLength(6)]
        public String NaoUtilizado7;

        [FieldFixedLength(100)]
        public String NaoUtilizado8;

        [FieldFixedLength(1)]
        public String TipoContabil;

        [FieldFixedLength(10)]
        public String CodigoDaMoeda;

        [FieldFixedLength(10)]
        public String CodigoDoIndexador;

        [FieldFixedLength(10)]
        public String CodigoDaContaCaixa;

        [FieldFixedLength(18)]        
        [FieldConverter(typeof(Decimal4Converter))]
        public decimal ValorOriginal;

        [FieldFixedLength(18)]
        public String NaoUtilizado9;

        [FieldFixedLength(18)]
        public String ValorDaCapitalizacao;

        [FieldFixedLength(18)]
        [FieldConverter(typeof(Decimal4Converter))]
        public decimal ValorDosJuros;

        [FieldFixedLength(18)]
        [FieldConverter(typeof(Decimal4Converter))]
        public decimal ValorDoDesconto;

        [FieldFixedLength(18)]
        public String ValorOpcional1;

        [FieldFixedLength(18)]
        public String ValorOpcional2;

        [FieldFixedLength(18)]
        public String ValorOpcional3;

        [FieldFixedLength(18)]
        public String ValorOpcional4;

        [FieldFixedLength(18)]
        public String ValorOpcional5;

        [FieldFixedLength(18)]
        public String TaxaDeJurosAoDia;

        [FieldFixedLength(18)]
        public String TaxaCapitalizacaoMensal;

        [FieldFixedLength(4)]
        public String NaoUtilizado10;

        [FieldFixedLength(20)]
        public String NumeroContabil;

        [FieldFixedLength(20)]
        public String SegundoNumero;

        [FieldFixedLength(1)]
        public int StatusDoLancamento;

        [FieldFixedLength(1)]
        public String LiberacaoAutorizada;

        [FieldFixedLength(1)]
        public String NaoUtilizado11;

        [FieldFixedLength(6)]
        public String NaoUtilizado12;

        [FieldFixedLength(10)]
        public String NaoUtilizado13;

        [FieldFixedLength(20)]
        public String NaoUtilizado14;

        [FieldFixedLength(6)]
        public String NaoUtilizado15;

        [FieldFixedLength(18)]
        public String NaoUtilizado16;

        [FieldFixedLength(1)]
        public String NaoUtilizado17;

        [FieldFixedLength(6)]
        public String DataOpcional1;

        [FieldFixedLength(6)]
        public String DataOpcional2;

        [FieldFixedLength(6)]
        public String DataOpcional3;

        [FieldFixedLength(10)]
        public String NaoUtilizado18;

        [FieldFixedLength(1)]
        public String NaoUtilizado19;

        [FieldFixedLength(1)]
        public String NaoUtilizado20;

        [FieldFixedLength(3)]
        public String CarteiraCNAB;

        [FieldFixedLength(2)]
        public String ComandoCNAB;

        [FieldFixedLength(1)]
        public String AceiteCNAB;

        [FieldFixedLength(2)]
        public String InstrucaoCodif1;

        [FieldFixedLength(2)]
        public String InstrucaoCodif2;

        [FieldFixedLength(20)]
        public String NossoNumeroCNAB;

        [FieldFixedLength(1)]
        public String StatusCNAB;

        [FieldFixedLength(10)]
        public String NaoUtilizado21;

        [FieldFixedLength(1)]
        public String Reembolsavel;

        [FieldFixedLength(4)]
        public Int32 CodigoColigadaCliFor;

        [FieldFixedLength(16)]
        public String CodigoDoVendedor;

        [FieldFixedLength(4)]
        public String CodColigadaContaCaixa;

        [FieldFixedLength(4)]
        public String CodigoColigadaDoExtrato;

        [FieldFixedLength(1)]
        public String TipoDoSacado;

        [FieldFixedLength(6)]
        public String PeriodoLetivo;

        [FieldFixedLength(2)]
        public String Parcela;

        [FieldFixedLength(2)]
        public String Cota;

        [FieldFixedLength(2)]
        public String NaoUtilizado22;

        [FieldFixedLength(30)]
        public String NaoUtilizado23;

        [FieldFixedLength(1)]
        public String LocalDePagamento;

        [FieldFixedLength(5)]
        public String NaoUtilizado24;

        [FieldFixedLength(18)]
        [FieldConverter(typeof(Decimal4Converter))]
        public decimal ValorDaMulta;

        [FieldFixedLength(8)]
        public String NumeroDoRecibo;

        [FieldFixedLength(18)]
        public String ValorDepositado;

        [FieldFixedLength(1)]
        public String ServicosExtras;

        [FieldFixedLength(6)]
        public String DataDePagamento;

        [FieldFixedLength(6)]
        public String DataDaUltimaAlteracao;

        [FieldFixedLength(6)]
        public String HoraUltimaAlteracao;

        [FieldFixedLength(15)]
        public String UsuarioQueAlterou;

        [FieldFixedLength(44)]
        public String CodigoDeBarras;

        [FieldFixedLength(47)]
        public String IPTE;

        [FieldFixedLength(10)]
        public String NaoUtilizado25;

        [FieldFixedLength(18)]
        public String ValorDoRepasse;

        [FieldFixedLength(1)]
        public String BaixaAutorizada;

        [FieldFixedLength(1)]
        public String TemChequeParcial;

        [FieldFixedLength(100)]
        public String CampoAlfaOp1;

        [FieldFixedLength(20)]
        public String CampoAlfaOp2;

        [FieldFixedLength(20)]
        public String CampoAlfaOp3;

        [FieldFixedLength(1)]
        public String BoletoEmitido;

        [FieldFixedLength(18)]
        public String NaoUtilizado26;

        [FieldFixedLength(10)]
        public String NaoUtilizado27;

        [FieldFixedLength(18)]
        public String NaoUtilizado28;

        [FieldFixedLength(10)]
        public String NaoUtilizado29;

        [FieldFixedLength(18)]
        public String ValorOpcional6;

        [FieldFixedLength(18)]
        public String ValorOpcional7;

        [FieldFixedLength(18)]
        public String ValorOpcional8;

        [FieldFixedLength(1)]
        public String NumeroDeBloqueios;

        [FieldFixedLength(15)]
        public String UsuarioDeDesbloqueio1;

        [FieldFixedLength(15)]
        public String UsuarioDeDesbloqueio2;

        [FieldFixedLength(6)]
        public String DataOpcional4;

        [FieldFixedLength(6)]
        public String DataOpcional5;

        [FieldFixedLength(15)]
        public String CodigoDoRepresentante;

        [FieldFixedLength(2)]
        public String CodigoAplicacaoDeOrigemLancamento;

        [FieldFixedLength(20)]
        public String NaoUtilizado30;

        [FieldFixedLength(5)]
        public String CodEventoContabil;

        [FieldFixedLength(5)]
        public String NaoUtilizado31;

        [FieldFixedLength(2)]
        public String NaoUtilizado32;

        [FieldFixedLength(14)]
        public String CotacaoNaInclusao;

        [FieldFixedLength(14)]
        public String NaoUtilizado33;

        [FieldFixedLength(1)]
        public String TipoDeJurosDoDia;

        [FieldFixedLength(18)]
        public String TaxaDeJurosDoVendor;

        [FieldFixedLength(18)]
        public String ValorDosJurosDeVendor;

        [FieldFixedLength(18)]
        public String NaoUtilizado34;

        [FieldFixedLength(6)]
        public String MesAnoDeCompetencia;

        [FieldFixedLength(5)]
        public String PagamentoEletronico;

        [FieldFixedLength(4)]
        public String Reutilizacao;

        [FieldFixedLength(8)]
        public String SerieDocumento;

        [FieldFixedLength(255)]
        public String HistoricoDeLancamentos;

        [FieldFixedLength(5)]
        public String OcorrenciasDoAutonomo;

        [FieldFixedLength(18)]
        public String INSSOutrasEmpresas;

        [FieldFixedLength(10)]
        public String CodigoDaReceita;

        [FieldFixedLength(18)]
        public String BaseINSSEmpregado;

        [FieldFixedLength(18)]
        public String BaseINSSEmpregador;

        [FieldIgnored()]
        public int ContaContabil;
        
    }
}
