using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MSiSvIT_Laba1
{
    class Initializer
    {
        protected Operator[] SplittedBody;
        protected VariableStacks Variables;

        public Initializer(string code, IEnumerable<Variable> classVariables)
        {
            this.Variables = new VariableStacks(classVariables);
            code = GetBody(code);
            DeleteUnusefulForChapinWords(ref code);

            this.SplittedBody = SplitCodeOnOperators(code);
            this.Analize();
        }
        private static string GetBody(string code)
        {
            int bodyStart = code.IndexOf('{') + 1;
            int bodyLen = code.LastIndexOf('}') - bodyStart - 1;
            code = code.Substring(bodyStart, bodyLen);

            CodePartsDeleter.TrimWhiteSpaceChars(ref code);
            return code;
        }
        
        private static void DeleteUnusefulForChapinWords(ref string code)
        {
            string defaultPattern = JavaReservedWordsAndOperators.GetWordPatternFromWord("default") + "[^:]*:";
            code = CodePartsDeleter.DeleteAccordingPattern(defaultPattern, code);

            string newPattern = JavaReservedWordsAndOperators.GetWordPatternFromWord("new");
            code = CodePartsDeleter.DeleteAccordingPattern(newPattern, code);

            string doPattern = JavaReservedWordsAndOperators.GetWordPatternFromWord("do");
            code = CodePartsDeleter.DeleteAccordingPattern(doPattern, code);

            string thenPattern = JavaReservedWordsAndOperators.GetWordPatternFromWord("then");
            code = CodePartsDeleter.DeleteAccordingPattern(thenPattern, code);

            string elsePattern = JavaReservedWordsAndOperators.GetWordPatternFromWord("else");
            code = CodePartsDeleter.DeleteAccordingPattern(elsePattern, code);

            const string closeBracketPattern = @"}";
            code = CodePartsDeleter.DeleteAccordingPattern(closeBracketPattern, code);
        }


        #region SplitionOfTheCodePart
        private static Operator[] SplitCodeOnOperators(string code)
        {
            List<Operator> splittedCode = SplitOnExpressions(code);
            SelectExpressionsAndNonExpressions(ref splittedCode);

            return splittedCode.ToArray();
        }
        private static List<Operator> SplitOnExpressions(string code)
        {
            const string separatorPattern = @"[\s\r\n\t]*(;|{)[\s\r\n\t]*";
            Regex separatorRegex = new Regex(separatorPattern);

            string[] splittedCode = separatorRegex.Split(code);
            return GetOperatorsFromSplittedCode(splittedCode);
        }
        private static List<Operator> GetOperatorsFromSplittedCode(string[] splittedCode)
        {
            List<Operator> operators = new List<Operator>();
            int lastOperatorInd = splittedCode.Length - 2;

            for (int operatorInd = 0; operatorInd <= lastOperatorInd; operatorInd += 2)
            {
                Operator newOperator = new Operator(splittedCode[operatorInd]);
                operators.Add(newOperator);
            }
            return operators;
        }
        private static void SelectExpressionsAndNonExpressions(ref List<Operator> splittedCode)
        {
            SelectFors(ref splittedCode);
            SelectOperatorsWithBracketsDespiteFor(ref splittedCode);
            SelectCases(ref splittedCode);
        }


        #region ForUnionAndRightSplittion;
        private static void SelectFors(ref List<Operator> splittedCode)
        {
            const string forWord = "for";

            for (int operatorInd = 0; operatorInd < splittedCode.Count; operatorInd++)
            {
                string codePart = splittedCode[operatorInd].OperatorCode;
                int indWhereWordForFound = ReturnIndWhereWordFounded(codePart, forWord);

                if (indWhereWordForFound != codePart.Length)
                {
                    UnitSplittedForAt(ref splittedCode, operatorInd);
                    codePart = splittedCode[operatorInd].OperatorCode;

                    string thePartAfterFor = CutNonBracketsOperatorPart(ref codePart, indWhereWordForFound);
                    splittedCode[operatorInd].OperatorCode = codePart;

                    splittedCode[operatorInd].Type = OperatorTypes.For;

                    InsertNewOperatorInSplittedCode(operatorInd + 1, thePartAfterFor, ref splittedCode);
                }
            }
        }
        private static void UnitSplittedForAt(ref List<Operator> splittedCode, int forInd)
        {
            const int ammountOfPartsOfSplittedFor = 3;
            splittedCode[forInd].OperatorCode += ';' + splittedCode[forInd + 1].OperatorCode +
                                                 ';' + splittedCode[forInd + 2].OperatorCode;
            splittedCode.RemoveRange(forInd + 1, ammountOfPartsOfSplittedFor - 1);
        }
        #endregion
        
        #region CaseSplittion
        private static void SelectCases(ref List<Operator> splittedCode)
        {
            const string caseWord = "case";

            for (int operatorInd = 0; operatorInd < splittedCode.Count; operatorInd++)
            {
                string codePart = splittedCode[operatorInd].OperatorCode;

                if (ReturnIndWhereWordFounded(codePart, caseWord) != codePart.Length)
                {
                    string notCasePart = CutNonCasePart(ref codePart);
                    splittedCode[operatorInd].OperatorCode = codePart;

                    splittedCode[operatorInd].Type = OperatorTypes.Case;

                    InsertNewOperatorInSplittedCode(operatorInd + 1, notCasePart, ref splittedCode);
                }
            }
        }
        private static string CutNonCasePart(ref string codePart)
        {
            int posWherCasePartEnds = codePart.IndexOf(':');
            string nonCasePart = codePart.Substring(posWherCasePartEnds + 1);
            codePart = codePart.Remove(posWherCasePartEnds + 1);
            return nonCasePart;
        }
        #endregion
        
        #region SplittionOfOthersOperators
        private static void SelectOperatorsWithBracketsDespiteFor(ref List<Operator> splittedCode)
        {
            for (int operatorInd = 0; operatorInd < splittedCode.Count; operatorInd++)
            {
                string codePart = splittedCode[operatorInd].OperatorCode;

                Match operatorMatch = ReturnOperatorMatch(codePart);
                if (operatorMatch.Success)
                {
                    string notPartOfOperatorWithBrackets = CutNonBracketsOperatorPart(ref codePart, operatorMatch.Index);
                    if (notPartOfOperatorWithBrackets == null)
                        return;
                    splittedCode[operatorInd].OperatorCode = codePart;

                    string operatorTypeName = operatorMatch.ToString();
                    splittedCode[operatorInd].Type = OperatorTypes.GetOperatorType(operatorTypeName);

                    InsertNewOperatorInSplittedCode(operatorInd + 1, notPartOfOperatorWithBrackets, ref splittedCode);
                }
            }
        }
        private static Match ReturnOperatorMatch(string codePart)
        {
            const string operatorsWithBracketsPattern = @"(if)|(while)|(switch)|(foreach)";
            string operatorsWithBracketsOnStartPattern = "^" + JavaReservedWordsAndOperators.
                GetWordPatternFromWord(operatorsWithBracketsPattern);
            Regex startOperatorsWithBracketsRegex = new Regex(operatorsWithBracketsOnStartPattern,
                RegexOptions.Singleline);

            return startOperatorsWithBracketsRegex.Match(codePart);
        }
        #endregion

        #region MethodsThatAreUsedInSeveralBlocks
        private static string CutNonBracketsOperatorPart(ref string codePart, int startFrom)
        {
            BracketExpressionFinder finder = new BracketExpressionFinder(codePart);
            finder.SetBrackets('(', ')');

            int operatorLastCloseBracket = finder.FindMatchingBracket(startFrom);

            if (WholeOperatorIsBracketOperator(codePart.Length, operatorLastCloseBracket))
                return null;
            else
            {
                string notIfPart = codePart.Substring(operatorLastCloseBracket + 1);
                codePart = codePart.Remove(operatorLastCloseBracket + 1);
                return notIfPart;
            }
        }

        private static bool WholeOperatorIsBracketOperator(int operatorLength, int indOfOperatorCloseBracket)
        {
            return operatorLength - 1 == indOfOperatorCloseBracket;
        }

        private static void InsertNewOperatorInSplittedCode(int place, string codePart, ref List<Operator> splittedCode)
        {
            CodePartsDeleter.TrimWhiteSpaceChars(ref codePart);
            Operator newOperator = new Operator(codePart);
            splittedCode.Insert(place, newOperator);
        }


        private static int ReturnIndWhereWordFounded(string codePart, string word)
        {
            string wordPattern = JavaReservedWordsAndOperators.GetWordPatternFromWord(word);
            Regex regex = new Regex(wordPattern, RegexOptions.Singleline);

            Match match = regex.Match(codePart);
            if (match.Length > 0)
                return match.Index;
            else
                return codePart.Length;
        }
        #endregion
        #endregion


        #region AnalizationOfTheSplittedCode
        private void Analize()
        {
            if (this.SplittedBody.Length > 0)
                this.AnalizeOperator(0);
        }

        private void AnalizeOperator(int operatorInd)
        {
            Operator curOperator = this.SplittedBody[operatorInd];
                        
            Variable announcedInOperatorVariable = curOperator.GetVariableAnnouncedInOperator();
            if (announcedInOperatorVariable != null)
                Variables.PushVariable(announcedInOperatorVariable);

            if (operatorInd + 1 < this.SplittedBody.Length)
                this.AnalizeOperator(operatorInd + 1);

            curOperator.ProcessOperatorAccordingItsType(ref this.Variables);

            if (announcedInOperatorVariable != null)
                this.Variables.PopIfExists(announcedInOperatorVariable.Name);
        }
        #endregion
    }
}
