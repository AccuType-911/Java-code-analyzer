using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSiSvIT_Laba1
{
    class ChapinResult
    {
        private string javaCode;
        private List<ClassChapinData> classes = new List<ClassChapinData>();
        private List<string> classesCodes;

        public ChapinResult(string javaCode)
        {
            this.javaCode = javaCode;
            Analize();
        }
        public void Analize()
        {
            CutAllClassesFromModul();
        }
        private void CutAllClassesFromModul()
        {
            CutClassesFromModul();
            CutClassesFromClasses();
        }

        private void CutClassesFromModul()
        {
            classesCodes = CutInnerClasses(ref javaCode);
            AddNewClassesFromClassesCodes();
        }
        private void CutClassesFromClasses()
        {
            for (int i = 0; i < classes.Count; i++)
            {
                string currentClassCode = classes[i].classCode;
                classesCodes = CutInnerClasses(ref currentClassCode);
                classes[i].classCode = currentClassCode;
                AddNewClassesFromClassesCodes();
            }
        }

        private void AddNewClassesFromClassesCodes()
        {
            foreach (string classCode in classesCodes)
            {
                classes.Add(new ClassChapinData(classCode));
            }
            classesCodes.Clear();
        }
       
        private static List<string> CutInnerClasses(ref string code)
        {
            List<string> cuttedClasses = new List<string>();
            string localClassPattern = @"(([\s\w]*\s)?(class)[^{]{2,})";
            string anonimClassPattern = @"([^\w](new)\s+[^{;]+{)";
            string classPattern = localClassPattern + '|' + anonimClassPattern;
            Regex classRegex = new Regex(classPattern, RegexOptions.RightToLeft);

            Match match = classRegex.Match(code);
            while (match.Success)
            {
                ScobeLimits localClassLimits = FindClassLimits(match, code);
                cuttedClasses.Add(GetClassCode(localClassLimits, code));
                DeleteClassFromCode(localClassLimits, ref code);
                
                match = match.NextMatch();
            }
            return cuttedClasses;
        }

        private static ScobeLimits FindClassLimits(Match classMatch, string code)
        {
            ScobeLimits classScobeLimits = ScobeExpressionFinder.
                FindLimitsOfNextBracketExpression(classMatch.Index, code);

            // Класс ScobeLimits использован для хранения границ всего внутреннего класса
            ScobeLimits classLimits = new ScobeLimits();
            classLimits.OpenScobeIndex = FindClassStartAccordingType(classMatch, code);
            classLimits.CloseScobeIndex = classScobeLimits.CloseScobeIndex;

            return classLimits;
        }
        private static int FindClassStartAccordingType(Match classMatch, string code)
        {
            if (IsAnonimus(classMatch))
                return classMatch.Index + classMatch.Length - 1;
            else
                return classMatch.Index;
        }
        private static bool IsAnonimus(Match match)
        {
            char lastSymbolInMatch = match.ToString()[match.Length - 1];
            return lastSymbolInMatch == '{';
        }

        private static string GetClassCode(ScobeLimits classLimits, string code)
        {                
            return getSubStrAccordingLimits(classLimits, code);
        }
        private static string getSubStrAccordingLimits(ScobeLimits limits, string code)
        {
            int subStrLen = limits.CloseScobeIndex - limits.OpenScobeIndex + 1;
            return code.Substring(limits.OpenScobeIndex, subStrLen);
        }

        private static void DeleteClassFromCode(ScobeLimits classLimits, ref string code)
        {
            code = CodePartsDeleter.DeleteCodePart(classLimits, code);
        }
    }
}
