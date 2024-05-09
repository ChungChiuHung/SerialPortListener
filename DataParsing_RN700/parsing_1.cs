using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataParsing_RN700
{
    public class parsing_1
    {
        public parsing_1(byte[] byteArray)
        {
            int index = Array.IndexOf(byteArray, (byte)0x7D);

            if (index != -1)
            {
                index += 1;

                // Split the array  into two parts at the index of 0x7D
                byte[] response_part = new byte[index];
                byte[] secondPart = new byte[byteArray.Length - index ];

                Array.Copy(byteArray, 0, response_part, 0, index);
                Array.Copy(byteArray, index , secondPart, 0, byteArray.Length - index);

                ProcessSecondPart(secondPart);

                Console.WriteLine("First part: ");
                
                string resultString = Encoding.UTF8.GetString(response_part);

                Console.WriteLine(resultString);
            }
        }


        private void ProcessSecondPart(byte[] secondPart)
        {
            if (secondPart.Length >= 8)
            {
                byte[] firstPart = new byte[4];
                byte[] lastPart = new byte[4];
                byte[] dataPart = new byte[secondPart.Length - 8];

                Array.Copy(secondPart, 0, firstPart, 0, 4);
                Array.Copy(secondPart, secondPart.Length - 4, lastPart, 0, 4);
                Array.Copy(secondPart, 4, dataPart, 0, secondPart.Length - 8);

                Console.WriteLine("First 4 bytes:");
                PrintBytes(firstPart);

                BigInteger firstPartBigInteger = new BigInteger(firstPart);
                Console.WriteLine(firstPartBigInteger);

                //Console.WriteLine("Last 4 bytes:");
                //PrintBytes(lastPart);

                //BigInteger lastPartBigInteger = new BigInteger(lastPart);
                //Console.WriteLine(lastPartBigInteger);

                //DisplayBigIntegersAsUTF8(dataPart, 2);

                byte[] delimiter = { 0x2c, 0x20 };

                List<byte[]> result = SplitByteArray(dataPart, delimiter);

                foreach (byte[] segment in result)
                {
                    //Console.WriteLine(BitConverter.ToString(segment));
                    string decodedString = Encoding.UTF8.GetString(segment);
                    Console.WriteLine($"Decoded String: {decodedString}");

                }

            }
            else
            {
                Console.WriteLine("Not enough bytes in second part to split as specified.");
            }
        }

        static List<byte[]> SplitByteArray(byte[] dataPart, byte[] delimiter)
        {
            List<byte[]> parts  = new List<byte[]>();
            int start = 0;
            for(int i = 0; i<=dataPart.Length - delimiter.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < delimiter.Length; j++)
                {
                    if (dataPart[i+j] != delimiter[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    int length = i - start;
                    if(length > 0)
                    {
                        byte[] part = new byte[length];
                        Array.Copy(dataPart, start, part, 0, length);
                        parts.Add(part);
                    }
                    start = i + delimiter.Length;
                    i = start - 1;
                }
            }

            if (start < dataPart.Length)
            {
                byte[] part = new byte[dataPart.Length - start];
                Array.Copy(dataPart, start, part, 0, part.Length);
                parts.Add(part);
            }

            return parts;
        }

        static void PrintBytes(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                Console.WriteLine($"0x{b:X2} ");
            }
            Console.WriteLine();
        }

        static BigInteger[] ConvertByteArrayToBigInteger(byte[] bytes, int segmentSize)
        {
            int numberOfBigIntegers = bytes.Length / segmentSize;
            BigInteger[] bigIntegers = new BigInteger[numberOfBigIntegers];

            for (int i = 0; i < numberOfBigIntegers; i++)
            {
                byte[] segment = new byte[segmentSize];
                Array.Copy(bytes, i* segmentSize, segment, 0, segmentSize);

                bigIntegers[i] = new BigInteger(segment);
            }
            return bigIntegers;
        }

        static void DisplayBigIntegersAsUTF8(byte[] dataPart, int segmentSize)
        {
            BigInteger[] bigIntegers = ConvertByteArrayToBigInteger(dataPart, segmentSize);
            Console.WriteLine("Data part BigIntegers parsed to UTF-8: ");

            foreach (BigInteger bigInt in bigIntegers)
            {
                string bitIntString = bigInt.ToString();
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(bitIntString);
                Console.WriteLine($"{bigInt} -> UTF-8: {BitConverter.ToString(utf8Bytes).Replace("-", " ")}");
            }
        }

    }
}
