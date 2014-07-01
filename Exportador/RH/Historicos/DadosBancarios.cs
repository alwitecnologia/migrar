using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Historicos
{
    [DelimitedRecord(";")]
    public sealed class DadosBancarios
    {
        public String CHAPA;

        public String BANCOPAGAMENTO;

        public String AGENCIAPAGAMENTO;

        public String CONTAPAGAMENTO;

        public String OPERACAOBANCARIA;

        public String DATAMUDANCA;
    }
}
