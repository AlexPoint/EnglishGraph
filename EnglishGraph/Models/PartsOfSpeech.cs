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
        public const byte Verb1stPersSingular = 15;
        public const byte Verb2ndPersSingular = 16;
        public const byte Modal = 17;
        public const byte NegativeContraction = 18;


        // Nouns ------------------------------
        public const byte Noun = 20;
        public const byte ProperNoun = 21;
        public const byte NounPlural = 22;
        public const byte ProperNounPlural = 23;
        

        // Adjectives -------------------------
        public const byte Adjective = 30;
        public const byte Comparative = 31;
        public const byte Superlative = 32;


        // Adverbs ----------------------------
        public const byte Adverb = 40;


        // Conjunctions -----------------------
        public const byte Conjunction = 50;
        public const byte CoordinatingConjunction = 51;
        public const byte SubordinatingConjunction = 52;


        // Determiners -----------------------
        public const byte Determiner = 60;
        public const byte ArticleDeterminer = 61;
        public const byte DemonstrativeDeterminer = 62;
        public const byte PossessiveDeterminer = 63;
        

        // Prepositions -----------------------
        public const byte Preposition = 70;


        // Pronouns -----------------------
        public const byte Pronoun = 80;
        public const byte SubjectPersonalPronoun = 81;
        public const byte ObjectPersonalPronoun = 82;
        public const byte ReflexivePersonalPronoun = 83;
        public const byte PossessivePronoun = 84;
        public const byte InterrogativePronoun = 85;
        public const byte IndefinitePronoun = 86;
        public const byte RelativePronoun = 87;

        // Acronyms & abbreviations ------
        // Are abbreviations a true part of speech?
        public const byte Abbreviation = 90;
        public const byte Contractions = 91;

        // Misc --------------------------
        public const byte Number = 100;
        public const byte Percentage = 101;
        public const byte Amount = 102;
        public const byte Fraction = 103;
        public const byte Time = 104;
        public const byte Punctuation = 105;
        public const byte Compound = 106;
        public const byte CompoundSlash = 107;


        public static bool IsVerb(byte pos)
        {
            return 10 <= pos && pos < 20;
        }

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
                case Verb1stPersSingular:
                    return "v1.";
                case Verb2ndPersSingular:
                    return "v2.";
                case Modal:
                    return "mod.";
                case NegativeContraction:
                    return "v. neg. cont.";
                case Noun:
                    return "n.";
                case ProperNoun:
                    return "pn.";
                case NounPlural:
                    return "n. pl.";
                case ProperNounPlural:
                    return "pn. pl.";
                case Adjective:
                    return "adj.";
                case Comparative:
                    return "adj. comp.";
                case Superlative:
                    return "adj. sup.";
                case Adverb:
                    return "adv.";
                case Conjunction:
                    return "conj.";
                case CoordinatingConjunction:
                    return "c. conj.";
                case SubordinatingConjunction:
                    return "s. conj.";
                case Determiner:
                    return "det.";
                case ArticleDeterminer:
                    return "art. det.";
                case DemonstrativeDeterminer:
                    return "dem. det.";
                case PossessiveDeterminer:
                    return "poss. det.";
                case Preposition:
                    return "prep.";
                case Pronoun:
                    return "pron.";
                case SubjectPersonalPronoun:
                    return "sub. pers. pron.";
                case ObjectPersonalPronoun:
                    return "obj. pers. pron.";
                case ReflexivePersonalPronoun:
                    return "ref. pers. pron.";
                case PossessivePronoun:
                    return "poss. pron.";
                case IndefinitePronoun:
                    return "indef. pron.";
                case InterrogativePronoun:
                    return "inter. pron.";
                case RelativePronoun:
                    return "rel. pron.";
                case Abbreviation:
                    return "abbr.";
                case Contractions:
                    return "cont.";
                case Number:
                    return "num.";
                case Percentage:
                    return "perc.";
                case Amount:
                    return "amou.";
                case Fraction:
                    return "frac.";
                case Time:
                    return "time";
                case Punctuation:
                    return "punct.";
                case Compound:
                    return "comp.";
                case CompoundSlash:
                    return "comp. /";
                default:
                    return "unk.";
            }
        }
    }
}
