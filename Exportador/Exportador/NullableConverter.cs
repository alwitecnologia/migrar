using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exportador
{
    public class NullableConverter : FileHelpers.ConverterBase
    {
        public override object StringToField(string from)
        {
            return from;
        }

        public override string FieldToString(object fieldValue)
        {
            if (fieldValue == null)
                return String.Empty;
            return fieldValue.ToString();
        }
    }
}
