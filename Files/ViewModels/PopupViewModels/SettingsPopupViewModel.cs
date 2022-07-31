using Files.Interfaces;
using Files.Resources.MultilingualResources;
using Files.Styles;
using Files.ViewModels.Base;
using Files.ViewModels.Commands;
using System;
using System.Globalization;
using System.Windows.Input;

namespace Files.ViewModels.PopupViewModels
{
    internal class SettingsPopupViewModel : BaseViewModel, IPopupViewModel
    {
        private Action<bool> Callback;

        private bool _isShowing = false;
        public bool IsShowing
        {
            get => _isShowing;
            set => Set(ref _isShowing, value);
        }

        private bool _isLightThemeSelected;
        public bool IsLightThemeSelected
        {
            get => _isLightThemeSelected;
            set
            {
                if (Set(ref _isLightThemeSelected, value) && value)
                {
                    Themes.SetTheme(Theme.Light, true);
                    Properties.Settings.Default.DarkMode = false;
                }
            }
        }

        private bool _isDarkThemeSelected;
        public bool IsDarkThemeSelected
        {
            get => _isDarkThemeSelected;
            set
            {
                if (Set(ref _isDarkThemeSelected, value) && value)
                {
                    Themes.SetTheme(Theme.Dark, true);
                    Properties.Settings.Default.DarkMode = true;
                }
            }
        }

        private bool _isENUSLanguageSelected;
        public bool IsENUSLanguageSelected
        {
            get => _isENUSLanguageSelected;
            set
            {
                if (Set(ref _isENUSLanguageSelected, value) && value)
                {
                    TranslationSource.Instance.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                    Properties.Settings.Default.Language = "en-US";
                }
            }
        }

        private bool _isUKUALanguageSelected;
        public bool IsUKUALanguageSelected
        {
            get => _isUKUALanguageSelected;
            set
            {
                if (Set(ref _isUKUALanguageSelected, value) && value)
                {
                    TranslationSource.Instance.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
                    Properties.Settings.Default.Language = "uk-UA";
                }
            }
        }

        private bool oldShowHiddenFiles;
        private bool _isShowHiddenFilesSelected;
        public bool IsShowHiddenFilesSelected
        {
            get => _isShowHiddenFilesSelected;
            set
            {
                if (Set(ref _isShowHiddenFilesSelected, value))
                    Properties.Settings.Default.ShowHiddenFiles = value;
            }
        }

        private bool _isOpenLastTabsSelected;
        public bool IsOpenLastTabsSelected
        {
            get => _isOpenLastTabsSelected;
            set
            {
                if (Set(ref _isOpenLastTabsSelected, value))
                    Properties.Settings.Default.OpenLastTabs = value;
            }
        }

        public void Init(Action<bool> callback)
        {
            IsShowing = true;
            Callback = callback;

            IsLightThemeSelected = !Properties.Settings.Default.DarkMode;
            IsDarkThemeSelected = Properties.Settings.Default.DarkMode;

            IsENUSLanguageSelected = Properties.Settings.Default.Language == "en-US";
            IsUKUALanguageSelected = Properties.Settings.Default.Language == "uk-UA";

            oldShowHiddenFiles = Properties.Settings.Default.ShowHiddenFiles;
            IsShowHiddenFilesSelected = oldShowHiddenFiles;

            IsOpenLastTabsSelected = Properties.Settings.Default.OpenLastTabs;
        }


        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??
                    (_closeCommand = new RelayCommand(obj =>
                    {
                        IsShowing = false;
                        Callback(IsShowHiddenFilesSelected != oldShowHiddenFiles);
                    }));
    }
}
