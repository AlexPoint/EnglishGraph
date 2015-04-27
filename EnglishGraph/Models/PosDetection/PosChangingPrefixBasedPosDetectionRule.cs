using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.PosDetection
{
    public class PosChangingPrefixBasedPosDetectionRule: PosDetectionRule
    {
        public PosChangingPrefixBasedPosDetectionRule(string prefix, byte originalPos, byte derivedFormType)
        {
            this.MatchingCondition = a => a.Token.StartsWith(prefix);
            this.DictionaryEntryCreator = tok => new DictionaryEntry()
            {
                Word = tok,
                PartOfSpeech = originalPos,
                StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                {
                    new DictionaryEntryRelationship()
                    {
                        Type = DictionaryEntryRelationshipTypes.Prefix,
                        Source = new DictionaryEntry()
                        {
                            Word = tok.Substring(prefix.Length),
                            PartOfSpeech = derivedFormType
                        }
                    }
                }
            };
        }
    }
}
