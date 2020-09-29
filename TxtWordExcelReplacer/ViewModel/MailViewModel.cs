using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TxtWordExcelReplacer.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainViewModel()
        {
            log.Debug("MainViewModel ctor");
        }
    }
}
