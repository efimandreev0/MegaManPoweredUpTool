using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaManLINK
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Extract("2.link");
        }
        public static void Extract(string arc)
        {
            var reader = new BinaryReader(File.OpenRead(arc));
            string str = arc.Replace(".LINK", "").Replace(".link","") + "//";
            reader.BaseStream.Position = 0xC;
            int count = (ReadUpToTwelveNulls(reader) + 2) / 8;
            int[] pointers = new int[count];
            int[] size = new int[count];
            reader.BaseStream.Position = 0xC;
            for (int i = 0; i < count; i++)
            {
                pointers[i] = reader.ReadInt32();
                size[i] = reader.ReadInt32();
                reader.BaseStream.Position += 4;
            }

            Directory.CreateDirectory(arc.Replace(".LINK", "").Replace(".link", ""));
            for (int i = 0; i < count; i++)
            {
                reader.BaseStream.Position = pointers[i];
                string name = Encoding.UTF8.GetString(reader.ReadBytes(4));
                reader.BaseStream.Position -= 4;
                byte[] bytes = reader.ReadBytes(size[i]);
                File.WriteAllBytes(str + i + "." + name, bytes);
            }
        }
        public static int ReadUpToTwelveNulls(BinaryReader binaryReader)
        {
            int nullBytesCount = 0;
            int totalBytesRead = 0;

            while (true)
            {
                byte byteRead = binaryReader.ReadByte();
                totalBytesRead++;

                if (byteRead == 0)
                {
                    nullBytesCount++;
                    if (nullBytesCount == 12)
                    {
                        break; // Найдено 12 нулевых байтов подряд, выходим из цикла.
                    }
                }
                else
                {
                    nullBytesCount = 0;
                }

                if (binaryReader.BaseStream.Position == binaryReader.BaseStream.Length)
                {
                    break; // Достигнут конец файла.
                }
            }

            return totalBytesRead - nullBytesCount;
        }




    }
}
