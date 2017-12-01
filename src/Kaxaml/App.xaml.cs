using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;
using Kaxaml.Properties;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Kaxaml.Plugins;
using Kaxaml.Plugins.Default;

namespace Kaxaml
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        private Snippets _Snippets;
        public Snippets Snippets
        {
            get { return _Snippets; }
            set { _Snippets = value; }
        }

        public References References { get; set; }

        void app_Startup(object sender, StartupEventArgs e)
        {
            //if (e.Args.Length > 0)
            //{
            //    StartupArgs = e.Args[0];
            //}

            _startupArgs = e.Args;

            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyName = executingAssembly.GetName().Name;
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
            {
                if (References == null) return null;
                var assemblyName = new AssemblyName(args.Name).Name + ".dll";

                var reference = References.ReferencesList.FirstOrDefault(r => r.Name == assemblyName);
                if (reference != null)
                {
//                    var bytes = File.ReadAllBytes(assemblyName);
//                    Assembly asm = Assembly.Load(bytes);
//                    var streams = asm.GetManifestResourceNames();
//                    return asm;
//                    using (var stream = executingAssembly.GetManifestResourceStream(reference.FullName))
//                    {
//                        if (stream != null)
//                        {
//                            var assemblyData = new Byte[stream.Length];
//                            stream.Read(assemblyData, 0, assemblyData.Length);
//                            return Assembly.Load(assemblyData);
//                        }
//                    }
                }

                //                var resourceName = executingAssemblyName + ".DllsAsResource." + new AssemblyName(args.Name).Name + ".dll";
                //                using (var stream = executingAssembly.GetManifestResourceStream(resourceName))
                //                {
                //                    if (stream != null)
                //                    {
                //                        var assemblyData = new Byte[stream.Length];
                //                        stream.Read(assemblyData, 0, assemblyData.Length);
                //                        return Assembly.Load(assemblyData);
                //                    }
                //                }
                return null;
            };
        }


        [PreserveSig()]
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetModuleFileName
        (
            [In]
            HandleRef module,

            [Out]
            StringBuilder buffer,

            [In]
            [MarshalAs(UnmanagedType.U4)]int capacity
        );

        private static String[] _startupArgs;
        public static String[] StartupArgs
        {
            get
            {
                return _startupArgs;
            }
        }

        private static String _startupPath;
        public static String StartupPath
        {
            get
            {
                // Only retrieve startup path when
                // it wans’t known.
                if (_startupPath == null)
                {
                    HandleRef nullHandle =
                        new HandleRef(null, IntPtr.Zero);

                    StringBuilder buffer =
                        new StringBuilder(260);

                    int lenght = GetModuleFileName(
                        nullHandle,
                        buffer,
                        buffer.Capacity);

                    if (lenght == 0)
                    {
                        // This ctor overload uses 
                        // GetLastWin32Error to
                        //get its error code.
                        throw new Win32Exception();
                    }

                    String moduleFilename =
                        buffer.ToString(0, lenght);
                    _startupPath =
                        Path.GetDirectoryName(moduleFilename);
                }

                return _startupPath;
            }
        }
    }
}