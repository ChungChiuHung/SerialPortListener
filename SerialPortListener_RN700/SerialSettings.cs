using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;

namespace SerialPortListener_RN700
{
    public class SerialSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SendPropertyChangedEvent(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _portName = "";
        string[] _portNameCollection;
        int _baudRate = 9600;
        BindingList<int> baudRateCollection = new BindingList<int>();
        Parity _parity = Parity.None;
        int _dataBits = 8;
        int[] _dataBitsCollection  = new int[] { 5, 6, 7, 8 };
        StopBits _stopBits = StopBits.One;

        public string PortName
        {
            get { return _portName; }
            set 
            {
                if (!_portName.Equals(value))
                {
                    _portName = value;
                    SendPropertyChangedEvent("PortName");
                }
            }
        }

        public int BaudRate
        { 
            get { return _baudRate; } 
            set 
            {
                if (!_baudRate.Equals(value))
                {
                    _baudRate = value;
                    SendPropertyChangedEvent("BaudRate");
                }
            }
        }

        public Parity Parity
        {
            get { return _parity; }
            set
            {
                if (_parity.Equals(value))
                {
                    _parity = value;
                    SendPropertyChangedEvent("Parity");
                }
            }
        }

        public int DataBits
        {
            get { return _dataBits; }
            set
            {
                if (!_dataBits.Equals(value))
                {
                    _dataBits = value;
                    SendPropertyChangedEvent("DataBits");
                }
            }
        }

        public StopBits StopBits
        {
            get { return _stopBits; }
            set 
            {
                if (!_stopBits.Equals(value))
                {
                    _stopBits = value;
                    SendPropertyChangedEvent("stopBits");
                }
            }
        }

        public string[] PortNameCollection
        {
            get { return _portNameCollection; }
            set
            {
                _portNameCollection = value;
            }
        }

        public BindingList<int> BaudRateCollection
        {
            get { return _baudRateCollection; }
        }

        public void UpdateBaudRateCollection(int possibleBaudRates)
        {
            const int BAUD_075
        }


    }
}
