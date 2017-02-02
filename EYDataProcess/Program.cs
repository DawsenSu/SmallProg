using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EYDataProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("+     JE、TB数据处理程序 Create by Angela ll.liu     +");
            Console.WriteLine("+ 2017.1.14 Beijing  e-mail:angela-ll.liu@cn.ey.com  +");
            Console.WriteLine("======================================================");
            Console.Write("请输入JE、TB数据存在的顶层目录：");
            string highestDirecPath = Console.ReadLine();
            ProcessAllJETBData pAllJETB = new ProcessAllJETBData(highestDirecPath);
            pAllJETB.ProcessAll();
            Console.WriteLine("数据处理完成,存放在各级子目录下！");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
    }
}
