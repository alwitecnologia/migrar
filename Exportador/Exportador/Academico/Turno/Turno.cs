using System;
using FileHelpers;

namespace Exportador.Academico.Turno
{
    [DelimitedRecord(";")]
    public sealed class Turno
    {

        public Int32 CodColigada;

        public Int32 CodTipoCurso;

        public Int32 CodFilial;

        public String Nome;

        public String HoraInicio;

        public String HoraFim;

        /// <summary>
        /// Tipo do turno. (M=Matutino:V=Vespertino:N=Noturno:I=Integral)
        /// </summary>
        public String Tipo;

        [FieldIgnored()]
        public string CodTurno;

    }
}