using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSP
{
    public struct Bit
    {
        byte value;
        public const int Zero = 0;
        public const int One = 1;

        public static bool operator >(Bit b1, Bit b2)
        {
            if (b1.value > b2.value)
            {
                return true;
            }
            return false;
        }

        public static bool operator <(Bit b1, Bit b2)
        {
            if (b1.value < b2.value)
            {
                return true;
            }
            return false;
        }
        public static bool operator ==(Bit b1, Bit b2)
        {
            if (b1.value == b2.value)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Bit b1, Bit b2)
        {
            if (b1.value != b2.value)
            {
                return true;
            }
            return false;
        }

        public static implicit operator Bit(uint x)
        {
            if (x == 0) return new Bit { value = 0 };
            return new Bit { value = 1 };
        }
        public static implicit operator uint(Bit x)
        {
            return x.value;
        }

        public static implicit operator Bit(bool x)
        {
            if (x == false) return new Bit { value = 0 };
            return new Bit { value = 1 };
        }
        public static implicit operator bool(Bit x)
        {
            if (x.value == 0) return false;
            return true;
        }

        static public void Set(ref uint value, int index, Bit bit)
        {
            if (index < 0 || index > 31) throw new ArgumentOutOfRangeException();
            if (bit == 0) value &= (uint)(~(1 << index));
            else value |=(uint)1 << index;
        }

        static public Bit Get(uint value, int index)
        {
            if (index < 0 || index > 31) throw new ArgumentOutOfRangeException();
            return value &=(uint)1 << index;
        }

        static public void Copy(uint source, int sourceIndex, ref uint destination, int destinationIndex)
        {
            Set(ref destination, destinationIndex, Get(source, sourceIndex));
        }
    }
}
