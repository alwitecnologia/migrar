using System;
using FileHelpers;
namespace Exportador.RH.Horario
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class Intervalo
    {
        [FieldFixedLength(2)]
        public String TipoRegistro;
        
        [FieldFixedLength(10)]
        public String CodHorario;

        [FieldFixedLength(4)]
        public Int32 IndiceDia;

        [FieldFixedLength(5)]
        public String InicioIntervalo;

        [FieldFixedLength(5)]
        public String TerminoIntervalo;

        #region Constructors

        public Intervalo() 
        { 
        }

        public Intervalo(String tipoRegistro)
        {
            this.TipoRegistro = tipoRegistro;
        }

        #endregion

    }

    public sealed class TipoRegistroIntervalo 
    {
        public static readonly string IntervaloDescanso = "02";
        public static readonly string IntervaloCompensacao = "03";
        public static readonly string IntervaloRefeicao = "04";
    }
}