using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Confluent.Kafka;
namespace KafkaClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process process;
        private IConsumer<Ignore, string> consumer;

        public MainWindow()
        {
            InitializeComponent();

        }



        private void btnStartConsume_Click(object sender, RoutedEventArgs e)
        {
            if (btnStartConsume.Content.Equals("Start Consume"))
            {
                Cursor = Cursors.Wait;
                string uri = txtUrl.Text;
                string topic = txtTopic.Text;
                ConnectToKafakaConsume(uri, topic).ConfigureAwait(false);
                btnStartConsume.Content = "Stop Consume";
                Cursor = Cursors.Arrow;
                txtTopic.IsEnabled = false;
                txtUrl.IsEnabled = false;
            }
            else
            {
                txtTopic.IsEnabled = true;
                txtUrl.IsEnabled = true;
                consumer.Unsubscribe();
                consumer.Close();
                btnStartConsume.Content = "Start Consume";
            }
        }

        private async Task ConnectToKafakaConsume(string uri, string topic)
        {
            var config = new ConsumerConfig
            {
                BrokerAddressFamily = BrokerAddressFamily.V4,
                BootstrapServers = uri,
                GroupId = "KafkaClient",

            };
            consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            CancellationToken cancellationToken = new CancellationToken();
           await ConsumeData(cancellationToken, txtTopic.Text);

        }

        private async Task ConsumeData(CancellationToken cancellationToken, string topic)
        {
            try
            {
                consumer.Subscribe(topic);

                var consumeResult = consumer.Consume(cancellationToken);
                await Task.Factory.StartNew(MonitorData, consumeResult, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                cmdTerminal.AppendText($"Error: {ex.Message}" + "\n");
                cmdTerminal.ScrollToEnd();
            }
        }

        async void MonitorData(object ObjData)
        {
            var data = (ConsumeResult<Ignore, string>)ObjData;
            Dispatcher.Invoke(() =>
            {
                cmdTerminal.Foreground =Brushes.Red;
                cmdTerminal.AppendText(data.Topic + " : " + data.Message.Value.ToString() + "\n");
                cmdTerminal.ScrollToEnd();
                cmdTerminal.Foreground = Brushes.White;
            });
        }


    }
}
