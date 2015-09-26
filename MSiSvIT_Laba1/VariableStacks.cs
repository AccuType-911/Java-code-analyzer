using System.Collections.Generic;
using System.Linq;

namespace MSiSvIT_Laba1
{
    public class VariableStacks
    {
        private readonly Dictionary<string, Stack<Variable>> variableStacks;


        public IEnumerable<string> VariableNames
        {
            get {
                return this.variableStacks.Select(variable => variable.Key);
            }
        }


        public VariableStacks(IEnumerable<Variable> variables)
        {
            this.variableStacks = new Dictionary<string, Stack<Variable>>();

            if (variables == null)
                return;

            foreach (Variable variable in variables)
            {
                this.PushVariable(variable);
            }
        }


        public void PushVariable(Variable variable)
        {
            if (!this.variableStacks.ContainsKey(variable.Name))
                this.variableStacks[variable.Name] = new Stack<Variable>();
            this.variableStacks[variable.Name].Push(variable);
        }
        public void PushVariable(string variableName, VariableType variableType)
        {
            this.PushVariable(new Variable(variableName, variableType));
        }
        public void PushLocalVariable(string variableName)
        {
            this.PushVariable(new Variable(variableName, VariableType.Local));
        }

        public void PopIfExists(string variableName)
        {
            if (this.variableStacks.ContainsKey(variableName) && this.variableStacks[variableName].Count > 0)
            {
                this.variableStacks[variableName].Pop();
            }
        }

        public Variable GetTopVariable(string variableName)
        {
            if (variableName != null && this.variableStacks.ContainsKey(variableName) && this.variableStacks[variableName].Count > 0)
            {
                return this.variableStacks[variableName].Peek();
            }
            return null;
        }

        public List<Variable> GetTopVariables()
        {
            List<Variable> topVariables = new List<Variable>(this.variableStacks.Count);
            topVariables.AddRange(this.variableStacks.Select(pair => pair.Value.Peek()));
            return topVariables;
        } 
    }
}
