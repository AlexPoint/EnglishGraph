using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.Resources
{

    public class Pronouns
    {
        // Subject personal

        private static readonly SingleValuesResourceLoader SubjectPersonalLoader =
            new SingleValuesResourceLoader(Properties.Resources.pronouns_subject_personal);

        public static List<string> SubjectPersonal = SubjectPersonalLoader.Elements;
        
        // Object personal

        private static readonly SingleValuesResourceLoader ObjectPersonalLoader =
            new SingleValuesResourceLoader(Properties.Resources.pronouns_object_personal);

        public static List<string> ObjectPersonal = ObjectPersonalLoader.Elements;

        // Reflexive personal ---
            
        private static readonly SingleValuesResourceLoader ReflexivePersonalLoader =
            new SingleValuesResourceLoader(Properties.Resources.pronouns_reflexive_personal);

        public static List<string> ReflexivePersonal = ReflexivePersonalLoader.Elements;
        
        // Possessive ----------
            
        private static readonly SingleValuesResourceLoader PossessiveLoader =
            new SingleValuesResourceLoader(Properties.Resources.pronouns_possessive);

        public static List<string> Possessive = PossessiveLoader.Elements;

        // Indefinite -----------
            
        private static readonly SingleValuesResourceLoader IndefiniteLoader =
            new SingleValuesResourceLoader(Properties.Resources.pronouns_indefinite);

        public static List<string> Indefinite = IndefiniteLoader.Elements;

        // Interrogative --------
            
        private static readonly SingleValuesResourceLoader InterrogativeLoader =
            new SingleValuesResourceLoader(Properties.Resources.pronouns_interrogative);

        public static List<string> Interrogative = InterrogativeLoader.Elements;

        // Relative -------------
            
        private static readonly SingleValuesResourceLoader RelativeLoader =
            new SingleValuesResourceLoader(Properties.Resources.pronouns_relative);

        public static List<string> Relative = RelativeLoader.Elements;

    }
}
