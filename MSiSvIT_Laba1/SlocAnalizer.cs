using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSiSvIT_Laba1
{
    class SlocAnalizer
    {
        private static SlocDataCountsWithCommentsAndEmptyStrs slocDataCountsWithCommentsAndEmptyStrs;
        private static SlocDataCountsWithoutCommentsAndEmptyStrs slocDataCountsWithoutCommentsAndEmptyStrs;

        public static string GetSlocReport(string codeForMetrics)
        {
            codeForMetrics = CodePartsDeleter.DeleteNonNextLineCharsInConstStringsAndComments(codeForMetrics);

            slocDataCountsWithCommentsAndEmptyStrs = new SlocDataCountsWithCommentsAndEmptyStrs(codeForMetrics);
            slocDataCountsWithoutCommentsAndEmptyStrs = new SlocDataCountsWithoutCommentsAndEmptyStrs(codeForMetrics);

            return FormReport();
        }

        internal static int CountAmountOfStrings(string code)
        {
            const string nextLineSymbol = "\n";
            int amountOfCNextLineSymbols = CodePartsFinder.TimesPatternIsFoundedInText(code, nextLineSymbol);
            return amountOfCNextLineSymbols + 1;
        }
        
        internal static void CountAverageAmountOfStringsInClassesAndMethods(
            out double averageAmountOfStringsInClass, out double averageAmountOfStringsInMethod, string code)
        {
            averageAmountOfStringsInClass = CountAverageAmountOfStringsInClasses(code);
            averageAmountOfStringsInMethod = CountAverageAmountOfStringsInMethods(code);
        }
        private static double CountAverageAmountOfStringsInClasses(string code)
        {
            List<string> classesCodes = CodePartsFinder.CutAllClassesWithoutInnerClasses(code);
            return CountAverageAmountOfStringsInStrList(classesCodes);
        }

        private static double CountAverageAmountOfStringsInMethods(string code)
        {
            List<string> methodCodes = CodePartsFinder.GetAllMethodsBodies(code);
            int amountOfStringsInMethods = CountAmountOfStringsInStrList(methodCodes);
            int amountOfMethods = methodCodes.Count;

            return (double) amountOfStringsInMethods / amountOfMethods;
        }


        private static int CountAmountOfStringsInStrList(IEnumerable<string> strList)
        {
            return strList.Aggregate(0, (current, str) => current + CountAmountOfStrings(str));
        }
        private static double CountAverageAmountOfStringsInStrList(List<string> strList)
        {
            return (double) CountAmountOfStringsInStrList(strList) / strList.Count;;
        }


        private static string FormReport()
        {
            return slocDataCountsWithCommentsAndEmptyStrs.FormReport() + "\r\n" +
                   slocDataCountsWithoutCommentsAndEmptyStrs.FormReport();
        }
    }



    internal class SlocDataCountsWithCommentsAndEmptyStrs
    {
        private readonly string code;

        public int AmountOfCodeStrings;
        public int AmountOfEmptyStrings;
        public int AmountOfComments;
        public double PercentOfStringsWithComments;
        public double AverageAmountOfStringsInMethod;
        public double AverageAmountOfStringsInClass;

        
        public SlocDataCountsWithCommentsAndEmptyStrs(string code)
        {
            this.code = code;

            CountAmountOfCodeStrings();
            CountAmountOfEmptyStrings();
            CountAmountOfComments();
            CountPercentOfStringsWithComments();
            CountAverageAmountOfStringsInClassesAndMethods();
        }
        private void CountAmountOfCodeStrings()
        {
            this.AmountOfCodeStrings = SlocAnalizer.CountAmountOfStrings(code);
        }
        
        private void CountAmountOfEmptyStrings()
        {
            const string emptyStringPattern = @"(\n|(^(?!$)))[\t \r]*((?=\n)|$)";
            this.AmountOfEmptyStrings = CodePartsFinder.TimesPatternIsFoundedInText(code, emptyStringPattern);
        }

        private const string singleCommentFeature = "//";
        private const string multyCommentFeaturePattern = @"/\*";
        private void CountAmountOfComments()
        {
            int amountofMultyComments = CodePartsFinder.TimesPatternIsFoundedInText(this.code, multyCommentFeaturePattern);
            int amountofSingleComments = CodePartsFinder.TimesPatternIsFoundedInText(this.code, singleCommentFeature);

            this.AmountOfComments = amountofMultyComments + amountofSingleComments;
        }
        
        private void CountPercentOfStringsWithComments()
        {
            int amountOfStringsSingleCommentsTake = CodePartsFinder.TimesPatternIsFoundedInText(this.code, singleCommentFeature);
            int amountOfStringsMultyCommentsTake = CountAmountOfStringsMultyCommentsTake();
            int amountOfStringsCommentsTake = amountOfStringsMultyCommentsTake + amountOfStringsSingleCommentsTake;

            const double wholePartInPercents = 100;
            this.PercentOfStringsWithComments = amountOfStringsCommentsTake * wholePartInPercents / this.AmountOfCodeStrings;
        }
        private int CountAmountOfStringsMultyCommentsTake()
        {
            const string multyCommentWithOnlyNextLineSymbolsPattern = @"(/\*[^\*]*\*/)";
            const string nextLineSymbol = "\n";
            int amountOfStringsMultyCommentsTake = 0;
            Regex multyCommentWithOnlyNextLineSymbolsRegex = new Regex(multyCommentWithOnlyNextLineSymbolsPattern);

            Match match = multyCommentWithOnlyNextLineSymbolsRegex.Match(this.code);
            while (match.Success)
            {
                string multyComment = match.ToString();
                int amountOfNextLineSymbolsInComment = CodePartsFinder.TimesPatternIsFoundedInText(multyComment,
                    nextLineSymbol);
                amountOfStringsMultyCommentsTake += amountOfNextLineSymbolsInComment + 1;

                match = match.NextMatch();
            }
            return amountOfStringsMultyCommentsTake;
        }

        private void CountAverageAmountOfStringsInClassesAndMethods()
        {
            SlocAnalizer.CountAverageAmountOfStringsInClassesAndMethods(out this.AverageAmountOfStringsInClass,
                out this.AverageAmountOfStringsInMethod, this.code);
        }

        public string FormReport()
        {
            string report = "";

            report += "Количество строк кода: " + AmountOfCodeStrings + "\r\n";
            report += "Количество пустых строк: " + AmountOfEmptyStrings + "\r\n";
            report += "Количество комментариев: " + AmountOfComments + "\r\n";
            report += "Процент комментариев: " + PercentOfStringsWithComments + "%\r\n";
            report += "Среднее количество строк в классе: " + AverageAmountOfStringsInClass + "\r\n";
            report += "Среднее количество строк в методе: " + AverageAmountOfStringsInMethod + "\r\n";

            return report;
        }
    }



    internal class SlocDataCountsWithoutCommentsAndEmptyStrs
    {
        private readonly string code;

        public int AmountOfCodeLogicalStrings;
        public double AverageAmountOfStringsInMethod;
        public double AverageAmountOfStringsInClass;


        public SlocDataCountsWithoutCommentsAndEmptyStrs(string code)
        {
            code = CodePartsDeleter.DeleteConstStrings(code);
            code = CodePartsDeleter.DeleteComments(code);
            code = CodePartsDeleter.DeleteEmptyStrings(code);
            this.code = code;

            CountAmountOfLogicalStrings();
            CountAverageAmountOfStringsInClassesAndMethods();
        }
        private void CountAmountOfLogicalStrings()
        {
            const string endOfSimpleOperator = ";";
            this.AmountOfCodeLogicalStrings += CodePartsFinder.TimesPatternIsFoundedInText(this.code, endOfSimpleOperator);
            this.AmountOfCodeLogicalStrings += GetAmountOfConstructions(this.code);
        }

        private static int GetAmountOfConstructions(string code)
        {
            return JavaReservedWordsAndOperators.GetConstructionsPatterns()
                .Sum(constructionsPattern => CodePartsFinder.TimesPatternIsFoundedInText(code, constructionsPattern));
        }

        private void CountAverageAmountOfStringsInClassesAndMethods()
        {
            SlocAnalizer.CountAverageAmountOfStringsInClassesAndMethods(out this.AverageAmountOfStringsInClass,
                out this.AverageAmountOfStringsInMethod, this.code);
        }

        public string FormReport()
        {
            string report = "";

            report += "Количество логических строк кода: " + AmountOfCodeLogicalStrings + "\r\n";
            report += "Среднее количество строк в классе, не учитывая пробельные строки и комментарии: " +
                       AverageAmountOfStringsInClass  + "\r\n";
            report += "Среднее количество строк в методе, не учитывая пробельные строки и комментарии: " +
                       AverageAmountOfStringsInMethod + "\r\n";

            return report;
        }
    }
}
