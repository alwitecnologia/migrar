using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exportador.RH.Funcionario
{
    [DelimitedRecord(";")]
    public sealed class PeriodoFichaFinanceira
    {


        public String Chapa;

        public String AnoCompetencia;

        public String MesCompetencia;

        public String NumeroPeriodo;

        public String MesCaixaComum;

        public String BaseINSS;

        public String BaseINSSdoOutroEmprego;

        public String INSS;

        public String INSSdeFerias;

        public String INSSdeOutroEmprego;

        public String BaseINSSdo13;

        public String BaseINSS13doOutroEmprego;

        public String INSSdo13;

        public String BaseSalarioFamilia;

        public String SalarioFamilia;

        public String BaseDeValeTransporte;

        public String ValeTransporteEntregue;

        public String ValeTransporteDescontado;

        public String BaseIRRF;

        public String IRRF;

        public String INSSCaixa;

        public String INSSCalculadoPeloUsuario;

        public String DedutivelEmIRRF;

        public String BaseIRRFFerias;

        public String IRFFParticipacao;

        public String IRRFFerias;

        public String IndicativoDeINSSComCPMF;

        public String BaseDeFGTS;

        public String BaseDeFGTSDe13Salario;

        public String SalarioPago;

        public String DescricaoDoPeriodo;

        public String BaseDeIRRFde13Salario;

        public String IRRFSob13;

        public String INSSDeFeruasComCPMF;

        public String INSSDeDiferencaSalarial;

        public String INSSDeDiferencaSalarialDe13;

        public String INSSDeDiferencaSalarialDeFerias;

        public String BaseFGTSDifSalarial;

        public String a1;

        public String a2;

        public String a3;

        public String a4;

        public String a5;

        public String a6;
    }
}
