using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class DictionaryEntry
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public byte PartOfSpeech { get; set; }
        public List<SynsetDictionaryEntry> Synsets { get; set; }
        public string Pronunciation { get; set; }


        public List<DictionaryEntryRelationship> DerivedRelationships { get; set; }
        public List<DictionaryEntryRelationship> StemmedFromRelationships { get; set; }


        public DictionaryEntry()
        {
            this.Synsets = new List<SynsetDictionaryEntry>();
            this.DerivedRelationships = new List<DictionaryEntryRelationship>();
            this.StemmedFromRelationships = new List<DictionaryEntryRelationship>();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})",  this.Word, PartsOfSpeech.Abbrev(this.PartOfSpeech));
        }
    }
}
