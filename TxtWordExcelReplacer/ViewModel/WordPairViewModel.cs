using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TxtWordExcelReplacer.ViewModel
{
    public class WordPairViewModel : ObservableObject
    {
        public WordPairViewModel()
        {
            this.SrcWord = "";
            this.DesWord = "";
        }
        public WordPairViewModel(string srcWord, string desWord)
        {
            this.SrcWord = srcWord;
            this.DesWord = desWord;
        }
        private string srcWord;
        public string SrcWord
        {
            get => srcWord;
            set => SetProperty(ref srcWord, value);
        }

        private string desWord;
        public string DesWord
        {
            get => desWord;
            set => SetProperty(ref desWord, value);
        }

        public bool IsValid
        {
            get => !string.IsNullOrWhiteSpace(this.SrcWord)
                && !string.IsNullOrWhiteSpace(this.DesWord)
                && this.SrcWord != this.DesWord;
        }

    }
}
