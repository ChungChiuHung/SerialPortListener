using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace DataParsing_RN700
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Construct the path to the 'retrieved_data' folder located at the project level
            string filePath = @"G:\CodeHere\SerialPortListener\DataParsing_RN700\retrived_data\output.txt";


            try 
            {
                string content = File.ReadAllText(filePath);

                byte[] byteArray = content.Split(' ')
                                            .Select(hex => Convert.ToByte(hex, 16))
                                            .ToArray();

                Console.Write("Byte Array: ");
                foreach (byte b in byteArray)
                {
                    Console.Write($"{b:X2} ");
                }
                Console.WriteLine();

                ConvertBytesToUTF8String(byteArray);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading the file or parsing the byte values: " + ex.Message);
            }

            Console.ReadLine();
        }

        static void ConvertBytesToUTF8String(byte[] bytes)
        {
            try
            {
                string utf8String = System.Text.Encoding.UTF8.GetString(bytes);
                Console.WriteLine("Decoded UTF-8 string: "+ utf8String);
            }
            catch(Exception ex) 
            {
                Console.WriteLine("Error decoding UTF8 string: "+ ex.Message);
            }
        }
    }
}
