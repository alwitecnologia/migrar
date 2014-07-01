using System;
using FileHelpers;

namespace Exportador.RH.Historicos
{
    //[FixedLengthRecord(FixedMode.ExactLength)]
    [DelimitedRecord(";")]
    public sealed class ContribuicaoSindical
    {

        //[FieldFixedLength(16, ";")]
        public String Chapa;

        //[FieldFixedLength(8, ";")]
        public DateTime DtContribuicao;

        //[FieldFixedLength(10, ";")]
        public String CodSindicato;

       // [FieldFixedLength(15, ";")]
        public Double Valor;

    }
}