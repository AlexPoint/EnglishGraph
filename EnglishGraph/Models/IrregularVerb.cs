using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class IrregularVerb
    {
        public string Infinitive { get; set; }
        public List<string> SimplePastForms { get; set; }
        public List<string> PastParticipleForms { get; set; }

        
        public IrregularVerb(string infinitive, IEnumerable<string> simplePastForms, IEnumerable<string> pastParticipleForms)
        {
            this.Infinitive = infinitive;
            this.SimplePastForms = simplePastForms.ToList();
            this.PastParticipleForms = pastParticipleForms.ToList();
        }
    }
}
