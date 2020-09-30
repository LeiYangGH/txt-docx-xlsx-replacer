using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string Replace(string fileName, IList<WordPairViewModel> wordPairViewModels)
        {
            string ext = Path.GetExtension(fileName);
            switch (ext)
            {
                case ".txt":
                    return TxtAndZipReplacer.ReplaceTxt(fileName, wordPairViewModels);
                case ".docx":
                    return ReplaceDocx(fileName, wordPairViewModels);
                case ".xlsx":
                    return ReplaceXlsx(fileName, wordPairViewModels);
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

        public static string ReplaceXlsx(string fileName, IList<WordPairViewModel> wordPairViewModels)
        {
            try
            {
                XSSFWorkbook xssfworkbook;
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    xssfworkbook = new XSSFWorkbook(fs);



                    for (int j = 0; j < xssfworkbook.NumberOfSheets; j++)
                    {
                        //XSSFSheet 
                        ISheet sheet = xssfworkbook.GetSheetAt(j);
                        //ROW
                        var rows = sheet.GetRowEnumerator();


                        while (rows.MoveNext())
                        {
                            var row = (XSSFRow)rows.Current;

                            for (var i = 0; i < row.LastCellNum; i++)
                            {
                                XSSFCell cell = (XSSFCell)row.GetCell(i);

                                if (cell != null)
                                {
                                    String cellValue = cell.ToString();
                                    foreach (var pair in wordPairViewModels)
                                    {
                                        if (cellValue.Contains(pair.SrcWord))
                                        {
                                            cell.SetCellValue(cellValue.Replace(pair.SrcWord, pair.DesWord));
                                        }
                                    }
                                }
                            }
                        }
                    }




                }
                //另存为
                MemoryStream stream = new MemoryStream();
                xssfworkbook.Write(stream);
                var buf = stream.ToArray();
                //保存为Excel文件  
                using (FileStream fs1 = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs1.Write(buf, 0, buf.Length);
                    fs1.Flush();
                }
                return $"替换完成{fileName}";
            }
            catch (Exception ex)
            {
                log.Error($"{fileName}:{ex.Message}");
                return ex.Message;
            }
        }

        public static string ReplaceDocx(string fileName, IList<WordPairViewModel> wordPairViewModels)
        {
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
                log.Error($"{fileName}:{ex.Message}");
                return ex.Message;
            }
        }
    }
}
