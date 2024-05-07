using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.IO;

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

            var command = CommandFactory.CreateCommand("getUser", new string[] { "Alice", "Bob" }, 123);
            var getStatus = CommandFactory.CreateCommand("getStatus", new string[] {}, 1);
            var getOperatingStatus = CommandFactory.CreateCommand("getOperatingStatus", new string[] { }, 1);
            // response: {"result":[Param1, Param2], "id":number}
            // Param1 = 0 : 測定可能 (Measurable)
            // Param1 = 3 : 測定可能狀態 (Mesurable state)
            // Param1 = 4 : 測定 (measurement)
            var getLimitSwitch = CommandFactory.CreateCommand("getLimitSwitch", new string[] { }, 1);
            // response: {"result:[Param1, Param2], "id":number}
            // Param1 TraySwitch1 (ON/OFF : 1/0)
            // Param2 TraySwitch2 (ON/OFF : 1/0)
            // Param3 Entrance shutter switch (ON/OFF : 1/0)
            var getLcdBitmap = CommandFactory.CreateCommand("getLcdBitmap", new string[] { }, 1);
            // response: {"result":"binary", "id":number}
            // Get the LCD display bitmap

            var getAnalysisResults = CommandFactory.CreateCommand("getAnalysisResults",new string[] {"result.csv"}, 1);
            // Get the specified measurement result data

            var getAnalysisType = CommandFactory.CreateCommand("getAnalysisType", new string[] { }, 1);
            // Acquires the set measurement type ID and measurement type name
            // response: {"result":[Param1, "Param2],"id":number}
            // Param1(int) : 測定粒種 ID
            // Param2(string) : 測定粒種名稱
            // Example: {“result”: [1, “うるち短粒種玄米”], “id”: 1}

            var getAnalysisLevelVal = CommandFactory.CreateCommand("getAnalysisLevelVal", 1, 1);
            // Retrieves the sorting level name for the specified sorting level ID
            // response: {“result”: [“Param1”], “id”: number}
            // Param1(string) : Sorting Level Name
            // Example: {“result”: [“選別レベル 1”], “id”: 1}

            var getSettingFile = CommandFactory.CreateCommand("getSettingFile", new string[] { "rn700.conf" }, 1);
            // Retrieves the configuration file with the specified file name form the /tmp/config folder of the RN700
            // The specified configuration file data (binary data) is transmitted.

            var getOperatingMode = CommandFactory.CreateCommand("getOperatingMode", new string[] { }, 1);
            // Gets the mode of operation.
            // response: {“result”: “Param1”, “id”: number}
            // Param1(string) : 00~FF
            // 1 bit : Debug Mode
            // 2 bit : Tray Detection Mode
            // 3 bit : Command Mode
            // (When you are in command acceptance state, you cannot use commands that transition to a state)

            var getData = CommandFactory.CreateCommand("getData", new string[] { }, 1);
            // response: {“result”: Param1, “id”: number}
            // Param1 : YYYYMMDDhhmm.ss
            // Example: {“result”: “201405150910.00”, “id”: 1}

            var getCalcType = CommandFactory.CreateCommand("getCalcType", null, 1);





            Console.WriteLine(command.ToString());
            Console.WriteLine(getStatus.ToString());
            Console.WriteLine(getOperatingStatus.ToString());
            Console.WriteLine(getLimitSwitch.ToString());
            Console.WriteLine(getAnalysisResults.ToString());
            Console.WriteLine(getAnalysisType.ToString());
            Console.WriteLine(getAnalysisLevelVal.ToString());
            Console.WriteLine(getSettingFile.ToString());
            Console.WriteLine(getData.ToString());
            Console.WriteLine(getCalcType.ToString());

            Console.ReadLine();



            SerialSettings settings = new SerialSettings
            {
                PortName = "COM5",
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
            byte delimiter = Encoding.UTF8.GetBytes("}")[0];

            int index = Array.IndexOf(responseData, delimiter);

            if (index != -1)
            {
                byte[] firstPart = new byte[index];
                byte[] secondPart = new byte[responseData.Length - index - 1];

                Array.Copy(responseData, 0, firstPart, 0, index);
                Array.Copy(responseData, index + 1, secondPart, 0, responseData.Length - index - 1);

                Console.WriteLine("First Part: " + Encoding.UTF8.GetString(firstPart));

                string byteValues = String.Join(",", secondPart);

                string filePath = "output.txt";

                File.WriteAllText(filePath, byteValues);

                Console.WriteLine("Second Part has been saved to " + filePath);
            }
            else
            {
                Console.WriteLine("Delimiter not found in the array.");
            }
            // string receivedData = Encoding.UTF8.GetString(e.Data);
            //Console.WriteLine("Data Received: " + receivedData);
        }


    }
}
