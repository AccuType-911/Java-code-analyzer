using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSiSvIT_Laba1
{
    enum VariableType { Local, Field }
    class VariablesChapinData
    {
        public string Name { get; private set; }
        public bool IsUsed { get; private set; }
        public bool IsControlVariable { get; private set; }
        public bool IsInputed { get; private set; }
        public bool IsModefied { get; private set; }

        VariablesChapinData(string name, VariableType type)
        {
            this.Name = name;

            IsControlVariable = false;
            IsInputed = false;

            if (type == VariableType.Field)
            {
                IsModefied = false;
                IsUsed = true;
            }
            else
            {
                IsModefied = true;
                IsUsed = true;
            }
        }

        public void SetUsed()
        {
            IsUsed = true;
        }
        public void SetControlVariable()
        {
            IsControlVariable = true;
        }
        public void SetInputed()
        {
            IsInputed = true;
        }
        public void SetModefied()
        {
            IsModefied = true;
        }

    }
}
