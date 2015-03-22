using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class Pronouns
    {
        private static Pronouns instance;

        public static Pronouns Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Pronouns();
                }
                return instance;
            }
        }

        public List<string> AllSubjectPersonalPronouns { get; private set; }
        public List<string> AllObjectPersonalPronouns { get; private set; }
        public List<string> AllReflexivePersonalPronouns { get; private set; }
        public List<string> AllPossessivePronouns { get; private set; }
        public List<string> AllIndefinitePronouns { get; private set; }
        public List<string> AllInterrogativePronouns { get; private set; }
        public List<string> AllRelativePronouns { get; private set; }


        private Pronouns()
        {
            this.AllSubjectPersonalPronouns = ReadAllLines(Properties.Resources.subject_personal_pronouns);
            this.AllObjectPersonalPronouns = ReadAllLines(Properties.Resources.object_personal_pronouns);
            this.AllReflexivePersonalPronouns = ReadAllLines(Properties.Resources.reflexive_personal_pronouns);
            this.AllPossessivePronouns = ReadAllLines(Properties.Resources.possessive_pronouns);
            this.AllIndefinitePronouns = ReadAllLines(Properties.Resources.indefinite_pronouns);
            this.AllInterrogativePronouns = ReadAllLines(Properties.Resources.interrogative_pronouns);
            this.AllRelativePronouns = ReadAllLines(Properties.Resources.relative_pronouns);
        }

        private List<string> ReadAllLines(string input)
        {
            var lines = new List<string>();

            using (var sr = new StringReader(input))
            {
                var line = sr.ReadLine();
                while (line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line.Trim());
                    }

                    line = sr.ReadLine();
                }
            }

            return lines;
        }
    }
}
