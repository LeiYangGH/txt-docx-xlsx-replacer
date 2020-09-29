using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using TxtWordExcelReplacer.ViewModel;

namespace TxtWordExcelReplacer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (DateTime.Now > new DateTime(2020, 10, 8))
            {
                MessageBox.Show("试用期结束，请联系供应商！");
                App.Current.Shutdown();
            }
            else if (DateTime.Now < new DateTime(2020, 9, 29))
            {
                MessageBox.Show("您不能让时间倒流！");
                App.Current.Shutdown();
            }
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo("TxtWordExcelReplacer.dll.config"));
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainViewModel>();
            services.AddScoped<IReplacer, TxtAndNPOIReplacer>();
            Ioc.Default.ConfigureServices(services);
            base.OnStartup(e);
        }
    }
}
