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
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Diagnostics;

namespace RobotClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Int32 _port = 88;
        private TcpClient _client;
        private NetworkStream _stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeUI(bool isConnected)
        {
            BtnConnect.IsEnabled = !isConnected;
            BtnSendMessage.IsEnabled = isConnected;
            BtnDisconnect.IsEnabled = isConnected;

            BtnFastForward.IsEnabled = isConnected;
            BtnSlowForward.IsEnabled = isConnected;
            BtnStop.IsEnabled = isConnected;
            BtnSlowBack.IsEnabled = isConnected;
            BtnFastBack.IsEnabled = isConnected;
            BtnFastLeft.IsEnabled = isConnected;
            BtnSlowLeft.IsEnabled = isConnected;
            BtnSlowRight.IsEnabled = isConnected;
            BtnFastRight.IsEnabled = isConnected;
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            var isConnected = Connect("192.168.1.11");
            ChangeUI(isConnected);
        }

        private void SendMessage(String message)
        {
            message += '\n';
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();

            _stream = _client.GetStream();

            // Send the message to the connected TcpServer. 
            _stream.Write(data, 0, data.Length);
            
            
            Debug.WriteLine(String.Format("Sent: {0}", message));
        }

        private string ReadMessage()
        {
            Byte[] data = new Byte[255];
            _stream = _client.GetStream();
            var length = _stream.Read(data, 0, 255);
            var message = System.Text.Encoding.ASCII.GetString(data, 0, length);
            return message;
        }

        private bool Connect(String server)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.

                _client = new System.Net.Sockets.TcpClient(server, _port);
                SendMessage("connect");

                var answer = ReadMessage();
                if (answer == "connected")
                {
                    return true;
                }

                return false;
                
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show(String.Format("ArgumentNullException: {0}", e));
                return false;
            }
            catch (SocketException e)
            {
                MessageBox.Show(String.Format("SocketException: {0}", e));
                return false;
            }
            
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            _stream.Close();
            _client.Close();
            ChangeUI(false);

        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(TxtMessage.Text);
            
        }

        private void BtnFastForward_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("FastForward");
        }

        private void BtnSlowForward_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("SlowForward");
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("Stop");
        }

        private void BtnSlowBack_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("SlowBack");
        }

        private void BtnFastBack_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("FastBack");
        }

        private void BtnFastLeft_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("FastLeft");
        }

        private void BtnSlowLeft_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("SlowLeft");
        }

        private void BtnSlowRight_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("SlowRight");
        }

        private void BtnFastRight_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("FastRight");
        }
    }
}
