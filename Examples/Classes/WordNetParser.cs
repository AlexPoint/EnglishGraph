using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models;

namespace Examples.Classes
{
    class WordNetParser
    {
        public List<DictionaryEntry> ParseEntries(string filePath, bool excludeMwes)
        {
            var lines = File.ReadAllLines(filePath);
            var entries = lines
                .Skip(1) // skip header
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => line.Split('\t'))
                .Select(parts => new
                {
                    Word = parts[0],
                    PartOfSpeech = ConvertWordNetPos(parts[1]),
                    Definition = parts[2]
                })
                .Where(l => !excludeMwes || !l.Word.Contains(' '))
                .GroupBy(l => new {l.Word, l.PartOfSpeech})
                .Select(grp => new DictionaryEntry()
                {
                    Word = grp.Key.Word,
                    PartOfSpeech = grp.Key.PartOfSpeech,
                    Synsets = grp.Select(e => new SynsetDictionaryEntry()
                    {
                        Synset = new Synset()
                        {
                            Definition = e.Definition
                        }
                    }).ToList()
                })
                .ToList();

            return entries;
        }


        private byte ConvertWordNetPos(string wordnetPos)
        {
            switch (wordnetPos)
            {
                case "n":
                    return PartsOfSpeech.Noun;
                case "v":
                    return PartsOfSpeech.Verb;
                case "s":
                case "a":
                    return PartsOfSpeech.Adjective;
                case "r":
                    return PartsOfSpeech.Adverb;
                default:
                    Console.Write("Unknown POS '{0}'", wordnetPos);
                    return PartsOfSpeech.Unknown;
            }
        }
    }
}
