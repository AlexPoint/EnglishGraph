using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public static class StringUtilities
    {
        private static readonly List<string> CurrencySymbols = new List<string>()
        {
            "\\$", "€", "£"
        };

        private const string NumberPattern = "[\\d\\.,\\s]+";
        private const string OnlyNumbersPattern = "^" + NumberPattern + "$";
        private const string PercentagePattern = "^" + NumberPattern + "%$";

        private static readonly string AmountPattern = string.Format("^({0})?{1}({2})?$", string.Join("|", CurrencySymbols),
            NumberPattern, string.Join("|", CurrencySymbols));


        public static bool IsFigure(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, OnlyNumbersPattern)
                || Regex.IsMatch(input, PercentagePattern)
                || Regex.IsMatch(input, AmountPattern);
        }

        public static bool IsPunctuation(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^\\p{P}+$");
        }

        public static bool IsCompoundWord(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^(\\w+\\-)+\\w+$");
        }

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
