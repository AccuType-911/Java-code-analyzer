using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSiSvIT_Laba1
{
    public static class CodePartsFinder
    {
        public static string FindWordBefore(string code, int wordFinish)
        {
            FindWordLastSymbolInd(code, ref wordFinish);

            int wordStart = wordFinish;
            FindWordFirstSymbolInd(code, ref wordStart);

            int wordLen = wordFinish - wordStart + 1;
            return code.Substring(wordStart, wordLen);
        }
        private static void FindWordLastSymbolInd(string code, ref int wordFinish)
        {
            while (!IsWordSymbol(code[wordFinish]))
                wordFinish--;
        }
        private static void FindWordFirstSymbolInd(string code, ref int wordStart)
        {
            while (wordStart > 0 && IsWordSymbol(code[wordStart - 1]))
                wordStart--;
        }
        public static bool IsWordSymbol(char symbol)
        {
            return (char.IsLetterOrDigit(symbol) || symbol == '_' || symbol == '[' || symbol == ']' || symbol == '(' ||
                    symbol == ')' || symbol == '.' || symbol == ',');
        }
        public static bool IsIdentifierSymbol(char symbol)
        {
            return (char.IsLetterOrDigit(symbol) || symbol == '_');
        }


        public static string GetPossibleVariableNameUsedForMethodCallOrWayToField(string methodCallOrWayToField)
        {
            const string variableNamePattern = @"(?<![\w_\.])(?<=(this\.)?)[\w_]+(?![\w_])";
            Regex variableNameInMethodCallOrWayToFieldRegex = new Regex(variableNamePattern);

            Match match = variableNameInMethodCallOrWayToFieldRegex.Match(methodCallOrWayToField);
            if (match.Success)
                return match.ToString();
            return null;
        }


        public static int TimesPatternIsFoundedInText(string text, string pattern)
        {
            Regex regex = new Regex(pattern);
            return regex.Matches(text).Count;
        }


        public static string FindMethod(string code, string methodName)
        {
            string methodNamePattern = @"[\w_\.]+[\s\r\n\t]+" + methodName + @"[\s\r\n\t]*\(";
            Regex methodNameRegex = new Regex(methodNamePattern);

            Match methodMatch = methodNameRegex.Match(code);
            int startInd = methodMatch.Index;

            BracketLimits bodyLimits = BracketExpressionFinder.FindLimitsOfNextBracketExpression(startInd, code);
            int methodLen = bodyLimits.CloseBracketIndex - methodMatch.Index + 1;
            return code.Substring(startInd, methodLen);
        }


        public const string LocalClassPattern = @"(([\s\w]*\s)?(class)[^{]{2,})";
        public const string AnonimClassPattern = @"((?<=[^\w](new)\s+[^{;]+){)";
        public static readonly string ClassPattern = LocalClassPattern + '|' + AnonimClassPattern;
        public static List<string> CutAllClassesWithoutInnerClasses(string code)
        {
            List<string> classesCodes = new List<string>();

            Match match = Regex.Match(code, ClassPattern, RegexOptions.RightToLeft);
            while (match.Success)
            {
                classesCodes.Add(CutNextClass(match.Index, ref code));
                match = Regex.Match(code, ClassPattern, RegexOptions.RightToLeft);
            }
            return classesCodes;
        }
        public static string CutNextClass(int startPos, ref string code)
        {
            BracketLimits localClassLimits = ReturnLimitsFromStartIndToEndOfNextDescription(startPos, code);
            string classCode = GetSubStrAccordingLimits(localClassLimits, code);
            CodePartsDeleter.TrimWhiteSpaceChars(ref classCode);
            code = CodePartsDeleter.DeleteCodePart(localClassLimits, code);
            return classCode;
        }
        public static BracketLimits ReturnLimitsFromStartIndToEndOfNextDescription(int startInd, string code)
        {
            BracketLimits limits = BracketExpressionFinder.FindLimitsOfNextBracketExpression(startInd, code);
            limits.OpenBracketIndex = startInd;
            return limits;
        }
        public static string GetSubStrAccordingLimits(BracketLimits limits, string code)
        {
            string str = code.Substring(686,20);
            int subStrLen = limits.CloseBracketIndex - limits.OpenBracketIndex + 1;
            return code.Substring(limits.OpenBracketIndex, subStrLen);
        }


        public static IEnumerable<string> GetClassesBodies(List<string> classesCodes)
        {
            const int classStartInd = 0;
            return classesCodes.Select(classCode => GetNextBody(classStartInd, classCode));
        }
        public static string GetNextBody(int startInd, string code)
        {
            BracketLimits bodyLimits = BracketExpressionFinder.FindLimitsOfNextBracketExpression(startInd, code);
            bodyLimits.OpenBracketIndex++;

            int bodyLen = bodyLimits.CloseBracketIndex - bodyLimits.OpenBracketIndex;
            return code.Substring(bodyLimits.OpenBracketIndex, bodyLen);
        }


        public static List<string> CutMethodsFromClassBody(ref string classCode)
        {
            Dictionary<string, string> methodsAndTheirNames = CutMethodsFromClassBodyAndReturnMethodsWithNames(ref classCode);
            return methodsAndTheirNames.Select(methodAndNamePair => methodAndNamePair.Key).ToList();
        }
        public static Dictionary<string, string> CutMethodsFromClassBodyAndReturnMethodsWithNames(ref string classCode)
        {
            Dictionary<string, string> methodsAndTheirName = new Dictionary<string, string>();
            const string initializerOrMethodHeaderPattern = @"(?<=[\s\r\n\t]*)[^{};]*{";

            Match match = Regex.Match(classCode, initializerOrMethodHeaderPattern);
            while (match.Success)
            {
                string methodName = GetNextMethodName(match.Index, classCode);
                string methodOrInitializerCode = CutNextMethodCode(match.Index, ref classCode);

                methodsAndTheirName.Add(methodOrInitializerCode, methodName);
                match = Regex.Match(classCode, initializerOrMethodHeaderPattern);
            }
            return methodsAndTheirName;
        }
        private static string GetNextMethodName(int index, string classCode)
        {
            const string nameOfInitializer = "Initializer";
            int nextBracketPos = classCode.IndexOf('{', index);
            int nextScobePos = classCode.IndexOf('(', index);

            if (nextBracketPos < nextScobePos)
                return nameOfInitializer;
            else
                return FindWordBefore(classCode, nextScobePos - 1);
        }
        private static string CutNextMethodCode(int index, ref string classCode)
        {
            BracketLimits methodOrInitializerLimits = ReturnLimitsFromStartIndToEndOfNextDescription(index, classCode);
            string methodOrInitializerCode = GetSubStrAccordingLimits(methodOrInitializerLimits, classCode);

            CodePartsDeleter.TrimWhiteSpaceChars(ref methodOrInitializerCode);
            classCode = CodePartsDeleter.DeleteCodePart(methodOrInitializerLimits, classCode);
            return methodOrInitializerCode;
        }


        public static List<string> GetAllMethodsBodies(string codeForMetric)
        {
            return
                GetAllMethodsBodiesAndNames(codeForMetric).Select(methodBodyAndName => methodBodyAndName.Key).ToList();
        }
        public static Dictionary<string, string> GetAllMethodsBodiesAndNames(string codeForMetric)
        {
            List<string> classesCodes = CutAllClassesWithoutInnerClasses(codeForMetric);
            IEnumerable<string> classesBodies = GetClassesBodies(classesCodes);

            return classesBodies.Select(
                classBodyCopy => CutMethodsFromClassBodyAndReturnMethodsWithNames(ref classBodyCopy))
                .SelectMany(classMethodsAndNames => classMethodsAndNames)
                .ToDictionary(classMethodAndName => classMethodAndName.Key,
                              classMethodAndName => classMethodAndName.Value);
        }
    }
}
