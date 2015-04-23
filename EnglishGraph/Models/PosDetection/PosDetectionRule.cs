using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.PosDetection
{
    public class PosDetectionRule
    {
        public Predicate<TokenAndPositionInSentence> MatchingCondition { get; set; }
        public Func<string, DictionaryEntry> DictionaryEntryCreator { get; set; }
    }
}
