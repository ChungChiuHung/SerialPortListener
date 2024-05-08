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
            // 1 bit : Debug Mode => 01
            // 2 bit : Tray Detection Mode => 02
            // 3 bit : Command Mode => 04
            // (When you are in command acceptance state, you cannot use commands that transition to a state)

            var getData = CommandFactory.CreateCommand("getData", new string[] { }, 1);
            // response: {“result”: Param1, “id”: number}
            // Param1 : YYYYMMDDhhmm.ss
            // Example: {“result”: “201405150910.00”, “id”: 1}

            var getCalcType = CommandFactory.CreateCommand("getCalcType", new string[] {}, 1);

            var setOperatingMode = CommandFactory.CreateCommand("setOperatingMode","02",1); // Command Mode
            // Set the operating mode. The setting value returns to the initial value when the RN-700 is powered On.
            // The initial values and details of the settings are described in the getOperatingMode command item

            var startAnalysis = CommandFactory.CreateCommand("startAnalysis", new string[] { }, 1);
            // Start measuring. Returns a response message after the measurement is complete

            var startAnalysisAsync = CommandFactory.CreateCommand("startAnalysisAsync", new string[] { }, 1);
            // Start measuring. Returns a response message before the measurement starts.

            var printResult = CommandFactory.CreateCommand("printResult", new int[] { 0, 1, -1 }, 1);
            // The last measured measurement result is printed according to the parameters.
            // The measurement results are deleted when the firmware starts.
            // Param1(int) : Printing Type (0: Detailed Classification 1: Standard Classification)
            // Param2(int) : Printing Calculation Method (0: Grain count % 1: Mass Conversion %)
            // Param3(int) : Specify the print result
            //               ( -1: Result without average
            //                  0: Average result at average measurement
            //                  1 or more: Nth result at average measurement)

            var setSaveData = CommandFactory.CreateCommand("setSaveData", new int[] { 1, 1, 0, 0 }, 0);
            // Set up data storage
            // Param1(int) : Measurement results (0: Do not save, 1: Save)
            // Param2(int) : Image Large (0: Do not save, 1: Save)
            // Param3(int) : Image/Small (0: Do not save 1: Save)
            // Param4(int) : Result list (0: Do not save, 1: Save)

            var chkTray = CommandFactory.CreateCommand("chkTray", new string[] { }, 1);
            // Perform a tray check
            // Response: {“result”:[Param1, Param2], “id”: number}
            // Param1(int): 
            // Param2(int): Error number




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
            Console.WriteLine(setOperatingMode.ToString());
            Console.WriteLine(startAnalysis.ToString());

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
                            string[] hexStringArray = ConvertByteArrayToHexStringArray(data);
                            Console.Write("Data Received: " + string.Join(" ", hexStringArray));
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

            readThread.IsBackground = true;
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
                string[] hexStringArray = ConvertByteArrayToHexStringArray(responseData);
                AppendTextToFile("output.txt", hexStringArray);
            }
            else
            {
                Console.WriteLine("Response timeout.");
            }
        }

        static void OnDataReceived(object sender, SerialDataEventArgs e)
        {
            responseData = e.Data;
            receivedEvent.Set();
        }

        private static void AppendTextToFile(string filePath, string[] hexStrings)
        {
            try
            {
                File.AppendAllLines(filePath, hexStrings);
                Console.WriteLine("Hex data appended to file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: "+ ex.Message);
            }
        }

        public static string[] ConvertByteArrayToHexStringArray(byte[] bytes)
        {
            string[] hexArray = new string[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                hexArray[i] = bytes[i].ToString("X2");
            }
            return hexArray;
        }

    }
}
