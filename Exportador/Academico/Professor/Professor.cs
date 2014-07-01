using System;
using FileHelpers;

namespace Exportador.Academico.Professor
{
    [DelimitedRecord(";")]
    public sealed class Professor
    {

        public String Nome;

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime DtNascimento;

        public String CPF;

        public String CartIdentidade;

        public String UFCartIdentidade;

        public String CarteiraTrab;

        public String SerieCartTrab;

        public String UFCartTrab;

        public Int32 CodColigada;

        public String CodProf;

        public String Chapa;

        public Double ValorAula;

        public String Titulacao;

        public String Naturalidade;

        public String EstadoNatal;

    }
}