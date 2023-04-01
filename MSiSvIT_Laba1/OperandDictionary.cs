using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JavaCodeAnalyzer
{
    class OperandDictionary
    {
        public int UniqueDictionaryLen
        {
            get { return operandDictionary.Count; } 
        }

        public int DictionaryLen
        {
            get { return this.operandDictionary.Sum(i => i.Value); }
        }

        private readonly Dictionary<string, int> operandDictionary = new Dictionary<string, int>();
        private readonly string code;

        public OperandDictionary(string code)
        {
            this.code = code;

            ProcessConstantsAsOperands();

            this.code = CodePartsDeleter.DeleteConstStrings(this.code);
            this.code = CodePartsDeleter.DeleteConstChars(this.code);

            ProcessVariablesAsOperands();
            ProcessFunctionsAsOperands();
        }

        private void ProcessVariablesAsOperands()
        {
            foreach (string variableName in Variable.AllVariableNames)
            {
                string variablePattern = @"(?<![\w_\.])(?<=(this\.[\s\r\n\t]*)?)" + variableName + @"(?![\s\r\n\t]*[\w_\(])";
                int timesVariableUsedIn = CodePartsFinder.TimesPatternIsFoundedInText(this.code, variablePattern);
                if (timesVariableUsedIn > 0)
                    operandDictionary.Add(variableName, timesVariableUsedIn);
            }
        }

        private void ProcessFunctionsAsOperands()
        {
            const string methodCallNotVoidTypePattern =
                @"(?<=[^\s\r\n\t\w;.{})]([\s\r\n\t]*\([\w]+\))?[\s\r\n\t]*(new[\s\r\n\t]+)?)([\w_\[\],]+\.)*[\w_]+(?=[\s\r\n\t]*\()";
            Regex methodCallNotVoidTypeRegex = new Regex(methodCallNotVoidTypePattern, RegexOptions.RightToLeft);

            Match methodNameMatch = methodCallNotVoidTypeRegex.Match(this.code);
            while (methodNameMatch.Success)
            {
                this.AddOperandToOperandDictionary(methodNameMatch.ToString());
                methodNameMatch = methodNameMatch.NextMatch();
            }
        }

        private void ProcessConstantsAsOperands()
        {
            ProcessConstStringsAndCharsAsOperands();
            this.ProcessTrueFalseNullsAsOperands();
            ProcessConstDigitsAsOperands();
        }
        private void ProcessConstStringsAndCharsAsOperands()
        {
            const string constStringOrCharPattern = "(\"(([\\\\]\")|[^\"])*\")|('.')";
            Regex constStringOrCharRegex = new Regex(constStringOrCharPattern);

            Match match = constStringOrCharRegex.Match(this.code);
            while (match.Success)
            {
                this.AddOperandToOperandDictionary(match.ToString());
                match = match.NextMatch();
            }
        }
        private void ProcessTrueFalseNullsAsOperands()
        {
            const string trueWord = "true";
            this.ProcessOperandIdentifier(trueWord);

            const string falseWord = "false";
            this.ProcessOperandIdentifier(falseWord);

            const string nullWord = "null";
            this.ProcessOperandIdentifier(nullWord);
        }
        private void ProcessOperandIdentifier(string identifier)
        {
            string identifierPattern = JavaReservedWordsAndOperators.GetWordPatternFromWord(identifier);
            int timesIdentifierUsedIn = CodePartsFinder.TimesPatternIsFoundedInText(this.code, identifierPattern);
            if (timesIdentifierUsedIn > 0)
                operandDictionary.Add(identifier, timesIdentifierUsedIn);
        }
        private void ProcessConstDigitsAsOperands()
        {
            // To exclude constDigits occured in constStrings and constChars like "Something 1 other 2..."
            string codeWithoutConstStringsAndChars = CodePartsDeleter.DeleteConstStrings(this.code);
            codeWithoutConstStringsAndChars = CodePartsDeleter.DeleteConstChars(codeWithoutConstStringsAndChars);

            // Also exclude numbers in identificators of methods and variables
            const string constDigitPattern = @"[^A-Za-z|_][0-9]\w*(\.\w+)?((e|E)(\+|-)\w+)?";
            Regex constDigitRegex = new Regex(constDigitPattern);
            Match constDigitMatch = constDigitRegex.Match(codeWithoutConstStringsAndChars);

            while (constDigitMatch.Success)
            {
                string constDigit = constDigitMatch.ToString();
                // Extract only number
                Regex onlyNumbersPattern = new Regex(@"[0-9]\w*(\.\w+)?((e|E)(\+|-)\w+)?|-[0-9]\w*(\.\w+)?((e|E)(\+|-)\w+)?");
                constDigit = onlyNumbersPattern.Match(constDigit).Value;
                this.AddOperandToOperandDictionary(constDigit);
                constDigitMatch = constDigitMatch.NextMatch();
            }
        }


        private void AddOperandToOperandDictionary(string operand)
        {
            if (!operandDictionary.ContainsKey(operand))
                operandDictionary.Add(operand, 1);
            else
                operandDictionary[operand]++;
        }
    }
}
