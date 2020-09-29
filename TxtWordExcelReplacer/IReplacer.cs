using System;
using System.Collections.Generic;
using System.Text;
using TxtWordExcelReplacer.ViewModel;

namespace TxtWordExcelReplacer
{
    public interface IReplacer
    {
        string Replace(string fileName, IList<WordPairViewModel> wordPairViewModels);
    }
}
