using System.Text.RegularExpressions;

namespace JavaCodeAnalyzer
{
    static class CodePartsDeleter
    {
        public static string DeleteConstStrings(string code)
        {
            const string emptyStr = "\"\"";
            const string constStrPattern = "\"(([\\\\]\")|[^\"])*\"";
            Regex constStrRegex = new Regex(constStrPattern);
            return constStrRegex.Replace(code, emptyStr);
        }

        public static string DeleteConstChars(string code)
        {
            const string emptyChar = "''";
            const string constCharPattern = "'.'";
            Regex constCharRegex = new Regex(constCharPattern);
            return constCharRegex.Replace(code, emptyChar);
        }

        public static string DeleteComments(string code)
        {
            code = DeleteMultyLineComments(code);
            return DeleteSingleLineComments(code);
        }
        public static string DeleteMultyLineComments(string code)
        {
            const string none = "";
            const string multyCommentPattern = @"(/\*((\*(?!/))|([^\*]))*\*/)";
            Regex multyCommentRegex = new Regex(multyCommentPattern);
            return multyCommentRegex.Replace(code, none);
        }
        public static string DeleteSingleLineComments(string code)
        {
            const string none = "";
            const string singleCommentPattern = @"//.*";
            Regex singleCommentRegex = new Regex(singleCommentPattern);
            return singleCommentRegex.Replace(code, none);
        }
        public static string DeleteAnnotations(string code)
        {
            return DeleteOverrideAnnotations(code);
        }

        public static string DeleteOverrideAnnotations(string code)
        {
            const string none = "";
            const string overrideAnnotationPattern = @"@\s*Override";
            Regex overrideAnnotationRegex = new Regex(overrideAnnotationPattern);
            return overrideAnnotationRegex.Replace(code, none);
        }
        public static string DeleteEnumsAndInterfaces(string code)
        {
            const string enumInterfacePattern = @"([\s\w]*\s)?((interface)|(enum))[^{]{2,}";
            return DeleteAllPatternsAndTheirDescriptions(enumInterfacePattern, code);
        }

        public static string DeleteAllPatternsAndTheirDescriptions(string pattern, string code)
        {
            Regex patternRegex = new Regex(pattern, RegexOptions.RightToLeft);

            Match match = patternRegex.Match(code);
            while (match.Success)
            {
                code = DeletePattern(match, code);
                code = FindAndDeleteDescription(match.Index, code);
                match = match.NextMatch();
            }
            return code;
        }
        private static string DeletePattern(Match patternMatch, string code)
        {
            return code.Remove(patternMatch.Index, patternMatch.Length);
        }
        public static string FindAndDeleteDescription(int descriptionStartInd, string code)
        {
            BracketLimits descriptionLimits =
                BracketExpressionFinder.FindLimitsOfNextBracketExpression(descriptionStartInd, code);
            return DeleteCodePart(descriptionLimits, code);
        }

        public static void TrimWhiteSpaceChars(ref string code)
        {
            const char spaceChar = ' ';
            const char carriageReturnChar = (char) 13;
            const char newLineChar = (char) 10;
            const char tabChar = (char) 9;

            char[] unusefulChars = { spaceChar, carriageReturnChar, newLineChar, tabChar };
            code = code.Trim(unusefulChars);
        }

        public static string DeleteCodePart(BracketLimits descriptionLimits, string code)
        {
            int descriptionLen = descriptionLimits.CloseBracketIndex - descriptionLimits.OpenBracketIndex + 1;
            return code.Remove(descriptionLimits.OpenBracketIndex, descriptionLen);
        }

        public static string DeleteAccordingPattern(string pattern, string code)
        {
            const string noneStr = "";
            Regex regex = new Regex(pattern);

            return regex.Replace(code, noneStr);
        }


        const string nextLinePattern = "\n";
        public static string DeleteNonNextLineCharsInConstStringsAndComments(string code)
        {
            const string constStringPattern = "(\"(([\\\\]\")|[^\"])*\")";
            const string multyCommentPattern = @"((?<=/\*)((\*(?!/))|([^\*]))*(?=\*/))";
            const string singleCommentPattern = @"((?<=//).*)";

            const string constStringOrCommentPattern = constStringPattern + "|" + multyCommentPattern + "|" +
                                                       singleCommentPattern;
            Regex constStringOrCommentRegex = new Regex(constStringOrCommentPattern);
            return constStringOrCommentRegex.Replace(code, ReturnNextLineSymbols);
        }
        private static string ReturnNextLineSymbols(Match match)
        {
            string codePart = match.ToString();

            if (IsConstStr(codePart))
            {
                const string emptyStr = "\"\"";
                return emptyStr;
            }

            int amountOfNextLineSymbols = CodePartsFinder.TimesPatternIsFoundedInText(codePart, nextLinePattern);

            string result = "";
            for (int i = 0; i < amountOfNextLineSymbols; i++)
                result += nextLinePattern;
            return result;
        }
        private static bool IsConstStr(string code)
        {
            return (code[0] == '"');
        }


        public static string DeleteEmptyStrings(string code)
        {
            const string emptyStringPattern = @"(\n[\s\r\t]*){2,}";
            Regex emptyStringRegex = new Regex(emptyStringPattern);

            return emptyStringRegex.Replace(code, nextLinePattern);
        }
    }
}
