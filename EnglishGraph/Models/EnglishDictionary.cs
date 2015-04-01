using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class EnglishDictionary
    {

        private Dictionary<DictionaryEntry, DictionaryEntry> entries;

        public EnglishDictionary(IEnumerable<DictionaryEntry> initialEntries)
        {
            entries = new Dictionary<DictionaryEntry, DictionaryEntry>(new WordAndPosComparer());
            foreach (var initialEntry in initialEntries)
            {
                Add(initialEntry);
            }
        }

        public bool TryGetEntry(string word, byte pos, out DictionaryEntry entry)
        {
            return entries.TryGetValue(new DictionaryEntry(){Word = word, PartOfSpeech = pos}, out entry);
        }

        public bool Contains(string word, byte pos)
        {
            return entries.ContainsKey(new DictionaryEntry()
            {
                Word = word,
                PartOfSpeech = pos
            });
        }

        public bool Contains(string word, StringComparison stringComparison = StringComparison.InvariantCulture)
        {
            return entries.Any(ent => string.Equals(ent.Key.Word, word, stringComparison));
        }

        public void Add(DictionaryEntry entry)
        {
            entries.Add(entry, entry);
        }
    }

    class WordAndPosComparer: IEqualityComparer<DictionaryEntry>
    {
        public bool Equals(DictionaryEntry x, DictionaryEntry y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else
            {
                return x.Word == y.Word && x.PartOfSpeech == y.PartOfSpeech;
            }
        }

        public int GetHashCode(DictionaryEntry obj)
        {
            int hash = 13;
            hash = (hash * 7) + obj.Word.GetHashCode();
            hash = (hash * 7) + obj.PartOfSpeech.GetHashCode();
            return hash;
        }
    }
}
