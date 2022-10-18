using GSMSoftware.Core;
using GSMSoftware.MVVM.View;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GSMSoftware.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public GlobalViewModel GlobalViewModel { get; } = GlobalViewModel.Instance;
        public ObservableCollection<string> SerialPortCollection { get; set; }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string? _selectedPort;

        public string? SelectedPort
        {
            get { return _selectedPort; }
            set { _selectedPort = value; OnPropertyChanged(); }
        }

        public RelayCommand OpenLogWindowCommand { get; set; }

        // Ports Command
        public RelayCommand OpenPortCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        public RelayCommand RefreshPortCommand { get; set; }
        public RelayCommand MoveWindowCommand { get; set; }
        public RelayCommand ShutdownWindowCommand { get; set; }
        public RelayCommand MaximizeWindowCommand { get; set; }
        public RelayCommand MinimizedWindowCommand { get; set; }

        SerialPort? serialPort { get; set; }
        public MainViewModel()
        {
            GlobalViewModel.IsPortOpen = false;
            serialPort = new SerialPort();

            SerialPortCollection = new ObservableCollection<string>();

            PopulatePortCollection();

            Application.Current.MainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            SendMessageCommand = new RelayCommand(o =>
            {
                try
                {
                    serialPort.Write(PhoneNumber + Message);
                }
                catch (Exception ex)
                {
                    GlobalViewModel.GSMLogs += ex.Message;
                }
            });

            OpenLogWindowCommand = new RelayCommand(o =>
            {
                LogsWindow logsWindow = new LogsWindow();
                logsWindow.Show();
            });

            RefreshPortCommand = new RelayCommand(o =>
            {
                try
                {
                    serialPort.Close();
                    GlobalViewModel.IsPortOpen = false;
                } catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                    GlobalViewModel.GSMLogs += err.Message + "\n";
                }
                PopulatePortCollection();
            });

            OpenPortCommand = new RelayCommand(o =>
            {
                if (GlobalViewModel.IsPortOpen == true)
                {
                    MessageBox.Show("Port is already open!");
                    return;
                }


                if (SelectedPort != null)
                {
                    serialPort.PortName = SelectedPort;
                    serialPort.BaudRate = 9600;
                    serialPort.Parity = Parity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;
                    serialPort.DtrEnable = true;
                    serialPort.RtsEnable = true;
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    try
                    {
                        serialPort.Open();
                        GlobalViewModel.IsPortOpen = serialPort.IsOpen;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        GlobalViewModel.GSMLogs += err.Message + "\n";
                    }
                }
            });

            MoveWindowCommand = new RelayCommand(o => Application.Current.MainWindow.DragMove());
            ShutdownWindowCommand = new RelayCommand(o => { Application.Current.Shutdown(); try
                {
                    serialPort.Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    GlobalViewModel.GSMLogs += err.Message + "\n";
                }
            });
            MaximizeWindowCommand = new RelayCommand(o =>
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                else
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
            });
            MinimizedWindowCommand = new RelayCommand(o => { Application.Current.MainWindow.WindowState = WindowState.Minimized; });
        }

        void PopulatePortCollection()
        {
            SerialPortCollection.Clear();
           
            foreach (var x in SerialPort.GetPortNames())
            {
                SerialPortCollection.Add(x);
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            GlobalViewModel.GSMLogs += indata;
        }

    }
}
