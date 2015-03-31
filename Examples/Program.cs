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

            var pathToToeknizeFile = PathToProject + "Input/sentences/wsj-tokenize.train";

            var sentence = "The sole limited partner of the partnership is Westwood Brick Lime Inc., an indirect subsidiary of Westwood Group Inc.";
            var sentenceParser = new SentenceParser();
            var test = sentenceParser.Tokenize(sentence);

            var nbOfSpaceTokens = 0;
            var validTokens = 0;
            var missingTokenization = 0;
            var unrelevantTokenization = 0;
            var lines = File.ReadAllLines(pathToToeknizeFile);
            foreach (var line in lines)
            {
                nbOfSpaceTokens += line.Count(c => c == ' ') + 1;
                var tokens = line.Split(new[] {' ', '|'});
                var computedTokens = sentenceParser.Tokenize(line.Replace("|", ""));

                var j = 0;
                for (var i = 0; i < tokens.Length; i++)
                {
                    var token = tokens[i];
                    if (j >= computedTokens.Count)
                    {
                        missingTokenization += token.Length - i;
                        break;
                    }
                    var computedToken = computedTokens[j];
                    if (token == computedToken)
                    {
                        validTokens++;
                        j++;
                    }
                    else if (token.Contains(computedToken))
                    {
                        missingTokenization++;
                        j++;
                        // don't increase i
                        if (i >= tokens.Length || (j < computedTokens.Count && tokens[i] != computedTokens[j]))
                        {
                            i--;
                        }
                    }
                    else if (computedToken.Contains(token))
                    {
                        unrelevantTokenization++;
                        // don't increase j
                        if (j < computedTokens.Count - 1 && computedTokens[j + 1] == tokens[i + 1])
                        {
                            j++;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error on line '{0}'", line);
                    }
                }
            }

            Console.WriteLine("{0} valid tokenizations", validTokens);
            Console.WriteLine("{0} space tokens", nbOfSpaceTokens);
            Console.WriteLine("{0} missing tokenizations", missingTokenization);
            Console.WriteLine("{0} unrelevant tokenizations", unrelevantTokenization);

            //RunUnknownWordDetection(db, PartsOfSpeech.NounPlural);

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
                        //Console.WriteLine("----");
                        //Console.WriteLine("'{0}' ignored", token);
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
                                //Console.WriteLine("'{0}' not in dictionary but '{1}' exist", token, string.Join("|", wordsWithDifferentCase)); 
                            }
                            continue;
                        }

                        var searchedEntry = posDetector.Detect(token, i == 0, i == tokens.Count - 1, db.DictionaryEntries);
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
                                words.Add(entryCreated.Word);
                            /*} */
                        }
                    }
                }
            }
        }
    }
}
