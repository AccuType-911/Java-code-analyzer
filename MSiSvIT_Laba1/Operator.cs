using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JavaCodeAnalyzer
{
    class Operator
    {
        public string OperatorCode { get; set; }
        public OperatorType Type { get; set; }

        public Operator(string operatorCode)
        {
            this.OperatorCode = operatorCode;
            this.Type = OperatorTypes.Other;
        }


        public Variable GetVariableAnnouncedInOperator()
        {
            if (this.Type == OperatorTypes.For)
            {
                const string forAnnouncePartPattern = @"(?<=\()[^;]*;";
                Regex forAnnouncePartRegex = new Regex(forAnnouncePartPattern);
                string announcePart = forAnnouncePartRegex.Match(this.OperatorCode).ToString();
                return GetVariableAnnouncedInExpression(announcePart);
            }

            if (this.Type == OperatorTypes.ForEach)
            {
                const string forEachAnnouncedVariableNamePattern = @"(?<=\([\s\r\n\t]*[\w\._]+[\s\r\n\t]+)[\w_]+;";
                Regex forEachAnnouncedVariableNameRegex = new Regex(forEachAnnouncedVariableNamePattern);
                string variableName = forEachAnnouncedVariableNameRegex.Match(this.OperatorCode).ToString();
                return new Variable(variableName, VariableType.Local);
            }

            if (this.Type == OperatorTypes.Other)
            {
                return GetVariableAnnouncedInExpression(this.OperatorCode);
            }

            return null;
        }
        private static Variable GetVariableAnnouncedInExpression(string expressionCode)
        {
            JavaReservedWordsAndOperators.DeleteUnusefulWordsInExpressionForChapin(ref expressionCode);

            const string variableAnnouncementPattern = @"(?<=^[\s\r\n\t]*[\w_\.\[\]<>]+[\s\r\n\t]+)[\w_]+";
            Regex variableAnnouncementRegex = new Regex(variableAnnouncementPattern);
            Match match = variableAnnouncementRegex.Match(expressionCode);

            return match.Success ? new Variable(match.ToString(), VariableType.Local) : null;
        }

        public void ProcessOperatorAccordingItsType(ref VariableStacks variables)
        {
            this.Type.ProcessOperator(OperatorCode, variables);
        }
    }
    

    public delegate void OperatorProcessor(string operatorCode, VariableStacks variables);

    public class OperatorTypes
    {
        #region Delegates
        #region ProcessOther
        private static readonly OperatorProcessor ProcessOther = delegate(string operatorCode, VariableStacks variables)
        {
            Match assignOperatorMatch = GetAssignmentOperatorMatch(operatorCode);
            
            if (assignOperatorMatch.Success)
            {
                ExpressionAnalizationMode mode = ProcessAssignedVariableAndReturnItsMode(operatorCode, variables);
                string expressionPart = operatorCode.Substring(assignOperatorMatch.Index);
                ExpressionVariablesAnalizer.AnalizeVariablesInExpression(expressionPart, variables, mode);
            }
            else
            {
                ExpressionAnalizationMode mode = new ExpressionAnalizationMode();
                if (OperatorCodeHasReurn(operatorCode))
                    mode = ExpressionAnalizationMode.IsUsedForComputingMode;
                if (HasMethodCall(operatorCode))
                {
                    mode = ExpressionAnalizationMode.IsUsedForComputingMode;
                    SetModefiedObjectThatCallsMethodIfExists(operatorCode, variables);
                }
                ExpressionVariablesAnalizer.AnalizeVariablesInExpression(operatorCode, variables, mode);
            }
        };
        private static void SetModefiedObjectThatCallsMethodIfExists(string operatorCode, VariableStacks variables)
        {
            string variableName = CodePartsFinder.GetPossibleVariableNameUsedForMethodCallOrWayToField(operatorCode);
            Variable variable = variables.GetTopVariable(variableName);
            if (variable != null)
                variable.SetModefied();
        }

        private static Match GetAssignmentOperatorMatch(string operatorCode)
        {
            const string assignmentPattern = @"(?<![<>!=])=(?![=])";
            Regex assignmentRegex = new Regex(assignmentPattern);
            return assignmentRegex.Match(operatorCode);
        }

        private static ExpressionAnalizationMode ProcessAssignedVariableAndReturnItsMode(string operatorCode, VariableStacks variables)
        {
            const char simpleAssignOperator = '=';
            int assignPos = operatorCode.IndexOf(simpleAssignOperator);

            string modefiedVariable =
                GetAssignedVariableNameUsingAssignOperatorPos(operatorCode,assignPos);
            Variable variableTookPartInAssignment = variables.GetTopVariable(modefiedVariable);

            if (variableTookPartInAssignment == null)
                return ExpressionAnalizationMode.IsUsedForComputingMode;
            ExpressionAnalizationMode variableMode = GetModeAccordingVariable(variableTookPartInAssignment);
            if (SimpleAssignmentOperatorExists(operatorCode))
            {
                variableTookPartInAssignment.IsCurrentlyUsedAsControl = false;
                variableTookPartInAssignment.IsCurrentlyUsedForComputing = false;
            }

            variableTookPartInAssignment.SetModefied();
            return variableMode;
        }
        private static bool SimpleAssignmentOperatorExists(string operatorCode)
        {
            const string simpleAssignmentOperatorPattern = @"(?<=[\w_\]][\s\r\n\t]*)=(?![\s\r\n\t]*=)";
            Regex simpleAssignmentOperatorRegex = new Regex(simpleAssignmentOperatorPattern);
            return simpleAssignmentOperatorRegex.IsMatch(operatorCode);
        }

        private static string GetAssignedVariableNameUsingAssignOperatorPos(string assignment, int assignOperatorPos)
        {
            string assignedOperand = CodePartsFinder.FindWordBefore(assignment, assignOperatorPos);
            return CodePartsFinder.GetPossibleVariableNameUsedForMethodCallOrWayToField(assignedOperand);
        }
        private static ExpressionAnalizationMode GetModeAccordingVariable(Variable variable)
        {
            ExpressionAnalizationMode mode = new ExpressionAnalizationMode();
            if (variable.IsCurrentlyUsedAsControl)
                mode |= ExpressionAnalizationMode.IsContolMode;
            if (variable.IsUsedForComputing)
                mode |= ExpressionAnalizationMode.IsUsedForComputingMode;
            return mode;
        }


        private static bool OperatorCodeHasReurn(string operatorCode)
        {
            string returnPattern = JavaReservedWordsAndOperators.GetWordPatternFromWord("return");
            Regex returnRegex = new Regex(returnPattern);
            return returnRegex.IsMatch(operatorCode);
        }
        private static bool HasMethodCall(string operatorCode)
        {
            return operatorCode.Contains("(");
        }
        #endregion

        #region ProcessIf
        private static readonly OperatorProcessor ProcessIf = delegate(string operatorCode, VariableStacks variables)
        {
            ProcessCodePartAsCondition(operatorCode, variables);
        };
        #endregion
        private static void ProcessCodePartAsCondition(string code, VariableStacks variables)
        {
            const ExpressionAnalizationMode modeForConditionPart = ExpressionAnalizationMode.IsContolMode;
            ExpressionVariablesAnalizer.AnalizeVariablesInExpression(code, variables, modeForConditionPart);
        }

        #region ProcessWhile
        private static readonly OperatorProcessor ProcessWhile = delegate(string operatorCode, VariableStacks variables)
        {
            ProcessCodePartAsCondition(operatorCode, variables);
        };
        #endregion

        #region ProcessSwitch
        private static readonly OperatorProcessor ProcessSwitch = delegate(string operatorCode, VariableStacks variables)
        {
            ProcessCodePartAsCondition(operatorCode, variables);
        };
        #endregion

        #region ProcessForEach
        private static readonly OperatorProcessor ProcessForEach = delegate(string operatorCode, VariableStacks variables)
        {
            ProcessCodePartAsCondition(operatorCode, variables);
        };
        #endregion

        #region ProcessFor
        private static readonly OperatorProcessor ProcessFor = delegate(string operatorCode, VariableStacks variables)
        {
            string conditionForPatrt = GetConditionForPart(operatorCode);
            const ExpressionAnalizationMode conditionMode = ExpressionAnalizationMode.IsContolMode;
            ExpressionVariablesAnalizer.AnalizeVariablesInExpression(conditionForPatrt, variables, conditionMode);

            string modificationForPart = GetModificationForPart(operatorCode);
            ProcessOther(modificationForPart, variables);

            string initializationForPart = GetInitializationForPart(operatorCode);
            ProcessOther(initializationForPart, variables);
        };
        private static string GetConditionForPart(string forCode)
        {
            const string conditionForPartPattern = @"(?<=;)[^;]*(?=;)";
            Regex conditionForRegex = new Regex(conditionForPartPattern);
            return conditionForRegex.Match(forCode).ToString();
        }
        private static string GetModificationForPart(string forCode)
        {
            const string modificationForPartPattern = @"(?<=;)[^;]*(?=\))";
            Regex modificationForRegex = new Regex(modificationForPartPattern);
            return modificationForRegex.Match(forCode).ToString();
        }
        private static string GetInitializationForPart(string forCode)
        {
            const string initializationForPartPattern = @"(?<=\()[^;]*(?=;)";
            Regex initializationForRegex = new Regex(initializationForPartPattern);
            return initializationForRegex.Match(forCode).ToString();
        }
        #endregion

        #region ProcessCase
        private static readonly OperatorProcessor ProcessCase = delegate(string operatorCode, VariableStacks variables)
        {
            ProcessCodePartAsCondition(operatorCode, variables);
        };
        #endregion
        #endregion

        public static readonly OperatorType Other = new OperatorType("Other", ProcessOther);
        public static readonly OperatorType If = new OperatorType("if", ProcessIf);
        public static readonly OperatorType While = new OperatorType("while", ProcessWhile);
        public static readonly OperatorType Switch = new OperatorType("switch", ProcessSwitch);
        public static readonly OperatorType ForEach = new OperatorType("foreach", ProcessForEach);
        public static readonly OperatorType For = new OperatorType("for", ProcessFor);
        public static readonly OperatorType Case = new OperatorType("case", ProcessCase);

        public static IEnumerable<OperatorType> Values
        {
            get
            {
                yield return Other;
                yield return If;
                yield return While;
                yield return Switch;
                yield return ForEach;
                yield return For;
                yield return Case;
            }
        }


        public static OperatorType GetOperatorType(string operatorTypeName)
        {
            foreach (OperatorType operatorType in Values.Where(operatorType => operatorType.Name == operatorTypeName))
            {
                return operatorType;
            }
            return Other;
        }
    }



    public class OperatorType
    {
        public string Name { get; private set; }
        public OperatorProcessor ProcessOperator { get; set; }

        internal OperatorType(string name, OperatorProcessor processOperator)
        {
            this.Name = name;
            ProcessOperator = processOperator;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
