using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class DictionaryEntryRelationship
    {
        public int Id { get; set; }
        public DictionaryEntry Source { get; set; }
        public DictionaryEntry Target { get; set; }
        public byte Type { get; set; }
    }

    public class DictionaryEntryRelationshipTypes
    {
        public const byte ThirdPersonPresent = 11;
        public const byte SimplePast = 12;
        public const byte PastParticiple = 13;
        public const byte Gerundive = 14;
        public const byte InfinitiveToNoun = 15;
        public const byte NegativeContractionToVerb = 18;

        public const byte NounPlural = 22;
        public const byte ProperNounPlural = 23;

        public const byte ComparativeToAdjective = 31;
        public const byte SuperlativeToAdjective = 32;


        public const byte NounToAdjective = 25;
        public const byte NounToNoun = 26;

        public const byte AdjectiveToNoun = 30;

        public const byte AdverbToAdjective = 40;

        public const byte ContractedFormOf = 91;

        // TODO: not sure this is the right approach (if yes, do the same for the suffix) 
        public const byte Prefix = 92;
        public const byte PartOf = 93;
    }
}
