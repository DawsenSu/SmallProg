using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using MVVMLightDemo.Model;
using System.Windows;

namespace MVVMLightDemo.ViewModel
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
            Welcome = new Model.WelcomeModel() { Introduction = "fds mvvmlight" };
            TextData = new ObservableCollection<Test>()
            {
                new Test(){ Text= "1"},
                new Test(){ Text= "2"},
                new Test(){ Text= "3"},
                new Test(){ Text= "4"}
            };
        }
        private Model.WelcomeModel welcome;

        public Model.WelcomeModel Welcome
        {
            get { return welcome; }
            set { welcome = value; RaisePropertyChanged(() => Welcome); }
        }

        private ObservableCollection<Test> textData;

        public ObservableCollection<Test> TextData
        {
            get { return textData; }
            set { textData = value; RaisePropertyChanged(() => textData); }
        }

        private RelayCommand<Test> deleteCmd;

        public RelayCommand<Test> DeleteCmd
        {
            get
            {
                if (deleteCmd == null)
                    return new RelayCommand<Test>(DeleteRow);
                return deleteCmd;
            }
            set { deleteCmd = value; }
        }
        private void DeleteRow(Test test)
        {
            TextData.Remove(test);
        }


    }
}
