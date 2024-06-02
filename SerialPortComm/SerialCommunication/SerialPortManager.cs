using System.IO.Ports;
using System.ComponentModel;
using System.Threading;
using RN700Communication.Config;
using RN700Communication.Models;
using System.Text;

namespace RN700Communication.SerialCommunication
{
    public class SerialPortManager : IDisposable, INotifyPropertyChanged
    {
        private readonly SerialPort _serialPort;
        private readonly Thread _readThread;
        private bool _keepReading;
        
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SerialPortManager(SerialPortSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _serialPort = new SerialPort
            {
                PortName = settings.PortName ?? throw new ArgumentNullException(nameof(settings.PortName)),
                BaudRate = settings.BaudRate,
                Parity = (Parity)Enum.Parse(typeof(Parity), settings.Parity ?? throw new ArgumentNullException(nameof(settings.Parity)), true),
                DataBits = settings.DataBits,
                StopBits = (StopBits)settings.StopBits
            };
            _serialPort.DataReceived += DataReceivedHandler;

            _receivedData = Array.Empty<byte>();

            _readThread = new Thread(ReadPort);
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
                _keepReading = true;
                _readThread.Start();
                Console.WriteLine($"Connected to {_serialPort.PortName}.");
            }
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _keepReading = false;
                _readThread.Join();
                _serialPort.Close();
                Console.WriteLine($"Disconnected from {_serialPort.PortName}");
            }
        }

        public void SendByte(byte[] data, int offset, int count)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _serialPort.Write(data, offset, count);
        }

        public void SendString(string message)
        {
            if(message == null) throw new ArgumentNullException( nameof(message));
            byte[] data = Encoding.UTF8.GetBytes(message);
            _serialPort.Write(data, 0, data.Length);
        }


        private void DataReceivedHandler(object? sender, SerialDataReceivedEventArgs e)
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

        private void ReadPort()
        {
            while(_keepReading)
            {
                try
                {
                    if(_serialPort.BytesToRead > 0)
                    {
                        int bytesToRead = _serialPort.BytesToRead;
                        byte[] buffer = new byte[bytesToRead];
                        _serialPort.Read(buffer, 0, bytesToRead);
                        ReceivedData = buffer;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Thread.Sleep(100);
            }
        }

        public void Dispose()
        {
            Close();
            _serialPort.Dispose();
        }
    }
}
