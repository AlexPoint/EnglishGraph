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
        private static readonly List<char> vowels = new List<char>(){'a', 'e', 'i', 'o', 'u'};

        public List<string> GetForm(string infinitive, VerbForm verbForm)
        {
            if (string.IsNullOrEmpty(infinitive))
            {
                // TODO: log something
                return new List<string>(){""};
            }

            switch (verbForm)
            {
                case VerbForm.ThirdPersonSingularPresent:
                    return new List<string>() {GetThirdPersonSingularPresentForm(infinitive)};
                case VerbForm.SimplePast:
                    return GetSimplePastForm(infinitive);
                case VerbForm.PastParticiple:
                    return GetPastParticipleForm(infinitive);
                    case VerbForm.Gerundive:
                    return new List<string>() {GetGerundiveForm(infinitive)};
                default:
                    return new List<string>(){""};
            }
        }

        private static readonly List<string> thirdPersonIrregularVerbSuffixes = new List<string>() { "SS", "X", "CH", "SH", "O" };
        private static readonly Regex thirdPersonIrregularVerbSuffixesRegex = new Regex(string.Format("({0})$", string.Join("|", thirdPersonIrregularVerbSuffixes)));
        private string GetThirdPersonSingularPresentForm(string infinitive)
        {
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
            string.Join("|", consonants), string.Join("|", vowels), string.Join("|", consonants)));
        private List<string> GetSimplePastForm(string infinitive)
        {
            var correspondingIrregularVerb = IrregularVerbs.Instance
                .AllIrregularVerbs
                .FirstOrDefault(iv => iv.Infinitive == infinitive);
            if (correspondingIrregularVerb != null)
            {
                return correspondingIrregularVerb.SimplePastForms;
            }
            else if (ConsonantVowelConsonantEnding.IsMatch(infinitive)) //and stress on last vowel
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
        
        private List<string> GetPastParticipleForm(string infinitive)
        {
            var correspondingIrregularVerb = IrregularVerbs.Instance
                .AllIrregularVerbs
                .FirstOrDefault(iv => iv.Infinitive == infinitive);
            if (correspondingIrregularVerb != null)
            {
                return correspondingIrregularVerb.PastParticipleForms;
            }
            else if (ConsonantVowelConsonantEnding.IsMatch(infinitive)) //and stress on last vowel
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
        private string GetGerundiveForm(string infinitive)
        {
            if (ConsonantVowelConsonantEnding.IsMatch(infinitive)) //+ stress on last vowel
            {
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
