using System;
using FileHelpers;
using FileHelpers.Converters;

namespace Exportador.RH.Ferias
{
    //[FixedLengthRecord(FixedMode.ExactLength)]
    [DelimitedRecord(";")]
    public sealed class AquisicaoFerias
    {
        public String Chapa;

        public String FimPeriodoAquisitivo;

        public String InicioPeriodoAquisitivo;

        public String Saldo;

        public String PeriodoAberto;

        public String PeriodoPerdido;

        public String MotivoPerda;

        public String Faltas;

        public String Bonus;
    }
}