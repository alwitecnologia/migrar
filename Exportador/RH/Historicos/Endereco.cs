using System;
using FileHelpers;

namespace Exportador.RH.Historicos
{
    [DelimitedRecord(";")]
    public sealed class Endereco
    {

        public String CodPessoa;

        [FieldConverter(ConverterKind.Date, "ddMMyyyy")]
        public DateTime DtMudanca;

        public String Rua;

        public String Numero;

        public String Complemento;

        public String Bairro;

        public String Estado;

        public String Cidade;

        public String CEP;

        public String Pais;

        public String Telefone;

    }
}