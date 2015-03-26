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
            new Regex(@"(?=\.\.\.)"),
            // tokenize 's|,|;|:|"|)|}|]|- suffixes
            new Regex("(?=('s|;|:|,|\\)|\"|\\}|!|\\?|\\]|-)$)"),
            // tokenize "|'{|(|[
            new Regex("(?<=^(\"|'|\\{|\\(|\\[))")
        };

        private const string AllPunctutionsExceptDotAndDashPattern = "([^\\P{P}\\.']+|\\.{2,})";
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
                    .ToList();
                result = tempTokens;
            }

            return result;
        }
    }

}
