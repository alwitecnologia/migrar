using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Exportador.Helpers
{
    public static class StringHelper
    {
        public static string RemoveSpecialChars(this string input)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = input.Normalize(NormalizationForm.FormD).ToCharArray();

            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString().Trim(); 
        }
    }
}
