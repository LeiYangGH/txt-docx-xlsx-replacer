using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchOption = System.IO.SearchOption;

namespace TxtWordExcelReplacer.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IReplacer replacer;
        public MainViewModel(IReplacer replacer)
        {
            log.Debug("MainViewModel ctor");
            this.TopDir = "files";
            this.InitDemoWordPairVMs();
            this.replacer = replacer;
        }

        private void InitDemoWordPairVMs()
        {
            this.ObsWordPairVMs = new ObservableCollection<WordPairViewModel>()
            {
                new WordPairViewModel("从","到"),
                new WordPairViewModel("from","to"),
            };
        }

        private string topDir;
        public string TopDir
        {
            get => topDir;
            set => SetProperty(ref topDir, value);
        }

        private ObservableCollection<WordPairViewModel> obsWordPairVMs;
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
        private async Task SearchAndReplace()
        {
            await Task.Run(() =>
            {
                foreach (string file in Directory.GetFiles(this.TopDir, "*", SearchOption.AllDirectories)
                .Where(s => new string[] { ".txt", ".docx", ".xlsx" }.Contains(Path.GetExtension(s))))
                {
                    this.Message = $"开始处理{file}";
                    this.Message = this.replacer.Replace(file, this.ObsWordPairVMs.Where(x => x.IsValid).ToList());
                }
                this.Message = "完成所有文件替换";
            });
        }
    }
}
