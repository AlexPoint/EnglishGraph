using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class GrammarTransformation
    {
        public Predicate<string> Condition { get; set; }
        public Func<string, string> Transform { get; set; }
    }
}
