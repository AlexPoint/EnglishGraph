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
            Pronunciations.IsStressOnLastVowel("'w/[@]/rs/@/n");

            var db = new EnglishGraphContext();

            var allVerbs = db.DictionaryEntries
                .Where(de => de.PartOfSpeech == PartsOfSpeech.Verb)
                .ToList();
            var verbsWithStressOnLastVowel = allVerbs
                .Select(v => new Tuple<string, bool>(v.Word, Pronunciations.IsStressOnLastVowel(v.Pronunciation)))
                .ToList();

            Console.WriteLine("{0} verbs with stress on last vowel (total of {1} verbs):", 
                verbsWithStressOnLastVowel.Count(tup => tup.Item2), verbsWithStressOnLastVowel.Count);
            foreach (var tuple in verbsWithStressOnLastVowel.Where(tup => tup.Item2))
            {
                Console.WriteLine(tuple.Item1);
            }

            Console.WriteLine("----------------");
            foreach (var tuple in verbsWithStressOnLastVowel.Where(tup => !tup.Item2))
            {
                Console.WriteLine(tuple.Item1);
            }

            // load wordnet entries
            //Routines.LoadWordnetEntries(db, PathToProject);

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
