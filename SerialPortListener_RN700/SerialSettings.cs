﻿using System;
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
            const int BAUD_075 = 0x00000001;
            const int BAUD_110 = 0x00000002;
            const int BAUD_150 = 0x00000008;
            const int BAUD_300 = 0x00000010;
            const int BAUD_600 = 0x00000020;
            const int BAUD_1200 = 0x00000040;
            const int BAUD_1800 = 0x00000080;
            const int BAUD_2400 = 0x00000100;
            const int BAUD_4800 = 0x00000200;
            const int BAUD_7200 = 0x00000400;
            const int BAUD_9600 = 0x00000800;
            const int BAUD_14400 = 0x00001000;
            const int BAUD_19200 = 0x00002000;
            const int BAUD_38400 = 0x00004000;
            const int BAUD_56K = 0x00008000;
            const int BAUD_57600 = 0x00040000;
            const int BAUD_115200 = 0x00020000;
            const int BAUD_128K = 0x00010000;

            _baudRateCollection.Clear();

            if ((possibleBaudRates & BAUD_075) > 0)
                _baudRateCollection.Add(75);
            if ((possibleBaudRates & BAUD_110) > 0)
                _baudRateCollection.Add(110);
            if ((possibleBaudRates & BAUD_150) > 0)
                _baudRateCollection.Add(150);
            if ((possibleBaudRates & BAUD_300) > 0)
                _baudRateCollection.Add(300);
            if ((possibleBaudRates & BAUD_600) > 0)
                _baudRateCollection.Add(600);
            if ((possibleBaudRates & BAUD_1200) > 0)
                _baudRateCollection.Add(1200);
            if ((possibleBaudRates & BAUD_1800) > 0)
                _baudRateCollection.Add(1800);
            if ((possibleBaudRates & BAUD_2400) > 0)
                _baudRateCollection.Add(2400);
            if ((possibleBaudRates & BAUD_4800) > 0)
                _baudRateCollection.Add(4800);
            if ((possibleBaudRates & BAUD_7200) > 0)
                _baudRateCollection.Add(7200);
            if ((possibleBaudRates & BAUD_9600) > 0)
                _baudRateCollection.Add(9600);
            if ((possibleBaudRates & BAUD_14400) > 0)
                _baudRateCollection.Add(14400);
            if ((possibleBaudRates & BAUD_19200) > 0)
                _baudRateCollection.Add(19200);
            if ((possibleBaudRates & BAUD_38400) > 0)
                _baudRateCollection.Add(38400);
            if ((possibleBaudRates & BAUD_56K) > 0)
                _baudRateCollection.Add(56000);
            if ((possibleBaudRates & BAUD_57600) > 0)
                _baudRateCollection.Add(57600);
            if ((possibleBaudRates & BAUD_115200) > 0)
                _baudRateCollection.Add(115200);
            if ((possibleBaudRates & BAUD_128K) > 0)
                _baudRateCollection.Add(128000);

            SendPropertyChangedEvent("BaudRateCollection");
        }


    }
}
