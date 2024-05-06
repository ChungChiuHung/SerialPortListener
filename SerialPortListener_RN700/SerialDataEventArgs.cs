using System;

namespace SerialPortListener_RN700
{
    public class SerialDataEventArgs : EventArgs
    {
        public byte[] Data;

        public SerialDataEventArgs(byte[] dataInByteArray) 
        {
            Data = dataInByteArray;    
        }
    }
}
