using System;
using System.Collections.Generic;
using FileHelpers;
namespace Exportador.Academico.Disciplina
{
    [DelimitedRecord(";")]
    public sealed class Disciplina
    {
        
        public Int32 CodColigada;

        public Int32 CodTipoCurso;

        public String CodDisc;

        public String CodDiscHist;

        public String Nome;

        public String NomeReduzido;

        public String Complemento;

        public String CursoLivre;

        public String TipoAula;

        public String TipoNota;

        public Double CargaHoraria;

        public Double CargaHorariaEstagio;

        public Int32 Decimais;

        public Int32 NumCreditos;

        public String Objetivo;

        public String TipoDiscProvao;

        public Double CargaHorariaTeoria;

        public Double CargaHorariaPratica;

        public Double CargaHorariaLaboratorial;

        public String CodGrupoComplemento;

    }

    public class DisciplinaComparer : IEqualityComparer<Disciplina>
    {
        public bool Equals(Disciplina x, Disciplina y)
        {
            if ((x.CodDisc == y.CodDisc) || (x.CodTipoCurso == y.CodTipoCurso))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Disciplina disc)
        {
            return String.Concat(disc.CodTipoCurso,disc.CodDisc).GetHashCode();
        }
    }

}