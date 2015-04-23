using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EnglishGraph.Models
{
    public class PartOfSpeechDetector
    {
        /// <summary>
        /// Detects the POS of a dictionary entry.
        /// Two options:
        /// - the entry already exist in the dictionary --> return it
        /// - the entry don't exist in the dictionary --> try to link it to existing entries (via suffixes/prefixes etc.)
        /// </summary>
        /// <param name="token">The token for the dictionary entry to find the POS</param>
        /// <param name="isFirstTokenInSentence">Whether it's the first token in sentence (not always known)</param>
        /// <param name="isLastTokenInSentence">Whether it's the last word token (not '.' typically) in sentence (not always known)</param>
        /// <param name="dictionary">The known English word in the dictionary</param>
        /// <returns>The dictionary entry (word + POS)</returns>
        public DictionaryEntry DetectPos(string token, bool? isFirstTokenInSentence, bool? isLastTokenInSentence, EnglishDictionary dictionary)
        {
            // Compound words
            if (token.Contains("-"))
            {
                // stop detection right away (for now)
                return new DictionaryEntry()
                {
                    Word = token,
                    PartOfSpeech = PartsOfSpeech.Unknown
                };
            }

            // Abbreviations
            if ((token.EndsWith(".") && isLastTokenInSentence.HasValue && !isLastTokenInSentence.Value) || Regex.IsMatch(token, "(^[A-Z]\\.)+$"))
            {
                return new DictionaryEntry()
                {
                    Word = token,
                    PartOfSpeech = PartsOfSpeech.Abbreviation
                };
            }

            // Proper nouns - TODO: handle plural forms?
            if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token) 
                && isFirstTokenInSentence.HasValue && !isFirstTokenInSentence.Value)
            {
                return new DictionaryEntry()
                {
                    Word = token,
                    PartOfSpeech = PartsOfSpeech.ProperNoun
                };
            }

            // Plural in "ies"
            if (token.EndsWith("ies"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                var singularForm = token.Substring(0, token.Length - 3) + 'y';
                DictionaryEntry singularFormInDb;
                if (dictionary.TryGetEntry(singularForm, PartsOfSpeech.Noun, out singularFormInDb))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.NounPlural,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.NounPlural,
                                Source = singularFormInDb
                            }
                        }
                    };
                    return entry;
                }
                else if (singularFormInDb != null && singularFormInDb.PartOfSpeech == PartsOfSpeech.ProperNoun)
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.ProperNounPlural,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.ProperNounPlural,
                                Source = singularFormInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", singularForm, token);
                }
            }

            // Plural in "es"
            if (token.EndsWith("es"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                var singularForm = token.Substring(0, token.Length - 2);
                DictionaryEntry singularFormInDb;
                if (dictionary.TryGetEntry(singularForm, PartsOfSpeech.Noun, out singularFormInDb))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.NounPlural,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.NounPlural,
                                Source = singularFormInDb
                            }
                        }
                    };
                    return entry;
                }
                else if (singularFormInDb != null && singularFormInDb.PartOfSpeech == PartsOfSpeech.ProperNoun)
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.ProperNounPlural,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.ProperNounPlural,
                                Source = singularFormInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", singularForm, token);
                }
            }

            // Plural in "s"
            if (token.EndsWith("s"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                var singularForm = token.Substring(0, token.Length - 1);
                DictionaryEntry singularFormInDb;
                if (dictionary.TryGetEntry(singularForm, PartsOfSpeech.Noun, out singularFormInDb))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.NounPlural,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.NounPlural,
                                Source = singularFormInDb
                            }
                        }
                    };
                    return entry;
                }
                else if (singularFormInDb != null && singularFormInDb.PartOfSpeech == PartsOfSpeech.ProperNoun)
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.ProperNounPlural,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.ProperNounPlural,
                                Source = singularFormInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", singularForm, token);
                }
            }

            // nouns
            if (token.EndsWith("ness"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract adjective from noun; 1 expection, adjectives finishing in 'y'; happy -> happiness
                var lastChar = token[token.Length - 5];
                var adjective = lastChar == 'i'
                    ? token.Substring(0, token.Length - 5) + 'y'
                    : token.Substring(0, token.Length - 4);
                DictionaryEntry adjectiveInDb;
                if (dictionary.TryGetEntry(adjective, PartsOfSpeech.Adjective, out adjectiveInDb))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.Noun,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.AdjectiveToNoun,
                                Source = adjectiveInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", adjective, token);
                }
            }

            if (token.EndsWith("ility"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract adjective from noun; 1 expection, possibility -> possible
                var adjective = token.Substring(0, token.Length - 5) + "le";
                DictionaryEntry adjectiveInDb;
                if (dictionary.TryGetEntry(adjective, PartsOfSpeech.Adjective, out adjectiveInDb))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.Noun,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.AdjectiveToNoun,
                                Source = adjectiveInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", adjective, token);
                }
            }

            if (token.EndsWith("ment"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract adjective from noun; 1 expection, arrangement -> arrange
                var verb = token.Substring(0, token.Length - 4);
                DictionaryEntry verbInDb;
                if (dictionary.TryGetEntry(verb, PartsOfSpeech.Verb, out verbInDb))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.Noun,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.InfinitiveToNoun,
                                Source = verbInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", verb, token);
                }
            }

            if (token.EndsWith("ship") || token.EndsWith("hood"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract noun from noun; 1 expection, partnership -> partner / neighbourhoud -> neighbour
                var noun = token.Substring(0, token.Length - 4);
                DictionaryEntry nounInDb;
                if (dictionary.TryGetEntry(noun, PartsOfSpeech.Noun, out nounInDb))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.Noun,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.NounToNoun,
                                Source = nounInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", noun, token);
                }
            }

            if (token.EndsWith("ity"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract adjective from noun; 1 expection, complexity -> complex / immunity -> immune
                // TODO handle plurals - complexities
                var adjective = token.Substring(0, token.Length - 3);
                var potentialAdjectives = new List<string>() {adjective, adjective + "e"};
                DictionaryEntry adjectiveInDb = null;
                if (potentialAdjectives.Any(adj => dictionary.TryGetEntry(adj, PartsOfSpeech.Adjective, out adjectiveInDb)))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.Noun,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.AdjectiveToNoun,
                                Source = adjectiveInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", adjective, token);
                }
            }

            // adjectives
            if (token.EndsWith("less") || token.EndsWith("free"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstCharUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // One exception: tireless -> don't come from noun 'tire' (but from the verb) - TODO: handle later
                // Extract noun from adjective
                var noun = token.Substring(0, token.Length - 4);
                DictionaryEntry nounInDb;
                if (dictionary.TryGetEntry(noun, PartsOfSpeech.Noun, out nounInDb))
                {
                    var entry = new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.Adjective,
                        StemmedFromRelationships = new List<DictionaryEntryRelationship>()
                        {
                            new DictionaryEntryRelationship()
                            {
                                Type = DictionaryEntryRelationshipTypes.NounToAdjective,
                                Source = nounInDb
                            }
                        }
                    };
                    return entry;
                }
                else
                {
                    Console.WriteLine("{0} not in db / {1} wasn't created", noun, token);
                }
            }

            return new DictionaryEntry()
            {
                Word = token,
                PartOfSpeech = PartsOfSpeech.Unknown
            };
        }
    }
}
