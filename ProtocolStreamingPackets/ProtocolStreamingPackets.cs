using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSP
{
    public enum Result
    {
        Error,
        Ok,
        End
    }

    public class ProtocolStreamingPackets
    {
        //TODO: добавить методы запуска и остановки чтения потока
        //TODO: добавить возможно очередь пакетов
        Stream stream;
        public PackagePSP Package { get; private set; }
        int[] packetBytes;
        public event EventHandler<PackagePSP> PackageReceived; // пакет получен
        public event EventHandler PackageError; // ошибка в пакете
        public event EventHandler EndStream; // конец потока
        public bool IsStart { get; private set; } = false;
        //
        public ProtocolStreamingPackets(Stream stream, PackagePSP packagePSP)
        {
            this.stream = stream;
            Package = packagePSP;
            packetBytes = new int[packagePSP.PacketSizeInBytes];

        }
        //Запуск декодирования 
        public async Task Start()
        {
            if (IsStart) return;
            IsStart = true;
            await Task.Run(() =>
            {
                while (IsStart)
                {
                    Result r = ReadPackage();
                    switch (r)
                    {
                        case Result.Error:
                            PackageError?.Invoke(this, new EventArgs());
                            break;
                        case Result.Ok:
                            PackageReceived?.Invoke(this, Package);
                            break;
                        case Result.End:
                            EndStream?.Invoke(this, new EventArgs());
                            Stop();
                            return;
                    }
                }
            });
        }

        private void Stop()
        {
            IsStart = false;
        }

        //Чтение пакета из потока         
        Result ReadPackage()
        {
            do
            {
                packetBytes[0] = stream.ReadByte();
                if (packetBytes[0] == -1) return Result.End;
            } while (!IsStartBit(packetBytes[0]));

            for (int i = 1; i < packetBytes.Length; i++)
            {
                packetBytes[i] = stream.ReadByte();
                if (IsStartBit(packetBytes[i]) || packetBytes[i] == -1)
                {
                    return Result.Error;
                }
            }
            Package.Decoding(packetBytes);
            return Result.Ok;
        }
        //Проверка на стартовый бит
        bool IsStartBit(int value)
        {
            int startBit = (value & 0b10000000) >> 7;
            if (startBit == (uint)Package.StartBit)
            {
                return true;
            }
            return false;
        }
        //
        //public IEnumerable<uint> GetDataArray()
        //{
        //    foreach (var item in Package)
        //    {
        //        yield return item.Value;
        //    }
        //    Console.WriteLine();
        //}
    }

}
