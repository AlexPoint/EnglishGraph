using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class IrregularSuperlatives
    {
        private static IrregularSuperlatives instance;

        public static IrregularSuperlatives Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IrregularSuperlatives();
                }
                return instance;
            }
        }

        public List<Tuple<string, string>> AllIrregularSuperlatives;

        private IrregularSuperlatives()
        {
            this.AllIrregularSuperlatives = ReadAllLines(Properties.Resources.irregular_superlatives);
        }

        private List<Tuple<string,string>> ReadAllLines(string input)
        {
            var contractions = new List<Tuple<string,string>>();

            using (var sr = new StringReader(input))
            {
                var line = sr.ReadLine();
                while (line != null)
                {
                    var parts = line.Split('\t');
                    if (parts.Length == 2)
                    {
                        contractions.Add(new Tuple<string, string>(parts.First(), parts.Last()));
                    }

                    line = sr.ReadLine();
                }
            }

            return contractions;
        }
    }
}
