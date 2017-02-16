using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.Data;
using System.Data.Odbc;

namespace EYDataProcess
{
    class ExcelProcess
    {
        public ExcelProcess(string filePath)
        {
            rawFileInfo = new FileInfo(filePath);
            jERank = new Dictionary<string, int>();
            jERank.Add("会计期间", 0);
            jERank.Add("公司代码", 0);
            jERank.Add("会计凭证号码", 0);
            jERank.Add("总分类帐帐目", 0);
            jERank.Add("凭证中的记帐日期", 0);
            jERank.Add("会计凭证输入日期", 0);
            jERank.Add("用户名", 0);
            jERank.Add("事务代码", 0);
            jERank.Add("凭证抬头文本", 0);
            jERank.Add("按本位币计的金额", 0);
            jERank.Add("凭证类型", 0);
        }
        public bool ProcessTBData()
        {
            List<string> content = new List<string>();

            StreamReader sr = new StreamReader(rawFileInfo.FullName);
            sr.ReadLine();
            string temp;
            while (!sr.EndOfStream)
            {
                temp = sr.ReadLine();
                if ("" != temp)
                {
                    content.Add(temp);
                }
            }
            sr.Close();

            if (0 == content.Count)
            {
                return false;
            }
            int colNum = content[0].Split('\t').Length-1;
            FileInfo outputFile = new FileInfo(rawFileInfo.DirectoryName + @"\" + rawFileInfo.Name.TrimEnd(".XLS".ToArray()) + @"_"+(content.Count-1).ToString()+"行.xlsx");
            if (outputFile.Exists)
            {
                outputFile.Delete();
            }

            using (ExcelPackage package = new ExcelPackage(outputFile))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Sheet1");
                string tempstr = string.Empty;
                for (int row = 1; row < content.Count+1; row++)
                {
                    string[] fields = content[row - 1].Split('\t');
                    int thisFiledColNum = fields.Length - 1;
                    for (int i = 0; i < colNum - thisFiledColNum; i++)
                    {
                        content[row - 1] += "\t0";
                    }
                    fields = content[row - 1].Split('\t');
                    for (int col = 1; col < colNum+1; col++)
                    {
                        tempstr = fields[col].Trim();
                        if (tempstr == string.Empty)
                        {
                            sheet.Cells[row, col].Value = "0";
                        }
                        else
                        {
                            sheet.Cells[row, col].Value = tempstr;
                        }                                              
                    }
                }
                package.Save();
            }
            return true;
        }
        //public bool ProcessJEData()
        //{
        //    if (0 == content.Count)
        //    {
        //        return false;
        //    }
        //    int colNum = content[0].Split('\t').Length - 1;
        //    FileInfo outputFile = new FileInfo(rawFileInfo.DirectoryName + @"\" + rawFileInfo.Name.TrimEnd(".XLS".ToArray()) + @"_" + (content.Count - 1).ToString() + "行.xlsx");
        //    if (outputFile.Exists)
        //    {
        //        outputFile.Delete();
        //    }
        //    using (ExcelPackage package = new ExcelPackage(outputFile))
        //    {
        //        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Sheet1");
        //        for (int row = 1; row < content.Count + 1; row++)
        //        {
        //            string[] fields = content[row - 1].Split('\t');
        //            int thisFiledColNum = fields.Length-1;
        //            for (int i = 0; i < colNum - thisFiledColNum; i++)
        //            {
        //                content[row - 1] += "\t";
        //            }
        //            fields = content[row - 1].Split('\t');
        //            for (int col = 1; col < colNum + 1; col++)
        //            {
        //                if (col == 5 && "H" == fields[3])
        //                {
        //                    sheet.Cells[row, col].Value = "-"+fields[col].Trim();
        //                }
        //                else
        //                {
        //                    sheet.Cells[row, col].Value = fields[col].Trim();
        //                }
        //            }
        //        }
        //        sheet.Cells[1, 5].Value = "Amount";
        //        package.Save();
        //    }
        //    return true;
        //}

        public bool ProcessJEDataWihtUsedInfo()
        {
            StreamReader sr = new StreamReader(rawFileInfo.FullName);
            sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
            string title = sr.ReadLine();
            string[] titleItems = title.Split('\t');
            for (int i = 0; i < titleItems.Length; i++)
            {
                titleItems[i] = titleItems[i].Trim();
            }
            int browLendFlag = 0;
            //find index of data
            for (int i = 0; i < jERank.Count; i++)
            {
                for (int j = 0; j < titleItems.Length; j++)
                {
                    string t = jERank.Keys.ElementAt(i);
                    if (jERank.Keys.ElementAt(i) == titleItems[j])
                    {
                        string tt = titleItems[j];
                        jERank[titleItems[j]] = j;
                        break;
                    }
                }
            }
            for (int j = 0; j < titleItems.Length; j++)
            {
                if (titleItems[j] == @"借方/贷方标识")
                {
                    browLendFlag = j;
                    break;
                }
            }

            FileInfo outputInfo = new FileInfo(rawFileInfo.DirectoryName + @"\" + rawFileInfo.Name.Replace(".XLS", ".xlsx"));
            ExcelPackage package = new ExcelPackage(outputInfo);
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("AdjustData");

            for (int i = 0; i < jERank.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = titleItems[jERank.Values.ElementAt(i)];
            }

            string temp;
            int excelRowNum = 2;
            while (!sr.EndOfStream)
            {
                temp = sr.ReadLine();
                if ("" != temp)
                {
                    string[] dataLine = temp.Split('\t');
                    if (dataLine.Length < 2)
                    {
                        continue;
                    }
                    for (int i = 0; i < jERank.Count; i++)
                    {
                        if (dataLine[browLendFlag] == @"H" && i == 9 /*need to be change if jERnad change*/)
                            sheet.Cells[excelRowNum, i + 1].Value = "-"+dataLine[jERank.Values.ElementAt(i)].Trim();
                        else
                            sheet.Cells[excelRowNum, i + 1].Value = dataLine[jERank.Values.ElementAt(i)].Trim();
                    }
                    excelRowNum++;
                }                
            }
            sr.Close();
            sheet.Cells[1, jERank.Keys.ToList().IndexOf("按本位币计的金额")+1].Value = "Amount";
            FileInfo saveAsInfo = new FileInfo(rawFileInfo.DirectoryName + @"\" + rawFileInfo.Name.Trim(".XLS".ToArray()) +"_"+ (excelRowNum-2).ToString() +"行_adjusted.xlsx");
            if (saveAsInfo.Exists)
            {
                saveAsInfo.Delete();
            }
            package.SaveAs(saveAsInfo);
            package.Dispose();
            return true;
        }
        Dictionary<string, int> jERank;
        FileInfo rawFileInfo;
    }
}
