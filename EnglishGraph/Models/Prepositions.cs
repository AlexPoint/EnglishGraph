using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class Prepositions
    {
        private static Prepositions instance;

        public static Prepositions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Prepositions();
                }
                return instance;
            }
        }

        public List<string> AllPrepositions { get; private set; }

        private Prepositions()
        {
            this.AllPrepositions = ReadAllLines(Properties.Resources.prepositions);
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
