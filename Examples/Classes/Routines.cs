using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models;
using EnglishGraph.Models.Resources;

namespace Examples.Classes
{
    public class Routines
    {

        public static void LoadIrregularVerbs(EnglishGraphContext db)
        {
            var irregularVerbs = IrregularVerbs.Instance.AllIrregularVerbs;

            var infinitives = irregularVerbs
                .Select(tup => new Tuple<string, byte>(tup.Infinitive, PartsOfSpeech.Verb))
                .ToList();
            var pastForms = irregularVerbs
                .SelectMany(tup => tup.SimplePastForms.Select(sp => new Tuple<string, byte>(sp, PartsOfSpeech.VerbSimplePast)))
                .ToList();
            var pastParticipleForms = irregularVerbs
                .SelectMany(tup => tup.PastParticipleForms.Select(pp => new Tuple<string, byte>(pp, PartsOfSpeech.VerbPastParticiple)))
                .ToList();

            var infinitiveEntities = DbUtilities.GetOrCreate(infinitives, db);
            var pastEntities = DbUtilities.GetOrCreate(pastForms, db);
            var pastParticipleEntities = DbUtilities.GetOrCreate(pastParticipleForms, db);

            // simple past relationships 
            var spRels = new List<DictionaryEntryRelationship>();
            foreach (var pastEntity in pastEntities)
            {
                var irregularVerb = irregularVerbs.First(iv => iv.SimplePastForms.Contains(pastEntity.Word));
                var infinitiveEntity = infinitiveEntities
                    .First(ent => ent.Word == irregularVerb.Infinitive);
                var relationship = new DictionaryEntryRelationship()
                {
                    Source = infinitiveEntity,
                    Target = pastEntity,
                    Type = DictionaryEntryRelationshipTypes.SimplePast
                };
                spRels.Add(relationship);
            }
            DbUtilities.GetOrCreate(spRels, db);

            // past participle relationships 
            var ppRels = new List<DictionaryEntryRelationship>();
            foreach (var pastParticipleEntity in pastParticipleEntities)
            {
                var irregularVerb = irregularVerbs.First(iv => iv.PastParticipleForms.Contains(pastParticipleEntity.Word));
                var infinitiveEntity = infinitiveEntities
                    .First(ent => ent.Word == irregularVerb.Infinitive);
                var relationship = new DictionaryEntryRelationship()
                {
                    Source = infinitiveEntity,
                    Target = pastParticipleEntity,
                    Type = DictionaryEntryRelationshipTypes.PastParticiple
                };
                ppRels.Add(relationship);
            }
            DbUtilities.GetOrCreate(ppRels, db);
        }

        /// <summary>
        /// Loads the words missing in wordnet that we couldn't find in any other list
        /// </summary>
        public static void LoadMissingWords(EnglishGraphContext db)
        {
            var entries = new List<Tuple<string, byte>>()
            {
                // Adverbs
                new Tuple<string, byte>("how", PartsOfSpeech.Adverb),
                new Tuple<string, byte>("anytime", PartsOfSpeech.Adverb),
                // Pronouns
                new Tuple<string, byte>("whichever", PartsOfSpeech.Pronoun),
                new Tuple<string, byte>("others", PartsOfSpeech.Pronoun),
                // Adjectives
                new Tuple<string, byte>("whichever", PartsOfSpeech.Adjective),
            };

            DbUtilities.GetOrCreate(entries, db);
        }

        public static void LoadIrregularPlurals(EnglishGraphContext db)
        {
            var exceptions = IrregularPlurals.AllIrregularPlurals;

            var singularNouns = exceptions
                .Select(tup => new Tuple<string, byte>(tup.Item1, PartsOfSpeech.Noun))
                .ToList();
            var pluralNouns = exceptions
                .Select(tup => new Tuple<string,byte>(tup.Item2, PartsOfSpeech.NounPlural))
                .ToList();

            var singularEntities = DbUtilities.GetOrCreate(singularNouns, db);
            var pluralEntities = DbUtilities.GetOrCreate(pluralNouns, db);

            var relationships = exceptions
                .Select(tup => new DictionaryEntryRelationship()
                {
                    Source = singularEntities.First(ent => ent.Word == tup.Item1),
                    Target = pluralEntities.First(ent => ent.Word == tup.Item2),
                    Type = DictionaryEntryRelationshipTypes.NounPlural
                })
                .ToList();
            DbUtilities.GetOrCreate(relationships, db);
        }

        public static void LoadNegativeContractions(EnglishGraphContext db)
        {
            var negativeContractions = NegativeContractions.AllContractions;

            var negContEntries = negativeContractions
                .Select(tup => new Tuple<string,byte>(tup.Item1, PartsOfSpeech.NegativeContraction))
                .ToList();
            var verbs = negativeContractions
                .Select(tup => tup.Item2)
                .ToList();

            var negContEntities = DbUtilities.GetOrCreate(negContEntries, db);
            var verbEntities = DbUtilities.GetEntries(verbs, db)
                .Where(ent => PartsOfSpeech.IsVerb(ent.PartOfSpeech) 
                    && ent.PartOfSpeech != PartsOfSpeech.VerbPastParticiple && ent.PartOfSpeech != PartsOfSpeech.Gerundive)
                .ToList();

            var relationships = negativeContractions
                .Select(tup => new DictionaryEntryRelationship()
                {
                    Source = negContEntities.First(ent => ent.Word == tup.Item1),
                    Target = verbEntities.First(ent => ent.Word == tup.Item2),
                    Type = DictionaryEntryRelationshipTypes.NegativeContractionToVerb
                })
                .ToList();
            DbUtilities.GetOrCreate(relationships, db);
        }

        public static void LoadIrregularSuperlatives(EnglishGraphContext db)
        {
            var irregularSuperlatives = Superlatives.Exceptions;

            var adjectives = irregularSuperlatives
                .Select(tup => new Tuple<string,byte>(tup.Item1, PartsOfSpeech.Adjective))
                .ToList();
            var superlatives = irregularSuperlatives
                .Select(tup => new Tuple<string, byte>(tup.Item2, PartsOfSpeech.Superlative))
                .ToList();

            var adjectiveEntities = DbUtilities.GetOrCreate(adjectives, db);
            var superlativeEntities = DbUtilities.GetOrCreate(superlatives, db);

            var relationships = irregularSuperlatives
                .Select(tup => new DictionaryEntryRelationship()
                {
                    Source = superlativeEntities.First(ent => ent.Word == tup.Item2),
                    Target = adjectiveEntities.First(ent => ent.Word == tup.Item1),
                    Type = DictionaryEntryRelationshipTypes.SuperlativeToAdjective
                })
                .ToList();
            DbUtilities.GetOrCreate(relationships, db);
        }

        public static void LoadIrregularComparatives(EnglishGraphContext db)
        {
            var irregularComparatives = Comparatives.Exceptions;

            var adjectives = irregularComparatives
                .Select(tup => new Tuple<string,byte>(tup.Item1, PartsOfSpeech.Adjective))
                .ToList();
            var comparatives = irregularComparatives
                .Select(tup => new Tuple<string, byte>(tup.Item2, PartsOfSpeech.Comparative))
                .ToList();

            var adjectiveEntities = DbUtilities.GetOrCreate(adjectives, db);
            var comparativeEntities = DbUtilities.GetOrCreate(comparatives, db);

            var relationships = irregularComparatives
                .Select(tup => new DictionaryEntryRelationship()
                {
                    Source = comparativeEntities.First(ent => ent.Word == tup.Item2),
                    Target = adjectiveEntities.First(ent => ent.Word == tup.Item1),
                    Type = DictionaryEntryRelationshipTypes.ComparativeToAdjective
                })
                .ToList();
            DbUtilities.GetOrCreate(relationships, db);
        }

        public static void LoadContractions(EnglishGraphContext db)
        {
            var pronouns = Contractions.AllContractions;

            var contractions = pronouns
                .Select(tup => new Tuple<string,byte>(tup.Item1, PartsOfSpeech.Contractions))
                .ToList();
            var nonContractedForms = pronouns
                .Select(tup => tup.Item2)
                .ToList();

            var contractionEntries = DbUtilities.GetOrCreate(contractions, db);
            var nonContractedFormEntries = DbUtilities.GetEntries(nonContractedForms, db)
                .Where(ent => PartsOfSpeech.IsVerb(ent.PartOfSpeech));

            var relationships = pronouns
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
            var modals = Modals
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
            var subjPersPronouns = Pronouns.SubjectPersonal
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.SubjectPersonalPronoun))
                .ToList();
            var objPersPronouns = Pronouns.ObjectPersonal
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.ObjectPersonalPronoun))
                .ToList();
            var reflPersPronouns = Pronouns.ReflexivePersonal
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.ReflexivePersonalPronoun))
                .ToList();
            var possPronouns = Pronouns.Possessive
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.PossessivePronoun))
                .ToList();
            var indefPronouns = Pronouns.Indefinite
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.IndefinitePronoun))
                .ToList();
            var interPronouns = Pronouns.Interrogative
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string, byte>(s, PartsOfSpeech.InterrogativePronoun))
                .ToList();
            var relPronouns = Pronouns.Relative
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
            var allPrepositions = Prepositions.AllPrepositions
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.Preposition))
                .ToList();
            DbUtilities.GetOrCreate(allPrepositions, db);
        }

        public static void LoadDeterminers(EnglishGraphContext db, bool includeMwes)
        {
            var generalDeterminers = Determiners.General
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.Determiner))
                .ToList();
            var articleDeterminers = Determiners.Articles
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.ArticleDeterminer))
                .ToList();
            var demonstrativeDeterminers = Determiners.Demonstrative
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.DemonstrativeDeterminer))
                .ToList();
            var possessiveDeterminers = Determiners.Possessive
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
            var coordinatingConjunctions = Conjunctions.Coordinating
                .Where(s => includeMwes || !s.Contains(' '))
                .Select(s => new Tuple<string,byte>(s, PartsOfSpeech.CoordinatingConjunction))
                .ToList();
            var subordinatingConjunctions = Conjunctions.Subordinating
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
