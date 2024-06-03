namespace SerialPortListener_RN700.Config
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
