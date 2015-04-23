using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models.PosDetection
{
    public class TokenAndPositionInSentence
    {
        public string Token { get; set; }
        public bool? IsFirstTokenInSentence { get; set; }
        public bool? IsLastWordTokenInSentence { get; set; }

        public TokenAndPositionInSentence(string token, bool? isFirstTokenInSentence, bool? isLastWordTokenInSentence)
        {
            this.Token = token;
            this.IsFirstTokenInSentence = isFirstTokenInSentence;
            this.IsLastWordTokenInSentence = isLastWordTokenInSentence;
        }
    }
}
