using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Exportador.Lancamentos
{
    [IgnoreEmptyLines()]
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class ItemArquivoLancamentoContabilizacao
    {
        [FieldFixedLength(1)]
        public String TipoDaLinha = "C";

        [FieldFixedLength(1)]
        public Int32 DebitoCredito;

        [FieldFixedLength(1)]
        public Int32 InclusaoBaixa;

        [FieldFixedLength(4)]
        public Int32 CodColigadaDaConta;

        [FieldFixedLength(40)]
        public String CodigoDaConta;

        [FieldFixedLength(18)]
        [FieldConverter(typeof(Decimal2Converter))]
        public Decimal Valor;

        [FieldFixedLength(4)]
        public Int32 CodigoDaFilial;

        [FieldFixedLength(25)]
        public String CodigoDoDepartamento;

        [FieldFixedLength(25)]
        public String CodigoDoCentroDeCusto;

        [FieldFixedLength(10)]
        public String CodigoDoHistoricoPadrao;

        [FieldFixedLength(250)]
        public String ComplementoDoHistorico;
    }
}
