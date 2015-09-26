using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSiSvIT_Laba1
{
    class Class
    {
        public string Name { get; set; }
        public string ClassBody { get; set; }
        public List<Initializer> InitializersAndMethods { get; private set; }
        public List<Variable> ClassVariables { get; private set; }

        private List<string> initializersAndMethodsCodes;



        public Class(string classCode, List<Variable> parrentVariables)
        {
            this.GetClassNameAndClassBody(classCode);
            this.ClassVariables = parrentVariables;
            this.ClassBody = CodePartsDeleter.DeleteEnumsAndInterfaces(this.ClassBody);
            this.AnalizeClass();
        }

        private void GetClassNameAndClassBody(string classCode)
        {
            this.ClassBody = classCode;
            this.Name = this.GetClassNameFromCode();
            this.ClassBody = this.GetClassBody();
        }

        private string GetClassNameFromCode()
        {
            const string nameForAnonim = "Anonim";
            Regex classAndSpaces = new Regex(@"class\s+");
            Match match = classAndSpaces.Match(this.ClassBody);

            if (IsAnonimClass(match))
                return nameForAnonim;
            else
            {
                int classNameBeginInd = match.Index + match.Length;
                int classNameLen = this.GetNextIdentifierLen(classNameBeginInd);
                return this.ClassBody.Substring(classNameBeginInd, classNameLen);
            }
        }

        private static bool IsAnonimClass(Match match)
        {
            return !match.Success;
        }

        private int GetNextIdentifierLen(int classNameBeginInd)
        {
            int lastSymbolPos = classNameBeginInd;
            while (CodePartsFinder.IsIdentifierSymbol(this.ClassBody[lastSymbolPos]))
                lastSymbolPos++;
            return lastSymbolPos - classNameBeginInd;
        }

        private string GetClassBody()
        {
            int classBodyBegin = this.FindFirstBracket();
            int classBodyEnd = this.FindLastBracket();
            return this.ClassBody.Substring(classBodyBegin + 1, classBodyEnd - classBodyBegin - 1);
        }
        private int FindFirstBracket()
        {
            return this.ClassBody.IndexOf('{');
        }
        private int FindLastBracket()
        {
            int i = this.ClassBody.Length - 1;
            while (this.ClassBody[i] != '}')
                i--;
            return i;
        }


        private void AnalizeClass()
        {
            string classBodyCopy = this.ClassBody;

            this.CutMethodsAndInitializers(ref classBodyCopy);
            this.CutVariables(ref classBodyCopy);

            this.ProcessInitializersAndMethodsCodes();
        }
        private void CutMethodsAndInitializers(ref string classBody)
        {
            this.initializersAndMethodsCodes = CodePartsFinder.CutMethodsFromClassBody(ref classBody);
        }


        private void CutVariables(ref string classCodeCopy)
        {
            const string variablePattern = @"[^;]+;";
            Regex variableRegex = new Regex(variablePattern);

            Match match = variableRegex.Match(classCodeCopy);
            while (match.Success)
            {
                this.AddNewClassVariable(match.ToString());
                match = match.NextMatch();
            }
        }
        private void AddNewClassVariable(string variableAnnouncement)
        {
            string variableName = GetVariableName(variableAnnouncement);
            this.ClassVariables.Add(new Variable(variableName, VariableType.Field));
        }

        private static string GetVariableName(string variableAnnouncement)
        {
            int indWhereVariableNameFinish = FindPlaceAfterVariableNameInAnnouncement(variableAnnouncement);

            return CodePartsFinder.FindWordBefore(variableAnnouncement, indWhereVariableNameFinish);
        }
        private static int FindPlaceAfterVariableNameInAnnouncement(string variableAnnouncement)
        {
            if (VaribleAnnoncementContainsAssignment(variableAnnouncement))
                return variableAnnouncement.IndexOf("=", StringComparison.Ordinal) - 1;
            else
                return variableAnnouncement.Length - 1;
        }
        private static bool VaribleAnnoncementContainsAssignment(string variacleAnnouncement)
        {
            return variacleAnnouncement.Contains('=');
        }

        
        private void ProcessInitializersAndMethodsCodes()
        {
            this.InitializersAndMethods = new List<Initializer>(this.initializersAndMethodsCodes.Count);

            foreach (string initializerOrMethodCode in this.initializersAndMethodsCodes)
            {
                this.InitializersAndMethods.Add(IsInitializer(initializerOrMethodCode)
                    ? new Initializer(initializerOrMethodCode, this.ClassVariables)
                    : new Method(initializerOrMethodCode, this.ClassVariables));
            }
            this.initializersAndMethodsCodes.Clear();
        }
        private static bool IsInitializer(string initializerOrMethodCode)
        {
            const string initializerCharacteristic = @"^[\s\t\r\n]*(static)?[\s\t\r\n]*{";
            Regex initializerCharacteristicRegex = new Regex(initializerCharacteristic);
            return initializerCharacteristicRegex.IsMatch(initializerOrMethodCode);
        }


        public static void DeleteEverythingBeforeClassName(ref string classCode)
        {
            const string beforeClassNamePattern = @"class[^\w_]+";
            Regex beforeClassNameRegex = new Regex(beforeClassNamePattern);

            Match match = beforeClassNameRegex.Match(classCode);
            classCode = classCode.Substring(match.Index + match.Length);
        }
    }
}
