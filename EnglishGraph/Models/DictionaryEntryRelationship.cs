using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class DictionaryEntryRelationship
    {
        public int Id { get; set; }
        public DictionaryEntry Source { get; set; }
        public DictionaryEntry Target { get; set; }
        public byte Type { get; set; }
    }

    public class DictionaryEntryRelationshipTypes
    {
        public const byte ThirdPersonPresent = 11;
        public const byte SimplePast = 12;
        public const byte PastParticiple = 13;
        public const byte Gerundive = 14;
    }
}
