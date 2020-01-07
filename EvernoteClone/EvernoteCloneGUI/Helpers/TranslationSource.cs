using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace EvernoteCloneGUI.Helpers
{
    public class TranslationSource
        : INotifyPropertyChanged
    {
        private static readonly TranslationSource _instance = new TranslationSource();
        
        private CultureInfo _currentCulture;

        public static TranslationSource Instance => 
            _instance;

        public string this[string key] => 
            Properties.Settings.Default[key].ToString();

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
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

    public class LocExtension
        : Binding
    {
        public LocExtension(string name)
            : base("[" + name + "]")
        {
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}