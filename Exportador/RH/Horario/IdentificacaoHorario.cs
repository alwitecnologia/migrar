using System;
using FileHelpers;
namespace Exportador.RH.Horario
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class IdentificacaoHorario
    {

        [FieldFixedLength(2)]
        public String TipoRegistro="00"; 

        [FieldFixedLength(10)]
        public String CodHorario;

        [FieldFixedLength(100)]
        public String DescricaoHorario;

        [FieldFixedLength(10)]
        public String DataBaseHorario;

        [FieldFixedLength(1)]
        public Int32 HorarioComTurnoRevezamento;

        [FieldFixedLength(1)]
        public Int32 HorarioInativo;

        [FieldFixedLength(1)]
        public Int32 HorarioAlternativoNoturno;

        [FieldFixedLength(1)]
        public Int32 ConsideraFeriadosNoCalculo;

        [FieldIgnored()]
        public string CodColigada;

        [FieldIgnored()]
        public string TipoHorario;

        [FieldIgnored()]
        public Int32 Inativo;

        [FieldIgnored()]
        public Int32 HorNoturno;

        [FieldIgnored()]
        public Int32 ConsFeriado;

        [FieldIgnored()]
        public Int32 ConsFerDiaAnt;

        [FieldIgnored()]
        public Int32 HorarioJor;

        [FieldIgnored()]
        public String RecCreatedBy;

        [FieldIgnored()]
        public String RecModifiedBy;
    }
}