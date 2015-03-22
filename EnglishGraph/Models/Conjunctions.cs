using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class Conjunctions
    {
        private static Conjunctions instance;

        public static Conjunctions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Conjunctions();
                }
                return instance;
            }
        }

        public List<string> AllSubordinatingConjunctions { get; private set; }
        public List<string> AllCoordinatingConjunctions { get; private set; }


        private Conjunctions()
        {
            this.AllCoordinatingConjunctions = ReadAllLines(Properties.Resources.coordinating_conjunctions);
            this.AllSubordinatingConjunctions = ReadAllLines(Properties.Resources.subordinating_conjunctions);
        }

        private List<string> ReadAllLines(string input)
        {
            var lines = new List<string>();

            using (var sr = new StringReader(input))
            {
                var line = sr.ReadLine();
                while (line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line.Trim());
                    }

                    line = sr.ReadLine();
                }
            }

            return lines;
        }
    }
}
