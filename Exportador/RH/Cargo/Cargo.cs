using FileHelpers;
using System;
using System.Reflection;

namespace Exportador.RH.Cargo {

    [DelimitedRecord(";")]
    public sealed class Cargo
    {
        public String Codigo;

        public String Nome;

        public String Descricao;

        public String Final;
    }
}