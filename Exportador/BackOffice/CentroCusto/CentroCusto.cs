using FileHelpers;
using System;

namespace Exportador.BackOffice.CentroCusto {

    [DelimitedRecord(";")]
    public sealed class CentroCusto
    {

        public Int32 CodColigada;

        public String CodCentroCusto;

        public String Nome;

        public Int32 CodColigadaContaGerencial;

        public String CodContaGerencial;

        public Int32 CodColigadaContabil;

        public String CodContabil;

        public String CodReduzido;

        public String CampoLivre;

        public String Ativo;

        public String PermiteLancamento;

        public String CodClassificacao;

    }

}