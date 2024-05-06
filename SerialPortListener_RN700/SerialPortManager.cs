using System;
using System.ComponentModel;
using System.IO.Ports;

namespace SerialPortListener_RN700
{
   public class SerialPortManager : IDisposable
    {
        private SerialPort _serialPort;
        private SerialSettings _currentSerialSettings;

        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;
        public event EventHandler<SerialErrorReceivedEventArgs> NewSerialErrorReceived;

        public bool SerialPortIsOpen => _serialPort?.IsOpen ?? false;


        public SerialSettings CurrentSerialSettings
        {
            get { return _currentSerialSettings; }
            set
            {
                if (_currentSerialSettings != value)
                {
                    if (_currentSerialSettings != null)
                    {
                        _currentSerialSettings.PropertyChanged -= OnSerialSettingsChanged;
                    }

                    _currentSerialSettings = value;

                    if (_currentSerialSettings != null)
                    {
                        _currentSerialSettings.PropertyChanged += OnSerialSettingsChanged;
                        ConfigureSerialPort();
                    }
                }
            }
        }

        public SerialPortManager(SerialSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _currentSerialSettings = settings;
            ConfigureSerialPort();
            _serialPort.Open();

        }

        private void ConfigureSerialPort()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.DataReceived -= OnDataReceived;
                _serialPort.ErrorReceived -= OnErrorReceived;
                _serialPort.Dispose();
            }

            _serialPort = new SerialPort
            {
                PortName = CurrentSerialSettings.PortName,
                BaudRate = CurrentSerialSettings.BaudRate,
                Parity = CurrentSerialSettings.Parity,
                DataBits = CurrentSerialSettings.DataBits,
                StopBits = CurrentSerialSettings.StopBits
            };

            _serialPort.DataReceived += OnDataReceived;
            _serialPort.ErrorReceived += OnErrorReceived;
        }

        private void OnSerialSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                ConfigureSerialPort();
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
            }
        }

        private bool shouldProcessData = true;

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!shouldProcessData || !SerialPortIsOpen) return;

            byte[] data = ReadData();
            if(data.Length > 0)
            {
                NewSerialDataRecieved?.Invoke(this, new SerialDataEventArgs(data));
            }
        }

        private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            NewSerialErrorReceived?.Invoke(this, e);
        }


        public void send(char[] data, int offset, int count)
        {
            _serialPort.Write(data, offset, count);
        }

        public void sendByte(byte[] data, int offset, int count)
        {
            _serialPort.Write(data, offset, count);
        }
        

        public byte[] ReadData()
        {   
            int dataLength = _serialPort.BytesToRead;
            byte[] data = new byte[dataLength];
            _serialPort.Read(data, 0, dataLength);
            return data;
        }

        public void StopReading()
        {
            shouldProcessData = false;
        }

        public void StartReading()
        {
            shouldProcessData = true;
        }

        public bool ShouldProcessData
        { get { return shouldProcessData; } }

        public void StartListening()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        public void StopListenning()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_serialPort != null)
                {
                    _serialPort.DataReceived -= OnDataReceived;
                    _serialPort.ErrorReceived -= OnErrorReceived;
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                    _serialPort.Dispose();
                }
            }
        }

        ~SerialPortManager()
        {
            Dispose(false);
        }


    }
}
