using Karin;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;

namespace TestWindow
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        string MyPath;
        KarinEngine karin;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            MyPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

            ResetEngine();
        }

        private void ResetEngine() {
            karin = new KarinEngine();

            //load plugin
            var files = Directory.GetFiles($"{MyPath}/plugin", "*.dll");
            foreach(var f in files) {
                karin.LoadPluginDll(f);
            }
        }

        private async Task<string> RunAsync(string script) {
            var t = new Task<string>(() => {
                try {
                    var ret = karin.Execute(script);
                    return $"{ret}";
                } catch (KarinException ex) {
                    return $"{ex.Message}{Environment.NewLine}{ex.ScriptStackTrace}";
                } catch (Exception ex) {
                    return ex.ToString();
                }
            });

            t.Start();
            return await t;
        }

        private async void Run_Click(object sender, RoutedEventArgs e) {
            if(karin.IsRunning) return;

            var script = _Input.Text;

            _Output.Text = "run script...";
            var ret = await RunAsync(script);
            _Output.Text = ret;
        }

        private void Stop_Click(object sender, RoutedEventArgs e) {
            if (karin.IsRunning) {
                karin.Stop();
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e) {
            if(karin.IsRunning) return;
            ResetEngine();
        }
    }
}
