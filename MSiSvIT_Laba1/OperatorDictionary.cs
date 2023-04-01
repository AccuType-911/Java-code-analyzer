using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JavaCodeAnalyzer
{
    class OperatorDictionary
    {
        public int UniqueDictionaryLen { get { return operatorDictionary.Count; } }
        public int DictionaryLen
        {
            get { return this.operatorDictionary.Sum(i => i.Value); }
        }

        private readonly Dictionary<string, int> operatorDictionary = new Dictionary<string, int>();
        private readonly string code;

        public OperatorDictionary(string code)
        {
            this.code = code;
            List<string> allOperatorsPatterns = JavaReservedWordsAndOperators.GetAllOperatorsPatterns();

            foreach (string operatorPattern in allOperatorsPatterns)
            {
                AddOperatorInDictionaryIfUsedInCode(operatorPattern);
            }
        }
        private void AddOperatorInDictionaryIfUsedInCode(string pattern)
        {
            Regex regex = new Regex(pattern);
            string foundedOperator = regex.Match(this.code).Value;
            int timesPatternIsInCode = regex.Matches(this.code).Count;

            if (timesPatternIsInCode > 0)
            {
                if (!operatorDictionary.ContainsKey(foundedOperator))
                    operatorDictionary.Add(foundedOperator, timesPatternIsInCode);
                else
                    operatorDictionary[foundedOperator] += timesPatternIsInCode;
            }
        }
    }
}
