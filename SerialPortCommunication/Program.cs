using System;
using System.IO.Ports;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;


namespace SerialPortCommunication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DisplaySerialPortInfo();
            string selectedPort = GetSelectedPortFromUser();
            ConfigureAndOpenSerialPort(selectedPort);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void DisplaySerialPortInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SerialPort");
            foreach(ManagementObject queryObj in searcher.Get())
            {
                Console.WriteLine("----------------------------");
                Console.WriteLine($"Caption: {queryObj["Caption"]}");
                Console.WriteLine($"DeviceID: {queryObj["DeviceID"]}");
                Console.WriteLine($"Name: {queryObj["Name"]}");
                Console.WriteLine($"Description: {queryObj["Description"]}");
                Console.WriteLine($"MaxBaudRate: {queryObj["MaxBaudRate"]}");
            }
        }

        static string GetSelectedPortFromUser()
        {
            Console.WriteLine("Enter the COM Port you want to use (e.g., COM3:");
            return Console.ReadLine();
        }

        static void ConfigureAndOpenSerialPort(string portName)
        {
            using (SerialPort serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One))
            {
                try
                {
                    serialPort.Open();
                    Console.WriteLine($"Port {portName} opened successfully.");
                    // Further operations with serialPort here...
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message );
                }
            }
        }

        
    }
}
