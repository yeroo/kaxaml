using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Controls;
using System.Windows.Markup;
using JetBrains.Annotations;
using Kaxaml.Core;
using KaxamlPlugins;

namespace Kaxaml.Plugins
{
    /// <summary>
    /// Interaction logic for References.xaml
    /// </summary>
    public partial class References : UserControl
    {
        public References()
        {
            this.DataContext = new ReferencesViewModel();
            InitializeComponent();
        }

        public bool AddNewReferences(string fileName)
        {
            return ((ReferencesViewModel) this.DataContext).AddNewReferences(fileName);
        }
    }

    public class ReferencesViewModel : INotifyPropertyChanged
    {
        private readonly HashSet<Reference> _addedReferences = new HashSet<Reference>();
        private ObservableCollection<Reference> _allReferences;

        public ReferencesViewModel()
        {
            this.AllReferences = new ObservableCollection<Reference>();
        }

        public bool AddNewReferences(string fileName)
        {
            if (!File.Exists(fileName)) return false;

            try
            {
                var fileInfo = new FileInfo(fileName);
                var reference = new Reference(fileInfo);

                if (_addedReferences.Contains(reference)) return false;

                _addedReferences.Add(reference);
                this.AllReferences.Add(reference);
            }
            catch (Exception ex)
            {
                if (ex.IsCriticalException())
                {
                    throw;
                }
                else
                {
                    var window = KaxamlInfo.MainWindow as MainWindow;
                    window?.DocumentsView?.SelectedView?.ReportError(ex);
                }
            }

            return true;
        }

        public ObservableCollection<Reference> AllReferences
        {
            get { return _allReferences; }
            set
            {
                if (Equals(value, _allReferences)) return;
                _allReferences = value;
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

    public class Reference
    {
        public Reference([NotNull] FileInfo fileInfo)
        {
            this.Name = fileInfo.Name;
            this.FullName = fileInfo.FullName;

            Assembly asm = null;
            try
            {
                var bytes = File.ReadAllBytes(this.FullName);
                asm = Assembly.Load(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                asm = null;
            }

            var assemblyVersionAttribute = asm?.GetCustomAttribute<AssemblyVersionAttribute>();
            var assemblyFileVersionAttribute = asm?.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var assemblyInformationalVersionAttribute = asm?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (assemblyVersionAttribute != null)
            {
                this.AssemblyFileVersion = assemblyVersionAttribute.Version;
            }
            else if (assemblyFileVersionAttribute != null)
            {
                this.AssemblyFileVersion = assemblyFileVersionAttribute.Version;
            }
            else if (assemblyInformationalVersionAttribute != null)
            {
                this.AssemblyFileVersion = assemblyInformationalVersionAttribute.InformationalVersion;
            }
            else
            {
                this.AssemblyFileVersion = FileVersionInfo.GetVersionInfo(this.FullName).FileVersion;
            }

            var targetFramework = asm?.GetCustomAttribute<TargetFrameworkAttribute>();
            this.TargetFramework = targetFramework?.FrameworkDisplayName;

            var resourceDictionaries = GetResourceDictionary(asm);
        }

        public IEnumerable<ResourceDictionary> GetResourceDictionary(Assembly assembly)
        {
            if (assembly == null)
            {
                return Enumerable.Empty<ResourceDictionary>();
            }

            var assemblyName = assembly.GetName().Name;
            var stream = assembly.GetManifestResourceStream(assemblyName + ".g.resources");
            if (stream == null)
            {
                return Enumerable.Empty<ResourceDictionary>();
            }

            var resourceDictionaries = new List<ResourceDictionary>();

            using (var reader = new ResourceReader(stream))
            {
                foreach (var readStream in reader.OfType<DictionaryEntry>().Select(entry => entry.Value).OfType<Stream>())
                {
                    var bamlReader = new Baml2006Reader(readStream);
                    try
                    {
                        var resDict = XamlReader.Load(bamlReader) as ResourceDictionary;
                        if (resDict != null)
                        {
                            if (resDict.Source != null && resDict.Source.OriginalString.Contains(assemblyName))
                            {
                                resourceDictionaries.Add(resDict);
                            }
                            foreach (var mergedDictionary in resDict.MergedDictionaries)
                            {
                                if (mergedDictionary.Source != null && mergedDictionary.Source.OriginalString.Contains(assemblyName))
                                {
                                    resourceDictionaries.Add(mergedDictionary);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return resourceDictionaries;
        }

        public string Name { get; }

        public string FullName { get; }

        public string AssemblyFileVersion { get; }

        public string TargetFramework { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Reference) obj);
        }

        protected bool Equals(Reference other)
        {
            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(AssemblyFileVersion, other.AssemblyFileVersion, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0) * 397) ^ (AssemblyFileVersion != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(AssemblyFileVersion) : 0);
            }
        }
    }
}