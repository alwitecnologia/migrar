using System;
using FileHelpers;

namespace Exportador.Academico.Horario
{
    [DelimitedRecord(";")]
    public sealed class Horario
    {
        public Int32 CodColigada;

        public Int32 CodFilial;

        public Int32 CodTipoCurso;

        public String NomeTurno;

        public Int32 DiaSemana;

        public String HoraInicial;

        public String HoraFinal;

        public String Aula;

    }
}