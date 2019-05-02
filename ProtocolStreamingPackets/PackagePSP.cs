using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSP
{
    public class PackagePSP : List<DataUnit>
    {

        public int PacketSizeInBytes { get; } //количество байт в пакете
        public Bit StartBit { get; }  //стартовый бит

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(StartBit + "|");
            foreach (var item in this)
            {
                str.Append(string.Format($"{item.Size}|"));
            }
            return str.ToString();
        }
        public PackagePSP(Bit startBit, params DataUnit[] dataUnit)
        {
            StartBit = startBit;
            int bit = 0;
            foreach (var item in dataUnit)
            {
                Add(item);
                bit += item.Size;
            }
            PacketSizeInBytes = bit / 7;
            if (bit % 7 > 0) PacketSizeInBytes++;
        }
        public PackagePSP(Bit startBit, List<DataUnit> dataUnits) : this(startBit, dataUnits.ToArray()) { }
        //
        public void Decoding(int[] bytes)
        {
            int position = 0;
            for (int i = 0; i < Count; i++)
            {
                this[i].Value = GetDataInBytes(bytes, position, this[i].Size);
                position += this[i].Size;
            }
        }
        //
        uint GetDataInBytes(int[] bytes, int position, int size)
        {
            int y = 0;
            int x = 0;
            uint value = 0;
            for (int i = 0; i < size; i++)
            {
                x = 6 - (i + position) % 7;
                y = (i + position) / 7;

                Bit.Copy((uint)bytes[y], x, ref value, size - i -1);
            }
            return value;
        }
        //
        // override object.Equals
        public override bool Equals(object obj)
        {
            PackagePSP p= obj as PackagePSP;
            if (p == null) return false;
            for (int i = 0; i < Count; i++)
            {
                if (!p[i].Equals(this[i])) return false;
            }
            if (p.StartBit != this.StartBit) return false;
            if (PacketSizeInBytes != p.PacketSizeInBytes) return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
    //class PackageStructure
    //{

    //    public int NumberOfBytesInPacket { get; } //количество байт в пакете
    //    public Bit StartBit { get; }  //стартовый бит
    //    List<DataUnit> data { get; set; }
    //    public DataUnit this[int index]
    //    {
    //        get
    //        {
    //            return data[index];
    //        }
    //        set
    //        {
    //            data[index] = value;
    //        }
    //    }
    //    public void Add(DataUnit dataUnit)
    //    {
    //        data.Add(dataUnit);
    //    }
    //    public override string ToString()
    //    {
    //        StringBuilder str = new StringBuilder();
    //        str.Append(StartBit + "|");
    //        foreach (var item in data)
    //        {
    //            str.Append(string.Format($"{item.Size}|"));
    //        }
    //        return str.ToString();
    //    }
    //    public PackageStructure(List<DataUnit> dataUnits, Bit startBit)
    //    {
    //        StartBit = startBit;
    //        data = dataUnits;
    //        NumberOfBytesInPacket = data.Count / 7;
    //        if (data.Count % 7 > 0) NumberOfBytesInPacket++;
    //    }

    //    void Decoding(int[] bytes)
    //    {
    //        int position = 0;
    //        for (int i = 0; i < data.Count; i++)
    //        {
    //            data[i].Value = GetDataInBytes(bytes, position, data[i].Size);
    //            position += data[i].Size;
    //        }
    //    }

    //    uint GetDataInBytes(int[] bytes, int position, int size)
    //    {
    //        int y = 0;
    //        int x = 0;
    //        uint value = 0;
    //        for (int i = 0; i < size; i++)
    //        {
    //            x = (i + position) % 7;
    //            y = (i + position) / 7;

    //            Bit.Copy(bytes[y], x, ref value, i);
    //        }
    //        return value;
    //    }
    //}
}
