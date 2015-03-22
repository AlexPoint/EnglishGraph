using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models;

namespace Examples.Classes
{
    public class Routines
    {

        public static void LoadPronouns(EnglishGraphContext db, bool includeMwes)
        {
            var pronouns = Pronouns.Instance;

            var subjPersPronouns = pronouns.AllSubjectPersonalPronouns
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.SubjectPersonalPronoun))
                .ToList();
            var objPersPronouns = pronouns.AllObjectPersonalPronouns
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.ObjectPersonalPronoun))
                .ToList();
            var reflPersPronouns = pronouns.AllReflexivePersonalPronouns
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.ReflexivePersonalPronoun))
                .ToList();
            var possPronouns = pronouns.AllPossessivePronouns
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.PossessivePronoun))
                .ToList();
            var indefPronouns = pronouns.AllIndefinitePronouns
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.IndefinitePronoun))
                .ToList();
            var interPronouns = pronouns.AllInterrogativePronouns
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.InterrogativePronoun))
                .ToList();
            var relPronouns = pronouns.AllRelativePronouns
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.RelativePronoun))
                .ToList();
            DbUtilities.GetOrCreate(subjPersPronouns, db);
            DbUtilities.GetOrCreate(objPersPronouns, db);
            DbUtilities.GetOrCreate(reflPersPronouns, db);
            DbUtilities.GetOrCreate(possPronouns, db);
            DbUtilities.GetOrCreate(indefPronouns, db);
            DbUtilities.GetOrCreate(interPronouns, db);
            DbUtilities.GetOrCreate(relPronouns, db);
        }

        public static void LoadPrepositions(EnglishGraphContext db, bool includeMwes)
        {
            var prepositions = Prepositions.Instance;

            var allPrepositions = prepositions.AllPrepositions
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.Preposition))
                .ToList();
            DbUtilities.GetOrCreate(allPrepositions, db);
        }

        public static void LoadDeterminers(EnglishGraphContext db, bool includeMwes)
        {
            var determiners = Determiners.Instance;

            var generalDeterminers = determiners.AllGeneralDeterminers
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.Determiner))
                .ToList();
            var articleDeterminers = determiners.AllArticleDeterminers
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.ArticleDeterminer))
                .ToList();
            var demonstrativeDeterminers = determiners.AllDemonstrativeDeterminers
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.DemonstrativeDeterminer))
                .ToList();
            var possessiveDeterminers = determiners.AllPossessiveDeterminers
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.PossessiveDeterminer))
                .ToList();
            DbUtilities.GetOrCreate(generalDeterminers, db);
            DbUtilities.GetOrCreate(articleDeterminers, db);
            DbUtilities.GetOrCreate(demonstrativeDeterminers, db);
            DbUtilities.GetOrCreate(possessiveDeterminers, db);
        }

        public static void LoadConjunctions(EnglishGraphContext db, bool includeMwes)
        {
            var conjunctions = Conjunctions.Instance;
            
            var coordinatingConjunctions = conjunctions.AllCoordinatingConjunctions
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.CoordinatingConjunction))
                .ToList();
            var subordinatingConjunctions = conjunctions.AllSubordinatingConjunctions
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.SubordinatingConjunction))
                .ToList();
            DbUtilities.GetOrCreate(coordinatingConjunctions, db);
            DbUtilities.GetOrCreate(subordinatingConjunctions, db);
        }

        public static void LoadGutembergPronunciations(EnglishGraphContext db, string pathToProject)
        {
            var parser = new GutembergParser();
            var filePath = pathToProject + "Input/gutemberg_pronunciation.txt";
            var wordsAndPronunciations = parser.ParseWordsAndPronunciations(filePath, true);

            const int batchSize = 1000;
            var nbOfEntriesFound = 0;
            var groups = wordsAndPronunciations
                .Select((wp, i) => new { WordAndPronunciation = wp, Index = i })
                .GroupBy(a => a.Index / batchSize);
            foreach (var grp in groups)
            {
                var words = grp.Select(a => a.WordAndPronunciation.Word).ToList();
                var entries = DbUtilities.GetEntries(words, db);
                nbOfEntriesFound += entries.Count;
                foreach (var entry in entries)
                {
                    var pronunciation = grp.Where(a => string.Equals(a.WordAndPronunciation.Word, entry.Word, StringComparison.InvariantCultureIgnoreCase))
                        .Select(a => a.WordAndPronunciation.Pronunication)
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(pronunciation))
                    {
                        entry.Pronunciation = pronunciation;
                    }
                    else
                    {
                        Console.WriteLine("No pronunciation found for entry '{0}'", entry.Word);
                    }
                }
                db.SaveChanges();
            }

            Console.WriteLine("Found {0} entries / {1} words parsed on Gutemberg project", nbOfEntriesFound, wordsAndPronunciations.Count);
        }
        
        public static void LoadWordnetEntries(EnglishGraphContext db, string pathToProject)
        {
            /* Lower cased words:
             * 
             * select w.lemma, syn.pos, syn.definition, c.name
             * from word w
             * join sense s on s.wordid = w.wordid
             * join synset syn on syn.synsetid = s.synsetid
             * join categorydef c on syn.categoryid = c.categoryid
             * where s.casedwordid is null
             */
            var lcFilePath = pathToProject + "Input/wordnet_lc_words_with_def.txt";
            ParseAndPersistWordnetEntries(lcFilePath, true, db);

            /* Upper cased words:
             * 
             * select w.lemma, syn.pos, syn.definition, c.name
             * from word w
             * join sense s on s.wordid = w.wordid
             * join synset syn on syn.synsetid = s.synsetid
             * join categorydef c on syn.categoryid = c.categoryid
             * where s.casedwordid is null
             */
            var ucFilePath = pathToProject + "Input/wordnet_uc_words_with_def.txt";
            ParseAndPersistWordnetEntries(ucFilePath, true, db);
        }

        private static void ParseAndPersistWordnetEntries(string filePath, bool excludeMwes, EnglishGraphContext db)
        {
            var parser = new WordNetParser();
            var parsedEntries = parser.ParseEntries(filePath, excludeMwes);

            var wordsAndPos = parsedEntries
                .Select(e => new Tuple<string, byte>(e.Word, e.PartOfSpeech))
                .ToList();
            var definitions = parsedEntries
                .SelectMany(e => e.Synsets.Select(se => se.Synset))
                .Select(syn => syn.Definition)
                .ToList();

            const int batchSize = 1000;
            var entries = wordsAndPos
                .Select((wp, i) => new { Index = i, Wp = wp })
                .GroupBy(a => a.Index / batchSize)
                .SelectMany(grp => DbUtilities.GetOrCreate(grp.Select(a => a.Wp).ToList(), db, true))
                .ToList();
            var synsets = definitions
                .Select((d, i) => new { Index = i, Def = d })
                .GroupBy(a => a.Index / batchSize)
                .SelectMany(grp => DbUtilities.GetOrCreate(grp.Select(a => a.Def).ToList(), db))
                .ToList();

            var newEntryAndSynsetIds = new List<Tuple<int, int>>();
            foreach (var entry in entries)
            {
                var associatedSynsets = parsedEntries
                    .Where(pe => pe.Word == entry.Word && pe.PartOfSpeech == entry.PartOfSpeech)
                    .SelectMany(pe => pe.Synsets)
                    .SelectMany(syn => synsets.Where(s => s.Definition == syn.Synset.Definition))
                    .ToList();
                if (!associatedSynsets.Any())
                {
                    Console.WriteLine("No definition associated to '{0}'", entry.Word);
                }
                else
                {
                    // create the links between synsets and dictionary entries
                    var entryAndSynsetIds = associatedSynsets
                        .Select(syn => new Tuple<int, int>(entry.Id, syn.Id))
                        .ToList();
                    newEntryAndSynsetIds.AddRange(entryAndSynsetIds);
                }

                if (newEntryAndSynsetIds.Count >= batchSize)
                {
                    DbUtilities.GetOrCreate(newEntryAndSynsetIds, db);
                    newEntryAndSynsetIds = new List<Tuple<int, int>>();
                }
            }

            DbUtilities.GetOrCreate(newEntryAndSynsetIds, db);
        }
    }
}
