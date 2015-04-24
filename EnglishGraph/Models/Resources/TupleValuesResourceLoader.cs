using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    /// <summary>
    /// Class to load a resource file for which each line correspond to a pair Item1, Item2
    /// </summary>
    public class TupleValuesResourceLoader
    {
        /// <summary>
        /// The elements in the resource file
        /// </summary>
        public List<Tuple<string, string>> Elements { get; private set; }

        private readonly char _separator;

        public TupleValuesResourceLoader(string resourceString, char separator = '\t')
        {
            this.Elements = ReadAllLines(resourceString);
            this._separator = separator;
        }

        private List<Tuple<string, string>> ReadAllLines(string input)
        {
            var contractions = new List<Tuple<string, string>>();

            using (var sr = new StringReader(input))
            {
                var line = sr.ReadLine();
                while (line != null)
                {
                    var parts = line.Split(_separator);
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
