using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                .SelectMany(p =>
                    p.EndsWith("'s")
                        ? new List<string>() {p.Substring(0, p.Length - 2), p.Substring(p.Length - 2)}
                        : new List<string>() {p})
                .ToList();

            return tokens;
        }
    }
}
