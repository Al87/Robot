using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WpRobotClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManualPage
    {

       
        
        public ManualPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
        }

        

        private void ChangeUI(bool isConnected)
        {
            BtnConnect.IsEnabled = !isConnected;
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


        #region Buttons events

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            var isConnected = await ConnectAsync(TxtIp.Text);
            ChangeUI(isConnected);
        }

        private async void BtnFastForward_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync("FastForward");
        }

        private async void BtnSlowForward_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync("SlowForward");
        }

        private async void BtnStop_Click(object sender, RoutedEventArgs e)
        {
           await SendMessageAsync("Stop");
        }

        private async void BtnSlowBack_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync("SlowBack");
        }

        private async void BtnFastBack_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync("FastBack");
        }

        private async void BtnFastLeft_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync("FastLeft");
        }

        private async void BtnSlowLeft_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync("SlowLeft");
        }

        private async void BtnSlowRight_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync("SlowRight");
        }

        private async void BtnFastRight_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync("FastRight");
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();

            ChangeUI(false);

        }

        #endregion

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
