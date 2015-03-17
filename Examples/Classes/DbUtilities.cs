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
        
        // SynsetDictionaryEntry -----------------------------------
        public static List<Synset> GetSynsets(List<int> ids, EnglishGraphContext db)
        {
            var entries = db.Synsets
                .Where(syn => ids.Contains(syn.Id))
                .ToList();
            return entries;
        }

        private static readonly object SynsetDictionaryEntryCreationLock = new object();

        public static List<SynsetDictionaryEntry> GetOrCreate(List<Tuple<int, int>> entryAndSynsetIds, EnglishGraphContext db)
        {
            var distinctEntryAndSynsetIds = entryAndSynsetIds.Distinct().ToList();
            var entryIds = distinctEntryAndSynsetIds.Select(tup => tup.Item1).ToList();
            var synsetIds = distinctEntryAndSynsetIds.Select(tup => tup.Item2).ToList();
            var existingSynsetAndEntries = db.SynsetsAndDictionaryEntries
                // take more than necessary and filter later
                .Where(se => entryIds.Contains(se.DictionaryEntry.Id) && synsetIds.Contains(se.Synset.Id))
                .ToList()
                .Where(se => distinctEntryAndSynsetIds.Any(dsid => dsid.Item1 == se.DictionaryEntry.Id && dsid.Item2 == se.Synset.Id))
                .ToList();

            var missingSynsetAndEntryIds = distinctEntryAndSynsetIds
                .Where(tup => !existingSynsetAndEntries.Any(e => tup.Item1 == e.DictionaryEntry.Id && tup.Item2 == e.Synset.Id))
                .ToList();
            if (missingSynsetAndEntryIds.Any())
            {
                lock (SynsetDictionaryEntryCreationLock)
                {
                    existingSynsetAndEntries = db.SynsetsAndDictionaryEntries
                        // take more than necessary and filter later
                        .Where(se => entryIds.Contains(se.DictionaryEntry.Id) && synsetIds.Contains(se.Synset.Id))
                        .ToList()
                        .Where(
                            se =>
                                distinctEntryAndSynsetIds.Any(
                                    dsid => dsid.Item1 == se.DictionaryEntry.Id && dsid.Item2 == se.Synset.Id))
                        .ToList();
                    missingSynsetAndEntryIds = distinctEntryAndSynsetIds
                        .Where(tup => !existingSynsetAndEntries.Any(e => tup.Item1 == e.DictionaryEntry.Id && tup.Item2 == e.Synset.Id))
                        .ToList();

                    var missingEntryIds = missingSynsetAndEntryIds.Select(tup => tup.Item1).ToList();
                    var missingSynsets = missingSynsetAndEntryIds.Select(tup => tup.Item2).ToList();
                    var entries = GetEntries(missingEntryIds, db);
                    var synsets = GetSynsets(missingSynsets, db);
                    var newSynsetAndEntries = missingSynsetAndEntryIds
                        .Select(tup => new SynsetDictionaryEntry()
                        {
                            DictionaryEntry = entries.FirstOrDefault(de => de.Id == tup.Item1),
                            Synset = synsets.FirstOrDefault(syn => syn.Id == tup.Item2)
                        })
                        .ToList();
                    db.SynsetsAndDictionaryEntries.AddRange(newSynsetAndEntries);
                    db.SaveChanges();

                    existingSynsetAndEntries.AddRange(newSynsetAndEntries);
                }
            }

            return existingSynsetAndEntries;
        }


        // DictionaryEntries ------------------------------------------------
        
        public static List<DictionaryEntry> GetEntries(List<string> words, EnglishGraphContext db)
        {
            var entries = db.DictionaryEntries
                .Where(de => words.Contains(de.Word))
                .ToList();
            return entries;
        }

        private static readonly object WordCreationLock = new object();
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
                lock (WordCreationLock)
                {
                    existingEntries = db.DictionaryEntries
                        .Where(e => distinctWordNames.Contains(e.Word))
                        .ToList()
                        .Where(e => distinctWordsAndPos
                            .Any(wp => string.Equals(wp.Item1, e.Word,
                                        caseSensitive ? StringComparison.InvariantCulture: StringComparison.InvariantCultureIgnoreCase)
                                    && wp.Item2 == e.PartOfSpeech))
                        .ToList();
                    missingWordAndPos = distinctWordsAndPos
                        .Where(tup => !existingEntries.Any(e => tup.Item1 == e.Word && tup.Item2 == e.PartOfSpeech))
                        .ToList();

                    var newEntries = missingWordAndPos
                        .Select(tup => new DictionaryEntry()
                        {
                            Word = tup.Item1,
                            PartOfSpeech = tup.Item2
                        })
                        .ToList();
                    db.DictionaryEntries.AddRange(newEntries);
                    db.SaveChanges();

                    foreach (var newEntry in newEntries)
                    {
                        Console.WriteLine("Created entry {0}", newEntry);
                    }

                    existingEntries.AddRange(newEntries); 
                }
            }

            return existingEntries;
        }


        // Synsets --------------------------------------------------------
        
        public static List<DictionaryEntry> GetEntries(List<int> ids, EnglishGraphContext db)
        {
            var entries = db.DictionaryEntries
                .Where(de => ids.Contains(de.Id))
                .ToList();
            return entries;
        }

        private static readonly object SynsetCreationLock = new object();
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
                lock (SynsetCreationLock)
                {
                    existingSynsets = db.Synsets
                        .Where(syn => distinctDefinitions.Contains(syn.Definition))
                        .ToList();
                    missingDefinitions = distinctDefinitions
                        .Where(def => existingSynsets.All(syn => syn.Definition != def))
                        .ToList();

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
            }

            return existingSynsets;
        }
    }
}
