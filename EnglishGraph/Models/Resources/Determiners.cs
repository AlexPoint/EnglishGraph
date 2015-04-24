using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    public class Determiners
    {
        // General ----------------

        private static readonly SingleValuesResourceLoader GeneralLoader =
            new SingleValuesResourceLoader(Properties.Resources.determiners_general);

        public static List<string> General = GeneralLoader.Elements;


        // Articles ---------------

        private static readonly SingleValuesResourceLoader ArticlesLoader =
            new SingleValuesResourceLoader(Properties.Resources.determiners_articles);

        public static List<string> Articles = ArticlesLoader.Elements;


        // Demonstrative ----------

        private static readonly SingleValuesResourceLoader DemonstrativeLoader =
            new SingleValuesResourceLoader(Properties.Resources.determiners_demonstratives);

        public static List<string> Demonstrative = DemonstrativeLoader.Elements;


        // Possessive -------------

        private static readonly SingleValuesResourceLoader PossessiveLoader =
            new SingleValuesResourceLoader(Properties.Resources.determiners_possessives);

        public static List<string> Possessive = PossessiveLoader.Elements;

    }
}
