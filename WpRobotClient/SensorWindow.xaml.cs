using System;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WpRobotClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SensorWindow: RobotBasePage
    {
        private Accelerometer _accelerometer;
        private uint _desiredReportInterval;

        private double _x;
        private double _y;
        private double _z;

        private bool _isConnected = false;


        public SensorWindow()
        {
            this.InitializeComponent();
            InitAccelerometerAsync();
        }

        private void ChangeUi()
        {
            BtnConnect.IsEnabled = !_isConnected;
            BtnDisconnect.IsEnabled = _isConnected;
        }

        private async Task InitAccelerometerAsync()
        {
            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                // Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
                // This value will be used later to activate the sensor.
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                _desiredReportInterval = minReportInterval > 16 ? minReportInterval : 16;
                _accelerometer.ReportInterval = _desiredReportInterval;

                Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
            else
            {
                var dlg = new MessageDialog("No accelerometer found");
                await dlg.ShowAsync();
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        private void VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {

            if (e.Visible)
            {
                // Re-enable sensor input (no need to restore the desired reportInterval... it is restored for us upon app resume)
                _accelerometer.ReadingChanged +=
                    new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
            else
            {
                // Disable sensor input (no need to restore the default reportInterval... resources will be released upon app suspension)
                _accelerometer.ReadingChanged -=
                    new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
        }

        async private void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                AccelerometerReading reading = e.Reading;

                _x = reading.AccelerationX;
                _y = reading.AccelerationY;
                _z = reading.AccelerationZ;

                LblX.Text = String.Format("{0,5:0.00}", _x);
                LblY.Text = String.Format("{0,5:0.00}", _y);
                LblZ.Text = String.Format("{0,5:0.00}", _z);

                var command = AnalyzeRobotCommand();

                if (command != "Nothing")
                {
                    lblCommand.Text = command;
                }

                if (command != "Nothing" && _isConnected)
                {
                    await SendMessageAsync(command);
                }
            });
        }

        private string AnalyzeRobotCommand()
        {
            #region Fast Forward
            if (IsValueInInterval(_x, -0.10, 0.10) && IsValueInInterval(_y, 0.21, 0.50) && IsValueInInterval(_z, -0.75, -1.00))
            {
                return "FastForward";
            }
            #endregion

            #region Slow forward
            if (IsValueInInterval(_x, -0.10, 0.10) && IsValueInInterval(_y, 0.10, 0.20) && IsValueInInterval(_z, -0.75, -1.00))
            {
                return "SlowForward";
            }
            #endregion

            #region Stop
            if (IsValueInInterval(_x, -0.10, 0.10) && IsValueInInterval(_y, -0.20, 0.10) && IsValueInInterval(_z, -0.90, -1.05))
            {
                return "Stop";
            }
            #endregion

            #region Slow Back
            if (IsValueInInterval(_x, -0.10, 0.10) && IsValueInInterval(_y, -0.50, -0.20) && IsValueInInterval(_z, -0.75, -1.00))
            {
                return "SlowBack";
            }
            #endregion

            #region Fast Back
            if (IsValueInInterval(_x, -0.10, 0.10) && IsValueInInterval(_y, -0.51, -0.70) && IsValueInInterval(_z, -0.65, -1.00))
            {
                return "FastBack";
            }
            #endregion

            #region Slow Left
            if (IsValueInInterval(_x, -0.50, 0.00) && IsValueInInterval(_y, -0.15, 0.0) && IsValueInInterval(_z, -1, -0.75))
            {
                return "SlowLeft";
            }
            #endregion

            #region Fast left
            if (IsValueInInterval(_x, -0.90, -0.51) && IsValueInInterval(_y, -0.15, 0.00) && IsValueInInterval(_z, -0.84, -0.45))
            {
                return "FastLeft";
            }
            #endregion

            #region Slow right
            if (IsValueInInterval(_x, 0.00, 0.50) && IsValueInInterval(_y, -0.15, 0.00) && IsValueInInterval(_z, -0.75, -1.00))
            {
                return "SlowRight";
            }
            #endregion

            #region Fast right
            if (IsValueInInterval(_x, 0.50, 0.90) && IsValueInInterval(_y, -0.15, 0.00) && IsValueInInterval(_z, -0.40, -0.84))
            {
                return "FastRight";
            }
            #endregion

            return "Nothing";
        }

        private bool IsValueInInterval(double value, double first, double second)
        {
            if (first > second)
            {
                var m = first;
                first = second;
                second = m;
            }

            if (value >= first && value <= second)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
            _isConnected = false;
            ChangeUi();
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            _isConnected = await ConnectAsync(TxtIp.Text);
            ChangeUi();
            
        }

        private void BtnGoBack_OnClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
