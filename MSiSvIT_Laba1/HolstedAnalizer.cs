
using System;

namespace JavaCodeAnalyzer
{
    class HolstedAnalizer
    {
        private static OperandDictionary operandDictionary;
        private static OperatorDictionary operatorDictionary;

        private static OperandDictionary theoreticalOperandDictionary;
        private static OperatorDictionary theoreticalOperatorDictionary;

        private static string modul;

        public static string GetHolstedReport(string code)
        {
            code = CodePartsDeleter.DeleteAnnotations(code);
            code = CodePartsDeleter.DeleteComments(code);
            modul = code;

            InitializeNonTheoreticalDictionary(modul);

            const string mainMethodName = "main";
            string mainMethod = CodePartsFinder.FindMethod(modul, mainMethodName);
            InitializeTheoreticalDictionary(mainMethod);

            return GetReport();
        }

        private static void InitializeNonTheoreticalDictionary(string codePart)
        {
            operandDictionary = new OperandDictionary(codePart);

            codePart = CodePartsDeleter.DeleteConstStrings(codePart);
            codePart = CodePartsDeleter.DeleteConstChars(codePart);
            operatorDictionary = new OperatorDictionary(codePart);
        }
        private static void InitializeTheoreticalDictionary(string codePart)
        {
            theoreticalOperandDictionary = new OperandDictionary(codePart);

            Modul.CutInnerClassesCodesFromClassBody(ref codePart);
            codePart = CodePartsDeleter.DeleteConstStrings(codePart);
            codePart = CodePartsDeleter.DeleteConstChars(codePart);
            theoreticalOperatorDictionary = new OperatorDictionary(codePart);
        }

        
        private static string GetReport()
        {
            string report = "";

            int amountOfUniqueOperators = operatorDictionary.UniqueDictionaryLen;
            report += "Количество уникальных операторов: " + amountOfUniqueOperators + "\r\n";
            int amountOfUniqueOperands = operandDictionary.UniqueDictionaryLen;
            report += "Количество уникальных операндов: " + amountOfUniqueOperands + "\r\n";

            int amountOfOperators = operatorDictionary.DictionaryLen;
            report += "Количество операторов: " + amountOfOperators + "\r\n";
            int amountOfOperands = operandDictionary.DictionaryLen;
            report += "Количество операндов: " + amountOfOperands + "\r\n";

            int programDictionary = amountOfUniqueOperands + amountOfUniqueOperators;
            report += "Словарь программы: " + programDictionary + "\r\n";
            int programLength = amountOfOperands + amountOfOperators;
            report += "Длина программы: " + programLength + "\r\n";

            report += "\r\n";


            int amountOfTheoreticalUniqueOperators = theoreticalOperatorDictionary.UniqueDictionaryLen;
            report += "Теоретическое количество уникальных операторов: " + amountOfTheoreticalUniqueOperators + "\r\n";
            int amountOfTheoreticalUniqueOperands = theoreticalOperandDictionary.UniqueDictionaryLen;
            report += "Теоретическое количество уникальных операндов: " + amountOfTheoreticalUniqueOperands + "\r\n"; 

            int theoreticalProgramDictionary = amountOfTheoreticalUniqueOperands + amountOfTheoreticalUniqueOperators;
            report += "Теоретичесский словарь программы: " + theoreticalProgramDictionary + "\r\n";

            const int basementTwo = 2;
            double programTheoreticalLength = amountOfTheoreticalUniqueOperands * Math.Log(amountOfTheoreticalUniqueOperands, basementTwo) +
                                              amountOfTheoreticalUniqueOperators* Math.Log(amountOfTheoreticalUniqueOperators, basementTwo);
            report += "Теоретическая длина программы: " + programTheoreticalLength + "\r\n";

            report += "\r\n";


            double programVolume = programLength * Math.Log(programLength, basementTwo);
            report += "Объём программы: " + programVolume + "\r\n";

            double programTheoreticalVolume = programTheoreticalLength * Math.Log(theoreticalProgramDictionary, basementTwo);
            report += "Теоретический объём программы: " + programTheoreticalVolume + "\r\n";

            report += "\r\n";


            double qualityOfProgram = programTheoreticalVolume / programVolume;
            report += "Теоретическое качество программы: " + qualityOfProgram + "\r\n";
            double realQualityOfProgram = basementTwo * (double)amountOfUniqueOperands /
                                          (amountOfUniqueOperators * amountOfOperands);
            report += "Реальное качество программы: " + realQualityOfProgram + "\r\n";

            double strengthOfProgrammer = programTheoreticalLength * Math.Log((programDictionary / qualityOfProgram), basementTwo);
            report += "Оценка интеллектуальных усилий: " + strengthOfProgrammer + "\r\n";
            

            return report;
        }
    }
}
