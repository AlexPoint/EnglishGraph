using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class Pronunciations
    {
        private readonly static List<string> VowelSounds = new List<string>()
        {
            "\"A\"","/y/","\"Y\"","/&/","/(@)/","/A/","/eI/","/@/","/-/","/E/","/i/","/I/","/aI/","/Oi/","/A/","/AU/","/O/","/oU/","/u/","/U/","/@/","/@r/","/j/", 
            "/[@]/"
        };

        private const char StressCharacter = '\'';

        public static bool IsStressOnLastVowel(string pronunciation)
        {
            if (string.IsNullOrEmpty(pronunciation)) { return false; }

            var lastStressIndex = pronunciation.LastIndexOf(StressCharacter);
            if (lastStressIndex >= 0)
            {
                var substring = pronunciation.Substring(lastStressIndex);
                var nbOfVowelSoundsAfterStress = substring.Split(VowelSounds.ToArray(), StringSplitOptions.None).Length - 1;
                if (nbOfVowelSoundsAfterStress == 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
