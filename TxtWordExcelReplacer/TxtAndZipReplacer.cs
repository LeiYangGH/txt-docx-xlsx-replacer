using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TxtWordExcelReplacer.ViewModel;

namespace TxtWordExcelReplacer
{
    public class TxtAndZipReplacer : IReplacer
    {
        public string Replace(string fileName, IList<WordPairViewModel> wordPairViewModels)
        {
            string ext = Path.GetExtension(fileName);
            switch (ext)
            {
                case ".txt":
                    return ReplaceTxt(fileName, wordPairViewModels);
                default:
                    return $"未知扩展名{fileName}";
            }
        }

        public static string ReplaceTxt(string fileName, IList<WordPairViewModel> wordPairViewModels)
        {
            try
            {
                string text = File.ReadAllText(fileName);
                foreach (WordPairViewModel pair in wordPairViewModels)
                {
                    text = text.Replace(pair.SrcWord, pair.DesWord);
                }
                File.WriteAllText(fileName, text);
                return $"替换完成{fileName}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
