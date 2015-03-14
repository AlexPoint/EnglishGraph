using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

            var infitives = db.DictionaryEntries
                .Where(de => de.PartOfSpeech == PartsOfSpeech.Verb)
                .Select(de => de.Word)
                .ToList();
            var conjugator = new VerbConjugator();
            foreach (var infitive in infitives)
            {
                Console.WriteLine("{0} -> {1}", infitive, string.Join("/", conjugator.GetForm(infitive, VerbConjugator.VerbForm.ThirdPersonSingularPresent)));
            }

            Console.WriteLine("OK");
            Console.ReadLine();
        }


        private static void ParseAndPersistWordnetEntries(string filePath, bool excludeMwes, EnglishGraphContext db)
        {
            var parser = new WordNetParser();
            var parsedEntries = parser.ParseEntries(filePath, excludeMwes);

            var wordsAndPos = parsedEntries
                .Select(e => new Tuple<string, byte>(e.Word, e.PartOfSpeech))
                .ToList();
            var definitions = parsedEntries
                .SelectMany(e => e.Synsets)
                .Select(syn => syn.Definition)
                .ToList();

            var batchSize = 1000;
            var entries = wordsAndPos
                .Select((wp, i) => new {Index = i, Wp = wp})
                .GroupBy(a => a.Index / batchSize)
                .SelectMany(grp => DbUtilities.GetOrCreate(grp.Select(a => a.Wp).ToList(), db, true))
                .ToList();
            var synsets = definitions
                .Select((d, i) => new { Index = i, Def = d })
                .GroupBy(a => a.Index / batchSize)
                .SelectMany(grp => DbUtilities.GetOrCreate(grp.Select(a => a.Def).ToList(), db))
                .ToList();

            foreach (var entry in entries)
            {
                var associatedSynsets = parsedEntries
                    .Where(pe => pe.Word == entry.Word && pe.PartOfSpeech == entry.PartOfSpeech)
                    .SelectMany(pe => pe.Synsets)
                    .SelectMany(syn => synsets.Where(s => s.Definition == syn.Definition))
                    .ToList();
                if (!associatedSynsets.Any())
                {
                    Console.WriteLine("No definition associated to '{0}'", entry.Word);
                }
                entry.Synsets = associatedSynsets;
            }
            db.SaveChanges();
        }
    }
}
