using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TestDiagonse
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\sudongsheng\Desktop\所需资料\构件关系表.xlsx";
            Process.Start(filePath);
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
