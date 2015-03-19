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
                    return new List<string>()
                    {
                        ApplyRules(verb.Word, InfinitiveToThirdPersonPresentRules)
                    };
                case VerbForm.SimplePast:
                    return ApplyRules(verb, InfintiveToSimplePastRules);
                case VerbForm.PastParticiple:
                    return ApplyRules(verb, InfintiveToPastParticpleRules);
                case VerbForm.Gerundive:
                    return new List<string>()
                    {
                        ApplyRules(verb, InfintiveToGerundiveRules)
                    };
                default:
                    return new List<string>(){""};
            }
        }

        private U ApplyRules<T, U>(T input, IEnumerable<GrammarTransformation<T, U>> transformations)
        {
            foreach (var transformation in transformations)
            {
                if (transformation.Condition(input))
                {
                    return transformation.Transform(input);
                }
            }

            // should never get here
            return default(U);
        }

        private static readonly List<string> thirdPersonIrregularVerbSuffixes = new List<string>() { "ss", "x", "ch", "sh", "o" };
        private static readonly Regex thirdPersonIrregularVerbSuffixesRegex = new Regex(string.Format("({0})$", string.Join("|", thirdPersonIrregularVerbSuffixes)));
        private static readonly Regex endsWithConsonantPlusY = new Regex(string.Format("({0})y$", string.Join("|", consonants)));

        private static readonly List<GrammarTransformation<string,string>> InfinitiveToThirdPersonPresentRules = new List<GrammarTransformation<string,string>>()
        {
            new GrammarTransformation<string,string>()
            {
                Condition = thirdPersonIrregularVerbSuffixesRegex.IsMatch,
                Transform = s => s + "es"
            },
            new GrammarTransformation<string,string>()
            {
                Condition = endsWithConsonantPlusY.IsMatch,
                Transform = s => s.Remove(s.Length - 1) + "ies"
            },
            new GrammarTransformation<string,string>()
            {
                Condition = s => true,
                Transform = verb => verb + "s"
            }
        };

        public static Regex ConsonantVowelConsonantEnding = new Regex(string.Format("({0})({1})({2})$", 
            string.Format("{0}|{1}", string.Join("|", consonants), "qu"), 
            string.Join("|", vowels), 
            string.Join("|", doubledConsonants)));

        private static readonly List<GrammarTransformation<DictionaryEntry, List<string>>> InfintiveToSimplePastRules = new List<GrammarTransformation<DictionaryEntry, List<string>>>()
        {
            // irregular verbs (take, abide etc.)
            new GrammarTransformation<DictionaryEntry, List<string>>()
            {
                Condition = de => IrregularVerbs.Instance.AllIrregularVerbs.Any(iv => iv.Infinitive == de.Word),
                Transform = de => IrregularVerbs.Instance.AllIrregularVerbs.First(iv => iv.Infinitive == de.Word).SimplePastForms
            },
            // stop -> stopped
            new GrammarTransformation<DictionaryEntry, List<string>>()
            {
                Condition = de => ConsonantVowelConsonantEnding.IsMatch(de.Word) 
                    && (Pronunciations.IsStressOnLastVowel(de.Pronunciation) || Pronunciations.CountNbOfSyllables(de.Pronunciation) == 1),
                Transform = de => new List<string>(){de.Word + de.Word.Last() + "ed"}
            },
            // charge -> charged
            new GrammarTransformation<DictionaryEntry, List<string>>()
            {
                Condition = de => de.Word.EndsWith("e"),
                Transform = de => new List<string>(){de.Word + "d"}
            },
            // default rule -> add "ed"
            new GrammarTransformation<DictionaryEntry, List<string>>()
            {
                Condition = de => true,
                Transform = de => new List<string>(){de.Word + "ed"}
            }
        };
        private static readonly List<GrammarTransformation<DictionaryEntry, List<string>>> InfintiveToPastParticpleRules = new List<GrammarTransformation<DictionaryEntry, List<string>>>()
        {
            // irregular verbs (take, abide etc.)
            new GrammarTransformation<DictionaryEntry, List<string>>()
            {
                Condition = de => IrregularVerbs.Instance.AllIrregularVerbs.Any(iv => iv.Infinitive == de.Word),
                Transform = de => IrregularVerbs.Instance.AllIrregularVerbs.First(iv => iv.Infinitive == de.Word).PastParticipleForms
            },
            // stop -> stopped
            new GrammarTransformation<DictionaryEntry, List<string>>()
            {
                Condition = de => ConsonantVowelConsonantEnding.IsMatch(de.Word) 
                    && (Pronunciations.IsStressOnLastVowel(de.Pronunciation) || Pronunciations.CountNbOfSyllables(de.Pronunciation) == 1),
                Transform = de => new List<string>(){de.Word + de.Word.Last() + "ed"}
            },
            // charge -> charged
            new GrammarTransformation<DictionaryEntry, List<string>>()
            {
                Condition = de => de.Word.EndsWith("e"),
                Transform = de => new List<string>(){de.Word + "d"}
            },
            // default rule -> add "ed"
            new GrammarTransformation<DictionaryEntry, List<string>>()
            {
                Condition = de => true,
                Transform = de => new List<string>(){de.Word + "ed"}
            }
        };
        
        public static List<Tuple<string,string>> GerundiveExceptions = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("cancel", "cancelling"),
            new Tuple<string, string>("travel", "travelling"),
            new Tuple<string, string>("input", "inputting")
        };

        private static readonly List<GrammarTransformation<DictionaryEntry, string>> InfintiveToGerundiveRules = new List<GrammarTransformation<DictionaryEntry, string>>()
        {
            // Exceptions (travel, cancel, input)
            new GrammarTransformation<DictionaryEntry, string>()
            {
                Condition = de => GerundiveExceptions.Any(g => g.Item1 == de.Word),
                Transform = de => GerundiveExceptions.First(tup => tup.Item1 == de.Word).Item2
            },
            // run -> running
            new GrammarTransformation<DictionaryEntry, string>()
            {
                Condition = de => ConsonantVowelConsonantEnding.IsMatch(de.Word) 
                    && (Pronunciations.IsStressOnLastVowel(de.Pronunciation) || Pronunciations.CountNbOfSyllables(de.Pronunciation) == 1),
                Transform = de => de.Word + de.Word.Last() + "ing"
            },
            // traffic -> trafficking
            new GrammarTransformation<DictionaryEntry, string>()
            {
                Condition = de => de.Word.EndsWith("c"),
                Transform = de => de.Word + "king"
            },
            // die -> dying
            new GrammarTransformation<DictionaryEntry, string>()
            {
                Condition = de => de.Word.EndsWith("ie"),
                Transform = de => de.Word.Remove(de.Word.Length - 2) + "ying"
            },
            // take -> taking
            new GrammarTransformation<DictionaryEntry, string>()
            {
                Condition = de => de.Word.EndsWith("e") 
                    && !de.Word.EndsWith("ee")
                    && !de.Word.EndsWith("nge")
                    && de.Word != "dye"
                    && de.Word != "shoe"
                    && de.Word!= "be",
                Transform = de => de.Word.Remove(de.Word.Length - 1) + "ing"
            },
            // default rule -> add "ing"
            new GrammarTransformation<DictionaryEntry, string>()
            {
                Condition = de => true,
                Transform = de => de.Word + "ing"
            }
        };


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
