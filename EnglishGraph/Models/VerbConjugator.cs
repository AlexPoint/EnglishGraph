using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class VerbConjugator
    {
        public enum Person { FirstSingular, SecondSingular, ThirdSingular, PastForm }
        //public enum Tense { Present, Past, }
        public enum VerbForm { ThirdPersonSingularPresent, SimplePast, PastParticiple, Gerundive }

        private static readonly List<char> consonants = new List<char>(){'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'};

        // Not a general case: revved, wowwed, and bookkeeper
        private static readonly List<char> rarelyDoubledConsonants = new List<char>() { 'h', 'j', 'q', 'v', 'w', 'x', 'y' };

        private static readonly List<char> doubledConsonants = consonants.Except(rarelyDoubledConsonants).ToList();

        private static readonly List<char> vowels = new List<char>(){'a', 'e', 'i', 'o', 'u'};

        public List<string> GetVerbForm(DictionaryEntry verb, VerbForm verbForm)
        {
            if (verb.PartOfSpeech != PartsOfSpeech.Verb || string.IsNullOrEmpty(verb.Word))
            {
                // TODO: log something
                return new List<string>(){""};
            }

            switch (verbForm)
            {
                case VerbForm.ThirdPersonSingularPresent:
                    return new List<string>() {GetThirdPersonSingularPresentForm(verb)};
                case VerbForm.SimplePast:
                    return GetSimplePastForm(verb);
                case VerbForm.PastParticiple:
                    return GetPastParticipleForm(verb);
                    case VerbForm.Gerundive:
                    return new List<string>() {GetGerundiveForm(verb)};
                default:
                    return new List<string>(){""};
            }
        }

        private static readonly List<string> thirdPersonIrregularVerbSuffixes = new List<string>() { "SS", "X", "CH", "SH", "O" };
        private static readonly Regex thirdPersonIrregularVerbSuffixesRegex = new Regex(string.Format("({0})$", string.Join("|", thirdPersonIrregularVerbSuffixes)), RegexOptions.IgnoreCase);
        private string GetThirdPersonSingularPresentForm(DictionaryEntry verb)
        {
            var infinitive = verb.Word;
            if (infinitive == "be")
            {
                return "is";
            }
            else if (thirdPersonIrregularVerbSuffixesRegex.IsMatch(infinitive))
            {
                return infinitive + "es";
            }
            else if (infinitive.Last() == 'y' && consonants.Contains(infinitive[infinitive.Length - 2]))
            {
                return infinitive.TrimEnd('y') + "ies";
            }
            else
            {
                return infinitive + "s";
            }
        }

        public static Regex ConsonantVowelConsonantEnding = new Regex(string.Format("({0})({1})({2})$", 
            string.Join("|", consonants), string.Join("|", vowels), string.Join("|", doubledConsonants)));
        private List<string> GetSimplePastForm(DictionaryEntry verb)
        {
            var infinitive = verb.Word;
            var correspondingIrregularVerb = IrregularVerbs.Instance
                .AllIrregularVerbs
                .FirstOrDefault(iv => iv.Infinitive == infinitive);
            if (correspondingIrregularVerb != null)
            {
                return correspondingIrregularVerb.SimplePastForms;
            }
            else if (ConsonantVowelConsonantEnding.IsMatch(infinitive) && Pronunciations.IsStressOnLastVowel(verb.Pronunciation))
            {
                return new List<string>() {infinitive + infinitive.Last() + "ed"};
            }
            else if (infinitive.EndsWith("e"))
            {
                return new List<string>() {infinitive + "d"};
            }
            else
            {
                return new List<string>(){infinitive + "ed"};
            }
        }
        
        private List<string> GetPastParticipleForm(DictionaryEntry verb)
        {
            var infinitive = verb.Word;
            var correspondingIrregularVerb = IrregularVerbs.Instance
                .AllIrregularVerbs
                .FirstOrDefault(iv => iv.Infinitive == infinitive);
            if (correspondingIrregularVerb != null)
            {
                return correspondingIrregularVerb.PastParticipleForms;
            }
            else if (ConsonantVowelConsonantEnding.IsMatch(infinitive) && Pronunciations.IsStressOnLastVowel(verb.Pronunciation))
            {
                return new List<string>() {infinitive + infinitive.Last() + "ed"};
            }
            else if (infinitive.EndsWith("e"))
            {
                return new List<string>() {infinitive + "d"};
            }
            else
            {
                return new List<string>(){infinitive + "ed"};
            }
        }
        private string GetGerundiveForm(DictionaryEntry verb)
        {
            var infinitive = verb.Word;
            // cancel and travel are two exceptions
            if (infinitive == "cancel")
            {
                return "cancelling";
            }
            else if (infinitive == "travel")
            {
                return "travelling";
            }
            else if (ConsonantVowelConsonantEnding.IsMatch(infinitive) && 
                (Pronunciations.IsStressOnLastVowel(verb.Pronunciation) || Pronunciations.CountNbOfSyllables(verb.Pronunciation) == 1))
            {
                // double last consonant
                return infinitive + infinitive.Last() + "ing";
            }
            else if (infinitive.EndsWith("ie"))
            {
                return infinitive.Remove(infinitive.Length - 2) + "ying";
            }
            else if (infinitive.EndsWith("e"))
            {
                return infinitive.Remove(infinitive.Length - 1) + "ing";
            }
            else
            {
                return infinitive + "ing";
            }
        }

    }
}
