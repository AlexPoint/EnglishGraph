using System;
using System.IO;
using System.Linq;
using EnglishGraph.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class VerbConjugatorTest
    {
        private static readonly string PathToProject = Directory.GetCurrentDirectory() + "/../../../UnitTests/";

        [TestMethod]
        public void TestInfinitiveToGerundiveForms()
        {
            var conjugator = new VerbConjugator();

            var pathToGerundiveFile = PathToProject + "Data/gerundives.txt";
            var testData = File.ReadAllLines(pathToGerundiveFile)
                .Select(line => line.Split('\t'))
                .Where(p => p.Length == 3)
                .Select(p => new Tuple<DictionaryEntry, string>(new DictionaryEntry()
                {
                    Word = p[0],
                    PartOfSpeech = PartsOfSpeech.Verb,
                    Pronunciation = p[2]
                }, p[1]))
                .ToList();
            foreach (var tuple in testData)
            {
                var entry = tuple.Item1;
                var result = tuple.Item2;
                var gerundive = conjugator.GetVerbForm(entry, VerbConjugator.VerbForm.Gerundive).First();
                Assert.AreEqual(gerundive, result);
            }
        }

        [TestMethod]
        public void TestGerundiveToInfinitiveForms()
        {
            var conjugator = new VerbConjugator();

            var pathToGerundiveFile = PathToProject + "Data/gerundives.txt";
            var testData = File.ReadAllLines(pathToGerundiveFile)
                .Select(line => line.Split('\t'))
                .Where(p => p.Length == 3)
                .Select(p => new Tuple<DictionaryEntry, string>(new DictionaryEntry()
                {
                    Word = p[0],
                    PartOfSpeech = PartsOfSpeech.Verb,
                    Pronunciation = p[2]
                }, p[1]))
                .ToList();
            foreach (var tuple in testData)
            {
                var entry = tuple.Item1;
                var gerundive = tuple.Item2;
                var potentialInfinitives = conjugator.GetPotentialInfinitiveFormsFromGerundive(gerundive);
                Assert.IsTrue(potentialInfinitives.Contains(entry.Word), 
                    string.Format("{0} not contained in ({1})", entry.Word, string.Join("|", potentialInfinitives)));
            }
        }
    }
}
