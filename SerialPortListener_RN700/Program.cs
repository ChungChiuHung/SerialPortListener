using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Runtime;

namespace SerialPortListener_RN700
{
    internal class Program
    {
        static SerialPortManager serialPortManager;
        static ManualResetEventSlim receivedEvent = new ManualResetEventSlim(false);
        static byte[] responseData;

        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            SerialSettings settings = new SerialSettings
            {
                PortName = "COM3",
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };

            serialPortManager = new SerialPortManager(settings);
            serialPortManager.NewSerialDataRecieved += OnDataReceived;
            SetupSerialPort();

            //serialPortManager.NewSerialErrorReceived += OnErrorReceived;

           while (true)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Read from Serial Port");
                Console.WriteLine("2. Write to Serial Port");
                Console.WriteLine("3. Exit");
                Console.WriteLine("Enter choice (1-3): ");

                var choice = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (choice)
                {
                    case '1':
                        ReadFromSerialPort();
                        break;
                    case '2':
                        WriteToSerialPortAndWaitForResponse();
                        break;
                    case '3':
                        cancellationTokenSource.Cancel();
                        serialPortManager.Dispose();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
                        break;
                }
            }

        }


        static void SetupSerialPort()
        {
            serialPortManager.StartListening();
            Console.WriteLine("Setup Serial Port and Start Listening!");
        }

        static void ReadFromSerialPort()
        {
            cancellationTokenSource = new CancellationTokenSource();
            Thread readThread = new Thread(() =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested && serialPortManager.ShouldProcessData)
                {
                    if(serialPortManager.SerialPortIsOpen)
                    {
                        byte[] data = serialPortManager.ReadData();
                        if (data.Length > 0)
                        {
                            string receivedData = Encoding.UTF8.GetString(data);
                            Console.WriteLine("Data Received: " + receivedData);
                        }
                    }
                    else 
                    {
                        Console.WriteLine("Serial port is not open.");
                        break;
                    }
                    
                    Thread.Sleep(100);  // Adjusted the sleep interval for more frequent checks
                }
            })
            {
                IsBackground = true
            };
            readThread.Start();
        }

        static void WriteToSerialPortAndWaitForResponse() 
        {
            cancellationTokenSource.Cancel();
            serialPortManager.StopReading();

            Console.WriteLine("Enter text to send: ");
            string input = Console.ReadLine();
            byte[] dataToSend = Encoding.UTF8.GetBytes(input);

            receivedEvent.Reset();
            responseData = null;

            serialPortManager.sendByte(dataToSend, 0, dataToSend.Length);
            Console.WriteLine("Data send. Waiting for response... ");

            serialPortManager.StartReading();
            cancellationTokenSource = new CancellationTokenSource();

            if (receivedEvent.Wait(5000))
            {
                string response = Encoding.UTF8.GetString(responseData);
                Console.WriteLine("Response Received: " + response);
            }
            else
            {
                Console.WriteLine("No response received within the timout peroid.");
            }

            serialPortManager.StartReading();
            cancellationTokenSource = new CancellationTokenSource();
        }

        static void OnDataReceived(object sender, SerialDataEventArgs e)
        {
            responseData = e.Data;
            receivedEvent.Set();
            string receivedData = Encoding.UTF8.GetString(e.Data);
            Console.WriteLine("Data Received: " + receivedData);
        }
    }
}
