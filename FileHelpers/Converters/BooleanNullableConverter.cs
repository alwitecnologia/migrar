using System;
using System.Collections.Generic;
using System.Text;

namespace FileHelpers.Converters
{
    public class BooleanNullableConverter: ConverterBase
    {
			private string mTrueString = null;
			private string mFalseString = null;
			private string mTrueStringLower = null;
			private string mFalseStringLower = null;

			public BooleanNullableConverter()
			{
			}

            public BooleanNullableConverter(string trueStr, string falseStr) 
			{
				mTrueString = trueStr;
				mFalseString = falseStr;
				mTrueStringLower = trueStr.ToLower();
				mFalseStringLower = falseStr.ToLower();
			}

			public override object StringToField(string from)
			{
				object val;
				try
				{
					string testTo = from.ToLower();
					
					if (mTrueString == null)
					{
						testTo = testTo.Trim();
						if (testTo == "true" || testTo == "1")
							val = true;
						else if (testTo == "false" || testTo == "0" || testTo == "")
							val = false;
						else
							throw new Exception();
					}
					else
					{
						if (testTo == mTrueStringLower || testTo.Trim() == mTrueStringLower)
							val = true;
						else if (testTo == mFalseStringLower || testTo.Trim() == mFalseStringLower)
							val = false;
						else
							throw new ConvertException(from, typeof(bool), "The string: " + from + " cant be recognized as boolean using the true/false values: " + mTrueString + "/" + mFalseString);
					}
				}
				catch
				{
					throw new ConvertException(from, typeof (Boolean));
				}

				return val;
			}

			public override string FieldToString(object from)
			{
                if (from == null)
                    return String.Empty;

				bool b = Convert.ToBoolean(from);
				if (b)
					if (mTrueString == null)
						return "True";
					else
						return mTrueString;
				else 
					if (mFalseString == null)
						return "False";
					else
						return mFalseString;

			}

    }
}
