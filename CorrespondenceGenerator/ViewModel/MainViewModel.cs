using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using System;
using System.IO;
using System.Windows;
using System.Text;
using System.Configuration;
using CorrespondenceGenerator.Properties;
using System.Diagnostics;
using System.IO.Compression;

using Aspose.Words;
using Aspose.Words.Saving;
using Aspose.Words.Reporting;


namespace CorrespondenceGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            var appSettings = ConfigurationManager.AppSettings;
            correspondencestoragefolder = new DirectoryInfo(appSettings["CorrespondenceStorageFolder"]);
            correspondencestemplate = new FileInfo(appSettings["CorrespondenceTemplate"]);
            approvalformstoragefolder = new DirectoryInfo(appSettings["ApprovalFormStorageFolder"]);
            approvalformstemplate = new FileInfo(appSettings["ApprovalFormTemplate"]);

        }


        /// <summary>
        /// The <see cref="Subject" /> property's name.
        /// </summary>
        public const string SubjectPropertyName = "Subject";

        private string _subject = string.Empty;

        /// <summary>
        /// Sets and gets the Subject property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                Set(() => Subject, ref _subject, value);
            }
        }

        /// <summary>
        /// The <see cref="AttachmentNames" /> property's name.
        /// </summary>
        public const string AttachmentNamesPropertyName = "AttachmentNames";

        private string _attachments = string.Empty;

        /// <summary>
        /// Sets and gets the AttachmentNames property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string AttachmentNames
        {
            get
            {
                return _attachments;
            }
            set
            {
                Set(() => AttachmentNames, ref _attachments, value);
            }
        }

        /// <summary>
        /// The <see cref="IsReply" /> property's name.
        /// </summary>
        public const string IsReplyPropertyName = "IsReply";

        private bool _isreply = false;

        /// <summary>
        /// Sets and gets the IsReply property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsReply
        {
            get
            {
                return _isreply;
            }
            set
            {
                Set(() => IsReply, ref _isreply, value);
            }
        }

        /// <summary>
        /// The <see cref="ReplyCorrespondence" /> property's name.
        /// </summary>
        public const string ReplyCorrespondencePropertyName = "ReplyCorrespondence";

        private string _replycorrespondence = string.Empty;

        /// <summary>
        /// Sets and gets the ReplyCorrespondence property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ReplyCorrespondence
        {
            get
            {
                return _replycorrespondence;
            }
            set
            {
                Set(() => ReplyCorrespondence, ref _replycorrespondence, value);
            }
        }

        /// <summary>
        /// The <see cref="Remark" /> property's name.
        /// </summary>
        public const string RemarkPropertyName = "Remark";

        private string _remark = string.Empty;

        /// <summary>
        /// Sets and gets the Remark property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Remark
        {
            get
            {
                return _remark;
            }
            set
            {
                Set(() => Remark, ref _remark, value);
            }
        }

        /// <summary>
        /// The <see cref="Prompt" /> property's name.
        /// </summary>
        public const string PromptPropertyName = "Prompt";

        private string _prompt = string.Empty;

        /// <summary>
        /// Sets and gets the Prompt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Prompt
        {
            get
            {
                return _prompt;
            }
            set
            {
                Set(() => Prompt, ref _prompt, value);
            }
        }

        private DirectoryInfo correspondencestoragefolder;
        private FileInfo correspondencestemplate;
        private DirectoryInfo approvalformstoragefolder;
        private FileInfo approvalformstemplate;
        private int numberofattachments = 0;
        private string[] attachmentfilenames;
        private RelayCommand _selectattachmentscommand;

        /// <summary>
        /// Gets the SelectAttachmentsCommand.
        /// </summary>
        public RelayCommand SelectAttachmentsCommand
        {
            get
            {
                return _selectattachmentscommand
                    ?? (_selectattachmentscommand = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog()
                            {
                                Title = "Choose the attachments",
                                Multiselect = true
                            };
                            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                numberofattachments = openFileDialog.FileNames.Length;
                                attachmentfilenames = openFileDialog.FileNames;
                                StringBuilder sb = new StringBuilder();
                                if (openFileDialog.SafeFileNames.Length == 1)
                                {
                                    sb.Append("The attachment is listed below.\n");
                                }
                                else if (openFileDialog.SafeFileNames.Length > 1)
                                {
                                    sb.Append("The attachments are listed below.\n");
                                }

                                for (int i = 0; i < openFileDialog.SafeFileNames.Length; i++)
                                {
                                    if (i == openFileDialog.SafeFileNames.Length - 1)
                                    {
                                        sb.Append((i + 1).ToString() + ") " + openFileDialog.SafeFileNames[i] + ".\n");
                                    }
                                    else
                                    {
                                        sb.Append((i + 1).ToString() + ") " + openFileDialog.SafeFileNames[i] + ";\n");
                                    }
                                }
                                AttachmentNames = sb.ToString();
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }));
            }
        }

        private void MergeCorrespondenceFields(string subject, int attachmentno, string content, string attachments, FileInfo templatefile, FileInfo fileInfo)
        {
            string attno = string.Empty;
            if (attachmentno > 0)
                attno = attachmentno == 1 ? $"+ {attachmentno} Attachment" : $"+ {attachmentno} Attachments";


            Document templatedoc = new Document(templatefile.FullName);
            templatedoc.MailMerge.Execute(
                new string[] { "Subject", "AttachmentNum", "Content", "Attachments" },
                new object[] { subject, attno, content, attachments });

            templatedoc.Save(fileInfo.FullName, SaveFormat.Docx);
        }

        private void MergeApprovalFormFields(string subject, string attachments, FileInfo approvalformfile, FileInfo fileInfo)
        {
            string content = string.Empty;
            content = Remark + "\n" + attachments;

            Document formdoc = new Document(approvalformfile.FullName);
            formdoc.MailMerge.Execute(
                new string[] { "Subject", "Content", "Date" },
                new object[] { subject, content, DateTime.Now.ToString("yyyy-MM-dd") });

            formdoc.Save(fileInfo.FullName, SaveFormat.Docx);
        }


        DirectoryInfo correspondencesubdir;
        FileInfo correspondencefilename;
        DirectoryInfo approvalformsubdir;
        bool iscreatecorres = false;
        private RelayCommand _createCommand;

        /// <summary>
        /// Gets the CreateCommand.
        /// </summary>
        public RelayCommand CreateCommand
        {
            get
            {
                return _createCommand
                    ?? (_createCommand = new RelayCommand(ExecuteCreateCommand, CanExecuteCreateCommand));
            }
        }

        private void ExecuteCreateCommand()
        {
            string content = string.Empty;
            if (IsReply && ReplyCorrespondence.Length >= 6)
            {
                string year = ReplyCorrespondence.Substring(0, 4);
                string month = ReplyCorrespondence.Substring(4, 2);
                string day = ReplyCorrespondence.Substring(6, 2);
                try
                {
                    DateTime engdate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                    content = $"We refer to Engineer's correspondence ref. {ReplyCorrespondence} dated at {engdate.ToString("dd MMMM, yyyy")} regarding above subject.";
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Can't retrieve ENG date", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            string subdirname = $"{DateTime.Now.ToString("yyyyMMdd")} - {Subject}";
            if (IsReply)
            {
                subdirname = $"{DateTime.Now.ToString("yyyyMMdd")} - reply for {ReplyCorrespondence.Substring(17)} {Subject}";
            }
            correspondencesubdir = correspondencestoragefolder.CreateSubdirectory(subdirname);
            string corresfilename = DateTime.Now.ToString("yyyyMMdd") + " XXXX - " + Subject + ".docx";
            correspondencefilename = new FileInfo(correspondencesubdir.FullName + "\\" + corresfilename);

            MergeCorrespondenceFields(Subject, numberofattachments, content, AttachmentNames, correspondencestemplate, correspondencefilename);

            approvalformsubdir = approvalformstoragefolder.CreateSubdirectory("[Internal]" + subdirname);
            string apffilename = DateTime.Now.ToString("yyyyMMdd") + " APF - " + Subject + ".docx";

            MergeApprovalFormFields(Subject, AttachmentNames, approvalformstemplate, new FileInfo(approvalformsubdir.FullName + "\\" + apffilename));
            if (attachmentfilenames !=null && attachmentfilenames.Length > 0)
            {
                foreach (var file in attachmentfilenames)
                {
                    File.Copy(file, correspondencesubdir.FullName + "\\" + Path.GetFileName(file));
                }
            }

            Prompt = "Create Successfully!";
            iscreatecorres = true;
        }

        private bool CanExecuteCreateCommand()
        {
            if (!string.IsNullOrEmpty(Subject))
            {
                if (IsReply)
                    return ReplyCorrespondence.Length > 6;
                else
                    return true;
            }
            else
                return false;
        }


        private RelayCommand _editcommand;

        /// <summary>
        /// Gets the EditCommand.
        /// </summary>
        public RelayCommand EditCommand
        {
            get
            {
                return _editcommand
                    ?? (_editcommand = new RelayCommand(
                    () =>
                    {
                        Process.Start(correspondencefilename.FullName);
                        Prompt = "Start Editing!";
                        iscreatecorres = false;
                    },
                    () => iscreatecorres));
            }
        }

        bool iscopied = false;
        private RelayCommand _copycommand;

        /// <summary>
        /// Gets the CopyCommand.
        /// </summary>
        public RelayCommand CopyCommand
        {
            get
            {
                return _copycommand
                    ?? (_copycommand = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            File.Copy(correspondencefilename.FullName, approvalformsubdir.FullName + "\\" + correspondencefilename.Name);
                            Prompt = "Copy Successfully!";
                            iscopied = true;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }));
            }
        }

        private RelayCommand _compressCommand;

        /// <summary>
        /// Gets the MyCommand.
        /// </summary>
        public RelayCommand CompressCommand
        {
            get
            {
                return _compressCommand
                    ?? (_compressCommand = new RelayCommand(
                    () =>
                    {
                        ZipFile.CreateFromDirectory(correspondencesubdir.FullName, correspondencestoragefolder.FullName + "//" + correspondencesubdir.Name + ".zip");
                        ZipFile.CreateFromDirectory(approvalformsubdir.FullName, approvalformstoragefolder.FullName + "//" + approvalformsubdir.Name + ".zip");
                        Prompt = "Compression Successfully!";
                        iscopied = false;
                    },
                    () => iscopied));
            }
        }

    }
}