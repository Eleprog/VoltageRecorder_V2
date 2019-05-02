using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using VRW.Model;
using VRW.Model.ControlStream;

namespace UnitTestPSP
{
    [TestClass]
    public class StreamControlTest
    {
        StreamControl streamControl;

        [TestInitialize]
        public void Init()
        {
            byte[] buffer = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            MemoryStream ms = new MemoryStream(buffer);

            byte[] buffer2 = new byte[10] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            Stream ms2 = new MemoryStream(buffer);

            streamControl = new StreamControl(2, 5);
            streamControl.AddStream("one", ms);
            streamControl.AddStream("two", ms2);
        }

        [TestMethod]
        public void ChangePosition()
        {
            var buffTest = new byte[2] { 2, 3 };
            Stream msTest = new MemoryStream(buffTest);

            streamControl.SeleсtStream("one");
            streamControl.Position = 1;

            byte[] buf = new byte[2];
            streamControl.ControlledStream.Read(buf, 0, 2);

            Assert.AreEqual(buffTest[0], buf[0]);
        }
    }
}
