using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    public class IrregularPlurals
    {
        private static readonly TupleValuesResourceLoader Loader =
            new TupleValuesResourceLoader(Properties.Resources.irregular_noun_plurals);

        public static List<Tuple<string, string>> AllIrregularPlurals = Loader.Elements;

    }
}
