using System.Collections.Generic;
using System.Linq;

namespace JavaCodeAnalyzer
{
    internal struct MethodMcCabeNumber
    {
        public int McCabeNumber;
        public string Name;
    }

    class McCabeAnalizer
    {
        private static readonly List<string> KeyWordAndOperatorsPatternsThatIncsMcCabeNumber = new List<string>
        {
            JavaReservedWordsAndOperators.GetWordPatternFromWord("while"),
            JavaReservedWordsAndOperators.GetWordPatternFromWord("for"),
            JavaReservedWordsAndOperators.GetWordPatternFromWord("foreach"),
            JavaReservedWordsAndOperators.GetWordPatternFromWord("if"),
            JavaReservedWordsAndOperators.GetWordPatternFromWord("case"),
            @"\?"
        };
        private const int mcCabeStartNumber = 1;
        public static List<MethodMcCabeNumber> MethodMcCabeNumbers { get; private set; }
        
        
        public static string GetMcCabeReport(string codeForMetric)
        {
            codeForMetric = MakeCommentsAndConstStringEmpty(codeForMetric);
            Dictionary<string, string> methodsAndTheirNames =
                CodePartsFinder.GetAllMethodsBodiesAndNames(codeForMetric);
            
            MethodMcCabeNumbers = methodsAndTheirNames.Select(methodAndItsName => new MethodMcCabeNumber
            {
                McCabeNumber = FindMcCabeNumberForMethod(methodAndItsName.Key),
                Name = methodAndItsName.Value
            }).ToList();  

            return FormReport();
        }


        private static string MakeCommentsAndConstStringEmpty(string code)
        {
            return CodePartsDeleter.DeleteNonNextLineCharsInConstStringsAndComments(code);
        }
        private static int FindMcCabeNumberForMethod(string codeForMetric)
        {
            string keyWordsAndOperatorsThatIncMcCabeNumberPattern =
                    GetOnePatternFromPatternList(KeyWordAndOperatorsPatternsThatIncsMcCabeNumber);
            return CodePartsFinder.TimesPatternIsFoundedInText(codeForMetric,
                    keyWordsAndOperatorsThatIncMcCabeNumberPattern) + mcCabeStartNumber;
        }
        private static string GetOnePatternFromPatternList(IEnumerable<string> patternList)
        {
            string operatorGroupPattern = patternList.Aggregate("",
                (current, operatorPattern) => current + (operatorPattern + "|"));
            return operatorGroupPattern.Remove(operatorGroupPattern.Length - 1);
        }
        private static string FormReport()
        {
            return MethodMcCabeNumbers.Aggregate("Цикломатичесские числа Маккейба: \r\n",
                (current, methodMcCabeNumber) =>
                    current + (methodMcCabeNumber.Name + ": " + methodMcCabeNumber.McCabeNumber + ";\r\n"));
        }
    }
}
