using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SmallProg
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("请输入文件名：");
            string filename = Console.ReadLine();
            if (SetBackgorundAccodingToRGB(filename))
            {
                Console.WriteLine("绘制完毕！");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("输入文件错误！");
                Console.ReadKey();
            }
        }

        static bool SetBackgorundAccodingToRGB(string filePath)
        {
            FileInfo excelfile = new FileInfo(filePath);
            if (!excelfile.Exists)
                return false;
            using (ExcelPackage package = new ExcelPackage(excelfile))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets[1];
                int usedRowNum = sheet.Dimension.Rows;
                for (int i = 2; i < usedRowNum+1; i++)
                {
                    string[] rgbs = sheet.Cells[i, 4].Text.Split(',');
                    sheet.Cells[i, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[i, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(int.Parse(rgbs[0]), int.Parse(rgbs[1]), int.Parse(rgbs[2])));
                }

                package.Save();
            }
            return true;
        }
    }
}
