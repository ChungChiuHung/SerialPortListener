using System;
using System.Collections.Generic;
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
            string filePath = @"E:\CodeHere\SerialPortListener\SerialPortListener\DataParsing_RN700\retrived_data\output.txt";


            try 
            {
                List<byte> hexValue = ReadHexValuesFromFile(filePath);

                StringBuilder str = new StringBuilder();

                Console.WriteLine("Read hex values: ");
                int i = 0;
                foreach (byte b in hexValue)
                {
                    if (i == 16)
                    {
                        Console.WriteLine();
                        str.AppendLine();
                        i = 0;
                    }
                    //Console.Write($"{b:X2}");
                    byte[] byteArray = { b };
                    string strFromByte = Encoding.UTF8.GetString(byteArray);
                    Console.Write(strFromByte);
                    str.Append(strFromByte);
                    i++;
                }


                File.WriteAllText("result.txt", str.ToString());
                



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

        public static List<byte> ReadHexValuesFromFile(string filePath)
        {
            List<byte> hexValues = new List<byte>();
            try
            {
                string content = File.ReadAllText(filePath);

                content = content.Replace("\r\n", " ").Replace("\n", " ");

                string[] hexStrings = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string hex in hexStrings)
                {
                    if(!string.IsNullOrWhiteSpace(hex))
                    {
                        hexValues.Add(Convert.ToByte(hex, 16));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return hexValues;
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
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
