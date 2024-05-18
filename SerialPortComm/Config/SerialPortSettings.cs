using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RN700Communication.Config
{
    public class SerialPortSettings
    {
        public string PortName { get; set; } = "COM1";
        public int BaudRate { get; set; }
        public string Parity { get; set; } = "N";
        public int DataBits { get; set; }
        public int StopBits { get; set; }
    }
}
