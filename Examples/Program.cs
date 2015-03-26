using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnglishGraph.Models;
using Examples.Classes;

namespace Examples
{
    class Program
    {
        private static readonly string PathToProject = Directory.GetCurrentDirectory() + "/../../";

        static void Main(string[] args)
        {
            var db = new EnglishGraphContext();

            var pathToSentenceFile = PathToProject + "Input/sentences/wsj.train";
            var sentenceParser = new SentenceParser();
            var posDetector = new PartOfSpeechDetector();

            var words = db.DictionaryEntries.Select(de => de.Word).ToList();

            var sentences = File.ReadLines(pathToSentenceFile);
            foreach (var sentence in sentences)
            {
                var tokens = sentenceParser.Tokenize(sentence);

                for(var i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    // if figure/punctuation -> can ignore
                    if (StringUtilities.IsFigure(token) || StringUtilities.IsPunctuation(token))
                    {
                        Console.WriteLine("----");
                        Console.WriteLine("'{0}' ignored", token);
                        continue;
                    }

                    var searchedTokens = new List<string>() {token};
                    if (Regex.IsMatch(token, "^\\p{P}+") && token.Length > 2)
                    {
                        var trimedToken = Regex.Replace(token, "^\\p{P}+", "");
                        searchedTokens.Add(trimedToken);
                    }
                    if (i == 0)
                    {
                        var lcTokens = searchedTokens.Select(st => StringUtilities.LowerFirstLetter(st)).ToList();
                        searchedTokens.AddRange(lcTokens);
                    }
                    if (Regex.IsMatch(token, "\\p{P}+$") && token.Length > 2)
                    {
                        var trimedToken = Regex.Replace(token, "\\p{P}+$", "");
                        searchedTokens.Add(trimedToken);
                    }
                    
                    var isInDictionary = words.Intersect(searchedTokens).Any();
                    if (!isInDictionary)
                    {
                        var searchedEntries = searchedTokens
                            .Select(tok => new Tuple<string, byte>(tok, posDetector.Detect(tok, i == 0, i == tokens.Count - 1)))
                            .ToList();
                        Console.WriteLine("----");
                        Console.WriteLine("'{0}' in '{1}'", token, sentence);
                        Console.WriteLine("Create:");
                        for (var j = 0; j < searchedEntries.Count; j++)
                        {
                            Console.WriteLine("{0}. {1} {2}", j, searchedEntries[j].Item1, PartsOfSpeech.Abbrev(searchedEntries[j].Item2));
                        }
                        var key = Console.ReadKey();
                        int selectedIndex;
                        var success = int.TryParse(key.KeyChar.ToString(), out selectedIndex);
                        if (success && selectedIndex < searchedTokens.Count)
                        {
                            Console.WriteLine();
                            // add to dictionary with unknown POS
                            var tokenToCreate = searchedEntries[selectedIndex];
                            var entryCreated = DbUtilities.GetOrCreate(tokenToCreate, db);
                            words.Add(entryCreated.Word);
                        }
                    }
                }
            }


            /*/*var verbShortList = new List<string>()
            {
                "stop", "refer", "visit", "rob", "sit", "begin", "prefer", "listen", "happen", "travel", "cancel", "start", "burn", "remain", "play", "snow",
                "carry", "marry", "kiss", "fix", "watch", "crash", "go"
            };
            var allVerbs = db.DictionaryEntries
                .Where(de => de.PartOfSpeech == PartsOfSpeech.Verb && verbShortList.Contains(de.Word))
                .ToList();#1#

            var conjugator = new VerbConjugator();
            /*foreach (var verb in allVerbs)
            {
                Console.WriteLine("{0} --> {1}", verb.Word, string.Join("|", conjugator.GetVerbForm(verb, VerbConjugator.VerbForm.ThirdPersonSingularPresent)));
            }#1#
            var infinitive = "carry";
            var verb = db.DictionaryEntries.First(de => de.Word == infinitive && de.PartOfSpeech == PartsOfSpeech.Verb);
            var simplePastForms = conjugator.GetVerbForm(verb, VerbConjugator.VerbForm.PastParticiple);
            Console.WriteLine(string.Join("|", simplePastForms));*/


            // load wordnet entries
            //Routines.Load3rdPresentForms(db);

            // load conjunction

            // load pronunciations
            //Routines.LoadGutembergPronunciations(db, PathToProject);

            /*var infitives = db.DictionaryEntries
                .Where(de => de.PartOfSpeech == PartsOfSpeech.Verb)
                .Select(de => de.Word)
                .ToList();
            var specificInfinitives = infitives
                .Where(i => VerbConjugator.ConsonantVowelConsonantEnding.IsMatch(i))
                .ToList();
            Console.WriteLine("{0} specific cases (out of {1} infinitives):", specificInfinitives.Count, infitives.Count);
            foreach (var specificInfinitive in specificInfinitives)
            {
                Console.WriteLine(specificInfinitive);
            }*/
            /*var conjugator = new VerbConjugator();
            foreach (var infitive in infitives)
            {
                Console.WriteLine("{0} -> {1}", infitive, string.Join("/", conjugator.GetForm(infitive, VerbConjugator.VerbForm.ThirdPersonSingularPresent)));
            }*/

            Console.WriteLine("OK");
            Console.ReadLine();
        }


    }
}
