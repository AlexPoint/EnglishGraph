using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models.Resources;

namespace EnglishGraph.Models.Resources
{
    public class IrregularVerbs
    {
        private static IrregularVerbs instance;

        public static IrregularVerbs Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IrregularVerbs();
                }
                return instance;
            }
        }

        public List<IrregularVerb> AllIrregularVerbs { get; private set; }

        private IrregularVerbs()
        {
            this.AllIrregularVerbs = new List<IrregularVerb>();

            using (var sr = new StringReader(Properties.Resources.irregular_verbs))
            {
                var line = sr.ReadLine();
                while (line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var parts = line.Split('\t');
                        this.AllIrregularVerbs
                            .Add(new IrregularVerb(parts[0], parts[1].Split('/'), parts[2].Split('/')));
                    }

                    line = sr.ReadLine();
                }
            }
        }
    }
}
