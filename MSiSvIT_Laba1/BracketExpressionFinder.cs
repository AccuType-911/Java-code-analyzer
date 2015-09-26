
namespace JavaCodeAnalyzer
{
    public class BracketLimits
    {
        public int OpenBracketIndex { get; set; }
        public int CloseBracketIndex { get; set; }

        public int GetLimitsLen()
        {
            return this.CloseBracketIndex - this.OpenBracketIndex + 1;
        }
    }

    class BracketExpressionFinder
    {
        private readonly string code;
        private char openBracket = '{';
        private char closeBracket = '}';

        public BracketExpressionFinder(string code)
        {
            this.code = code;
        }
        public void SetBrackets(char openScobe, char closeScobe) 
        {
            this.openBracket = openScobe;
            this.closeBracket = closeScobe;
        }

        public static BracketLimits FindLimitsOfNextBracketExpression(int start, string code)
        {
            BracketExpressionFinder finder = new BracketExpressionFinder(code);
            return finder.FindLimitsOfNextBracketExpression(start);
        }
        public BracketLimits FindLimitsOfNextBracketExpression(int start)
        {
            BracketLimits result = new BracketLimits();
            result.OpenBracketIndex = this.FindFirstOpenBracket(start);
            result.CloseBracketIndex = this.FindMatchingBracket(result.OpenBracketIndex);
            return result;
        }
        public int FindFirstOpenBracket(int startPoint)
        {
            int codeSize = this.code.Length;
            for (int bracketInd = startPoint; bracketInd < codeSize; bracketInd++)
                if (this.code[bracketInd] == this.openBracket)
                    return bracketInd;
            return codeSize;
        }
        public int FindMatchingBracket(int firstScobeIndex)
        {
            int codeSize = this.code.Length;
            int amountOfNonclosedBrackets = 0;

            for (int ind = firstScobeIndex; ind < codeSize; ind++)
            {
                if (this.code[ind] == this.openBracket)
                {
                    amountOfNonclosedBrackets++;
                    continue;
                }
                if (this.code[ind] == this.closeBracket)
                {
                    amountOfNonclosedBrackets--;
                    if (amountOfNonclosedBrackets == 0)
                        return ind;
                }
            }
            return codeSize;
        }


        public static BracketLimits FindLimitsOfBracketExpressionBefore(int end, string code)
        {
            BracketExpressionFinder finder = new BracketExpressionFinder(code);
            return finder.FindLimitsOfBracketExpressionBefore(end);
        }
        public BracketLimits FindLimitsOfBracketExpressionBefore(int end)
        {
            BracketLimits result = new BracketLimits();
            result.CloseBracketIndex = this.FindLastCloseBracket(end);
            result.OpenBracketIndex = this.FindMatchingBracketBefore(result.CloseBracketIndex);
            
            return result;
        }
        public int FindLastCloseBracket(int endPoint)
        {
            int bracketInd;
            for (bracketInd = endPoint; bracketInd >= 0; bracketInd--)
                if (this.code[bracketInd] == this.closeBracket)
                    break;
            return bracketInd;
        }
        public int FindMatchingBracketBefore(int lastScobeIndex)
        {
            int amountOfBracketsWithoutPair = 0;
            int ind;

            for (ind = lastScobeIndex; ind >= 0; ind--)
            {
                if (this.code[ind] == this.openBracket)
                {
                    amountOfBracketsWithoutPair--;
                    if (amountOfBracketsWithoutPair == 0)
                        break;
                }
                if (this.code[ind] == this.closeBracket)
                    amountOfBracketsWithoutPair++;
            }
            return ind;
        }

    }
}
