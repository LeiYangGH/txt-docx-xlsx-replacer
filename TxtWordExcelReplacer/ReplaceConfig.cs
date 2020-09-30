using log4net.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TxtWordExcelReplacer.ViewModel;

namespace TxtWordExcelReplacer
{
    public class ReplaceConfig
    {
        public ReplaceConfig()
        {

        }
        public ReplaceConfig(string topDir, IList<WordPairViewModel> wordPairViewModels)
        {
            this.TopDir = topDir;
            this.ReplacePairs = wordPairViewModels.ToList();
        }

        public string TopDir { get; set; }
        public List<WordPairViewModel> ReplacePairs { get; set; }
    }
}
