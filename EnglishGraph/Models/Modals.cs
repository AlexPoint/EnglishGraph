using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class Modals
    {
        private static Modals instance;

        public static Modals Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Modals();
                }
                return instance;
            }
        }

        public List<string> AllModals { get; private set; }

        private Modals()
        {
            this.AllModals = ReadAllLines(Properties.Resources.modals);
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
