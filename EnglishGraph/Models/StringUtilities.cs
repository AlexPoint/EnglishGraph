using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public static class StringUtilities
    {
        public static bool IsFirstLetterUpperCased(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return char.IsUpper(input.First());
        }

        public static bool IsAllUpperCased(string input)
        {
            if (string.IsNullOrEmpty(input)) { return false; }

            return input.Where(char.IsLetter).All(char.IsUpper);
        }

        public static string LowerFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToLower(input.First()) + input.Substring(1);
        }
    }
}
