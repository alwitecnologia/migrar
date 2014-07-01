using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Ferias
{
    [DelimitedRecord(";")]
    public sealed class PeriodosGozo
    {
        public String Chapa;

        public String FimPeriodoAquisitivo;

        public String DataPgto;

        public String DataInicio;

        public String DataFim;

        public String DataAviso;

        public String NdiasAbono;

        public String Pag13Salario;

        public String NdiasLicRem1;

        public String NdiasLicRem2;

        public String FeriasColetivas;

        public String FeriasPerdidas;

        public String Observacao;

        public String SituacaoFerias;

        public String DataInicioDeEmprestimo;

        public String NumeroVezesEmprestimo;

        public String Faltas;

        public String NDiasAntecipados;

        public String FimPerAquisAntec;

        public String DataPgtoAntec;

        public String PeriodoAntecipado;

        public String NroDiasFeriado;

        public String EndLine;

    }
}
