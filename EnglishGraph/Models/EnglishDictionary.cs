using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class EnglishDictionary
    {

        private readonly Dictionary<DictionaryEntry, DictionaryEntry> entries;
        //private readonly HashSet<string> lowerCasedWords;
        private readonly Dictionary<string, List<DictionaryEntry>> words;

        
        // Constructors ---------------------------

        public EnglishDictionary(IEnumerable<DictionaryEntry> initialEntries)
        {
            // create empty dictionaries with appropriate comparers
            entries = new Dictionary<DictionaryEntry, DictionaryEntry>(new WordAndPosComparer());
            words = new Dictionary<string, List<DictionaryEntry>>();

            // fill the dictionaries
            foreach (var initialEntry in initialEntries)
            {
                Add(initialEntry);
            }

            //lowerCasedWords = new HashSet<string>(initialEntries.Select(ent => ent.Word.ToLower()));
        }


        // Methods --------------------------------

        /// <summary>
        /// Retrieves an entry (if it exists) in the dictionary from a word and a POS
        /// </summary>
        public bool TryGetEntry(string word, byte pos, out DictionaryEntry entry)
        {
            var searchedEntry = new DictionaryEntry() {Word = word, PartOfSpeech = pos};
            return entries.TryGetValue(searchedEntry, out entry);
        }

        /// <summary>
        /// Retrieves the entries (several entries if several POS) in the dictionary from a word
        /// </summary>
        public bool TryGetEntries(string word, out List<DictionaryEntry> entries)
        {
            return words.TryGetValue(word, out entries);
        }

        /// <summary>
        /// Whether the dictionary contains the word/POS entry
        /// </summary>
        public bool Contains(string word, byte pos)
        {
            return entries.ContainsKey(new DictionaryEntry()
            {
                Word = word,
                PartOfSpeech = pos
            });
        }

        /// <summary>
        /// Whether the dictionary contains at least one entry for a given word
        /// </summary>
        public bool Contains(string word)
        {
            return words.ContainsKey(word);
        }

        /*public bool ContainsIgnoreCase(string word)
        {
            return lowerCasedWords.Contains(word.ToLower());
        }*/

        /// <summary>
        /// Adds an entry to the dictionary.
        /// Handles the update of all the underlying dictionaries.
        /// </summary>
        public void Add(DictionaryEntry entry)
        {
            entries.Add(entry, entry);

            if (words.ContainsKey(entry.Word))
            {
                words[entry.Word].Add(entry);
            }
            else
            {
                words.Add(entry.Word, new List<DictionaryEntry>(){ entry });
            }
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
