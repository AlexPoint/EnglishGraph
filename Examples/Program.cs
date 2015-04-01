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
using Examples.Migrations;

namespace Examples
{
    class Program
    {
        private static readonly string PathToProject = Directory.GetCurrentDirectory() + "/../../";

        static void Main(string[] args)
        {
            var db = new EnglishGraphContext();

            Routines.LoadVerb1stAnd2ndForms(db);

            Routines.LoadContractions(db);

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

                for (var i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    // if figure/punctuation -> can ignore
                    if (StringUtilities.IsFigure(token) || StringUtilities.IsPunctuation(token)
                        || StringUtilities.IsCompoundWord(token))
                    {
                        //Console.WriteLine("----");
                        //Console.WriteLine("'{0}' ignored", token);
                        continue;
                    }
                    
                    var isInDictionary = dictionary.Contains(token);
                    if (!isInDictionary)
                    {
                        // test if the word already exist with a different case
                        var hasWordWithDifferentCase = dictionary.Contains(token, StringComparison.InvariantCultureIgnoreCase);
                        if (hasWordWithDifferentCase)
                        {
                            continue;
                        }

                        var searchedEntry = posDetector.Detect(token, i == 0, i == tokens.Count - 1, dictionary);
                        if (searchedEntry.PartOfSpeech == onlyEntriesOfPos)
                        {
                            Console.WriteLine("----");
                            Console.WriteLine("'{0}' in '{1}'", token, sentence);
                            /*Console.WriteLine("Create: {0} {1} ('y' for yes)", searchedEntry,
                                searchedEntry.StemmedFromRelationships != null && searchedEntry.StemmedFromRelationships.Any() ?
                                string.Format("{0} of {1}", searchedEntry.StemmedFromRelationships.First().Type, searchedEntry.StemmedFromRelationships.First().Source.Word) :
                                "");

                            var key = Console.ReadKey();
                            if (key.KeyChar == 'y')
                            {
                                Console.WriteLine();*/
                                // add to dictionary
                                var entryCreated = DbUtilities.GetOrCreate(searchedEntry, db);
                                dictionary.Add(entryCreated);
                            /*} */
                        }
                    }
                }
            }
        }
    }
}
