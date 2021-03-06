﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EnglishGraph.Models.PosDetection
{
    public class PartOfSpeechDetector
    {
        private static readonly List<PosDetectionRule> PosDetectionRules = new List<PosDetectionRule>()
        {
            // unknown POS for compound words - TODO: handle compound words
            new PosDetectionRule()
            {
                MatchingCondition = a => a.Token.Contains("-"),
                DictionaryEntryCreator = tok => new DictionaryEntry()
                {
                    Word = tok,
                    PartOfSpeech = PartsOfSpeech.Unknown
                }
            },
            // abbreviations - TODO: consolidate those conditions
            // Ex: Dr., Mr. ... // R.I.P, p.m... // FAQ, PhD... // AT&T, S&P...
            new PosDetectionRule()
            {
                MatchingCondition = a => Regex.IsMatch(a.Token, "^[a-zA-Z]+\\.$") 
                    || Regex.IsMatch(a.Token, "^([a-zA-Z]\\.)+[a-zA-Z]$")
                    || Regex.IsMatch(a.Token, "^[A-Z][a-zA-Z]*[A-Z]$")
                    || Regex.IsMatch(a.Token, "^[A-Z][A-Z\\&]+[A-Z]$"),
                DictionaryEntryCreator = tok => new DictionaryEntry()
                {
                    Word = tok,
                    PartOfSpeech = PartsOfSpeech.Abbreviation
                }
            },
            // proper nouns
            new PosDetectionRule()
            {
                MatchingCondition = a => 
                    StringUtilities.IsFirstCharUpperCased(a.Token) 
                    && !StringUtilities.IsAllUpperCased(a.Token),
                DictionaryEntryCreator = tok => new DictionaryEntry()
                {
                    Word = tok,
                    PartOfSpeech = PartsOfSpeech.ProperNoun
                }
            },

            // Plural nouns from singular nouns
            // "s" suffix; strangers -> stranger
            new SuffixBasedPosDetectionRule("s", "", PartsOfSpeech.NounPlural, DictionaryEntryRelationshipTypes.NounPlural, PartsOfSpeech.Noun),
            // "es" suffix; wishes -> wish
            new SuffixBasedPosDetectionRule("es", "", PartsOfSpeech.NounPlural, DictionaryEntryRelationshipTypes.NounPlural, PartsOfSpeech.Noun),
            // 'ies' suffix; difficulties -> difficulty
            new SuffixBasedPosDetectionRule("ies", "y", PartsOfSpeech.NounPlural, DictionaryEntryRelationshipTypes.NounPlural, PartsOfSpeech.Noun),

            // Plural nouns from singular nouns
            // "s" suffix; strangers -> stranger
            new SuffixBasedPosDetectionRule("s", "", PartsOfSpeech.ProperNounPlural, DictionaryEntryRelationshipTypes.ProperNounPlural, PartsOfSpeech.ProperNoun),
            // "es" suffix; wishes -> wish
            new SuffixBasedPosDetectionRule("es", "", PartsOfSpeech.ProperNounPlural, DictionaryEntryRelationshipTypes.ProperNounPlural, PartsOfSpeech.ProperNoun),
            // 'ies' suffix; difficulties -> difficulty
            new SuffixBasedPosDetectionRule("ies", "y", PartsOfSpeech.ProperNounPlural, DictionaryEntryRelationshipTypes.ProperNounPlural, PartsOfSpeech.ProperNoun),

            // Nouns from adjectives
            // 'iness' suffix; happiness -> happy
            new SuffixBasedPosDetectionRule("iness", "y", PartsOfSpeech.Noun, DictionaryEntryRelationshipTypes.AdjectiveToNoun, PartsOfSpeech.Adjective),
            // 'ness' suffix; restlessness -> restless
            new SuffixBasedPosDetectionRule("ness", "", PartsOfSpeech.Noun, DictionaryEntryRelationshipTypes.AdjectiveToNoun, PartsOfSpeech.Adjective),
            // 'ility' suffix; possibility -> possible
            new SuffixBasedPosDetectionRule("ility", "le", PartsOfSpeech.Noun, DictionaryEntryRelationshipTypes.AdjectiveToNoun, PartsOfSpeech.Adjective),
            // 'ity' suffix; complexity -> complex
            new SuffixBasedPosDetectionRule("ity", "", PartsOfSpeech.Noun, DictionaryEntryRelationshipTypes.AdjectiveToNoun, PartsOfSpeech.Adjective),
            // 'ity' suffix; immunity -> immune
            new SuffixBasedPosDetectionRule("ity", "e", PartsOfSpeech.Noun, DictionaryEntryRelationshipTypes.AdjectiveToNoun, PartsOfSpeech.Adjective),

            // Adjectives from nouns
            // 'less' suffix; homeless -> home (exception: tireless -> come from verb 'tire' and not noun 'tire' - TODO: handle later)
            new SuffixBasedPosDetectionRule("less", "", PartsOfSpeech.Adjective, DictionaryEntryRelationshipTypes.NounToAdjective, PartsOfSpeech.Noun),
            // 'free' suffix; sugarfree -> sugar
            new SuffixBasedPosDetectionRule("free", "", PartsOfSpeech.Adjective, DictionaryEntryRelationshipTypes.NounToAdjective, PartsOfSpeech.Noun),
            
            // Nouns from verbs
            // 'ment' suffix; judgement -> judge
            new SuffixBasedPosDetectionRule("ment", "", PartsOfSpeech.Noun, DictionaryEntryRelationshipTypes.InfinitiveToNoun, PartsOfSpeech.Verb),

            // Nouns from nouns
            // 'ship' suffix; relationship -> relation
            new SuffixBasedPosDetectionRule("ship", "", PartsOfSpeech.Noun, DictionaryEntryRelationshipTypes.NounToNoun, PartsOfSpeech.Noun),
            // 'hood' suffix; brotherhood -> brother
            new SuffixBasedPosDetectionRule("hood", "", PartsOfSpeech.Noun, DictionaryEntryRelationshipTypes.NounToNoun, PartsOfSpeech.Noun),

            // Adverbs from adjectives
            // 'ly' suffix; exceptionally -> exceptional
            new SuffixBasedPosDetectionRule("ly", "", PartsOfSpeech.Adverb, DictionaryEntryRelationshipTypes.AdverbToAdjective, PartsOfSpeech.Adjective),

            // Comparatives from adjectives
            // 'er' suffix; harder -> hard
            new SuffixBasedPosDetectionRule("er", "", PartsOfSpeech.Comparative, DictionaryEntryRelationshipTypes.ComparativeToAdjective, PartsOfSpeech.Adjective),
            // 'er' suffix; larger -> large
            new SuffixBasedPosDetectionRule("er", "e", PartsOfSpeech.Comparative, DictionaryEntryRelationshipTypes.ComparativeToAdjective, PartsOfSpeech.Adjective),
            // 'er' suffix + doubling ending consonant; bigger -> big
            new DeduplicateDoubleEndingConsonantSuffixBasedPosDetectionRule("er", "", PartsOfSpeech.Comparative, DictionaryEntryRelationshipTypes.ComparativeToAdjective, PartsOfSpeech.Adjective),
            // 'ier' suffix; happier -> happy
            new SuffixBasedPosDetectionRule("ier", "y", PartsOfSpeech.Comparative, DictionaryEntryRelationshipTypes.ComparativeToAdjective, PartsOfSpeech.Adjective),

            // Superlatives from adjectives
            // 'est' suffix; fastest -> fast
            new SuffixBasedPosDetectionRule("est", "", PartsOfSpeech.Superlative, DictionaryEntryRelationshipTypes.SuperlativeToAdjective, PartsOfSpeech.Adjective),
            // 'est' suffix; largest -> large
            new SuffixBasedPosDetectionRule("est", "e", PartsOfSpeech.Superlative, DictionaryEntryRelationshipTypes.SuperlativeToAdjective, PartsOfSpeech.Adjective),
            // 'est' suffix + doubling ending consonant; biggest -> big
            new DeduplicateDoubleEndingConsonantSuffixBasedPosDetectionRule("est", "", PartsOfSpeech.Superlative, DictionaryEntryRelationshipTypes.SuperlativeToAdjective, PartsOfSpeech.Adjective),
            // 'iest' suffix; happiest -> happy
            new SuffixBasedPosDetectionRule("iest", "y", PartsOfSpeech.Superlative, DictionaryEntryRelationshipTypes.SuperlativeToAdjective, PartsOfSpeech.Adjective),

            // Gerundives from verbs
            // 'ing' suffix; doing -> do
            new SuffixBasedPosDetectionRule("ing", "", PartsOfSpeech.Gerundive, DictionaryEntryRelationshipTypes.Gerundive, PartsOfSpeech.Verb),
            // 'ing' suffix; charging -> charge
            new SuffixBasedPosDetectionRule("ing", "e", PartsOfSpeech.Gerundive, DictionaryEntryRelationshipTypes.Gerundive, PartsOfSpeech.Verb),
            // 'ing' suffix + doubling ending consonant: running -> run
            new DeduplicateDoubleEndingConsonantSuffixBasedPosDetectionRule("int", "", PartsOfSpeech.Gerundive, DictionaryEntryRelationshipTypes.Gerundive, PartsOfSpeech.Verb),

            // simple past forms from verbs
            // 'ed' suffix; looked -> look
            new SuffixBasedPosDetectionRule("ed", "", PartsOfSpeech.VerbSimplePast, DictionaryEntryRelationshipTypes.SimplePast, PartsOfSpeech.Verb),
            // 'ed' suffix; charged -> charge
            new SuffixBasedPosDetectionRule("ed", "e", PartsOfSpeech.VerbSimplePast, DictionaryEntryRelationshipTypes.SimplePast, PartsOfSpeech.Verb),
            // 'ed' suffix + doubling ending consonant: tanned -> tan
            new DeduplicateDoubleEndingConsonantSuffixBasedPosDetectionRule("ed", "", PartsOfSpeech.VerbSimplePast, DictionaryEntryRelationshipTypes.SimplePast, PartsOfSpeech.Verb),

            // TODO: exact duplicate of above: do otherwise?
            // past participle forms from verbs
            // 'ed' suffix; looked -> look
            new SuffixBasedPosDetectionRule("ed", "", PartsOfSpeech.VerbPastParticiple, DictionaryEntryRelationshipTypes.PastParticiple, PartsOfSpeech.Verb),
            // 'ed' suffix; charged -> charge
            new SuffixBasedPosDetectionRule("ed", "e", PartsOfSpeech.VerbPastParticiple, DictionaryEntryRelationshipTypes.PastParticiple, PartsOfSpeech.Verb),
            // 'ed' suffix + doubling ending consonant: tanned -> tan
            new DeduplicateDoubleEndingConsonantSuffixBasedPosDetectionRule("ed", "", PartsOfSpeech.VerbPastParticiple, DictionaryEntryRelationshipTypes.PastParticiple, PartsOfSpeech.Verb),

            // Prefixes
            // 'anti' prefix
            new PrefixBasedPosDetectionRule("anti", PartsOfSpeech.Adjective),
            new PrefixBasedPosDetectionRule("anti", PartsOfSpeech.Noun),
            // 'co' prefix
            new PrefixBasedPosDetectionRule("co", PartsOfSpeech.Noun),
            new PrefixBasedPosDetectionRule("co", PartsOfSpeech.Verb),
            // 'dis' prefix
            new PrefixBasedPosDetectionRule("dis", PartsOfSpeech.Verb),
            // 'il' prefix
            new PrefixBasedPosDetectionRule("il", PartsOfSpeech.Adjective),
            // 'im' prefix
            new PrefixBasedPosDetectionRule("im", PartsOfSpeech.Adjective),
            // 'in' prefix
            new PrefixBasedPosDetectionRule("in", PartsOfSpeech.Adjective),
            // 'inter' prefix
            new PrefixBasedPosDetectionRule("inter", PartsOfSpeech.Adjective),
            // 'ir' prefix
            new PrefixBasedPosDetectionRule("ir", PartsOfSpeech.Adjective),
            // 'mis' prefix: ex: misguide -> guide; miscommunication -> communication
            new PrefixBasedPosDetectionRule("mis", PartsOfSpeech.Verb),
            new PrefixBasedPosDetectionRule("mis", PartsOfSpeech.Noun),
            // 'over' prefix
            new PrefixBasedPosDetectionRule("over", PartsOfSpeech.Verb), // overreact
            new PrefixBasedPosDetectionRule("over", PartsOfSpeech.Verb), // overactive
            new PosChangingPrefixBasedPosDetectionRule("over", PartsOfSpeech.Adjective, PartsOfSpeech.Noun), // overweight
            // 'out' prefix
            new PrefixBasedPosDetectionRule("out", PartsOfSpeech.Verb),
            // 'post' prefix
            new PrefixBasedPosDetectionRule("post", PartsOfSpeech.Verb),
            new PrefixBasedPosDetectionRule("post", PartsOfSpeech.Noun),
            // 'pre' prefix
            new PrefixBasedPosDetectionRule("pre", PartsOfSpeech.Verb),
            new PrefixBasedPosDetectionRule("pre", PartsOfSpeech.Noun),
            // 'pro' prefix
            new PrefixBasedPosDetectionRule("pro", PartsOfSpeech.Adjective),
            new PrefixBasedPosDetectionRule("pro", PartsOfSpeech.Noun),
            // 'sub' prefix
            new PrefixBasedPosDetectionRule("sub", PartsOfSpeech.Adjective),
            // 'super' prefix
            new PrefixBasedPosDetectionRule("super", PartsOfSpeech.Adjective),
            new PrefixBasedPosDetectionRule("super", PartsOfSpeech.Noun),
            // 'trans' prefix
            new PrefixBasedPosDetectionRule("trans", PartsOfSpeech.Verb),
            new PrefixBasedPosDetectionRule("trans", PartsOfSpeech.Noun),
            // 'un' prefix
            new PrefixBasedPosDetectionRule("un", PartsOfSpeech.Verb),
            new PrefixBasedPosDetectionRule("un", PartsOfSpeech.Adjective),
            // 'under' prefix; ex: underachieve (v.) -> achieve (v.), underweight (adj.) -> weight (n.)
            new PrefixBasedPosDetectionRule("under", PartsOfSpeech.Verb), // underreact
            new PrefixBasedPosDetectionRule("under", PartsOfSpeech.Adjective), // underactive
            new PosChangingPrefixBasedPosDetectionRule("under", PartsOfSpeech.Adjective, PartsOfSpeech.Noun), // underweight

            // rules below are added from experience
            // 're' prefix; ex: reinvest (v.) -> invest (v.); recapitalization (n.) -> capitalization
            new PrefixBasedPosDetectionRule("re", PartsOfSpeech.Verb),
            new PrefixBasedPosDetectionRule("re", PartsOfSpeech.Noun),
            // 'non' prefix; ex: nonvoting -> voting
            new PrefixBasedPosDetectionRule("non", PartsOfSpeech.Adjective),
            new PrefixBasedPosDetectionRule("non", PartsOfSpeech.Verb),
            // 'multi' prefix; ex: multibillion (adj.) -> billion (n.); 
            new PrefixBasedPosDetectionRule("multi", PartsOfSpeech.Verb),
            new PosChangingPrefixBasedPosDetectionRule("multi", PartsOfSpeech.Adjective, PartsOfSpeech.Noun),
            // 'counter' prefix;
            new PrefixBasedPosDetectionRule("counter", PartsOfSpeech.Verb),//counterattack
            new PrefixBasedPosDetectionRule("counter", PartsOfSpeech.Noun),//counterattack
            new PrefixBasedPosDetectionRule("counter", PartsOfSpeech.Adjective),//counterproductive
        };

        private readonly static List<TokenClassificationRule> TokenClassificationRules = new List<TokenClassificationRule>()
        {
            new TokenClassificationRule(StringUtilities.IsPunctuation, PartsOfSpeech.Punctuation),
            new TokenClassificationRule(StringUtilities.IsSymbol, PartsOfSpeech.Symbol),
            new TokenClassificationRule(StringUtilities.IsNumber, PartsOfSpeech.Number),
            new TokenClassificationRule(StringUtilities.IsTime, PartsOfSpeech.Time),
            new TokenClassificationRule(StringUtilities.IsFraction, PartsOfSpeech.Fraction),
            new TokenClassificationRule(StringUtilities.IsPercentage, PartsOfSpeech.Percentage),
            new TokenClassificationRule(StringUtilities.IsAmount, PartsOfSpeech.Amount),
            new TokenClassificationRule(s => s.Contains("-"), PartsOfSpeech.Compound),
            new TokenClassificationRule(s => s.Contains("/"), PartsOfSpeech.CompoundSlash),
        };

        /// <summary>
        /// Detects the POSs of a dictionary entry.
        /// Two options:
        /// - the entry already exist in the dictionary --> return it
        /// - the entry don't exist in the dictionary --> try to link it to existing entries (via suffixes/prefixes etc.)
        /// </summary>
        /// <param name="token">The token for the dictionary entry to find the POS</param>
        /// <param name="isFirstTokenInSentence">Whether it's the first token in sentence (not always known)</param>
        /// <param name="isLastTokenInSentence">Whether it's the last word token (not '.' typically) in sentence (not always known)</param>
        /// <param name="dictionary">The known English word in the dictionary</param>
        /// <returns>The dictionary entry (word + POS)</returns>
        public List<DictionaryEntry> DetectPos(string token, bool? isFirstTokenInSentence, bool? isLastTokenInSentence, EnglishDictionary dictionary)
        {
            var specificEntries = TokenClassificationRules
                .Where(rule => rule.IsMatch(token))
                .Select(rule => rule.GetEntry(token))
                .ToList();
            // update POS of derived forms
            foreach (var entry in specificEntries.Where(ent => ent.StemmedFromRelationships.Any()))
            {
                foreach (var relationship in entry.StemmedFromRelationships)
                {
                    var targetEntries = DetectPos(relationship.Target.Word, isFirstTokenInSentence,
                        isLastTokenInSentence, dictionary);
                    if (targetEntries.Any(e => e.PartOfSpeech != PartsOfSpeech.Unknown))
                    {
                        relationship.Target.PartOfSpeech =
                            targetEntries.First(e => e.PartOfSpeech != PartsOfSpeech.Unknown).PartOfSpeech;
                    }
                }
            }
            var relevantEntries = specificEntries
                .Where(ent => !ent.StemmedFromRelationships.Any()
                    || ent.StemmedFromRelationships.All(rel => rel.Target.PartOfSpeech != PartsOfSpeech.Unknown))
                .ToList();
            if (relevantEntries.Any())
            {
                return relevantEntries;
            }
            
            
            // If we get here, the token was not recognized by any of the classification rules.
            // Best case scenario, the word is in db and if it is not, we run the detection rules on this new ly encountered token

            // Special case when all caps, and the lower case form is in the dictionary
            if (StringUtilities.IsAllUpperCased(token))
            {
                var lcTokens = new List<string>() {token.ToLower(), StringUtilities.UpperFirstLetter(token.ToLower())};
                foreach (var lcToken in lcTokens)
                {
                    List<DictionaryEntry> lcEntries;
                    var success = dictionary.TryGetEntries(lcToken, out lcEntries);
                    if (success)
                    {
                        return lcEntries;
                    }
                }
            }

            var tokensToSearch = new List<string>() { token };
            // If the first letter is capitalized, and the word is not all upper cased,
            // also look for the lower cased token.
            // (being inside the sentence is not relevant because:
            // - sentence are not necessarily well detected
            // - several nouns can be used as proper nouns for brands: xxx Research Institute, etc.
            if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
            {
                tokensToSearch.Add(StringUtilities.LowerFirstLetter(token));
            }

            // Find the entries already in the dictionary for those tokens
            var existingEntries = new List<DictionaryEntry>();
            foreach (var tokenToSearch in tokensToSearch)
            {
                List<DictionaryEntry> entriesInDictionary;
                if (dictionary.TryGetEntries(tokenToSearch, out entriesInDictionary))
                {
                    existingEntries.AddRange(entriesInDictionary);
                }
            }

            // Probably the most important assumption in this method:
            // if we find at least one matching entity, we return it and don't try to detect others through our rules
            // -> means that if a word already exist with a POS in the dictionary, we won't detect potential other POSs here
            if (existingEntries.Any())
            {
                return existingEntries;
            }
            else
            {
                var validEntries = new List<DictionaryEntry>();

                foreach (var tokenToSearch in tokensToSearch)
                {
                    // If our dictionary is complete, we should very rarely execute this code
                    // (hence the little consideration for the performance)
                    var entries = PosDetectionRules
                        .Where(rule =>
                                rule.MatchingCondition(
                                new TokenAndPositionInSentence(tokenToSearch, isFirstTokenInSentence, isLastTokenInSentence)))
                        .Select(rule => rule.DictionaryEntryCreator(tokenToSearch))
                        .ToList();

                    foreach (var entry in entries)
                    {
                        // if the entry does not derived from another, return it as is
                        if (!entry.StemmedFromRelationships.Any())
                        {
                            validEntries.Add(entry);
                        }
                        else
                        {
                            // Otherwise, check that at least one of the derivation rules is valid
                            var validRelationships = entry.StemmedFromRelationships
                                .Where(rel =>
                                        this.DetectPos(rel.Source.Word, isFirstTokenInSentence, isLastTokenInSentence,
                                            dictionary).Any(ent => ent.PartOfSpeech == rel.Source.PartOfSpeech))
                                .ToList();
                            if (validRelationships.Any())
                            {
                                entry.StemmedFromRelationships = validRelationships;
                                validEntries.Add(entry);
                            }
                        }
                    }
                }
                
                return validEntries;
            }
        }
    }
}
