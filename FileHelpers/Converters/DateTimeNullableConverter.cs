using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileHelpers.Converters
{
    public class DateTimeNullableConverter: ConverterBase
    {
        string mFormat;

        public DateTimeNullableConverter() : this(ConverterBase.DefaultDateTimeFormat)
		{
		}

        public DateTimeNullableConverter(string format)
		{
			if (String.IsNullOrEmpty(format))
				throw new BadUsageException("The format of the DateTime Converter can be null or empty.");

			try
			{
				string tmp = DateTime.Now.ToString(format);
			}
			catch
			{
				throw new BadUsageException("The format: '" + format + " is invalid for the DateTime Converter.");
			}

			mFormat = format;
		}


        public override object StringToField(string from)
        {
            if (from == null) from = string.Empty;

            object val;
            try
            {
                val = DateTime.ParseExact(from.Trim(), mFormat, null);
            }
            catch
            {
                string extra = String.Empty;
                if (from.Length > mFormat.Length)
                    extra = " There are more chars than in the format string: '" + mFormat + "'";
                else if (from.Length < mFormat.Length)
                    extra = " There are less chars than in the format string: '" + mFormat + "'";
                else
                    extra = " Using the format: '" + mFormat + "'";


                throw new ConvertException(from, typeof(DateTime), extra);
            }
            return val;
        }

        public override string FieldToString(object from)
        {
            if (from == null)
                return String.Empty;

            return Convert.ToDateTime(from).ToString(mFormat);		
        }
    }
}
