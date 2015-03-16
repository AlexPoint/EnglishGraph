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
        public const byte ProperNoun = 21;


        // Adjectives -------------------------
        public const byte Adjective = 30;


        // Adverbs ----------------------------
        public const byte Adverb = 40;


        public static string Abbrev(byte pos)
        {
            switch (pos)
            {
                case Verb:
                    return "v.";
                case Verb3RdPersSingular:
                    return "v3.";
                case VerbSimplePast:
                    return "vsp";
                case VerbPastParticiple:
                    return "vpp";
                case Gerundive:
                    return "ger";
                case Noun:
                    return "n.";
                case ProperNoun:
                    return "pn.";
                case Adjective:
                    return "adj.";
                case Adverb:
                    return "adv.";
                default:
                    return "unk.";
            }
        }
    }
}
