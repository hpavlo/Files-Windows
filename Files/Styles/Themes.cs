using System;
using System.Windows;

namespace Files.Styles
{
    public static class Themes
    {
        /// <summary>
        /// Select theme for application
        /// </summary>
        /// <param name="theme">Types of theme</param>
        /// <param name="remove">Can we remove last dictionarie before adding</param>
        public static void SetTheme(Theme theme, bool remove)
        {
            switch (theme)
            {
                case Theme.Light:
                    ApplyThame(new Uri("../Styles/LightTheme.xaml", UriKind.Relative), remove);
                    Properties.Settings.Default.DarkMode = false;
                    break;
                case Theme.Dark:
                    ApplyThame(new Uri("../Styles/DarkTheme.xaml", UriKind.Relative), remove);
                    Properties.Settings.Default.DarkMode = true;
                    break;
            }            
        }

        private static void ApplyThame(Uri uri, bool remove)
        {
            if (remove) Application.Current.Resources.MergedDictionaries.RemoveAt(
                Application.Current.Resources.MergedDictionaries.Count - 1);

            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }

    public enum Theme
    {
        Light,
        Dark
    }
}
