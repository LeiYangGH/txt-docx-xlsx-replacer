using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
//using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using log4net.Repository.Hierarchy;

namespace TxtWordExcelReplacer.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IReplacer replacer;
        private string configFile = "replaceconfigs.json";
        public MainViewModel(IReplacer replacer)
        {
            log.Debug("MainViewModel ctor");
            this.TopDir = "files";
            this.InitDemoWordPairVMs();
            this.replacer = replacer;
        }

        private void InitDemoWordPairVMs()
        {
            this.LoadConfigs();
            //this.ObsWordPairVMs = new ObservableCollection<WordPairViewModel>()
            //{
            //    new WordPairViewModel("从","到"),
            //    new WordPairViewModel("from","to"),
            //};
        }

        private string topDir;
        public string TopDir
        {
            get => topDir;
            set => SetProperty(ref topDir, value);
        }

        private ObservableCollection<WordPairViewModel> obsWordPairVMs = new ObservableCollection<WordPairViewModel>();
        public ObservableCollection<WordPairViewModel> ObsWordPairVMs
        {
            get => obsWordPairVMs;
            set => SetProperty(ref obsWordPairVMs, value);
        }

        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }


        private bool isDirBrowsing;
        private RelayCommand dirBrowseCommand;

        public RelayCommand DirBrowseCommand
        {
            get
            {
                return dirBrowseCommand
                  ?? (dirBrowseCommand = new RelayCommand(
                      () =>
                    {
                        if (isDirBrowsing)
                        {
                            return;
                        }

                        isDirBrowsing = true;
                        DirBrowseCommand.NotifyCanExecuteChanged();

                        DirBrowse();

                        isDirBrowsing = false;
                        DirBrowseCommand.NotifyCanExecuteChanged();
                    },
                    () => !isDirBrowsing));
            }
        }
        private void DirBrowse()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (!(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK))
            {
                return;
            }
            this.TopDir = dialog.SelectedPath;
        }

        private bool isWordAdding;
        private RelayCommand wordAddCommand;

        public RelayCommand WordAddCommand
        {
            get
            {
                return wordAddCommand
                  ?? (wordAddCommand = new RelayCommand(
                    async () =>
                    {
                        if (isWordAdding)
                        {
                            return;
                        }

                        isWordAdding = true;
                        WordAddCommand.NotifyCanExecuteChanged();

                        await WordAdd();

                        isWordAdding = false;
                        WordAddCommand.NotifyCanExecuteChanged();
                    },
                    () => !isWordAdding));
            }
        }
        private async Task WordAdd()
        {
            await Task.Run(() =>
            {
                if (this.ObsWordPairVMs.All(x => x.IsValid))
                {

                    if (App.Current != null)//walkaround
                        App.Current.Dispatcher.BeginInvoke(new Action(
                            () =>
                            {
                                this.ObsWordPairVMs.Add(new WordPairViewModel());
                            }));
                }

            });
        }

        private bool isSearchAndReplacing;
        private RelayCommand searchAndReplaceCommand;

        public RelayCommand SearchAndReplaceCommand
        {
            get
            {
                return searchAndReplaceCommand
                  ?? (searchAndReplaceCommand = new RelayCommand(
                    async () =>
                    {
                        if (isSearchAndReplacing)
                        {
                            return;
                        }

                        isSearchAndReplacing = true;
                        SearchAndReplaceCommand.NotifyCanExecuteChanged();

                        await SearchAndReplace();

                        isSearchAndReplacing = false;
                        SearchAndReplaceCommand.NotifyCanExecuteChanged();
                    },
                    () => !isSearchAndReplacing));
            }
        }

        private void WorkInFolder(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories)
.Where(s => new string[] { ".txt", ".docx", ".xlsx" }.Contains(Path.GetExtension(s.Name))))
            {
                if (Helpers.IsFileLocked(fi))
                {
                    log.Info($"{fi.FullName}被其他程序占用，跳过");
                    continue;
                }
                log.Info($"开始处理{fi.FullName}");
                this.Message = $"开始处理{fi.FullName}";
                this.Message = this.replacer.Replace(fi.FullName, this.ObsWordPairVMs.Where(x => x.IsValid).ToList());
                log.Info($"结束处理{fi.FullName}");

            }
            foreach (DirectoryInfo subdir in di.GetDirectories())
            {
                log.Info($"开始处理文件夹{subdir.FullName}");
                this.Message = $"开始处理文件夹{subdir.FullName}";
                WorkInFolder(subdir);
                log.Info($"结束处理文件夹{subdir.FullName}");
                this.Message = $"结束处理文件夹{subdir.FullName}";
            }
        }

        private async Task SearchAndReplace()
        {
            log.Info($"开始查找替换");
            await Task.Run(() =>
            {
                WorkInFolder(new DirectoryInfo(this.TopDir));
                this.Message = "完成所有文件替换";
            });
        }

        public void SaveConfigs()
        {
            try
            {
                ReplaceConfig replaceConfig = new ReplaceConfig(this.TopDir, this.ObsWordPairVMs);
                string jsonString = JsonSerializer.Serialize(replaceConfig);
                File.WriteAllText(this.configFile, jsonString);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

        }

        public void LoadConfigs()
        {
            try
            {
                if (!File.Exists(this.configFile))
                {
                    return;
                }
                string jsonString = File.ReadAllText(this.configFile);
                ReplaceConfig replaceConfig = JsonSerializer.Deserialize<ReplaceConfig>(jsonString);
                this.TopDir = replaceConfig.TopDir;
                this.ObsWordPairVMs = new ObservableCollection<WordPairViewModel>(replaceConfig.ReplacePairs);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

        }
    }
}
