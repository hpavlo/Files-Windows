using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace Files.Resources.MultilingualResources
{
    //https://www.codinginfinity.me/posts/localization-of-a-wpf-app-the-simple-approach/
    public class TranslationSource : INotifyPropertyChanged
    {
        private static readonly TranslationSource instance = new TranslationSource();

        public static TranslationSource Instance
        {
            get => instance;
        }

        private readonly ResourceManager resManager = files.ResourceManager;
        private CultureInfo currentCulture = CultureInfo.CurrentUICulture;

        private static string standartCulture = "en-US";

        public string this[string key]
        {
            get
            {
                try
                {
                    return resManager.GetString(key, currentCulture);
                }
                catch (MissingManifestResourceException)
                {
                    currentCulture = CultureInfo.GetCultureInfo(standartCulture);
                    return resManager.GetString(key, currentCulture);
                }
            }
        }

        public CultureInfo CurrentCulture
        {
            get { return currentCulture; }
            set
            {
                if (currentCulture != value)
                {
                    currentCulture = value;
                    var @event = PropertyChanged;
                    if (@event != null)
                    {
                        @event.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class LocExtension : Binding
    {
        public LocExtension(string name) : base("[" + name + "]")
        {
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}
