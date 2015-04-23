using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.PosDetection
{
    public class SuffixBasedPosDetectionRule: PosDetectionRule
    {

        public SuffixBasedPosDetectionRule(string suffix, string suffixToDerivedForm, 
            byte deType, byte deRelationshipType, byte derivedFormType)
        {
            this.MatchingCondition = a => a.Token.EndsWith(suffix);
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
                            Word = tok.Substring(0, tok.Length - suffix.Length) + suffixToDerivedForm,
                            PartOfSpeech = derivedFormType
                        }
                    }
                }
            };
        }
    }
}
