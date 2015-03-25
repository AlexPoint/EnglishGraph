using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Classes
{
    public class Stats
    {

        public static void PrintPartOfSpeechDensity(EnglishGraphContext db)
        {
            var groups = db.DictionaryEntries
                .ToList()
                .GroupBy(de => de.Word)
                .Select(grp => new {Word = grp.Key, PartsOfSpeech = string.Join("|", grp.Select(de => de.PartOfSpeech).OrderBy(pos => pos))})
                .GroupBy(a => a.PartsOfSpeech)
                .Select(grp => new {PartsOfSpeech = grp.Key, Count = grp.Count()})
                .ToList();

            foreach (var group in groups.GroupBy(g => g.PartsOfSpeech.Length))
            {
                Console.WriteLine("POS length: {0} -> {1} entries", group.Key, group.Sum(g => g.Count));
            }
        }
    }
}
