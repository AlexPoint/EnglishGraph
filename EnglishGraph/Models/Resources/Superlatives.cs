using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    public class Superlatives
    {
        private readonly static TupleValuesResourceLoader ExceptionLoader =
            new TupleValuesResourceLoader(Properties.Resources.superlatives_exceptions);

        public static List<Tuple<string, string>> Exceptions = ExceptionLoader.Elements;

    }
}
