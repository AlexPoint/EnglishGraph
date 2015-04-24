using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    public static class Conjunctions
    {
        // Subordinating -------------

        private static readonly SingleValuesResourceLoader SubordinatingLoader = 
            new SingleValuesResourceLoader(Properties.Resources.conjunctions_subordinating);

        public static List<string> Subordinating = SubordinatingLoader.Elements;

        // Coordinating --------------

        private static readonly SingleValuesResourceLoader CoordinatingLoader = 
            new SingleValuesResourceLoader(Properties.Resources.conjunctions_coordinating);

        public static List<string> Coordinating = CoordinatingLoader.Elements;

    }
}
