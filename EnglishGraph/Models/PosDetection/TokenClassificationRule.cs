using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.PosDetection
{
    public class TokenClassificationRule
    {
        private readonly Predicate<string> isTypePredicate;
        private readonly byte entryPos;

        public TokenClassificationRule(Predicate<string> isTokenOfType, byte entryPos)
        {
            isTypePredicate = isTokenOfType;
            this.entryPos = entryPos;
        }

        public bool IsMatch(string token)
        {
            return isTypePredicate(token);
        }

        public virtual DictionaryEntry GetEntry(string token)
        {
            return new DictionaryEntry()
            {
                Word = token,
                PartOfSpeech = entryPos
            };
        }
    }
}
