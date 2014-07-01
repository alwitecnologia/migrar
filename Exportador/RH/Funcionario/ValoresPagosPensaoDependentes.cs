using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Funcionario
{
    [DelimitedRecord(";")]
    public sealed class ValoresPagosPensaoDependentes
    {
        public String CHAPA;

        public String NDEPENDENTE;

        public String ANOCOMPETENCIA;

        public String MESCOMPETENCIA;

        public String MESCAIXA;

        public String TIPOMOVIMENTACAOPENSAO;

        public String NUMEROPERIODO;

        public String VALOR;

        public String VALORORIGINAL;

        public String INDICATIVOALTERACAOMANUAL;

        public DateTime AUX1;

        public Int32 AUX2;

        public Int32 AUX3;

        public Int32 AUX4;

    }
}
