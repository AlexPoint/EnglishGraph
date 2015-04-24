using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models.Resources;

namespace EnglishGraph.Models.Resources
{
    public class Contractions
    {
        private static readonly TupleValuesResourceLoader Loader = 
            new TupleValuesResourceLoader(Properties.Resources.contractions);

        public static List<Tuple<string, string>> AllContractions = Loader.Elements;
    }
}
