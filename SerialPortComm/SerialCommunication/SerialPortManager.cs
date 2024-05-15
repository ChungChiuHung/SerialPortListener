using System.IO.Ports;
using System.ComponentModel;
using RN700Communication.Config;
using RN700Communication.Models;

namespace RN700Communication.SerialCommunication
{
    public class SerialPortManager : IDisposable, INotifyPropertyChanged
    {
        private readonly SerialPort _serialPort;
        
        private byte[] _receivedData;

        public byte[] ReceivedData
        { 
            get { return _receivedData; }
            private set
            {
                _receivedData = value;
                OnPropertyChanged(nameof(ReceivedData));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SerialPortManager(SerialPortSettings settings)
        {
            _serialPort = new SerialPort
            {
                PortName = settings.PortName,
                BaudRate = settings.BaudRate,
                Parity = (Parity)Enum.Parse(typeof(Parity), settings.Parity, true),
                DataBits = settings.DataBits,
                StopBits = (StopBits)settings.StopBits
            };
            _serialPort.DataReceived += DataReceivedHandler;
        }

        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Open()
        {
            if(!_serialPort.IsOpen)
            {
                _serialPort.Open();
                Console.WriteLine($"Connected to {_serialPort.PortName}.");
            }
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                Console.WriteLine($"Disconnected from {_serialPort.PortName}");
            }
        }

        public void sendByte(byte[] data, int offset, int count)
        {
            _serialPort.Write(data, offset, count);
        }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Read all available data from the serial port
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);

                ReceivedData = buffer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Close();
            _serialPort.Dispose();
        }
    }
}
