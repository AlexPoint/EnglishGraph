using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    public class NegativeContractions
    {
        private static readonly TupleValuesResourceLoader Loader =
            new TupleValuesResourceLoader(Properties.Resources.negative_contractions);

        public static List<Tuple<string, string>> AllContractions = Loader.Elements;

    }
}
