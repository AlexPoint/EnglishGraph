using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    public class Prepositions
    {
        private static readonly SingleValuesResourceLoader Loader =
            new SingleValuesResourceLoader(Properties.Resources.prepositions);

        public static List<string> AllPrepositions = Loader.Elements;
    }
}
