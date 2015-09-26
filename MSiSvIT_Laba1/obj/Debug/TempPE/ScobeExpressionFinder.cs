using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MSiSvIT_Laba1
{
    public class ScobeLimits
    {
        public int OpenScobeIndex { get; set; }
        public int CloseScobeIndex { get; set; }
    }

    class ScobeExpressionFinder
    {
        private string code;
        private char openBracket = '{';
        private char closeBracket = '}';

        public ScobeExpressionFinder(string code)
        {
            this.code = code;
        }
        public void SetBrackets(char openScobe, char closeScobe) 
        {
            this.openBracket = openScobe;
            this.closeBracket = closeScobe;
        }
        public ScobeLimits FindLimitsOfNextBracketExpression(int start)
        {
            ScobeLimits result = new ScobeLimits();
            result.OpenScobeIndex = FindFirstOpenBracket(start);
            result.CloseScobeIndex = FindMatchingScobe(result.OpenScobeIndex);
            return result;
        }
        public static ScobeLimits FindLimitsOfNextBracketExpression(int start, string code)
        {
            ScobeExpressionFinder finder = new ScobeExpressionFinder(code);
            return finder.FindLimitsOfNextBracketExpression(start);
        }
        public int FindFirstOpenBracket(int startPoint)
        {
            int codeSize = code.Length;
            for (int scobeInd = startPoint; scobeInd < codeSize; scobeInd++)
                if (code[scobeInd] == openBracket)
                    return scobeInd;
            return codeSize;
        }
        public int FindMatchingScobe(int firstScobeIndex)
        {
            
            int codeSize = code.Length;
            int amountOfNonclosedBrackets = 0;

            for (int ind = firstScobeIndex; ind < codeSize; ind++)
            {
                if (code[ind] == openBracket)
                {
                    string k = code.Substring(ind - 10, 11);
                    amountOfNonclosedBrackets++;
                    continue;
                }
                if (code[ind] == closeBracket)
                {
                    string k = code.Substring(ind-10, 11);
                    amountOfNonclosedBrackets--;
                    if (amountOfNonclosedBrackets == 0)
                        return ind;
                }
            }
            return codeSize;
        }

    }
}
