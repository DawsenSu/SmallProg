using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using netDxf;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Objects;
using netDxf.Tables;

namespace netdxf_text
{
    class Program
    {
        /// <summary>
        /// This is just a simple test of work in progress for the netdxf library.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Text();
        }

        private static void Text()
        {
            // use a font that has support for chinese characters
            TextStyle textstyle = new TextStyle("Chinese text", "simsun.ttf");

            // for dxf database version 2007 and later you casn use directly the characters,
            DxfDocument dxf1 = new DxfDocument(DxfVersion.AutoCad2010);
            Text text1 = new Text("这是中国文字", Vector2.Zero, 10, textstyle);
            dxf1.AddEntity(text1);
            dxf1.Save("textcad2010.dxf");

            foreach (Text text in dxf1.Texts)
            {
                Console.WriteLine(text.Value);
            }
            Console.WriteLine("Press a key to continue...");
            Console.ReadLine();
        }
    }
}
