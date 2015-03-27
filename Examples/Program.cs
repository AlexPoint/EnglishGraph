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

            var pathToSentenceFile = PathToProject + "Input/sentences/wsj.train";
            var sentenceParser = new SentenceParser();

            RunUnknownWordDetection(db);

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

        
        private static void RunUnknownWordDetection(EnglishGraphContext db)
        {
            var pathToSentenceFile = PathToProject + "Input/sentences/wsj.train";
            var sentenceParser = new SentenceParser();

            var entries = db.DictionaryEntries.ToList();
            var posDetector = new PartOfSpeechDetector();

            var words = entries.Select(de => de.Word).ToList();

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
                        Console.WriteLine("----");
                        Console.WriteLine("'{0}' ignored", token);
                        continue;
                    }
                    
                    var isInDictionary = words.Any(w => w == token);
                    if (!isInDictionary)
                    {
                        // test if the word already exist with a different case
                        var wordsWithDifferentCase = words.Where(w => string.Equals(w, token, StringComparison.CurrentCultureIgnoreCase)).ToList();
                        if (wordsWithDifferentCase.Any())
                        {
                            if (i > 0)
                            {
                                Console.WriteLine("'{0}' not in dictionary but '{1}' exist", token, string.Join("|", wordsWithDifferentCase)); 
                            }
                            continue;
                        }

                        var searchedEntry = posDetector.Detect(token, i == 0, i == tokens.Count - 1, db.DictionaryEntries);
                        Console.WriteLine("----");
                        Console.WriteLine("'{0}' in '{1}'", token, sentence);
                        Console.WriteLine("Create: {0} {1} ('y' for yes)", searchedEntry,
                            searchedEntry.StemmedFromRelationships != null && searchedEntry.StemmedFromRelationships.Any() ? 
                            string.Format("{0} of {1}", searchedEntry.StemmedFromRelationships.First().Type, searchedEntry.StemmedFromRelationships.First().Source.Word):
                            "");
                        
                        var key = Console.ReadKey();
                        if (key.KeyChar == 'y')
                        {
                            Console.WriteLine();
                            // add to dictionary
                            var entryCreated = DbUtilities.GetOrCreate(searchedEntry, db);
                            words.Add(entryCreated.Word);
                        }
                        /*int selectedIndex;
                        var success = int.TryParse(key.KeyChar.ToString(), out selectedIndex);
                        if (success && selectedIndex < searchedTokens.Count)
                        {
                            Console.WriteLine();
                            // add to dictionary with unknown POS
                            var tokenToCreate = searchedEntries[selectedIndex];
                            var entryCreated = DbUtilities.GetOrCreate(tokenToCreate, db);
                            words.Add(entryCreated.Word);
                        }*/
                    }
                }
            }
        }
    }
}
