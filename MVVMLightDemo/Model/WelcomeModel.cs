using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MVVMLightDemo.Model
{
    public class WelcomeModel : ObservableObject
    {
        private string introduction;

        public string Introduction
        {
            get { return introduction; }
            set { introduction = value; RaisePropertyChanged(() => Introduction); }
        }

    }

    public class Test : ObservableObject
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; RaisePropertyChanged(() => text); }
        }

    }
}
