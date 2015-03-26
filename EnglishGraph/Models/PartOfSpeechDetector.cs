using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class PartOfSpeechDetector
    {

        public byte Detect(string token, bool isFirstTokenInSentence, bool isLastTokenInSentence)
        {
            if (StringUtilities.IsFirstLetterUpperCased(token) && !StringUtilities.IsAllUpperCased(token) &&
                !isFirstTokenInSentence)
            {
                return PartsOfSpeech.ProperNoun;
            }

            return PartsOfSpeech.Unknown;
        }
    }
}
