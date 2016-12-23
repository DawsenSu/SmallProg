using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using MathNet.Numerics;

using netDxf;
using netDxf.Entities;

namespace TestMathNet
{
    class Program
    {
        static void Main(string[] args)
        {
            NetDxfTest();
            Console.Write("Dxfdoc is created!");
            Console.ReadKey();
        }

        static void NetDxfTest()
        {
            DxfDocument dxfdoc = new DxfDocument(netDxf.Header.DxfVersion.AutoCad2010);

            Circle c = new Circle(new Vector2(100, 100), 100);
            
            dxfdoc.AddEntity(c);
            dxfdoc.Save(@"D:\dxfTest.dxf");
        }
        static void MathNetTest()
        {
            Tuple<Complex, Complex, Complex> results = FindRoots.Cubic(1, 2, 3, 4);
            Console.WriteLine("The result of 1+2x+3x^2+4x^3=0 is:");
            Console.WriteLine(results.Item1);
            Console.WriteLine(results.Item2);
            Console.WriteLine(results.Item3);
        }
    }
}
