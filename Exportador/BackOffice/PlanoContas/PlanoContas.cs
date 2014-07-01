using FileHelpers;
using System;

namespace Exportador.BackOffice.PlanoContas
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public sealed class PlanoContas
    {
        [FieldFixedLength(20)]
        public String CodContabil;

        [FieldFixedLength(10)]
        public String CodReduzido;

        [FieldFixedLength(40)]
        public String Descricao;

        /// <summary>
        /// Analítica / Sintética
        /// </summary>
        [FieldFixedLength(1)]
        public String AnaSin;

        [FieldFixedLength(1)]
        public String DevCred;

        [FieldFixedLength(1)]
        public String Contabil;

        [FieldFixedLength(1)]
        public String DistrGer;
    }
}

