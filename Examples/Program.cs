using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models;
using Examples.Classes;

namespace Examples
{
    class Program
    {
        private static readonly string PathToProject = Directory.GetCurrentDirectory() + "/../../";

        static void Main(string[] args)
        {
            var parser = new WordNetParser();

            var pathToWordnetFile = PathToProject + "Input/wordnet_words_with_def.txt";
            var entries = parser.ParseEntries(pathToWordnetFile, true);

            Console.WriteLine("{0} entries parsed:", entries.Count);
            foreach (var grp in entries.GroupBy(e => e.PartOfSpeech))
            {
                Console.WriteLine("{0} --> {1}", grp.Key, grp.Count());
            }


            Console.WriteLine("OK");
            Console.ReadLine();
        }

    }
}
