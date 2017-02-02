using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EYDataProcess
{
    class ProcessAllJETBData
    {
        public ProcessAllJETBData(string path)
        {
            HighestDirectory = new DirectoryInfo(path);
        }
        public void ProcessAll()
        {
            if (!HighestDirectory.Exists)
            {
                Console.WriteLine("所选目录不存在！");
                return;
            }
            DirectoryInfo[] subDierc = HighestDirectory.GetDirectories();
            if (subDierc.Length <1)
            {
                Console.WriteLine("所选目录下不存在公司级的子文件夹！");
                return;
            }
            foreach (var subDirecItem in subDierc)
            {
                foreach (var subsubDiecItem in subDirecItem.GetDirectories())
                {
                    if (subsubDiecItem.Name.ToUpper() == "TB")
                    {
                        foreach (var xlsFile in subsubDiecItem.GetFiles("*.XLS"))
                        {
                            ExcelProcess ep = new ExcelProcess(xlsFile.FullName);
                            Console.Write(xlsFile.Name + "   正在处理...   ");
                            ep.ProcessTBData();
                            Console.WriteLine("处理完成！");
                        }
                    }
                    if (subsubDiecItem.Name.ToUpper() == "JE")
                    {
                        foreach (var xlsFile in subsubDiecItem.GetFiles("*.XLS"))
                        {
                            ExcelProcess ep = new ExcelProcess(xlsFile.FullName);
                            Console.Write(xlsFile.Name + "   正在处理...   ");
                            ep.ProcessJEDataWihtUsedInfo();
                            Console.WriteLine("处理完成！");
                        }
                    }
                }                
            }
        }

        DirectoryInfo HighestDirectory;
    }
}
