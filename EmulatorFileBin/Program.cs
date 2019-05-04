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

            bool ret = false;
            using (FileStream fs = File.OpenWrite("test3.bvr"))
            {
                uint time = 0;
                for (int i = 0; i < 10000; i++)
                {
                    time++;
                    pack.Item[0].Value = time/1000;
                    pack.Item[1].Value = time % 1000;

                    //pack.Item[2].Value = (uint)i;

                    if (i>100)
                    {
                        if (i % 100 == 0)
                        {
                            ret = !ret;
                        }
                        if (ret)
                        {
                            pack.Item[2].Value = 100;
                        }
                        else
                        {
                            pack.Item[2].Value = 3000;
                        }
                    }
                    else
                    {
                        pack.Item[2].Value = (uint)i;
                    }

                    //for (int k = 0; k < 1; k++)
                    //{
                    //    pack.Item[k + 2].Value = (uint)r.Next(4095);
                    //    if (k>4)
                    //    {
                    //        pack.Item[k + 2].Value = (uint)(100 * k);
                    //    }
                    //}

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
