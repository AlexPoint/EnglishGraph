using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.PosDetection
{
    public class PrefixBasedPosDetectionRule:PosDetectionRule
    {
        public PrefixBasedPosDetectionRule(string prefix, byte derivedFormType)
        {
            this.MatchingCondition = a => a.Token.StartsWith(prefix);
            this.DictionaryEntryCreator = tok => new DictionaryEntry()
            {
                Word = tok,
                // the POS is always the same as the derived form
                PartOfSpeech = derivedFormType,
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
