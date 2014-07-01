using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Ferias
{
    [DelimitedRecord(";")]
    public sealed class ReciboFerias
    {
        public String CHAPA;

        public String FIMPERAQUIS;

        public String DATAPAGTO;

        public String INSS1;

        public String INSS2;

        public String IRRF;

        public String BASEINSS1;

        public String BASEINSS2;

        public String BASEIRRF;

        public String LIQUIDO;

        public String OBSERVACAO;

        public String EXECID;

        public String PENSAO;

        public String BASEPENSAO;

        public String VALORESFORCADOS;

        public String MEDIAPERAQATUAL;

        public String MEDIAPROXPERAQ;

        public String SALARIO;
    }
}
