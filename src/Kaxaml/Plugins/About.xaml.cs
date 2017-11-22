using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Reflection;

namespace Kaxaml.Plugins
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            this.AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(this.HandleRequestNavigate), false);
            this.Loaded += AboutLoaded;
        }

        private void AboutLoaded(object sender, RoutedEventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            this.TheVersionRun.Text = string.Format("v{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

        void HandleRequestNavigate(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (e.OriginalSource as Hyperlink);
            if (hl != null)
            {
                string navigateUri = hl.NavigateUri.ToString();
                Process.Start(new ProcessStartInfo(navigateUri));
                e.Handled = true;
            }
        }

    }
}
