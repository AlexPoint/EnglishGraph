using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{
    public class Modals
    {
        private static readonly SingleValuesResourceLoader Loader =
            new SingleValuesResourceLoader(Properties.Resources.modals);

        public static List<string> AllModals = Loader.Elements;
    }
}
