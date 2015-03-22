using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class Determiners
    {
        private static Determiners instance;

        public static Determiners Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Determiners();
                }
                return instance;
            }
        }

        public List<string> AllGeneralDeterminers { get; private set; }
        public List<string> AllArticleDeterminers { get; private set; }
        public List<string> AllDemonstrativeDeterminers { get; private set; }
        public List<string> AllPossessiveDeterminers { get; private set; }


        private Determiners()
        {
            this.AllGeneralDeterminers = ReadAllLines(Properties.Resources.determiners);
            this.AllArticleDeterminers = ReadAllLines(Properties.Resources.determiners_articles);
            this.AllDemonstrativeDeterminers = ReadAllLines(Properties.Resources.determiners_demonstratives);
            this.AllPossessiveDeterminers = ReadAllLines(Properties.Resources.determiners_possessives);
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
