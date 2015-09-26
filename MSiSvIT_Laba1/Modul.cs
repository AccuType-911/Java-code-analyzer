using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MSiSvIT_Laba1
{
    class UnprocessedClass
    {
        public string ClassCode { get; set; }
        public List<Variable> ParrentClassVariables { get; set; }

        public UnprocessedClass(string classCode, List<Variable> parrentClassVariables)
        {
            this.ClassCode = classCode;
            this.ParrentClassVariables = parrentClassVariables;
        }
    }

    class Modul
    {
        private string modulCode;
        private readonly List<Class> classes = new List<Class>();
        private readonly List<UnprocessedClass> unprocessedClassesQueue = new List<UnprocessedClass>();

        public Modul(string modulCode)
        {
            this.modulCode = modulCode;
            this.DeleteUnnecessaryCodeParts();
            this.SplitModulOnProcessedClasses();
            this.modulCode = modulCode;
        }
        private void DeleteUnnecessaryCodeParts()
        {
            this.modulCode = CodePartsDeleter.DeleteNonNextLineCharsInConstStringsAndComments(modulCode);
            this.modulCode = CodePartsDeleter.DeleteConstChars(modulCode);
            this.modulCode = CodePartsDeleter.DeleteAnnotations(this.modulCode);
        }

        private void SplitModulOnProcessedClasses()
        {
            this.CutUnprocessedClassesFromModul();
            this.ProcessUnprocessedClasses();
        }
        private void CutUnprocessedClassesFromModul()
        {
            List<string> unprocessedClassesCodes = CutInnerClassesCodes(ref this.modulCode);

            foreach (string unprocessedClassCode in unprocessedClassesCodes)
            {
                this.unprocessedClassesQueue.Add(new UnprocessedClass(unprocessedClassCode, new List<Variable>()));
            }
        }
        private void ProcessUnprocessedClasses()
        {
            for (int i = 0; i < this.unprocessedClassesQueue.Count; i++)
            {
                UnprocessedClass parrentUnprocessedClass = this.unprocessedClassesQueue[i];

                IEnumerable<string> childClassesCodes = CutChildClasses(ref parrentUnprocessedClass);

                Class parrentClass = ProccessClass(parrentUnprocessedClass);
                this.classes.Add(parrentClass);

                this.AddUnprocessedClassesInQueue(childClassesCodes, parrentClass.ClassVariables);
            }
        }

        private static IEnumerable<string> CutChildClasses(ref UnprocessedClass unprocessedClass)
        {
            string classCode = unprocessedClass.ClassCode;
            IEnumerable<string> cuttedClasses = CutInnerClassesCodesFromClassBody(ref classCode);
            unprocessedClass.ClassCode = classCode;
            return cuttedClasses;

        }
        public static IEnumerable<string> CutInnerClassesCodesFromClassBody(ref string classCode)
        {
            List<string> cuttedClasses;
            if (!IsAnonimusUnprocessedClass(classCode))
            {
                Class.DeleteEverythingBeforeClassName(ref classCode);
                cuttedClasses = CutInnerClassesCodes(ref classCode);
                classCode = "class " + classCode;
            }
            else
            {
                cuttedClasses = CutInnerClassesCodes(ref classCode);
            }
            return cuttedClasses;
        }
        private static bool IsAnonimusUnprocessedClass(string classCode)
        {
            return classCode[0] == '{';
        }

        private static Class ProccessClass(UnprocessedClass unprocessedClass)
        {
            return new Class(unprocessedClass.ClassCode, unprocessedClass.ParrentClassVariables);
        }
        private void AddUnprocessedClassesInQueue(IEnumerable<string> childClassesCodes, List<Variable> parrentClassVariables)
        {
            foreach (string cuttedClassCode in childClassesCodes)
            {
                this.unprocessedClassesQueue.Add(new UnprocessedClass(cuttedClassCode, parrentClassVariables));
            }
        }
       

        private static List<string> CutInnerClassesCodes(ref string code)
        {
            List<string> cuttedClasses = new List<string>();

            Match match = Regex.Match(code, CodePartsFinder.ClassPattern);
            while (match.Success)
            {
                cuttedClasses.Add(CodePartsFinder.CutNextClass(match.Index, ref code));
                match = Regex.Match(code, CodePartsFinder.ClassPattern);
            }
            return cuttedClasses;
        }

        public string GetChapinReport()
        {
            string report = Variable.GetChapinReport();
            Variable.NullStaticData();
            return report;
        }

        public string GetHolstedReport()
        {
            string report =  HolstedAnalizer.GetHolstedReport(modulCode);
            Variable.NullStaticData();
            return report;
        }
    }
}
