using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JetBrains.Annotations;

namespace Kaxaml.Plugins
{
    /// <summary>
    /// Interaction logic for References.xaml
    /// </summary>
    public partial class References : UserControl
    {
        public References()
        {
            InitializeComponent();

            this.ReferencesList = new ObservableCollection<Reference>(
                new []
                {
                    new Reference("c:\\Users\\punker76\\Documents\\Git\\code-samples\\MahAppsMetroSample\\MahAppsMetroSample\\bin\\Debug\\MahApps.Metro.dll"), 
                    new Reference("c:\\Users\\punker76\\Documents\\Git\\code-samples\\MahAppsMetroSample\\MahAppsMetroSample\\bin\\Debug\\ControlzEx.dll"), 
                }
                );
        }

        public ObservableCollection<Reference> ReferencesList { get; }
    }

    public class Reference
    {
        public Reference(string fileName)
        {
            var fileInfo = new System.IO.FileInfo(fileName);
            this.Name = fileInfo.Name;
            this.FullName = fileInfo.FullName;

            var bytes = File.ReadAllBytes(FullName);
            Assembly asm = Assembly.Load(bytes);
            this.AssemblyFileVersion = asm.GetCustomAttribute<System.Reflection.AssemblyFileVersionAttribute>().Version;
        }

        public string Name { get; }

        public string FullName { get; }

        public string AssemblyFileVersion { get; }
    }
}