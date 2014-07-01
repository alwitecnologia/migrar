using System;
using FileHelpers;
using FileHelpers.Converters;
namespace Exportador.Academico.MatrizCurricular.DisciplinaGrade
{
    [DelimitedRecord(";")]
    public sealed class DisciplinaGrade
    {
        public Int32 CodColigada;

        public String CodCurso;

        public String CodHabilitacao;

        public String CodGrade;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CodPeriodo;

        public String CodDisc;

        public String CodGrupoDisciplina;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PreReqCred;

        public String Descricao;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PosicaoNoHistorico;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? CargaHoraria;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? NumCreditosCobranc;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? ValorCredito;

        public String Objetivo;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? PercentAulaNaoPres;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? PrioridadeMatricula;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? NumCasasDecimaisNota;

        public String Atividade;

        public String CalcMediaGlobal;

        public String DesempenhoAluno;

        public String ImprimeBoletim;

        public String TipoNota;

        [FieldConverter(typeof(Int32NullableConverter))]
        public Int32? NumMinDisc;

        [FieldConverter(typeof(DoubleNullableConverter))]
        public Double? CargaHorariaDiscPreRequisito;

        public String TipoDisciplina;

        public String Aplicacao;

        public String CodFormulaCoRequisito;

        public String CodFormulaPreRequisito;

    }
}