using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnglishGraph.Models;
using EnglishGraph.Models.PosDetection;
using Examples.Classes;
using Examples.Migrations;

namespace Examples
{
    class Program
    {
        private static readonly string PathToProject = Directory.GetCurrentDirectory() + "/../../";

        static void Main(string[] args)
        {
            var db = new EnglishGraphContext();

            Routines.LoadIrregularComparatives(db);
            Routines.LoadIrregularSuperlatives(db);
            Routines.LoadNegativeContractions(db);

            var pathToToeknizeFile = PathToProject + "Input/sentences/exceptions.train";

            var sentenceParser = new SentenceParser();
            

            RunUnknownWordDetection(db, PartsOfSpeech.NounPlural);

            /*var testSentence = "\"And there has been a drastic decline in the R.O.I. of unincorporated business assets -- thanks to industry consolidation and a decline in family farms.\"";
            var testTokens = sentenceParser.Tokenize(testSentence);
            Console.WriteLine(testSentence);
            Console.WriteLine(string.Join(" | ", testTokens));*/

            // load entries
            //Routines.Load3rdPresentForms(db);

            // load pronunciations
            //Routines.LoadGutembergPronunciations(db, PathToProject);
            
            Console.WriteLine("OK");
            Console.ReadLine();
        }

        
        private static void RunUnknownWordDetection(EnglishGraphContext db, byte onlyEntriesOfPos)
        {
            var pathToSentenceFile = PathToProject + "Input/sentences/wsj.train";
            var sentenceParser = new SentenceParser();

            var entries = db.DictionaryEntries.ToList();
            var dictionary = new EnglishDictionary(entries);
            var posDetector = new PartOfSpeechDetector();

            var sentences = File.ReadLines(pathToSentenceFile);
            foreach (var sentence in sentences)
            {
                var tokens = sentenceParser.Tokenize(sentence);
                var firstWordToken = tokens.FirstOrDefault(StringUtilities.IsWordToken);
                var indexOfFirstWordToken = firstWordToken != null ? tokens.IndexOf(firstWordToken) : -1;
                var lastWordToken = tokens.LastOrDefault(StringUtilities.IsWordToken);
                var indexOfLastWordToken = lastWordToken != null ? tokens.LastIndexOf(lastWordToken) : -1;

                for (var i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    if (StringUtilities.IsFigure(token) 
                        || StringUtilities.IsPunctuation(token)
                        || StringUtilities.IsTime(token))
                    {
                        // if figure/punctuation -> can ignore
                        continue;
                    }
                    else if (StringUtilities.IsCompoundWord(token))
                    {
                        // if compound word, ignore for the moment
                        //Console.WriteLine("'{0}' ignored", token);
                        continue;
                    }

                    var correspondingEntries = posDetector.DetectPos(token, i == indexOfFirstWordToken, i == indexOfLastWordToken,
                        dictionary);
                    if (!correspondingEntries.Any() ||
                        correspondingEntries.All(ent => ent.PartOfSpeech == PartsOfSpeech.Unknown))
                    {
                        Console.WriteLine("Couldn't detect POS for '{0}'", token);
                    }
                    
                    /*var isInDictionary = dictionary.Contains(token);
                    if (!isInDictionary)
                    {
                        // test if the word already exist with a different case
                        var hasWordWithDifferentCase = dictionary.ContainsIgnoreCase(token);
                        if (hasWordWithDifferentCase)
                        {
                            continue;
                        }

                        var searchedEntry = posDetector.DetectPos(token, i == 0, i == indexOfLastWordToken, dictionary);
                        if (searchedEntry.PartOfSpeech == onlyEntriesOfPos)
                        {
                            Console.WriteLine("----");
                            Console.WriteLine("'{0}' in '{1}'", token, sentence);
                            
                            // add to dictionary
                            var entryCreated = DbUtilities.GetOrCreate(searchedEntry, db);
                            dictionary.Add(entryCreated);
                        }
                    }*/
                }
            }
        }
    }
}
