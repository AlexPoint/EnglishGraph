using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.PosDetection
{
    public class CompoundTokenClasificationRule: TokenClassificationRule
    {
        private readonly char _separator;
        public CompoundTokenClasificationRule(char separator, byte entryPos):
            base(tok => tok.Contains(separator), entryPos)
        {
            this._separator = separator;
        }

        public override DictionaryEntry GetEntry(string token)
        {
            var entry = base.GetEntry(token);
            entry.StemmedFromRelationships = token
                .Split(_separator)
                .Select(part => new DictionaryEntryRelationship()
                {
                    Source = entry,
                    Target = new DictionaryEntry()
                    {
                        Word = part,
                        PartOfSpeech = PartsOfSpeech.Unknown
                    },
                    Type = DictionaryEntryRelationshipTypes.PartOf
                })
                .ToList();
            return entry;
        }
    }
}
