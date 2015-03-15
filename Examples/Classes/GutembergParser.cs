using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Classes
{
    public class GutembergParser
    {
        public List<WordAndPronunciation> ParseWordsAndPronunciations(string filePath, bool excludeMwes)
        {
            var lines = File.ReadAllLines(filePath);
            var wps = lines
                .Select(l => l.Split(' '))
                .Where(p => !excludeMwes || p[0].Contains('_'))
                .Select(p => new WordAndPronunciation()
                {
                    Word = p[0],
                    Pronunication = p[1]
                })
                .ToList();
            return wps;
        }
    }

    public class WordAndPronunciation
    {
        public string Word { get; set; }
        public string Pronunication { get; set;1 }
    }
}
