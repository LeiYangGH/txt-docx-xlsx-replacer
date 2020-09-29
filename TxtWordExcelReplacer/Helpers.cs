using System;
using System.Collections.Generic;
using System.Text;

namespace TxtWordExcelReplacer
{
    public static class Helpers
    {
        public static string GetBackedUpFileName(string fileName)
        {
            return fileName + ".bak";
        }
    }
}
