using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models;
using Examples.Classes;

namespace Examples
{
    class Program
    {
        private static readonly string PathToProject = Directory.GetCurrentDirectory() + "/../../";

        static void Main(string[] args)
        {
            var db = new EnglishGraphContext();

            /*/*var verbShortList = new List<string>()
            {
                "stop", "refer", "visit", "rob", "sit", "begin", "prefer", "listen", "happen", "travel", "cancel", "start", "burn", "remain", "play", "snow",
                "carry", "marry", "kiss", "fix", "watch", "crash", "go"
            };
            var allVerbs = db.DictionaryEntries
                .Where(de => de.PartOfSpeech == PartsOfSpeech.Verb && verbShortList.Contains(de.Word))
                .ToList();#1#

            var conjugator = new VerbConjugator();
            /*foreach (var verb in allVerbs)
            {
                Console.WriteLine("{0} --> {1}", verb.Word, string.Join("|", conjugator.GetVerbForm(verb, VerbConjugator.VerbForm.ThirdPersonSingularPresent)));
            }#1#
            var infinitive = "carry";
            var verb = db.DictionaryEntries.First(de => de.Word == infinitive && de.PartOfSpeech == PartsOfSpeech.Verb);
            var simplePastForms = conjugator.GetVerbForm(verb, VerbConjugator.VerbForm.PastParticiple);
            Console.WriteLine(string.Join("|", simplePastForms));*/


            // load wordnet entries
            //Routines.LoadWordnetEntries(db, PathToProject);

            // load conjunction

            // load pronunciations
            //Routines.LoadGutembergPronunciations(db, PathToProject);

            /*var infitives = db.DictionaryEntries
                .Where(de => de.PartOfSpeech == PartsOfSpeech.Verb)
                .Select(de => de.Word)
                .ToList();
            var specificInfinitives = infitives
                .Where(i => VerbConjugator.ConsonantVowelConsonantEnding.IsMatch(i))
                .ToList();
            Console.WriteLine("{0} specific cases (out of {1} infinitives):", specificInfinitives.Count, infitives.Count);
            foreach (var specificInfinitive in specificInfinitives)
            {
                Console.WriteLine(specificInfinitive);
            }*/
            /*var conjugator = new VerbConjugator();
            foreach (var infitive in infitives)
            {
                Console.WriteLine("{0} -> {1}", infitive, string.Join("/", conjugator.GetForm(infitive, VerbConjugator.VerbForm.ThirdPersonSingularPresent)));
            }*/

            Console.WriteLine("OK");
            Console.ReadLine();
        }


    }
}
