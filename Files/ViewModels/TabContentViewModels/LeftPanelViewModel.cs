using Files.Models;
using Files.Services;
using Files.ViewModels.Base;
using Files.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Files.ViewModels.TabContentViewModels
{
    internal class LeftPanelViewModel : BaseViewModel
    {
        internal TabContentViewModel TabContentViewModel;

        private GetDrives GetDrivesService;

        public ObservableCollection<DriveDetails> Drives { get; set; }
        public ObservableCollection<FavoriteDetails> Favorites { get; set; }
        public string Path
        {
            get => TabContentViewModel.CurrentPath;
            set => TabContentViewModel.CurrentPath = value;
        }

        public LeftPanelViewModel(TabContentViewModel tabContentViewModel)
        {
            TabContentViewModel = tabContentViewModel;

            Drives = new ObservableCollection<DriveDetails>();
            Favorites = new ObservableCollection<FavoriteDetails>();

            GetDrivesService = new GetDrives(UpdateDrivesList);

            UpdateDrivesList();
            UpdateFavoritesList();
        }

        public void PathChanged() => OnPropertyChanged(nameof(Path));

        public void UpdateDrivesList()
        {
            GetDrivesService.GetGrivesList(Drives);
            OnPropertyChanged(nameof(Drives));
        }

        public void UpdateFavoritesList()
        {
            GetFavorites.GetFolders(Favorites);
            OnPropertyChanged(nameof(Favorites));
        }


        private ICommand _goToDriveCommand;
        public ICommand GoToDriveCommand => _goToDriveCommand ??
                    (_goToDriveCommand = new RelayCommand(obj => Path = (string)obj));

        private ICommand _goToFavoriteCommand;
        public ICommand GoToFavoriteCommand => _goToFavoriteCommand ??
                    (_goToFavoriteCommand = new RelayCommand(obj => Path = (string)obj));
    }
}
