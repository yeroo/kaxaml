using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;

namespace Kaxaml.Plugins
{
    /// <summary>
    /// Interaction logic for ReferencesResourceDictionaryChooser.xaml
    /// </summary>
    public partial class ReferencesResourceDictionaryChooser : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// The reference being edited
        /// </summary>
        public Reference Reference
        {
            get { return (Reference) GetValue(ReferenceProperty); }
            set { SetValue(ReferenceProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for Reference
        /// </summary>
        public static readonly DependencyProperty ReferenceProperty
            = DependencyProperty.Register("Reference",
                typeof(Reference),
                typeof(ReferencesResourceDictionaryChooser),
                new FrameworkPropertyMetadata(default(Reference)));

        public ReferencesResourceDictionaryChooser()
        {
            InitializeComponent();
        }

        private static ReferencesResourceDictionaryChooser instance = null;
        private IEnumerable _resourceDictionaries;

        public static ReferencesResourceDictionaryChooser Show(Reference reference, Window owner)
        {
            if (instance == null)
            {
                instance = new ReferencesResourceDictionaryChooser();
            }

            instance.Owner = owner;
            instance.Reference = reference;
            instance.ResourceDictionaries = reference.GetResourceDictionary(reference.Assembly);
            instance.Show();

            return instance;
        }

        public IEnumerable ResourceDictionaries
        {
            get { return _resourceDictionaries; }
            set
            {
                if (Equals(value, _resourceDictionaries)) return;
                _resourceDictionaries = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}