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

        public DictionaryEntry Detect(string token, bool isFirstTokenInSentence, bool isLastTokenInSentence, List<DictionaryEntry> dictionary)
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
            if ((token.EndsWith(".") && !isLastTokenInSentence) || Regex.IsMatch(token, "(^[A-Z]\\.)+$"))
            {
                /*var trimmedToken = token.Trim('.');
                var isTrimmedTokenInDb = dictionary.Any(de => de.Word == trimmedToken);
                if (!isTrimmedTokenInDb)
                {*/
                    return new DictionaryEntry()
                    {
                        Word = token,
                        PartOfSpeech = PartsOfSpeech.Abbreviation
                    };
                /*}
                else
                {
                    Console.WriteLine("Didn't created abbreviation '{0}' - '{1}' already in db", token, trimmedToken);
                }*/
            }

            // Proper nouns - TODO: handle plural forms?
            if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token) 
                && !isFirstTokenInSentence)
            {
                return new DictionaryEntry()
                {
                    Word = token,
                    PartOfSpeech = PartsOfSpeech.ProperNoun
                };
            }

            // Plural in "es"
            if (token.EndsWith("es"))
            {
                // lower case the first letter if necessary
                if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                var singularForm = token.Substring(0, token.Length - 2);
                var singularFormInDb = dictionary
                    .Where(ent => ent.Word == singularForm && ent.PartOfSpeech == PartsOfSpeech.Noun)
                    .ToList()
                    // case sensitive search
                    .FirstOrDefault(ent => ent.Word == singularForm);
                if (singularFormInDb != null && singularFormInDb.PartOfSpeech == PartsOfSpeech.Noun)
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
                if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                var singularForm = token.Substring(0, token.Length - 1);
                var singularFormInDb = dictionary
                    .Where(ent => ent.Word == singularForm && ent.PartOfSpeech == PartsOfSpeech.Noun)
                    .ToList()
                    // case sensitive search
                    .FirstOrDefault(ent => ent.Word == singularForm);
                if (singularFormInDb != null && singularFormInDb.PartOfSpeech == PartsOfSpeech.Noun)
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
                if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract adjective from noun; 1 expection, adjectives finishing in 'y'; happy -> happiness
                var lastChar = token[token.Length - 5];
                var adjective = lastChar == 'i'
                    ? token.Substring(0, token.Length - 5) + 'y'
                    : token.Substring(0, token.Length - 4);
                var adjectiveInDb = dictionary
                    .Where(ent => ent.Word == adjective && ent.PartOfSpeech == PartsOfSpeech.Adjective)
                    .ToList()
                    .FirstOrDefault(ent => ent.Word == adjective);
                if (adjectiveInDb != null)
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
                if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract adjective from noun; 1 expection, possibility -> possible
                var adjective = token.Substring(0, token.Length - 5) + "le";
                var adjectiveInDb = dictionary
                    .Where(ent => ent.Word == adjective && ent.PartOfSpeech == PartsOfSpeech.Adjective)
                    .ToList()
                    .FirstOrDefault(ent => ent.Word == adjective);
                if (adjectiveInDb != null)
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
                if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract adjective from noun; 1 expection, arrangement -> arrange
                var verb = token.Substring(0, token.Length - 4);
                var verbInDb = dictionary
                    .Where(ent => ent.Word == verb && ent.PartOfSpeech == PartsOfSpeech.Verb)
                    .ToList()
                    .FirstOrDefault(ent => ent.Word == verb);
                if (verbInDb != null)
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
                if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract noun from noun; 1 expection, partnership -> partner / neighbourhoud -> neighbour
                var noun = token.Substring(0, token.Length - 4);
                var nounIndDb = dictionary
                    .Where(ent => ent.Word == noun && ent.PartOfSpeech == PartsOfSpeech.Noun)
                    .ToList()
                    .FirstOrDefault(ent => ent.Word == noun);
                if (nounIndDb != null)
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
                                Source = nounIndDb
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
                if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // Extract adjective from noun; 1 expection, complexity -> complex / immunity -> immune
                // TODO handle plurals - complexities
                var adjective = token.Substring(0, token.Length - 3);
                var potentialAdjectives = new List<string>() {adjective, adjective + "e"};
                var adjectiveInDb = dictionary
                    .Where(ent => potentialAdjectives.Contains(ent.Word) && ent.PartOfSpeech == PartsOfSpeech.Adjective)
                    .ToList()
                    .FirstOrDefault(ent => potentialAdjectives.Contains(ent.Word));
                if (adjectiveInDb != null)
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
                if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token))
                {
                    token = char.ToLower(token.First()) + token.Substring(1);
                }

                // One exception: tireless -> don't come from noun 'tire' (but from the verb) - TODO: handle later
                // Extract noun from adjective
                var noun = token.Substring(0, token.Length - 4);
                var nounInDb = dictionary
                    .Where(ent => ent.Word == noun && ent.PartOfSpeech == PartsOfSpeech.Noun)
                    .ToList()
                    .FirstOrDefault(ent => ent.Word == noun);
                if (nounInDb != null)
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
