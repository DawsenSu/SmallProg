using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using BentleyResourceStringReader.Model;
using System.Windows.Forms;
using System.IO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BentleyResourceStringReader.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            StatusString = "Started";
            CurrentFolderPath = null;
            MsgLists = new ObservableCollection<MessageInfo>();
        }
        private string _statusString;
        /// <summary>
        /// Property Description
        /// </summary>
        public string StatusString
        {
            get => _statusString;
            set => Set(ref _statusString, value);
        }

        private string _currentFolderPath;
        /// <summary>
        /// Property Description
        /// </summary>
        public string CurrentFolderPath
        {
            get => _currentFolderPath;
            set => Set(ref _currentFolderPath, value);
        }


        private ObservableCollection<MessageInfo> _msgLists;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<MessageInfo> MsgLists
        {
            get => _msgLists;
            set => Set(ref _msgLists, value);
        }


        private MessageInfo _selectedMsgInfo;
        /// <summary>
        /// Property Description
        /// </summary>
        public MessageInfo SelectedMsgInfo
        {
            get => _selectedMsgInfo;
            set => Set(ref _selectedMsgInfo, value);
        }

        private string _currentFolderName;

        private RelayCommand _openFile;

        /// <summary>
        /// Gets the Open.
        /// </summary>
        public RelayCommand OpenFile
        {
            get
            {
                return _openFile
                    ?? (_openFile = new RelayCommand(
                    () =>
                    {
                        var folderBrowserDialog = new FolderBrowserDialog
                        {
                            Description = "Select a directory containing resource (.r) files",
                            ShowNewFolderButton = true,
                            RootFolder = System.Environment.SpecialFolder.Desktop
                        };
                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            CurrentFolderPath = folderBrowserDialog.SelectedPath;

                            _currentFolderName = CurrentFolderPath.Split('\\').Last();
                            StatusString = _currentFolderName + " opened!";
                            string englishResourceFileName = Path.Combine(CurrentFolderPath, "english", _currentFolderName + "msg.r");
                            string chineseResourceFileName = Path.Combine(CurrentFolderPath, "chinese", _currentFolderName + "msg.r");
                            if (File.Exists(englishResourceFileName) && File.Exists(chineseResourceFileName))
                            {
                                var englishResult = ReadResourceFile(englishResourceFileName);
                                var chineseResult = ReadResourceFile(chineseResourceFileName, false);
                                var mergedResult = MergeTwoLanguages(englishResult, chineseResult);

                                MsgLists = new ObservableCollection<MessageInfo>(mergedResult);
                            }
                        }
                    }));
            }
        }


        private RelayCommand _save;

        /// <summary>
        /// Gets the Save.
        /// </summary>
        public RelayCommand Save
        {
            get
            {
                return _save
                    ?? (_save = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            if (!isMsgListValid(out string duplicatestrings))
                            {
                                MessageBox.Show("List contains duplicate strings:\r\n" + duplicatestrings,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                                StatusString = "Error!";
                            }
                            else
                            {
                                WriteResourcesAndHeaderFiles();

                                StatusString = "Save Success!";
                            }
                        }
                        catch (Exception e)
                        {
                            StatusString = "Error!";
                            MessageBox.Show(e.Message);
                        }
                    },
                    () => MsgLists.Count != 0));
            }
        }


        private RelayCommand _addNewTab;

        /// <summary>
        /// Gets the AddNewTab.
        /// </summary>
        public RelayCommand AddNewTab
        {
            get
            {
                return _addNewTab
                    ?? (_addNewTab = new RelayCommand(
                    () =>
                    {
                        MsgLists.Add(new MessageInfo
                            {Name = "MessageListName", Entries = new ObservableCollection<MessageEntry>()});
                    },
                    ()=>CurrentFolderPath != null));
            }
        }

        private RelayCommand _deleteCurrentTab;

        /// <summary>
        /// Gets the DeleteCurrentTab
        /// </summary>
        public RelayCommand DeleteCurrentTab
        {
            get
            {
                return _deleteCurrentTab
                    ?? (_deleteCurrentTab = new RelayCommand(
                    () =>
                    {
                        MsgLists.Remove(SelectedMsgInfo);
                    },
                    () => SelectedMsgInfo != null));
            }
        }
        #region functions

        private bool isMsgListValid(out string duplicatedStrings)
        {
            List<string> names = new List<string>();
            duplicatedStrings = string.Empty;

            foreach (var messageInfo in MsgLists)
            {
                names.Add(messageInfo.Name);
                foreach (var entry in messageInfo.Entries)
                {
                    names.Add(entry.MsgName);
                }
            }

            var distinctNames = names.Distinct();
            var duplicatedNames = names.GroupBy(x=>x).Where(g=>g.Count()>1).Select(y=>y.Key);
            foreach (var name in duplicatedNames)
            {
                duplicatedStrings += name + "\r\n";
            }
            return !duplicatedNames.Any();
        }

        private List<MessageInfo> ReadResourceFile(string filePath, bool isEnglish = true)
        {
            var messageInfoList = new List<MessageInfo>();

            var rLines = !isEnglish ? File.ReadLines(filePath, Encoding.GetEncoding("gb2312")) : File.ReadLines(filePath);

                string msgListName = string.Empty;
            MessageInfo msgInfo = new MessageInfo();

            var listNamePattern = "\\s*MessageList\\s*(?<MsgListName>\\w*)\\s*=\\s*";
            var listNameRegex = new Regex(listNamePattern, RegexOptions.None, TimeSpan.FromMilliseconds(1000));

            var entryPattern = "(?<=\\{)[^}]*(?=\\})";
            var entryRegex = new Regex(entryPattern, RegexOptions.None, TimeSpan.FromMilliseconds(1000));

            var listEndPattern = @"\w*\};\w*";
            var listEndRegex = new Regex(listEndPattern, RegexOptions.None, TimeSpan.FromMilliseconds(1000));

            foreach (var l in rLines)
            {
                //Match the listName
                var match = listNameRegex.Match(l);
                if (match.Success)
                {
                    msgInfo = new MessageInfo
                    {
                        Name = match.Groups["MsgListName"].Value,
                        Entries = new ObservableCollection<MessageEntry>()
                    };
                    continue;
                }

                //Match list enrty
                var entryMatch = entryRegex.Match(l);
                if (entryMatch.Success)
                {
                    var str = entryMatch.Value;
                    var splitStrs = str.Split(',');
                    var entry = new MessageEntry { MsgName = splitStrs[0].Trim() };
                    if (isEnglish)
                        entry.English = splitStrs[1].Trim().Trim('"');
                    else
                        entry.Chinese = splitStrs[1].Trim().Trim('"');
                    msgInfo.Entries.Add(entry);
                    continue;
                }

                var listEndMatch = listEndRegex.Match(l);
                if (listEndMatch.Success)
                {
                    messageInfoList.Add(msgInfo);
                    msgInfo = new MessageInfo();
                }
            }
            return messageInfoList;
        }

        private string GB2312ToUtf8(string gb2312String)
        {
            Encoding fromEncoding = Encoding.GetEncoding("gb2312");
            Encoding toEncoding = Encoding.UTF8;
            return EncodingConvert(gb2312String, fromEncoding, toEncoding);
        }

        private string Utf8ToGB2312(string utf8String)
        {
            Encoding fromEncoding = Encoding.UTF8;
            Encoding toEncoding = Encoding.GetEncoding("gb2312");
            return EncodingConvert(utf8String, fromEncoding, toEncoding);
        }

        private string EncodingConvert(string fromString, Encoding fromEncoding, Encoding toEncoding)
        {
            byte[] fromBytes = fromEncoding.GetBytes(fromString);
            byte[] toBytes = Encoding.Convert(fromEncoding, toEncoding, fromBytes);

            string toString = toEncoding.GetString(toBytes);
            return toString;
        }

        private List<MessageInfo> MergeTwoLanguages(List<MessageInfo> englishInfos, List<MessageInfo> chineseInfos)
        {
            List<MessageInfo> mergedList = new List<MessageInfo>();
            foreach (var englishInfo in englishInfos)
            {
                var cinfo = (from chineseInfo in chineseInfos where englishInfo.Name == chineseInfo.Name select chineseInfo).ToList();
    
                if (cinfo.Count == 1)
                {
                    foreach (var entry in englishInfo.Entries)
                    {
                        var cEntry = (from chineseEntry in cinfo.First().Entries
                            where entry.MsgName == chineseEntry.MsgName
                            select chineseEntry).ToList();
                        if (cEntry.Count == 1)
                            entry.Chinese = cEntry.First().Chinese;
                    }
                }
                mergedList.Add(englishInfo);
            }

            return mergedList;
        }

        private void WriteResourcesAndHeaderFiles()
        {
            string resourceFileHeader = $"/*----------------------------------------------------------------------+\r\n| {_currentFolderName} Application message resource\t\t\t\t\t|\r\n+----------------------------------------------------------------------*/\r\n#include <Mstn\\MdlApi\\rscdefs.r.h>\r\n#include \"..\\{_currentFolderName + "ids.h"}\"\r\n";

            string headerFileHeader = $"#pragma once\r\n/*\r\n * {_currentFolderName} application IDs\r\n */\r\n";

            StringBuilder englishRBuilder = new StringBuilder(resourceFileHeader), chineseRBuilder = new StringBuilder(resourceFileHeader), headerBuilder = new StringBuilder(headerFileHeader);

            uint idNumber = 1;
            foreach (var info in MsgLists)
            {
                headerBuilder.AppendLine($"#define {info.Name}\t\t\t\t\t\t{idNumber}");
                ++idNumber;
            }
            foreach (var info in MsgLists)
            {
                englishRBuilder.AppendLine();
                chineseRBuilder.AppendLine();
                headerBuilder.AppendLine();
                string headerInfoHeader =
                    $"/*----------------------------------------------------------------------+\r\n |   {info.Name} IDS                                                     |\r\n+----------------------------------------------------------------------*/";
                headerBuilder.AppendLine(headerInfoHeader);
                englishRBuilder.Append(info.OutputString());
                chineseRBuilder.Append(info.OutputString(false));

                foreach (var entry in info.Entries)
                {
                    headerBuilder.AppendLine($"#define {entry.MsgName}\t\t\t\t\t\t{idNumber}");
                    ++idNumber;
                }
            }

            string englishResourceFilePath = Path.Combine(CurrentFolderPath, "english", _currentFolderName + "msg.r");
            string chineseResourceFilePath = Path.Combine(CurrentFolderPath, "chinese", _currentFolderName + "msg.r");
            string headerFilePath = Path.Combine(CurrentFolderPath, _currentFolderName + "ids.h");
            if (!File.Exists(englishResourceFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(englishResourceFilePath));
            }
            if (!File.Exists(chineseResourceFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(chineseResourceFilePath));
            }

            if (!File.Exists(headerFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(headerFilePath));
            }

            File.WriteAllText(englishResourceFilePath, englishRBuilder.ToString());
            File.WriteAllText(chineseResourceFilePath, chineseRBuilder.ToString(), Encoding.GetEncoding("gb2312"));
            File.WriteAllText(headerFilePath, headerBuilder.ToString());
        }

        #endregion

    }

    public class MessageInfo : ObservableObject
    {

        private string _name;
        /// <summary>
        /// Property Description
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private ObservableCollection<MessageEntry> _entries;
        /// <summary>
        /// Property Description
        /// </summary>
        public ObservableCollection<MessageEntry> Entries
        {
            get => _entries;
            set => Set(ref _entries, value);
        }

        public string OutputString(bool isEnglish = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"/*----------------------------------------------------------------------+");
            sb.AppendLine($"| {Name} Message List									                |");
            sb.AppendLine(@"+-----------------------------------------------------------------------*/");
            sb.AppendLine($"MessageList {Name} =");
            sb.AppendLine("{");
            sb.AppendLine("\t{");
            foreach (var entry in Entries)
            {
                if(entry == Entries.Last())
                    sb.AppendLine(entry.OutputString(isEnglish).TrimEnd(','));
                else
                    sb.AppendLine(entry.OutputString(isEnglish));
            }
            sb.AppendLine("\t}");
            sb.AppendLine("};");
            sb.AppendLine("");

            return sb.ToString();
        }

    }

    public class MessageEntry : ObservableObject
    {
        private string _msgName;
        /// <summary>
        /// Property Description
        /// </summary>
        public string MsgName
        {
            get => _msgName;
            set => Set(ref _msgName, value);
        }

        private string _english;
        /// <summary>
        /// Property Description
        /// </summary>
        public string English
        {
            get => _english;
            set => Set(ref _english, value);
        }

        private string _chinese;
        /// <summary>
        /// Property Description
        /// </summary>
        public string Chinese
        {
            get => _chinese;
            set => Set(ref _chinese, value);
        }

       public string OutputString(bool isEnglish = true)
        {
            string content = isEnglish ? English : Chinese;
            return $"\t\t{{{MsgName},\t\t\t\"{content}\"}},";
        }
    }
}