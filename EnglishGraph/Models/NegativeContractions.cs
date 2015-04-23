using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class NegativeContractions
    {
        private static NegativeContractions instance;

        public static NegativeContractions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NegativeContractions();
                }
                return instance;
            }
        }

        public List<Tuple<string, string>> AllNegativeContractions;

        private NegativeContractions()
        {
            this.AllNegativeContractions = ReadAllLines(Properties.Resources.negative_contractions);
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
