using Microsoft.Extensions.Configuration;
using RN700Communication.Config;
using RN700Communication.SerialCommunication;
using System.Text;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

SerialPortSettings? serialPortSettings = config.GetSection("SerialPortSettings").Get<SerialPortSettings>();

if(serialPortSettings == null )
{
    Console.WriteLine("Serial port settings are not configured properly.");
    return;
}

Console.WriteLine("Available serial ports: ");
string[] ports = SerialPortManager.GetAvailablePorts();
foreach (string port in ports)
{
    Console.WriteLine(port);
}

Console.WriteLine($"Configured serial port: {serialPortSettings.PortName}");
Console.WriteLine($"Configured serial baud rate: {serialPortSettings.BaudRate}");
Console.WriteLine($"Configured serial parity: {serialPortSettings.Parity}");
Console.WriteLine($"Configured serial data bits: {serialPortSettings.DataBits}");
Console.WriteLine($"Configured serial stop bits: {serialPortSettings.StopBits}");

var getOperatingStatus = CommandFactory.CreateCommand("getOperatingStatus", new string[] { }, 1);
// response: {"result":[Param1, Param2], "id":number}
// Param1 = 0 : 測定可能 (Measurable)
// Param1 = 3 : 測定可能狀態 (Mesurable state) 量測結束 
// Param1 = 4 : 測定 (measurement) 量測中

var getAnalysisResults = CommandFactory.CreateCommand("getanalysisResults", new string[] { "print.csv" }, 1);
// Get the specified measurement result data
// We could get the measurement result from the "print.csv"

Console.WriteLine(getOperatingStatus);
Console.WriteLine(getAnalysisResults);


using (var serialPortManger = new SerialPortManager(serialPortSettings))
{
    serialPortManger.PropertyChanged += (sender, e) =>
    {
        if (e.PropertyName == nameof(SerialPortManager.ReceivedData))
        {
            byte[] data = serialPortManger.ReceivedData;
            Console.WriteLine($"Data received: {BitConverter.ToString(data)}");
            string strData = Encoding.UTF8.GetString(data);
            Console.WriteLine(strData);

            //if(strData == "analysis finish!\r\n")
            //{
            //    serialPortManger.SendString("getResult");
            //}
            
        }
    };

    serialPortManger.Open();

    string input = Console.ReadLine();
    
    serialPortManger.SendString(input);


    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();

    //serialPortManger.Close();
}