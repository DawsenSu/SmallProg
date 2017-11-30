using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Aspose.Words;

namespace CodeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("输入所要搜索的文件夹：");
            string dirpath = Console.ReadLine();
            if(Directory.Exists(dirpath))
            {
                CodeBook codeBook = new CodeBook(dirpath);
                codeBook.CreateWord();
                Console.WriteLine($"生成成功，文档保存至{dirpath}");
            }
            Console.ReadKey();
        }


    }

    public class CodeBook
    {
        public CodeBook(string path)
        {
            m_path = path;
        }
        string m_path;
        List<string> codeext = new List<string>() { ".h", ".cpp", ".xaml", ".cs", ".r" };

        List<string> FindAllCodeFile(string directorypath)
        {
            var result = from filepath in Directory.EnumerateFiles(directorypath, "*", SearchOption.AllDirectories)
                         let ext = Path.GetExtension(filepath).ToLower()
                         where codeext.Contains(ext) && !filepath.Contains("obj") || filepath.Contains("Commands.xml")
                         select filepath;
            return result.ToList();
        }

        public void CreateWord()
        {
            List<string> filelist = FindAllCodeFile(m_path);
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Font bfont = builder.Font;
            bfont.Name = "宋体"; 
            bfont.NameAscii = "Times New Roman";
            foreach (var file in filelist)
            {
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading3;
                bfont.Bold = true;
                bfont.Size = 15;
                builder.Writeln(Path.GetFileName(file));

                builder.ParagraphFormat.ClearFormatting();
                bfont.Bold = false;
                bfont.Size = 10.5;
                builder.Writeln(File.ReadAllText(file,Encoding.GetEncoding("GB2312")));
                builder.InsertBreak(BreakType.ParagraphBreak);
                builder.InsertBreak(BreakType.ParagraphBreak);
            }
            doc.Save(Path.Combine(m_path, "2017年MS开发程序集.docx"));
        }
    }
}
