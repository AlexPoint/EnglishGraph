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

            var counter = 0;
            var abbrevTokens = new List<string>();
            var sentences = File.ReadLines(pathToSentenceFile);
            foreach (var sentence in sentences)
            {
                var tokens = sentenceParser.Tokenize(sentence);

                Console.WriteLine(string.Join(" | ", tokens));
            }

            /*foreach (var abbrevToken in abbrevTokens.Distinct())
            {
                Console.WriteLine(abbrevToken);
            }*/

            // load entries
            //Routines.Load3rdPresentForms(db);

            // load pronunciations
            //Routines.LoadGutembergPronunciations(db, PathToProject);
            
            Console.WriteLine("OK");
            Console.ReadLine();
        }

        
        private void RunUnknownWordDetection(EnglishGraphContext db)
        {
            var pathToSentenceFile = PathToProject + "Input/sentences/wsj.train";
            var sentenceParser = new SentenceParser();
            var posDetector = new PartOfSpeechDetector();

            var words = db.DictionaryEntries.Select(de => de.Word).ToList();

            var sentences = File.ReadLines(pathToSentenceFile);
            foreach (var sentence in sentences)
            {
                var tokens = sentenceParser.Tokenize(sentence);

                for (var i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    // if figure/punctuation -> can ignore
                    if (StringUtilities.IsFigure(token) || StringUtilities.IsPunctuation(token))
                    {
                        Console.WriteLine("----");
                        Console.WriteLine("'{0}' ignored", token);
                        continue;
                    }

                    var searchedTokens = new List<string>() { token };
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
        }
    }
}
