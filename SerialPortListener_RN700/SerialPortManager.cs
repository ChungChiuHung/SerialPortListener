using System;
using System.IO.Ports;

namespace SerialPortListener_RN700
{
   public class SerialPortManager : IDisposable
    {
        public SerialPortManager()
        {
            
        }

        private SerialPort _serialPort;
        private SerialSettings _currentSerialSettings = new SerialSettings();

    }
}
