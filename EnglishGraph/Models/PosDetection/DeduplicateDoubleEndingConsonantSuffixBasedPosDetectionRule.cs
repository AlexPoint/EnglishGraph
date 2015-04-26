using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.PosDetection
{
    public class DeduplicateDoubleEndingConsonantSuffixBasedPosDetectionRule: PosDetectionRule
    {
        public DeduplicateDoubleEndingConsonantSuffixBasedPosDetectionRule(string suffix, 
            string suffixToDerivedForm, byte deType, byte deRelationshipType, byte derivedFormType)
        {
            this.MatchingCondition = a => a.Token.EndsWith(suffix)
                && a.Token.Length > suffix.Length + 2
                // the last two letters must be the same - TODO: include the consonant condition if necessary
                && a.Token.Substring(0, a.Token.Length - suffix.Length).LastOrDefault() == a.Token.Substring(0, a.Token.Length - suffix.Length).Reverse().Skip(1).FirstOrDefault();
            this.DictionaryEntryCreator = tok => new DictionaryEntry()
            {
                Word = tok,
                PartOfSpeech = deType,
                StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                {
                    new DictionaryEntryRelationship()
                    {
                        Type = deRelationshipType,
                        Source = new DictionaryEntry()
                        {
                            // deduplicate last letter (hence the -1)
                            Word = tok.Substring(0, tok.Length - suffix.Length -1) + suffixToDerivedForm,
                            PartOfSpeech = derivedFormType
                        }
                    }
                }
            };
        }
    }
}
