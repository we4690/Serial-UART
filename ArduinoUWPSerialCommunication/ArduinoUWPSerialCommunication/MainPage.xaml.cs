using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.UI.Xaml.Controls;

namespace ArduinoUWPSerialCommunication
{

    public sealed partial class MainPage : Page
    {
        FMKSerials serialcustom = new FMKSerials();

        public MainPage()
        {
            this.InitializeComponent();
            
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            serialcustom.InitializeConnection();
        }
    }

    public sealed class FMKSerials
    {
        public DataReader dataReader;
        public SerialDevice serialPort;
        public DataReader dataReaderObject;

        public async void InitializeConnection()
        {
            var aqs = SerialDevice.GetDeviceSelectorFromUsbVidPid(0x2341, 0x0043);//0x10C4, 0xEA60
            var info = await DeviceInformation.FindAllAsync(aqs);

            // Get connection data
            serialPort = await SerialDevice.FromIdAsync(info[0].Id);

            // Configure serial settings
            serialPort.DataBits = 8;
            serialPort.BaudRate = 9600;
            serialPort.Parity = SerialParity.None;
            serialPort.Handshake = SerialHandshake.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);

            dataReader = new DataReader(serialPort.InputStream);
            dataReaderObject = new DataReader(serialPort.InputStream);
            while (true)
            {
                await ReadAsync();
            }
        }


        private async Task ReadAsync()
        {
            try
            {
                Task<UInt32> loadAsyncTask;
                uint ReadBufferLength = 1024;

                // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
                dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

                // Create a task object to wait for data on the serialPort.InputStream
                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask();

                // Launch the task and wait
                UInt32 bytesRead = await loadAsyncTask;
                if (bytesRead > 0)
                {
                    string data = dataReaderObject.ReadString(bytesRead);
                    System.Diagnostics.Debug.Write(data);
                }
                //Task<uint> loadAsyncTask;
                //dataReader.ByteOrder = ByteOrder.BigEndian;
                //dataReader.InputStreamOptions = InputStreamOptions.Partial;
                //dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                //uint readBufferLength = 20;

                //loadAsyncTask = dataReader.LoadAsync(readBufferLength).AsTask();

                //uint ReadAsyncBytes = await loadAsyncTask;

                //if (ReadAsyncBytes > 0)
                //{
                //    string data = dataReader.ReadString(readBufferLength);
                //    System.Diagnostics.Debug.Write(data);
                //}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
