using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class SynsetDictionaryEntry
    {
        public int Id { get; set; }
        public virtual Synset Synset { get; set; }
        public virtual DictionaryEntry DictionaryEntry { get; set; }
    }
}
