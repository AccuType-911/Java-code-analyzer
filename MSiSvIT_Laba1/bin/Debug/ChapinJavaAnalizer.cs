using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSiSvIT_Laba1
{
    static class ChapinJavaAnalizer
    {
        private static string javaCode;
        private static ChapinResult result;

        public static ChapinResult Analize (string code)
        {
            javaCode = code;

            DeleteUnnecessaryCodeParts();
            result = new ChapinResult(javaCode);

            return result;
        }

        private static void DeleteUnnecessaryCodeParts () 
        {
            javaCode = CodePartsDeleter.DeleteConstStrings(javaCode);
            javaCode = CodePartsDeleter.DeleteComments(javaCode);
            javaCode = CodePartsDeleter.DeleteAnnotations(javaCode);
        }

       
    }
}
