using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.RH.Funcao
{
    //[FixedLengthRecord(FixedMode.ExactLength)]
    [DelimitedRecord(";")]
    public sealed class Funcao
    {
        //[FieldFixedLength(10,";")]
        public String Codigo;

        //[FieldFixedLength(40, ";")]
        public String Nome;

       // [FieldFixedLength(8, ";")]
        [FieldConverter(typeof(Decimal2Converter))]
        public Double? NumPontos;

       // [FieldFixedLength(8, ";")]
        public String CBO;

        //[FieldFixedLength(16, ";")]
        public String CodCargo;

      //  [FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter),"1","0")]
        public bool? IndAtividade;

       // [FieldFixedLength(1, ";")]
        [FieldConverter(typeof(BooleanNullableConverter), "1", "0")]
        public bool? IndTransporte;

       // [FieldFixedLength(250, ";")]
        public String Descricao;

       // [FieldFixedLength(10, ";")]
        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? LimiteFuncionarios;

      //  [FieldFixedLength(10, ";")]
        public String CBO2002;
    }
}