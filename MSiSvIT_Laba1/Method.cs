using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MSiSvIT_Laba1
{
    class Method : Initializer
    {
        public Method(string code, List<Variable> classVariables) : base(code, AddMethodParameters(classVariables, code))
        { }
        private static IEnumerable<Variable> AddMethodParameters(List<Variable> classVariables, string methodCode)
        {
            List<Variable> classAndMethodVariables = new List<Variable>(classVariables);

            string methodHeader = GetMethodHeaderFromMethodCode(methodCode);
            const string variableNameOfMethodParametуrPattern = @"[\w_]+(?=(\s\r\n\t)*[,)])";
            Regex variableNameOfMethodParametуrRegex = new Regex(variableNameOfMethodParametуrPattern);

            Match match = variableNameOfMethodParametуrRegex.Match(methodHeader);
            while (match.Success)
            {
                classAndMethodVariables.Add(GetVariableFromMethodParametуrMatch(match));

                match = match.NextMatch();
            }

            return classAndMethodVariables;
        }
        private static string GetMethodHeaderFromMethodCode(string methodCode)
        {
            int indWhereBodyStarts = methodCode.IndexOf('{');
            return methodCode.Substring(0, indWhereBodyStarts);
        }
        private static Variable GetVariableFromMethodParametуrMatch(Match match)
        {
            return new Variable(match.ToString(), VariableType.Local);
        }
    }
}