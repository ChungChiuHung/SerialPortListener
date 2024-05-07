using System;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace DataParsing_RN700
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Construct the path to the 'retrieved_data' folder located at the project level
            string filePath = @"E:\CodeHere\SerialPortListener\SerialPortListener\DataParsing_RN700\retrived_data\output.txt";


            try 
            {
                string content = File.ReadAllText(filePath);

                byte[] byteArray = content.Split(',')
                                            .Where(b => !string.IsNullOrWhiteSpace(b))
                                            .Select(b => Convert.ToByte(b.Trim()))
                                            .ToArray();

                string utf8String = Encoding.UTF8.GetString(byteArray);
                Console.WriteLine("UTF-8 String: ");
                Console.WriteLine(utf8String);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading the file or parsing the byte values: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}
