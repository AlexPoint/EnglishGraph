using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    /// <summary>
    /// Class to load a resource file for which each line correspond to an element
    /// </summary>
    public class SingleValuesResourceLoader
    {
        /// <summary>
        /// The elements in the resource file
        /// </summary>
        public List<string> Elements { get; private set; }


        public SingleValuesResourceLoader(string resourceString)
        {
            this.Elements = ReadAllLines(resourceString);
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
