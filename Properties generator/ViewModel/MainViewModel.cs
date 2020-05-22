using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Properties_generator.Model;

namespace Properties_generator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    InputTexts = string.Empty;
                    OutputTexts = string.Empty;
                });
        }


        private string _inputTexts;
        /// <summary>
        /// Property Description
        /// </summary>
        public string InputTexts
        {
            get { return _inputTexts; }
            set { Set(ref _inputTexts, value); }
        }

        private string _outputTexts;
        /// <summary>
        /// Property Description
        /// </summary>
        public string OutputTexts
        {
            get { return _outputTexts; }
            set { Set(ref _outputTexts, value); }
        }

        private RelayCommand _generate;

        /// <summary>
        /// Gets the Generate
        /// </summary>
        public RelayCommand Generate
        ///
        {
            get
            {
                return _generate
                    ?? (_generate = new RelayCommand(
                    () =>
                    {
                        OutputTexts = string.Empty;
                        var stringSeparators = new[] { "\r\n" };
                        var strs= InputTexts.Split(stringSeparators, StringSplitOptions.None);
                        foreach (var pair in strs)
                        {
                            string[] declarationPair = pair.Split(';');
                            string firstUpperStr = ToFirstLetterUpper(declarationPair[1]);
                            OutputTexts +=
                                $"private {declarationPair[0]} _{declarationPair[1]};\r\npublic {declarationPair[0]} {firstUpperStr}\r\n{{\r\n    get => _{declarationPair[1]};\r\n    set => Set(ref _{declarationPair[1]}, value); \r\n}}\r\n\r\n";
                        }
                    }));
            }
        }

        public string ToFirstLetterUpper(string str)
        {
            string outstring = string.Empty;
            if (str.Length == 0)
                outstring = string.Empty;
            else if (str.Length == 1)
                outstring =char.ToUpper(str[0]).ToString();
            else
                outstring = char.ToUpper(str[0]) + str.Substring(1);

            return outstring;
        }
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}