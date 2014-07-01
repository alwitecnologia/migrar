using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileHelpers.Converters
{
    public class DoubleNullableConverter: ConverterBase
    {
        public override object StringToField(string from)
        {
            return Double.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number);
        }

        public override string FieldToString(object from)
        {
            if (from == null)
                return String.Empty;

            return from.ToString();
        }
    }
}
