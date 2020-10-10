using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TxtWordExcelReplacer.ViewModel;

namespace TxtWordExcelReplacer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private MainViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
            DataContext = mainViewModel;
            this.ShowVersion();
            log.Debug("主窗口开始运行!");
        }

        private void ShowVersion()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly()
                                           .GetName()
                                           .Version
                                           .ToString();
            this.Title += " -" + version;

            log.Info($"---------------------------------");
            log.Info($"当前版本{version}");
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainViewModel.SaveConfigs();
        }
    }
}
