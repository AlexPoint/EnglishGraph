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
        private static readonly List<char> rarelyDoubledConsonants = new List<char>() { 'h', 'w', 'x', 'y', 'j', 'q', /*'v'*/ };

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

        private static readonly List<string> thirdPersonIrregularVerbSuffixes = new List<string>() { "ss", "x", "ch", "sh", "o" };
        private static readonly Regex thirdPersonIrregularVerbSuffixesRegex = new Regex(string.Format("({0})$", string.Join("|", thirdPersonIrregularVerbSuffixes)));
        private static readonly Regex endsWithConsonantPlusY = new Regex(string.Format("({0})y$", string.Join("|", consonants)));

        private static readonly List<GrammarTransformation> ThirdPersonPresentRules = new List<GrammarTransformation>()
        {
            new GrammarTransformation()
            {
                Condition = thirdPersonIrregularVerbSuffixesRegex.IsMatch,
                Transform = s => s + "es"
            },
            new GrammarTransformation()
            {
                Condition = endsWithConsonantPlusY.IsMatch,
                Transform = s => s.Remove(s.Length - 1) + "ies"
            },
            new GrammarTransformation()
            {
                Condition = s => true,
                Transform = verb => verb + "s"
            }
        };
        private string GetThirdPersonSingularPresentForm(DictionaryEntry verb)
        {
            /*var infinitive = verb.Word;
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
            }*/
            var infinitive = verb.Word;
            foreach (var thirdPersonPresentRule in ThirdPersonPresentRules)
            {
                if (thirdPersonPresentRule.Condition(infinitive))
                {
                    return thirdPersonPresentRule.Transform(infinitive);
                }
            }

            // no matching rule
            return "";
        }

        public static Regex ConsonantVowelConsonantEnding = new Regex(string.Format("({0})({1})({2})$", 
            string.Format("{0}|{1}", string.Join("|", consonants), "qu"), 
            string.Join("|", vowels), 
            string.Join("|", doubledConsonants)));
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

        public static List<Tuple<string,string>> GerundiveExceptions = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("cancel", "cancelling"),
            new Tuple<string, string>("travel", "travelling"),
            new Tuple<string, string>("input", "inputting")
        };
        private string GetGerundiveForm(DictionaryEntry verb)
        {
            var infinitive = verb.Word;
            var gerundiveException = GerundiveExceptions.FirstOrDefault(tup => tup.Item1 == infinitive);
            if (gerundiveException != null)
            {
                return gerundiveException.Item2;
            }
            else if (ConsonantVowelConsonantEnding.IsMatch(infinitive) && 
                (Pronunciations.IsStressOnLastVowel(verb.Pronunciation) || Pronunciations.CountNbOfSyllables(verb.Pronunciation) == 1))
            {
                // run -> running
                return infinitive + infinitive.Last() + "ing";
            }
            else if (infinitive.EndsWith("c"))
            {
                // traffic -> trafficking
                return infinitive + "king";
            }
            else if (infinitive.EndsWith("ie"))
            {
                // die -> dying
                return infinitive.Remove(infinitive.Length - 2) + "ying";
            }
            else if (infinitive.EndsWith("e") 
                && !infinitive.EndsWith("ee")
                && !infinitive.EndsWith("nge")
                && infinitive != "dye"
                && infinitive != "shoe"
                && infinitive != "be")
            {
                // take -> taking
                return infinitive.Remove(infinitive.Length - 1) + "ing";
            }
            else
            {
                return infinitive + "ing";
            }
        }

        public List<string> GetPotentialInfinitiveFormsFromGerundive(string gerundive)
        {
            if (string.IsNullOrEmpty(gerundive)) { return new List<string>(){""};}

            var gerundiveException = GerundiveExceptions.FirstOrDefault(tup => tup.Item2 == gerundive);
            if (gerundiveException != null)
            {
                return new List<string>() {gerundiveException.Item1};
            }
            else
            {
                if (gerundive.EndsWith("ing"))
	            {
		            // trim ing at the end
	                var withoutIngTermination = gerundive.Remove(gerundive.Length - 3);
	                if (withoutIngTermination.EndsWith("ck"))
	                {
                        // sticking -> stick // trafficking -> traffic
	                    return new List<string>()
	                    {
                            withoutIngTermination,
	                        withoutIngTermination.Remove(withoutIngTermination.Length - 1)
	                    };
	                }
                    else if (withoutIngTermination.Last() == 'y')
                    {
                        // playing -> play // dying -> die
                        return new List<string>()
                        {
                            withoutIngTermination, 
                            withoutIngTermination.Remove(withoutIngTermination.Length - 1) + "ie"
                        };
                    }
                    else if (withoutIngTermination[withoutIngTermination.Length - 1] == withoutIngTermination[withoutIngTermination.Length - 2]
                        && consonants.Contains(withoutIngTermination[withoutIngTermination.Length - 1]))
                    {
                        // dwelling -> dwell // running -> run
                        return new List<string>()
                        {
                            withoutIngTermination,
                            withoutIngTermination.Remove(withoutIngTermination.Length - 1)
                        };
                    }
                    else if (consonants.Contains(withoutIngTermination.Last()))
                    {
                        // taking -> take // fucking -> fuck
                        return new List<string>()
                        {
                            withoutIngTermination,
                            withoutIngTermination + "e"
                        };
                    }
                    else
                    {
                        // ends with a vowel
                        // doing -> do
                        return new List<string>(){ withoutIngTermination };
                    }
	            }
            }

            // if we get here, log a warning
            return new List<string>(){""};
        }

    }
}
