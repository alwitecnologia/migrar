using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Globais
{
    [DelimitedRecord(";")]
    public sealed class Agencias
    {
        public String NUMBANCO;

        public String NUMAGENCIA;

        public String NOME;

        public String PRACA;

        public String CODCOMPENSACAO;

        public String RUA;

        public String NUMERO;

        public String COMPLEMENTO;

        public String BAIRRO;

        public String ESTADO;

        public String CIDADE;

        public String CEP;

        public String PAIS;

        public String TIPOAGENCIA;

        public String DIGAG;

        public String TELEFONE;
    }
}
