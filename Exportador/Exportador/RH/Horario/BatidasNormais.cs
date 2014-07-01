using System;
using FileHelpers;
namespace Exportador.RH.Horario
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class BatidasNormais
    {

        [FieldFixedLength(2)]
        public String TipoRegistro="01";

        [FieldFixedLength(10)]
        public String CodHorario;

        [FieldFixedLength(4)]
        public Int32 IndiceDia;

        [FieldFixedLength(5)]
        public Int32 HorarioBatida1;

        [FieldFixedLength(1, ";")]
        public Int32 NaturezaBatida1;

        [FieldFixedLength(5)]
        public Int32 HorarioBatida2;

        [FieldFixedLength(1, ";")]
        public Int32 NaturezaBatida2;

        [FieldFixedLength(5)]
        public Int32 HorarioBatida3;

        [FieldFixedLength(1, ";")]
        public Int32 NaturezaBatida3;

        [FieldFixedLength(5)]
        public Int32 HorarioBatida4;

        [FieldFixedLength(1, ";")]
        public Int32 NaturezaBatida4;

        [FieldFixedLength(5)]
        public Int32 HorarioBatida5;

        [FieldFixedLength(1, ";")]
        public Int32 NaturezaBatida5;

        [FieldFixedLength(5)]
        public Int32 HorarioBatida6;

        [FieldFixedLength(1, ";")]
        public Int32 NaturezaBatida6;

        [FieldIgnored()]
        public Int32 CodColigada;

        [FieldIgnored()]
        public String LetraDia;

        [FieldIgnored()]
        public String RecCreatedBy;

        [FieldIgnored()]
        public String RecModifiedBy;

        [FieldIgnored()]
        public Int32 TipoBatida;

        [FieldIgnored()]
        public Int32 Fim;

        [FieldIgnored()]
        public Int32 Inicio;

        [FieldIgnored()]
        public Int32 TempoMinRef;

        [FieldIgnored()]
        public Int32 IndLimRef;

        [FieldIgnored()]
        public Int32 ExtraMinRef;

        [FieldIgnored()]
        public Int32 AtrasoMinRef;
    }
}