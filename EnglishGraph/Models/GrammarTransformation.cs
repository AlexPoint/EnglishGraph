using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class GrammarTransformation<T, U>
    {
        public Predicate<T> Condition { get; set; }
        public Func<T, U> Transform { get; set; }
    }
}
