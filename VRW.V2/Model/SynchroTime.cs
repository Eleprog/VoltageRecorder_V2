using PSP1N;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRW.Model
{
    class SynchroTime
    {
        PackagePSP1N package = new PackagePSP1N(new PackagePSP1NStructure(StartBit.ZERO, 32));

        public void SendTime(SerialPort serialPort)
        {
            WriteTimeInPackage();
            var bytes = package.Encode();
            serialPort.Write(bytes, 0, bytes.Length);
        }

        private void WriteTimeInPackage()
        {           
            package.Item[0].Value = (uint)(DateTimeOffset.UtcNow.AddHours(3).ToUnixTimeSeconds());
            Debug.Print(package.Item[0].Value.ToString());
        }
    }
}
