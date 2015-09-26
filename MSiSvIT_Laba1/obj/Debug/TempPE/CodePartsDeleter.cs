using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSiSvIT_Laba1
{
    static class CodePartsDeleter
    {
        public static string DeleteConstStrings(string code)
        {
            const string emptyStr = "\"\"";
            string constStrPattern = "\"[^\"]*\"";
            Regex constStrRegex = new Regex(constStrPattern);
            return constStrRegex.Replace(code, emptyStr);
        }

        public static string DeleteComments(string code)
        {
            code = DeleteMultyLineComments(code);
            return DeleteSingleLineComments(code);
        }
        public static string DeleteMultyLineComments(string code)
        {
            const string none = "";
            string multyCommentPattern = @"/\*([^*])*\*+/";
            Regex multyCommentRegex = new Regex(multyCommentPattern);
            return multyCommentRegex.Replace(code, none);
        }
        public static string DeleteSingleLineComments(string code)
        {
            const string none = "";
            string singleCommentPattern = @"//.*";
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
            string overrideAnnotationPattern = @"@\s*Override";
            Regex overrideAnnotationRegex = new Regex(overrideAnnotationPattern);
            return overrideAnnotationRegex.Replace(code, none);
        }
        public static string DeleteEnumsAndInterfaces(string code)
        {
            string enumInterfacePattern = @"([\s\w]*\s)?((interface)|(enum))[^{]{2,}";
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
            ScobeLimits descriptionLimits = FindDescription(descriptionStartInd, code);
            return DeleteDescription(descriptionLimits, code);
        }
        private static ScobeLimits FindDescription(int descriptionStartInd, string code)
        {
            ScobeExpressionFinder bracketsFinder = new ScobeExpressionFinder(code);
            bracketsFinder.SetBrackets('{', '}');
            return bracketsFinder.FindLimitsOfNextBracketExpression(descriptionStartInd);
        }
        public static string DeleteDescription(ScobeLimits descriptionLimits, string code)
        {
            return DeleteCodePart(descriptionLimits, code);
        }
        public static string DeleteCodePart(ScobeLimits descriptionLimits, string code)
        {
            int descriptionLen = descriptionLimits.CloseScobeIndex - descriptionLimits.OpenScobeIndex + 1;
            return code.Remove(descriptionLimits.OpenScobeIndex, descriptionLen);
        }
    }
}
