using System;
using System.IO.Ports;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using RN700Communication.Config;
using RN700Communication.SerialCommunication;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

SerialPortSettings serialPortSettings = config.GetSection("SerialPortSettings").Get<SerialPortSettings>();


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


using (var serialPortManger = new SerialPortManager(serialPortSettings))
{
    serialPortManger.PropertyChanged += (sender, e) =>
    {
        if (e.PropertyName == nameof(SerialPortManager.ReceivedData))
        {
            byte[] data = serialPortManger.ReceivedData;
            Console.WriteLine($"Data received: {BitConverter.ToString(data)}");
        }
    };

    serialPortManger.Open();
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}