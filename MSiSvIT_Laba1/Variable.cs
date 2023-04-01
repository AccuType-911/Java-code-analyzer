using System.Collections.Generic;

namespace JavaCodeAnalyzer
{
    public enum VariableType { Local, Field }

    public class Variable
    {
        public static SortedSet<string> AllVariableNames { get; set; } 
        public static int AmountOfVariables { get; private set; }
        public static int AmountOfUsedForComputingVariables { get; private set; }
        public static int AmountOfUsedAsControlVariables { get; private set; }
        public static int AmountOfInputtedVariables { get; private set; }
        public static int AmountOfModificatedVariables { get; private set; }
        public static int AmountOfUsedVariables { get; private set; }


        static Variable()
        {
            AmountOfVariables = 0;
            AmountOfUsedForComputingVariables = 0;
            AmountOfUsedAsControlVariables = 0;
            AmountOfInputtedVariables = 0;
            AmountOfModificatedVariables = 0;
            AmountOfUsedVariables = 0;
            AllVariableNames = new SortedSet<string>();
        }


        public string Name { get; private set; }
        public bool IsUsedForComputing { get; private set; }
        public bool IsUsedAsControl { get; private set; }
        public bool IsInputed { get; private set; }
        public bool IsModefied { get; private set; }
        public bool IsCurrentlyUsedForComputing { get; set; }
        public bool IsCurrentlyUsedAsControl { get; set; }



        public Variable(string name, VariableType type)
        {
            AmountOfVariables++;
            this.Name = name.Trim();

            this.IsUsedAsControl = false;
            this.IsInputed = false;
            this.IsUsedForComputing = false;
            if (type == VariableType.Field)
                this.IsModefied = false;
            else 
                this.SetModefied();

            if (!AllVariableNames.Contains(name))
                AllVariableNames.Add(name);
        }


        public void SetUsedForComputing()
        {
            if (this.IsUsedForComputing == false)
            {
                AmountOfUsedForComputingVariables++;
                this.IsUsedForComputing = true;

                if (this.IsUsedAsControl == false)
                    AmountOfUsedVariables++;
            }
            this.IsCurrentlyUsedForComputing = true;
        }
        public void SetUsedAsControl()
        {
            if (this.IsUsedAsControl == false)
            {
                AmountOfUsedAsControlVariables++;
                this.IsUsedAsControl = true;

                if (this.IsUsedForComputing == false)
                    AmountOfUsedVariables++;
            }
            this.IsCurrentlyUsedAsControl = true;
        }
        public void SetInputed()
        {
            if (this.IsInputed == false)
            {
                AmountOfInputtedVariables++;
                this.IsInputed = true;
            }
        }
        public void SetModefied()
        {
            if (this.IsModefied == false)
            {
                AmountOfModificatedVariables++;
                this.IsModefied = true;
            }
        }


        public const float IsInputtedСoeff = 1;
        public const float IsModifiedСoeff = 2;
        public const float IsControlСoeff = 3;
        public const float IsUnusedСoeff = 0.5f;


        public static float GetChapinResult()
        {
            return GetInputtedResult() + GetModefiedResult() + GetControlResult() + GetUnusedResult();
        }

        private static float GetInputtedResult()
        {
            return AmountOfInputtedVariables * IsInputtedСoeff;
        }
        private static float GetModefiedResult()
        {
            return AmountOfModificatedVariables * IsModifiedСoeff;
        }
        private static float GetControlResult()
        {
            return AmountOfUsedAsControlVariables * IsControlСoeff;
        }
        private static float GetUnusedResult()
        {
            return (AmountOfVariables - AmountOfUsedVariables) * IsUnusedСoeff;
        }

        public static string GetChapinReport()
        {
            string result = 
                @"Total amount of variables: " + AmountOfVariables + "\r\n" +
                @"Amount of Inputted variables: " + AmountOfInputtedVariables + "\r\n" +
                @"Amount of Modefied variables: " + AmountOfModificatedVariables + "\r\n" +
                @"Amount of Control variables: " + AmountOfUsedAsControlVariables + "\r\n" +
                @"Amount of Unused variables: " + (AmountOfVariables - AmountOfUsedVariables) + "\r\n" +
                @"ChapinResult: " + GetChapinResult();
            return result;
        }

        public static void NullStaticData()
        {
            AmountOfVariables = 0;
            AmountOfUsedForComputingVariables = 0;
            AmountOfUsedAsControlVariables = 0;
            AmountOfInputtedVariables = 0;
            AmountOfModificatedVariables = 0;
            AmountOfUsedVariables = 0;
            AllVariableNames = new SortedSet<string>();
        }
    }
}
