using System;
using FileHelpers;

namespace Exportador.Academico.Documento
{
    [DelimitedRecord(";")]
    public sealed class Documento
    {
        public Int32 Codigo;

        public String Descricao;
    }
}