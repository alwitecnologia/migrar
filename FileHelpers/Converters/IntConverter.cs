using System;
using System.Collections.Generic;
using System.Text;
using FileHelpers;

namespace FileHelpers
{
    public class IntConverter: ConverterBase
    {
        private string RemoveBlanks(string source)
        {
            StringBuilder sb = null;
            int i = 0;

            while (i < source.Length && Char.IsWhiteSpace(source[i]))
            {
                i++;
            }

            if (i < source.Length && (source[i] == '+' || source[i] == '-'))
            {
                i++;
                while (i < source.Length && Char.IsWhiteSpace(source[i]))
                {
                    if (sb == null)
                        sb = new StringBuilder(source[i - 1].ToString());

                    i++;
                }
            }

            if (sb == null)
                return source;
            else if (i < source.Length)
                sb.Append(source.Substring(i));

            return sb.ToString();
        }

        private string RemoveSemicolons(string source)
        {
            StringBuilder sb = null;
            int i = 0;

            while (i < source.Length && source[i] == ';')
            {
                i++;
            }

            if (i < source.Length && (source[i] == '+' || source[i] == '-'))
            {
                i++;
                while (i < source.Length && Char.IsWhiteSpace(source[i]))
                {
                    if (sb == null)
                        sb = new StringBuilder(source[i - 1].ToString());

                    i++;
                }
            }

            if (sb == null)
                return source;
            else if (i < source.Length)
                sb.Append(source.Substring(i));

            return sb.ToString();
        }


        public override object StringToField(string from)
        {
            from = RemoveSemicolons(from);

            return RemoveBlanks(from);
        }

        public override string FieldToString(object fieldValue)
        { 
            return fieldValue.ToString();
        }
    }
}
