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

        public DictionaryEntry Detect(string token, bool isFirstTokenInSentence, bool isLastTokenInSentence, DbSet<DictionaryEntry> dictionary)
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
                // log something?
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
                // log something?
            }

            return new DictionaryEntry()
            {
                Word = token,
                PartOfSpeech = PartsOfSpeech.Unknown
            };
        }
    }
}
