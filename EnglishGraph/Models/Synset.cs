using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class Synset
    {
        public int Id { get; set; }
        public string Definition { get; set; }

        public List<DictionaryEntry> DictionaryEntries { get; set; }
    }
}
