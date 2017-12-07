using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public static ReferencesResourceDictionaryChooser Show(Reference reference, Window owner)
        {
            var instance = new ReferencesResourceDictionaryChooser();

            instance.Owner = owner;
            instance.Reference = reference;
            instance.ResourceDictionaries = new ObservableCollection<ReferenceResourceDictionary>(reference.GetResourceDictionary(reference.Assembly).OrderBy(r => r.DictionaryEntryKey, StringComparer.InvariantCultureIgnoreCase));
            instance.SelectedResourceDictionaries = new ObservableCollection<ReferenceResourceDictionary>();

            return instance;
        }

        private IEnumerable _resourceDictionaries;

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

        private IEnumerable _selectedResourceDictionaries;

        public IEnumerable SelectedResourceDictionaries
        {
            get { return _selectedResourceDictionaries; }
            set
            {
                if (Equals(value, _selectedResourceDictionaries)) return;
                _selectedResourceDictionaries = value;
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