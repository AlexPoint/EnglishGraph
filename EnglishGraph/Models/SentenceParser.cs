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

            // tokenize the last .
            var lastTokenWithWordChar = tokens.LastOrDefault(t => Regex.IsMatch(t, "\\w+"));
            if (lastTokenWithWordChar != null)
            {
                var indexOfLastTokenWithWordChar = tokens.LastIndexOf(lastTokenWithWordChar);
                if (lastTokenWithWordChar.EndsWith("."))
                {
                    tokens.RemoveAt(indexOfLastTokenWithWordChar);
                    var lastTokenParts = Regex.Split(lastTokenWithWordChar, "(?=\\.$)");
                    tokens.InsertRange(indexOfLastTokenWithWordChar, lastTokenParts);
                } 
            }

            return tokens;
        }

        // TODO: regroup all those tokenization in a unique big) regex?
        // Tokenize rules 
        private static readonly List<Regex> TokenizationRegexes = new List<Regex>()
        {
            // split before .{2,} if not preceded by '.'
            new Regex("(?<!\\.)(?=\\.{2,})"),
            // split after .{2,} if not followed by '.'
            new Regex("(?<=\\.{2,})(?!\\.)"),
            
            // split before !+ if not preceded by '!'
            new Regex("(?<!!)(?=!+)"),
            // split after !+ if not followed by '!'
            new Regex("(?<=!+)(?!!)"),

            // split before ?+ if not preceded by '?'
            new Regex("(?<!\\?)(?=\\?+)"),
            // split after ?+ if not followed by '?'
            new Regex("(?<=\\?+)(?!\\?)"),
            
            // split after ',' if not followed directly by figure
            new Regex("(?<=,)(?!\\d)"), 
            // split before ',' if not followed directly by figure
            new Regex("((?=,\\D)|(?=,$))"),
            
            // split after ':' if not followed directly by figure
            new Regex("(?<=:)(?!\\d)"), 
            // split before ':' if not followed directly by figure
            new Regex("((?=:\\D)|(?=:$))"),

            // split before 's, 'm, 've, 'll, 're, 'd when at the end of a token (’ == ')
            new Regex("(?=\\'s$|\\'m$|\\'ve$|\\'ll$|\\'re$|\\'d$|’s$|’m$|’ve$|’ll$|’re$|’d$)", RegexOptions.IgnoreCase),

            // split after ' at the beginning of a token (and not 's, 'm, 'll, 've, 're or 'd)
            new Regex("(?<=^\\')(?!s$|m$|ll$|ve$|re$|d$)", RegexOptions.IgnoreCase),
            new Regex("(?<=^’)(?!s$|m$|ll$|ve$|re$|d$)", RegexOptions.IgnoreCase),
            // split before ' at the end of a token
            new Regex("(?=\\'$)"),
            new Regex("(?=’$)"),

            // split before - when at the end of a token and not preceded by -
            new Regex("(?<!\\-)(?=\\-$)"),
            // split after - when at the beginning of a token and not followed by -
            new Regex("(?<=^\\-)(?!\\-)"),
            
            // split before ;, (, ), [, ], {, }, " in all cases
            new Regex("(?=;|\\(|\\)|\\{|\\}|\\[|\\]|\"|…)"),
            // split after ;, (, ), [, ], {, }, " in all cases
            new Regex("(?<=;|\\(|\\)|\\{|\\}|\\[|\\]|\"|…)")
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
                /*if (result.Count != tempTokens.Count)
                {
                    Console.WriteLine("{0} ==> {1}", string.Join("|", result), string.Join("|", tempTokens));
                }*/
                result = tempTokens;
            }

            return result;
        }
    }

}
