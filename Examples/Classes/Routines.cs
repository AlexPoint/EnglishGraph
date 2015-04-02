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
        public static void LoadContractions(EnglishGraphContext db)
        {
            var pronouns = Contractions.Instance;

            var contractions = pronouns.AllContractions
                .Select(tup => new Tuple<string,byte>(tup.Item1, PartsOfSpeech.Contractions))
                .ToList();
            var nonContractedForms = pronouns.AllContractions
                .Select(tup => tup.Item2)
                .ToList();

            var contractionEntries = DbUtilities.GetOrCreate(contractions, db);
            var nonContractedFormEntries = DbUtilities.GetEntries(nonContractedForms, db)
                .Where(ent => PartsOfSpeech.IsVerb(ent.PartOfSpeech));

            var relationships = pronouns.AllContractions
                .Select(tup => new DictionaryEntryRelationship()
                {
                    Source = nonContractedFormEntries.First(ent => ent.Word == tup.Item2),
                    Target = contractionEntries.First(ent => ent.Word == tup.Item1),
                    Type = DictionaryEntryRelationshipTypes.ContractedFormOf
                })
                .ToList();
            DbUtilities.GetOrCreate(relationships, db);
        }

        public static void LoadModals(EnglishGraphContext db)
        {
            var modals = Modals.Instance
                .AllModals
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.Modal))
                .ToList();

            // Delete modals already added as verb
            var existingModalsAsVerb = DbUtilities.GetEntries(modals.Select(m => m.Item1).ToList(), db)
                .Where(ent => PartsOfSpeech.IsVerb(ent.PartOfSpeech) && ent.PartOfSpeech != PartsOfSpeech.Modal)
                .ToList();
            db.DictionaryEntries.RemoveRange(existingModalsAsVerb);
            db.SaveChanges();

            DbUtilities.GetOrCreate(modals, db);
        }
        public static void LoadVerb1stAnd2ndForms(EnglishGraphContext db)
        {
            var firstForms = new List<string>() {"am"}
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.Verb1stPersSingular))
                .ToList();
            DbUtilities.GetOrCreate(firstForms, db);

            var secondForms = new List<string>() {"are"}
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.Verb2ndPersSingular))
                .ToList();
            DbUtilities.GetOrCreate(secondForms, db);
        }


        public static void LoadSimplePastForms(EnglishGraphContext db)
        {
            LoadVerbforms(db, PartsOfSpeech.VerbSimplePast, VerbConjugator.VerbForm.SimplePast, 
                DictionaryEntryRelationshipTypes.SimplePast);
        }
        public static void LoadPastParticipleForms(EnglishGraphContext db)
        {
            LoadVerbforms(db, PartsOfSpeech.VerbPastParticiple, VerbConjugator.VerbForm.PastParticiple, 
                DictionaryEntryRelationshipTypes.PastParticiple);
        }
        public static void LoadGerundiveForms(EnglishGraphContext db)
        {
            LoadVerbforms(db, PartsOfSpeech.Gerundive, VerbConjugator.VerbForm.Gerundive, 
                DictionaryEntryRelationshipTypes.Gerundive);
        }
        public static void Load3rdPresentForms(EnglishGraphContext db)
        {
            LoadVerbforms(db, PartsOfSpeech.Verb3RdPersSingular, VerbConjugator.VerbForm.ThirdPersonSingularPresent, 
                DictionaryEntryRelationshipTypes.ThirdPersonPresent);
        }

        private static void LoadVerbforms(EnglishGraphContext db, byte pos, VerbConjugator.VerbForm verbForm, byte relationshipType)
        {
            const int batchSize = 1000;
            var infinitives = db.DictionaryEntries
                .Where(de => de.PartOfSpeech == PartsOfSpeech.Verb)
                .ToList();
            Console.WriteLine("Found {0} infinitives in db", infinitives.Count);

            var conjugator = new VerbConjugator();

            // Add verb forms in dictionary entries table
            var verbForms = infinitives
                .SelectMany(inf => conjugator.GetVerbForm(inf, verbForm))
                .Select(s => new Tuple<string, byte>(s, pos))
                .ToList();
            var thirdPresentEntries = verbForms
                .Select((wp, i) => new { Index = i, Wp = wp })
                .GroupBy(a => a.Index / batchSize)
                .SelectMany(grp => DbUtilities.GetOrCreate(grp.Select(a => a.Wp).ToList(), db))
                .ToList();

            // Then add relationships between entries
            var relationships = new List<DictionaryEntryRelationship>();
            foreach (var infinitive in infinitives)
            {
                var forms = conjugator.GetVerbForm(infinitive, verbForm);
                var relatedEntries = forms
                    .Select(f => thirdPresentEntries.FirstOrDefault(ent => ent.Word == f))
                    .ToList();
                if (relatedEntries.Count != forms.Count)
                {
                    Console.WriteLine("Missing entry from ({0}) in ({1})", string.Join("|", forms), string.Join("|", relatedEntries));
                }
                var newRelationships = relatedEntries
                    .Select(ent => new DictionaryEntryRelationship()
                    {
                        Source = infinitive,
                        Target = ent,
                        Type = relationshipType
                    });
                relationships.AddRange(newRelationships);
            }
            var dictionaryEntryRelationships = relationships
                .Select((wp, i) => new { Index = i, Wp = wp })
                .GroupBy(a => a.Index / batchSize)
                .SelectMany(grp => DbUtilities.GetOrCreate(grp.Select(a => a.Wp).ToList(), db))
                .ToList();
            Console.WriteLine("Created {0} relationships in db", dictionaryEntryRelationships.Count);
        }

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
