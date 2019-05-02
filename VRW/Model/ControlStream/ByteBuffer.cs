using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRW.Model.ControlStream
{
    class ByteBuffer
    {
        List<byte> buffer;
        public int Lenght { get; }
        public byte[] Buffer { get => buffer.ToArray(); }
        public int Count { get => buffer.Count; }

        public ByteBuffer(int sizeBuffer)
        {
            Lenght = sizeBuffer;
            buffer = new List<byte>(sizeBuffer);
        }

        public void Add(byte data)
        {
            if (buffer.Count >= Lenght)
            {
                buffer.Remove(buffer.First());
            }
            buffer.Add(data);
        }

        public void Clear()
        {
            buffer.Clear();
        }
    }
}
