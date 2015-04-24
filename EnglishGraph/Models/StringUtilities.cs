using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
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
        private const string FractionPattern = "^\\d+/\\d+$";
        private const string FractionPercentagePattern = "^\\d+/\\d+%$";
        private const string WordTokenPattern = "[a-zA-Z0-9]+";

        private static readonly string AmountPattern = string.Format("^({0})?{1}({2})?$", string.Join("|", CurrencySymbols),
            NumberPattern, string.Join("|", CurrencySymbols));

        /// <summary>
        /// Whether a string is a number (includes currencies, decimals, exponents, thousands)
        /// </summary>
        public static bool IsNumber(string input)
        {
            double number;
            var success = double.TryParse(input, NumberStyles.Number, 
                new CultureInfo("en-US"), out number);
            return success;
        }

        /// <summary>
        /// Whether a string is a money amount,
        /// ie a currency + a number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsAmount(string input)
        {
            double number;
            var success = double.TryParse(input, NumberStyles.Currency,
                new CultureInfo("en-US"), out number);
            return success;
        }

        /// <summary>
        /// Whether a string is a percentage, 
        /// ie if it finishes by '%' and the rest is a number/fraction
        /// </summary>
        public static bool IsPercentage(string input)
        {
            return !string.IsNullOrEmpty(input)
                   && input.Last() == '%'
                   && (IsNumber(input.Substring(0, input.Length - 1)) || IsFraction(input.Substring(0, input.Length - 1)));
        }

        /// <summary>
        /// Whether a string is a fraction
        /// Ex: 1/2
        /// </summary>
        public static bool IsFraction(string input)
        {
            return Regex.IsMatch(input, FractionPattern);
        }
        
        /// <summary>
        /// Whether a string is a time
        /// Supports only en-US time formats for the moment
        /// </summary>
        public static bool IsTime(string input)
        {
            TimeSpan time;
            // TODO - pass the culture with the input
            var success = TimeSpan.TryParse(input, new CultureInfo("en-US"), out time);
            if (success)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Whether a string contains only punctuation characters
        /// </summary>
        public static bool IsPunctuation(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^\\p{P}+$");
        }

        /// <summary>
        /// Whether a string is a compound word, ie if it contains a '-' inside the word.
        /// Ex:
        /// -test -> not compound
        /// six-figure -> compound
        /// </summary>
        public static bool IsCompoundWord(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^(\\w+\\-)+\\w+$");
        }

        /// <summary>
        /// Whether the first character of a string is upper cased. 
        /// Info: if char = '0' or char = '.', IsUpper(char) = false.
        /// </summary>
        public static bool IsFirstCharUpperCased(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return char.IsUpper(input.First());
        }

        /// <summary>
        /// Whether the all string is upper cased, 
        /// ie whether all the letters in the string are upper cased
        /// </summary>
        public static bool IsAllUpperCased(string input)
        {
            if (string.IsNullOrEmpty(input)) { return false; }

            return input.Where(char.IsLetter).All(char.IsUpper);
        }

        /// <summary>
        /// Lowercased the first letter of a string
        /// </summary>
        public static string LowerFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToLower(input.First()) + input.Substring(1);
        }

        /// <summary>
        /// Whether the input contains at least one letter of number
        /// Ex:
        /// - word tokens: 'test', 'O.K.', '12%'
        /// - non word tokens: '!', ';', '.'
        /// </summary>
        public static bool IsWordToken(string input)
        {
            return Regex.IsMatch(input, WordTokenPattern);
        }
    }
}
