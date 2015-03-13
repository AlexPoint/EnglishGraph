using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public static class PartsOfSpeech
    {
        public const byte Unknown = 0;
        
        // Verbs ------------------------------
        public const byte Verb = 10;
        public const byte Verb3RdPersSingular = 11;
        public const byte VerbSimplePast = 12;
        public const byte VerbPastParticiple = 13;
        public const byte Gerundive = 14;


        // Nouns ------------------------------
        public const byte Noun = 20;


        // Adjectives -------------------------
        public const byte Adjective = 30;


        // Adverbs ----------------------------
        public const byte Adverb = 40;
    }
}
