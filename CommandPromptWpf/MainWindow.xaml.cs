using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace CommandPromptWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process process;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CmdConfig()
        {
            process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.BeginOutputReadLine();
        }

        private void ExicutetCommand(string command)
        {
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                cmdTerminal.AppendText(e.Data + "\n");
            });

        }

        private void btnSendCommnad_Click(object sender, RoutedEventArgs e)
        {
            ExicutetCommand(txtCommand.Text);
        }
    }
}
