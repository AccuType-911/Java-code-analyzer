
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSiSvIT_Laba1
{
    internal class JavaReservedWordsAndOperators
    {
        public static string[] UnaryOperatorsPatterns =
        {
            GetPatternFromOperations("++"), "--", @"![\s\r\n\t]*[\w_\(]", @"~[\s\r\n\t]*[\w_\(]", @"\[(?![\s\r\n\t]*\])"
        };

        public static string[] BinaryOperators =
        {
            "*", "/", "%", ">>", "+", "-", "=", "==", "!=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "<<=", ">>=",
            "<=", ">=", ">", "<", "<<", "&", "^", "^=", "|", "&&", "||", ">>>="
        };
        public static List<string> GetBinaryOperatorsPatterns()
        {
            string[] binaryOperatorsPatterns = new string[BinaryOperators.Length];
            for (int i = 0; i < BinaryOperators.Length; i++)
            {
                binaryOperatorsPatterns[i] = @"(?<=[\w_\)\]][\s\r\n\t]*(((\+\+)|(--))[\s\r\n\t]*)*)" + 
                        GetPatternFromOperations(BinaryOperators[i]) + 
                        @"(?=[\s\r\n\t]*(((\+\+)|(--))[\s\r\n\t]*)*[\w_\(~!])";
            }
            return binaryOperatorsPatterns.ToList();
        }
        private static string GetPatternFromOperations(string str)
        {
            return str.Aggregate("", (current, character) => current + ("\\" + character));
        }


        public static string[] WordOperators =
        {
            "new", "instaceof", "return", "break", "continue", "throw", "try", 
            "for", "foreach", @"(?<!}[\s\r\n\t]*)while", "do", "if", "switch"
        };
        private static List<string> GetWordOperatorsPatterns(string[] wordOperators)
        {
            string[] wordOperatorsPatterns = new string[wordOperators.Length];
            for (int i = 0; i < wordOperators.Length; i++)
            {
                wordOperatorsPatterns[i] = GetWordPatternFromWord(wordOperators[i]);
            }
            return wordOperatorsPatterns.ToList();
        }
        public static string GetWordPatternFromWord(string word)
        {
            return @"(?<![\w_])" + word + @"(?![\w_])";
        }
        public static List<string> GetWordOperatorsPatterns()
        {
            return GetWordOperatorsPatterns(WordOperators);
        }

        public static string[] Delimeters =
        {
            ";", "{", "}", ","
        };

        public static string[] SpecialOperatorsPatterns =
        {
            @"(?<=[^\w_\s\r\n\t][\s\r\n\t]*(new[\s\r\n\t]+)?([\w_\[\],]+\.)*[\w_]+(?<!(if)|(switch)|(while)|(for)|(foreach)|(catch)|(return))[\s\r\n\t]*)\(",
            @"(\((?=[\w_\.]+\)[\s\r\n\t]*[\w_\(]))",
            @"(?<=[\w_\)\]])\.(?=[\w_*])",
            @"\?"
        };


        public static List<string> GetAllOperatorsPatterns()
        {
            List<string> result = new List<string>();

            result.AddRange(SpecialOperatorsPatterns.ToList());
            result.AddRange(GetBinaryOperatorsPatterns());
            result.AddRange(UnaryOperatorsPatterns.ToList());
            result.AddRange(Delimeters.ToList());
            result.AddRange(GetWordOperatorsPatterns());

            return result;
        }  


        public static char[] OperationSymbols =
        {
            '+', '-', '*', '/', '<', '>', '|', '&', '^', '%', '?', ':'
        };
        public static string[] UnusefullJavaKeyWordsInExpressionsForChapin =
        {
            "new", "throw", "instanceof", "return", "try", "volatile", "const"
        };
        public static void DeleteUnusefulWordsInExpressionForChapin(ref string expression)
        {
            foreach (string unusefulWord in UnusefullJavaKeyWordsInExpressionsForChapin)
            {
                const string none = "";
                string unusefulWordPattern = GetWordPatternFromWord(unusefulWord);
                Regex unusefulWordRegex = new Regex(unusefulWordPattern);

                expression = unusefulWordRegex.Replace(expression, none);
            }
        }



        public static string[] ConstructionsWords =
        {
            "try", "catch", "for", "foreach", @"(?<!}[\s\r\n\t]*)while", "do", "finally", "if", "switch"
        };
        public static List<string> GetConstructionsPatterns()
        {
            return GetWordOperatorsPatterns(ConstructionsWords);
        } 
    }
}
