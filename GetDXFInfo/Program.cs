using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using netDxf;
using netDxf.Entities;
using OfficeOpenXml;

namespace GetDXFInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            ExportDXFInfoToExcel();
            Console.WriteLine("输出完成");
            Console.ReadKey();
        }
        
        static void ExportDXFInfoToExcel()
        {
            DxfDocument dxfdoc = DxfDocument.Load(@"D:\项目\BIM项目\厄瓜多尔Posorja\道堆\跑道梁_箱间通道_箱脚基础.dxf");
            System.Collections.ObjectModel.ReadOnlyCollection<netDxf.Entities.LwPolyline> polylines = dxfdoc.LwPolylines;

            FileInfo file = new FileInfo(@"D:\项目\BIM项目\厄瓜多尔Posorja\道堆\跑道梁_箱间通道_箱脚基础_Result.xlsx");
            if (file.Exists)
            {
                file.Delete();
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Results");
                List<LwPolylineVertex> vertexs;
                for (int i = 0; i < polylines.Count; i++)
                { 
                    sheet.Cells[i + 1, 1].Value = polylines[i].Layer.Name;
                    vertexs = polylines[i].Vertexes;
                    for (int j = 0; j <vertexs.Count; j++)
                    {
                        sheet.Cells[i + 1, j + 2].Value = vertexs[j].Location.ToString();
                    }
                }
                package.Save();
            }
        }
    }
}
