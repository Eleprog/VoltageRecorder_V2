using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSP1N;

namespace VRW.Model.ControlStream
{
    interface IDecodingStream
    {
        Stream Stream { get; }
        PPoints Decoding(int numberOfPoints, long position = 0);
    }
}
