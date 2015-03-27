using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class SentenceParser
    {

        public List<string> Tokenize(string sentence)
        {
            if (string.IsNullOrEmpty(sentence)) { return new List<string>(); }

            // split on spaces
            var parts = sentence.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
            // split tokens ending in 's
            var tokens = parts
                .SelectMany(SplitToken)
                .ToList();

            return tokens;
        }

        // Tokenize rules 
        private static readonly List<Regex> TokenizationRegexes = new List<Regex>()
        {
            // always tokenize ...
            //new Regex("((?=\\.{2,}|,+(\\D|$)|\"+|;|:|!+|\\?+|\\(|\\)|\\{|\\}|\\[|\\]|'s$|\\-$)|(?<=\\.{2,}|,+(\\D|$)|\"+|;|:|!+|\\?+|\\(|\\)|\\{|\\}|\\[|\\]|'s$|\\-$|^'\\w{2,}|^\\-))"),
            new Regex("((?=\\.{2,}|,+(\\D|$)|\"+|;|:|!+|\\?+|\\(|\\)|\\{|\\}|\\[|\\]|'s$|\\-$)|(?<=\\.{2,}|,+(\\D|$)|\"+|;|:|!+|\\?+|\\(|\\)|\\{|\\}|\\[|\\]|'s$|\\-$|^'\\w{2,}|^\\-))"),
        };

        private List<string> SplitToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new List<string>();
            }

            var result = new List<string>() { token };
            foreach (var tokenizationRegex in TokenizationRegexes)
            {
                var tempTokens = result
                    .SelectMany(tok => tokenizationRegex.Split(tok))
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToList();
                if (result.Count != tempTokens.Count)
                {
                    //Console.WriteLine("{0} ==> {1}", string.Join("|", result), string.Join("|", tempTokens));
                }
                result = tempTokens;
            }

            return result;
        }
    }

}
