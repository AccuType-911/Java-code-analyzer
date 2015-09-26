using System.Collections.Generic;
using System.Linq;

namespace MSiSvIT_Laba1
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
            int timesPatternIsInCode = CodePartsFinder.TimesPatternIsFoundedInText(code, pattern);

            if (timesPatternIsInCode > 0)
                operatorDictionary.Add(pattern, timesPatternIsInCode);
        }
    }
}
