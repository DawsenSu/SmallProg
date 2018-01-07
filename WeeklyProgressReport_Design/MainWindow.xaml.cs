using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OfficeOpenXml;

namespace WeeklyProgressReport_Design
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = GetWeekFirstDayMon(DateTime.Now);
            EndDatePicker.SelectedDate = GetWeekLastDaySun(DateTime.Now);
        }

        /// <summary>  
        /// 得到本周第一天(以星期一为第一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        private DateTime GetWeekFirstDayMon(DateTime datetime)
        {
            //星期一为第一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);

            //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。  
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;

            //本周第一天  
            string FirstDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(FirstDay);
        }
        /// <summary>  
        /// 得到本周最后一天(以星期天为最后一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        private DateTime GetWeekLastDaySun(DateTime datetime)
        {
            //星期天为最后一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            weeknow = (weeknow == 0 ? 7 : weeknow);
            int daydiff = (7 - weeknow);

            //本周最后一天  
            string LastDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(LastDay);
        }


        string[] EmailAttachmentsFolder = { "CGR反馈", "COWI反馈", "EXE提交文件", "提交至CGR", "提交至COWI" };

        List<ReportContent> GetReportItmes(string folderpath)
        {
            List<ReportContent> results = new List<ReportContent>();
            DirectoryInfo subrootdirectinfo = new DirectoryInfo(folderpath);
            string subrootname = subrootdirectinfo.Name;

            bool iscgr = subrootname == "CGR反馈" ? true : false;
            bool iscowi = subrootname == "COWI反馈" ? true : false;
            string comment = string.Empty;
            if (iscgr)
                comment = "CGR反馈意见";
            else if (iscowi)
                comment = "COWI反馈意见";


            string[] subfolders = Directory.GetDirectories(folderpath);

            foreach (var subfolder in subfolders)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(subfolder);
                string docname = directoryInfo.Name;
                string datestring = docname.Split(' ')[0];
                //DateTime dateTime = new DateTime();

                if (datestring.Length == 6)
                    datestring = "20" + datestring;
                datestring = datestring.Substring(0, 4) + "/" + datestring.Substring(4, 2) + "/" + datestring.Substring(6, 2);
                if (!DateTime.TryParse(datestring, out DateTime dateTime))
                {
                    System.Windows.Forms.MessageBox.Show($"{datestring} Can not be parsed to Type DateTime!");
                    continue;
                }
                results.Add(new ReportContent { DocName = docname, DocDate = dateTime, IsCOWI = iscowi, ISCGR = iscgr, Comment = comment });
            }
            return results;
        }

        private void GenerateExcelReport(string outputpath, IEnumerable<ReportContent> reportContent)
        {
            string[] titles = { "No.", "DocumentName", "DocumentTime", "COWI", "CGR", "Commnets" };
            using (ExcelPackage package = new ExcelPackage(new FileInfo(outputpath)))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Design File Summary");

                int colnum = 1;
                foreach (var title in titles)
                {
                    sheet.Cells[1, colnum].Value = title;
                    colnum++;
                }

                int startrownum = 2;
                foreach (var content in reportContent)
                {
                    sheet.Cells[startrownum, 1].Value = startrownum - 1;
                    sheet.Cells[startrownum, 2].Value = content.DocName;
                    sheet.Cells[startrownum, 3].Value = content.DocDate.Month.ToString() +"."+content.DocDate.Day.ToString();
                    sheet.Cells[startrownum, 4].Value = content.IsCOWI ? "√" : "";
                    sheet.Cells[startrownum, 5].Value = content.ISCGR ? "√" : "";
                    sheet.Cells[startrownum, 6].Value = content.Comment;

                    startrownum++;
                }
                sheet.Cells[1, 1, sheet.Dimension.Rows, sheet.Dimension.Columns].AutoFitColumns();
                package.Save();
            }
        }

        private void Directory_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Enter the output excel name",
                Filter = "Excel 2010-2017|*.xlsx",
                InitialDirectory = @"D:\设计一所\项目\Posorja\往来邮件附件\周报更新表格"
            };
            try
            {
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                        File.Delete(sfd.FileName);
                    ExcelFilePathBox.Text = sfd.FileName;
                    ProcessBtn.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void Process_Button_Click(object sender, RoutedEventArgs e)
        {
            string rootfolderpath = @"D:\设计一所\项目\Posorja\往来邮件附件\分类整理";
            if (!Directory.Exists(rootfolderpath))
            {
                System.Windows.MessageBox.Show($"Can Not Found Root Folder {rootfolderpath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (StartDatePicker.SelectedDate >= EndDatePicker.SelectedDate)
            {
                System.Windows.MessageBox.Show($"The StartDate must less than EndDate!");
                return;
            }
                

            List<ReportContent> allcontent = new List<ReportContent>();
            foreach (var subfolder in EmailAttachmentsFolder)
            {
                string subfolderpath = System.IO.Path.Combine(rootfolderpath, subfolder);
                var subcontent = GetReportItmes(subfolderpath);
                allcontent.AddRange(subcontent);
            }

            allcontent = (from content in allcontent
                         where content.DocDate >= StartDatePicker.SelectedDate && content.DocDate <= EndDatePicker.SelectedDate
                         select content).ToList();

            allcontent.Sort((a, b) => {
                if (a.DocDate > b.DocDate)
                    return 1;
                else if (a.DocDate == b.DocDate)
                    return a.DocName.CompareTo(b.DocName);
                else return -1; });
            GenerateExcelReport(ExcelFilePathBox.Text, allcontent);

            System.Windows.Forms.MessageBox.Show($"File is save in {ExcelFilePathBox.Text}!");
            ProcessBtn.IsEnabled = false;
        }

    }

    public class ReportContent
    {
        public string DocName { get; set; }
        public DateTime DocDate { get; set; }
        public bool IsCOWI { get; set; }
        public bool ISCGR { get; set; }
        public string Comment { get; set; }
    }
}
