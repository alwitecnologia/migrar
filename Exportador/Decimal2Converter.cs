using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Exportador
{
    public class Decimal2Converter : ConverterBase
    {
        private int mDecimals = 2;

        public override object StringToField(string from)
        {
            return Convert.ToDecimal(Decimal.Parse(from) / (10 ^ mDecimals));
        }

        public override string FieldToString(object fieldValue)
        {
            if (fieldValue == null)
                return String.Empty;

            Decimal v = Convert.ToDecimal(fieldValue);

            // ugly but works =) 
            string res = Decimal.ToUInt32(Decimal.Truncate(v)).ToString();
            res += Decimal.Round(Decimal.Remainder(v, 1), mDecimals)
                       .ToString(".##").Replace(",", "").Replace(".", "").PadLeft(mDecimals, '0');

            return res;

            // a more elegant option that also works 
            // return Convert.ToInt32(Convert.ToDecimal(fieldValue) * (10 ^ mDecimals)).ToString();  
        }
    }
}
