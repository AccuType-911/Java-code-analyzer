using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace JavaCodeAnalyzer
{
    [Flags]
    enum ExpressionAnalizationMode
    {
        IsUsedForComputingMode = 1,
        IsContolMode = 2
    }

    static class ExpressionVariablesAnalizer
    {
        private static string expressionForAnalization;
        private static VariableStacks avaliableVariables;
        private static ExpressionAnalizationMode curMode;


        public static void AnalizeVariablesInExpression(string expression, VariableStacks variables, ExpressionAnalizationMode mode)
        {
            expressionForAnalization = expression;
            avaliableVariables = variables;
            curMode = mode;

            ProcessVariables();
        }

        private static void ProcessVariables()
        {
            ProcessVariablesInExpressionAccordingMode(expressionForAnalization, curMode);
            ProcessIncsAndDecs();
            ProcessTernaryOperators();
        }

        private static void ProcessIncsAndDecs()
        {
            foreach (string variableName in avaliableVariables.VariableNames)
            {
                SetVariableModefiedIfItIsIncedOrDeced(variableName);
            }
        }
        private static void SetVariableModefiedIfItIsIncedOrDeced(string variableName)
        {
            string firstVariableThenOperation = "(" + GetVariablePattern(variableName) + @"(?=[\s\t\r\n]*((\+\+)|(--)))" +
                                                ")";
            string firstOperationThenVariable = "(" + @"(?<=[^+-](((\+\+)|(--))[\s\r\n\t]*)+)" +
                                                GetVariablePattern(variableName) + ")";
            string incedOrDecedVariablePattern = firstVariableThenOperation + "|" + firstOperationThenVariable;
                
            Regex incedOrDecedVariableRegex = new Regex(incedOrDecedVariablePattern);

            if (incedOrDecedVariableRegex.IsMatch(expressionForAnalization))
                SetVariableModefied(variableName);
        }
        private static void SetVariableModefied(string variableName)
        {
            Variable curVariable = avaliableVariables.GetTopVariable(variableName);
            if (curVariable != null)
                curVariable.SetModefied();
        }

        private static void ProcessTernaryOperators()
        {
            const ExpressionAnalizationMode modeForTernaryOperators = ExpressionAnalizationMode.IsContolMode;
            const char signOfEndOfConditionPart = '?';
            
            int indOfEndOfConditionPart = expressionForAnalization.IndexOf(signOfEndOfConditionPart);
            const int returnIndIfThereIsNoIndexOf = -1;
            while (indOfEndOfConditionPart != returnIndIfThereIsNoIndexOf)
            {
                string conditionPart = FindComplexOperandBefore(indOfEndOfConditionPart);
                ProcessVariablesInExpressionAccordingMode(conditionPart, modeForTernaryOperators);

                indOfEndOfConditionPart = expressionForAnalization.IndexOf(signOfEndOfConditionPart,
                    indOfEndOfConditionPart + 1);
            }
        }
        private static string FindComplexOperandBefore(int complexOperandEndInd)
        {
            int complexOperandStartInd = FindComplexOperandStartIndBefore(complexOperandEndInd);
            int complexOperandLen = complexOperandEndInd - complexOperandStartInd + 1;
            return expressionForAnalization.Substring(complexOperandStartInd, complexOperandLen);
        }
        private static int FindComplexOperandStartIndBefore(int complexOperandEndInd)
        {
            const char openScobe = '(';
            const char closeScobe = ')';
            int amountOfNonmatchedScobes = 0;
            int startInd;

            for (startInd = complexOperandEndInd;
                amountOfNonmatchedScobes != 0 || !IsOperationSymbol(expressionForAnalization[startInd]);
                startInd--)
            {
                if (expressionForAnalization[startInd] == openScobe)
                    amountOfNonmatchedScobes--;
                if (expressionForAnalization[startInd] == closeScobe)
                    amountOfNonmatchedScobes++;
            }
            return startInd + 1;
        }
        private static bool IsOperationSymbol(char symbol)
        {
            return JavaReservedWordsAndOperators.OperationSymbols.Contains(symbol);
        }

        private static void ProcessVariablesInExpressionAccordingMode(string expression, ExpressionAnalizationMode mode)
        {
            foreach (string variableName in avaliableVariables.VariableNames)
            {
                ProcessVariableInExpression(expression, mode, variableName);
            }
        }
        private static void ProcessVariableInExpression(string expression, ExpressionAnalizationMode mode, string variableName)
        {
            string variableInExpressionPattern = GetVariablePattern(variableName);
            Regex variableInExpressionRegex = new Regex(variableInExpressionPattern);

            if (variableInExpressionRegex.IsMatch(expression))
            {
                Variable curVariable = avaliableVariables.GetTopVariable(variableName);
                if(curVariable != null)
                    ProcessVariableAccordingMode(curVariable, mode);
            }
        }
        private static void ProcessVariableAccordingMode(Variable curVariable, ExpressionAnalizationMode mode)
        {
            if (mode.HasFlag(ExpressionAnalizationMode.IsUsedForComputingMode))
                curVariable.SetUsedForComputing();
            if (mode.HasFlag(ExpressionAnalizationMode.IsContolMode))
                curVariable.SetUsedAsControl();
        }


        private static string GetVariablePattern(string variableName)
        {
            return @"(?<![\w_\.])(?<=(this\.)?)" + variableName + @"(?![\w_])";
        }
    }
}
