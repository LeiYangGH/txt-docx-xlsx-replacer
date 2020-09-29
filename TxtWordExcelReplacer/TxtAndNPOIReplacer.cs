using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TxtWordExcelReplacer.ViewModel;

namespace TxtWordExcelReplacer
{
    public class TxtAndNPOIReplacer : IReplacer
    {
        public string Replace(string fileName, IList<WordPairViewModel> wordPairViewModels)
        {
            string ext = Path.GetExtension(fileName);
            switch (ext)
            {
                case ".txt":
                    return TxtAndZipReplacer.ReplaceTxt(fileName, wordPairViewModels);
                case ".docx":
                    return ReplaceDocx(fileName, wordPairViewModels);
                default:
                    return $"未知扩展名{fileName}";
            }
        }

        private static void ReplaceKey(XWPFParagraph para, IList<WordPairViewModel> wordPairViewModels)
        {

            string text = para.ParagraphText;
            var runs = para.Runs;
            string styleid = para.Style;
            for (int i = 0; i < runs.Count; i++)
            {
                var run = runs[i];
                text = run.ToString();
                foreach (WordPairViewModel pair in wordPairViewModels)
                {
                    if (text.Contains(pair.SrcWord))
                    {
                        string replaced = text.Replace(pair.SrcWord, pair.DesWord);
                        runs[i].SetText(replaced, 0);
                        //break;
                    }
                }
            }
        }
        public static string ReplaceDocx(string fileName, IList<WordPairViewModel> wordPairViewModels)
        {
            string backFileName = Helpers.GetBackedUpFileName(fileName);
            if (!File.Exists(backFileName))
                File.Copy(fileName, backFileName, false);
            XWPFDocument doc = null;
            try
            {
                using (FileStream stream = File.OpenRead(fileName))
                {
                    doc = new XWPFDocument(stream);
                    foreach (var para in doc.Paragraphs)
                    {
                        ReplaceKey(para, wordPairViewModels);
                    }
                    var tables = doc.Tables;
                    foreach (var table in tables)
                    {
                        foreach (var row in table.Rows)
                        {
                            foreach (var cell in row.GetTableCells())
                            {
                                foreach (var para in cell.Paragraphs)
                                {
                                    ReplaceKey(para, wordPairViewModels);
                                }
                            }
                        }
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {

                    doc.Write(ms);
                    using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                    {
                        fsWrite.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    };
                }
                return $"替换完成{fileName}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
