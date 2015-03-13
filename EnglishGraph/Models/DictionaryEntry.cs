using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class DictionaryEntry
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public byte PartOfSpeech { get; set; }
        public List<Synset> Synsets { get; set; }

    }
}
