using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TxtWordExcelReplacer
{
    public static class Helpers
    {
        public static string GetBackedUpFileName(string fileName)
        {
            return fileName + ".bak";
        }

        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
    }
}
