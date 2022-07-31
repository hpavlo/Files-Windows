using Files.Enums;
using Files.Resources.MultilingualResources;
using Files.Services;
using Files.ViewModels.Base;
using Files.ViewModels.TabContentViewModels;
using System.IO;

namespace Files.ViewModels
{
    internal class TabContentViewModel : BaseViewModel
    {
        internal MainViewModel MainViewModel;

        public TopPanelViewModel TopPanelViewModel { get; set; }
        public LeftPanelViewModel LeftPanelViewModel { get; set; }
        public CenterPanelViewModel CenterPanelViewModel { get; set; }

        private string _path;
        public string CurrentPath
        {
            get => _path.Replace('/', '\\');
            set
            {
                if (_path == null)
                    _path = value;
                else
                {
                    _path = value;
                    TopPanelViewModel.PathListChange();

                    if (Directory.Exists(value))
                    {
                        if (CenterPanelViewModel.SearchMode)
                            CenterPanelViewModel.SearchMode = false;
                        PathChanged();
                    }
                    else
                    {
                        MessageDialog.Show(TranslationSource.Instance["TabContentWrongPath"],
                            TranslationSource.Instance["CaptionInformation"],
                            MessageDialogButton.OK, MessageDialogImage.Information);

                        CenterPanelViewModel.PreviousPath();
                    }
                }
            }
        }
        public string Header
        {
            get
            {
                string str = CurrentPath.Split("\\")[^1];
                string header = str == string.Empty ? CurrentPath.Split("\\")[0] : str;
                
                switch (header)
                {
                    case "Desktop": return TranslationSource.Instance["KnownFolderDesktopName"];
                    case "Documents": return TranslationSource.Instance["KnownFolderDocumentsName"];
                    case "Downloads": return TranslationSource.Instance["KnownFolderDownloadsName"];
                    case "Music": return TranslationSource.Instance["KnownFolderMusicName"];
                    case "Pictures": return TranslationSource.Instance["KnownFolderPicturesName"];
                    case "Videos": return TranslationSource.Instance["KnownFolderVideosName"];
                    default: return header;
                }
            }
        }

        public TabContentViewModel(string path, MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            CurrentPath = path;

            TopPanelViewModel = new TopPanelViewModel(this);
            LeftPanelViewModel = new LeftPanelViewModel(this);
            CenterPanelViewModel = new CenterPanelViewModel(this);
        }

        private void PathChanged()
        {
            TopPanelViewModel.PathChanged();
            LeftPanelViewModel.PathChanged();
            CenterPanelViewModel.PathChanged();

            MainViewModel.TabButtonsViewModel.UpdateHeader();
        }
    }
}
