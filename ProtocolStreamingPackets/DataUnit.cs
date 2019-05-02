using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSP
{
    public class DataUnit
    {
        //количество бит 
        int size;
        public int Size
        {
            get => size;
            protected set
            {
                if (value <= 0 || value>32) throw new ArgumentOutOfRangeException();
                else size = value;
            }
        }
        //
        uint minValue;
        public uint MinValue
        {
            get => minValue;
            protected set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                else minValue = value;
            }
        }
        //
        uint maxValue;
        public uint MaxValue
        {
            get => maxValue;
            protected set
            {
                if (value < 1) throw new ArgumentOutOfRangeException();
                else maxValue = value;
            }
        }
        //
        private uint value;
        public uint Value
        {
            get { return value; }
            set
            {
                if (value >= MinValue && value <= MaxValue)
                {
                    this.value = value;
                }
                else throw new ArgumentOutOfRangeException("Value");
            }
        }
        //
        public DataUnit(uint maxValue, uint minValue = 0)
        {

            if (maxValue < 1 || minValue < 0) throw new ArgumentOutOfRangeException();
            MinValue = minValue;
            MaxValue = maxValue;
            do
            {
                Size++;
                maxValue /= 2;
            } while (maxValue > 0);
        }
        //
        public override string ToString()
        {
            return string.Format($"Value: {Value} | Size: {Size} | MinValue: {MinValue} | MaxValue: {MaxValue}");
        }

        public override bool Equals(object obj)
        {
            DataUnit du = obj as DataUnit;
            if (du == null) return false;

            if (du.MaxValue != MaxValue ||
                du.MinValue != MinValue ||
                du.Size != Size ||
                du.Value != Value) return false;

            return true;
        }

        //public DataUnit(int numberOfBits, uint minValue = 0)
        //{
        //    Size = numberOfBits;
        //    MinValue = minValue;

        //    uint size = 0;
        //    for (int i = 0; i <= numberOfBits; i++)
        //    {
        //        size = 1 << i;
        //    }
        //    MaxValue = size;
        //}

    }

    public class DataUnitBit : DataUnit
    {
        public DataUnitBit(int numberOfBits, uint minValue = 0) : base(1, minValue)
        {
            Size = numberOfBits;
            for (int i = 0; i < numberOfBits; i++)
            {
                MaxValue |= (uint)1 << i;
            }
        }
    }
}
