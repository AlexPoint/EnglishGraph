using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models;

namespace Examples.Classes
{
    static class DbUtilities
    {
        public static List<DictionaryEntry> GetOrCreate(List<Tuple<string, byte>> wordAndPos, EnglishGraphContext db,
            bool caseSensitive = true)
        {
            var distinctWordsAndPos = wordAndPos.Distinct().ToList();
            var distinctWordNames = distinctWordsAndPos.Select(d => d.Item1).ToList();
            var existingEntries = db.DictionaryEntries
                .Where(e => distinctWordNames.Contains(e.Word))
                .ToList()
                .Where(e => distinctWordsAndPos
                    .Any(wp => string.Equals(wp.Item1, e.Word, caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) 
                        && wp.Item2 == e.PartOfSpeech))
                .ToList();

            var missingWordAndPos = distinctWordsAndPos
                .Where(tup => !existingEntries.Any(e => tup.Item1 == e.Word && tup.Item2 == e.PartOfSpeech))
                .ToList();
            if (missingWordAndPos.Any())
            {
                var newEntries = missingWordAndPos.Select(tup => new DictionaryEntry()
                {
                    Word = tup.Item1,
                    PartOfSpeech = tup.Item2
                })
                .ToList();
                db.DictionaryEntries.AddRange(newEntries);
                db.SaveChanges();

                existingEntries.AddRange(newEntries);
            }

            return existingEntries;
        }

        public static List<Synset> GetOrCreate(List<string> defintions, EnglishGraphContext db)
        {
            var distinctDefinitions = defintions.Distinct().ToList();
            var existingSynsets = db.Synsets
                .Where(syn => distinctDefinitions.Contains(syn.Definition))
                .ToList();

            var missingDefinitions = distinctDefinitions
                .Where(def => existingSynsets.All(syn => syn.Definition != def))
                .ToList();
            if (missingDefinitions.Any())
            {
                var newSynsets = missingDefinitions
                    .Select(def => new Synset()
                    {
                        Definition = def
                    })
                    .ToList();
                db.Synsets.AddRange(newSynsets);
                db.SaveChanges();

                existingSynsets.AddRange(newSynsets);
            }

            return existingSynsets;
        }
    }
}
