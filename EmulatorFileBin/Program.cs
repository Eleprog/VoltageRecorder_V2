using PSP1N;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorFileBin
{
    class Program
    {
        static void Main(string[] args)
        {
            PackagePSP pack = new PackagePSP(new PackagePSPStructure(StartBit.ZERO, 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12));
            Random r = new Random();

            using (FileStream fs = File.OpenWrite("test3.bvr"))
            {
                uint time = 0;
                for (int i = 0; i < 100000; i++)
                {
                    time++;
                    pack.Item[0].Value = time/1000;
                    pack.Item[1].Value = time % 1000;
                   
                    for (int k = 0; k < 10; k++)
                    {
                        pack.Item[k + 2].Value = (uint)r.Next(4095);
                    }

                    var p = PSP.Encode(pack);

                    fs.Write(p, 0, p.Length);

                }
            }
            Console.WriteLine("OK");
            Console.ReadKey();
        }
        private static byte[] intToBytes(int value)
        {
            byte[] b = new byte[2];
            b[0] = (byte)(value % 100);
            b[1] = (byte)(value / 100);
            return b;
        }
    }
}
