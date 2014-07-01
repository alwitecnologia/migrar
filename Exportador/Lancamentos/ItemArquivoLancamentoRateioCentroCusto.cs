using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Exportador.Lancamentos
{
    [IgnoreEmptyLines()]
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class ItemArquivoLancamentoRateioCentroCusto
    {

        [FieldFixedLength(1)]
        public String TipoDaLinha = "U";

        [FieldFixedLength(4)]
        public Int32 CodigoDaColigada;

        [FieldFixedLength(25)]
        public String CodigoDoCentroDeCusto;

        [FieldFixedLength(18)]
        [FieldConverter(typeof(Decimal2Converter))]
        public Decimal Valor;

        [FieldFixedLength(255)]
        public String Historico;


    }
}
