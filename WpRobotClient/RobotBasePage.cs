using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace WpRobotClient
{
    public class RobotBasePage : Page
    {
        private StreamSocket _socket;
        HostName _hostname = new HostName("192.168.1.11");
        private Int32 _port = 88;
        private DataReader _reader;
        private DataWriter _writer;

        protected async Task SendMessageAsync(String message)
        {
            message += '\n';

            _writer.WriteString(message);
            await _writer.StoreAsync();

            Debug.WriteLine(String.Format("Sent: {0}", message));
        }

        protected async Task<string> ReadMessageAsync()
        {
            string data;
            _reader.InputStreamOptions = InputStreamOptions.Partial;
            var count = await _reader.LoadAsync(512);

            if (count > 0)
            {
                var message = _reader.ReadString(count);
                Debug.WriteLine("Get data: {0}", message);
                return message;
            }

            return String.Empty;
        }

        protected async Task<bool> ConnectAsync(String server)
        {
            try
            {
                _socket = new StreamSocket();
                _hostname = new HostName(server);
                await _socket.ConnectAsync(_hostname, _port.ToString());
                _reader = new DataReader(_socket.InputStream);
                _writer = new DataWriter(_socket.OutputStream);

                await SendMessageAsync("connect");

                var answer = await ReadMessageAsync();
                if (answer == "connected")
                {
                    return true;
                }

                return false;

            }
            catch (Exception e)
            {
                switch (SocketError.GetStatus(e.HResult))
                {
                    case SocketErrorStatus.HostNotFound:
                        MessageDialog dlg = new MessageDialog("Host not found " + _hostname + " port: " + _port.ToString());
                        await dlg.ShowAsync();
                        break;
                    default:
                        MessageDialog dlg2 = new MessageDialog("SocketException: {0}", e.Message);
                        await dlg2.ShowAsync();
                        break;
                }

                return false;
            }
        }

        protected void Disconnect()
        {
            _reader.Dispose();
            _reader = null;
            _writer.Dispose();
            _writer = null;

            _socket.Dispose();
            _socket = null;
        }
    }
}
