using PSP1N;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VRW.Model.ControlStream
{
    public class DecodingStreamPSP : IDecodingStream
    {
        PSP psp;
        ByteBuffer buffer;
        int sizeBuffer;
        public int SizeBuffer
        {
            get => sizeBuffer;
            private set
            {
                if (sizeBuffer != value)
                {
                    sizeBuffer = value;
                    buffer = new ByteBuffer(sizeBuffer);
                }
            }
        }

        public Stream Stream { get; }

        PPoints pPoints;
        int numberOfPoints = 0;


        public PPoints Decoding(int numberOfPoints, long position = 0)
        {
            if (Stream == null) throw new NullReferenceException("No stream selected for reading");
            if (!Stream.CanRead) throw new Exception("Unable to read stream");

            var pack = psp.CreateNewPackage();

            if (numberOfPoints != this.numberOfPoints || Stream.CanSeek)
            {
                pPoints = new PPoints(10);
                this.numberOfPoints = numberOfPoints;
            }

            if (Stream.CanSeek)
            {
                Stream.Position = position;
            }

            while (!PSP.Decode(Stream, ref pack))
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(pack.Item[0].Value);
                dateTime = dateTime.AddMilliseconds(pack.Item[1].Value);

                float[] f = new float[10];
                for (int i = 0; i < 10; i++)
                {
                    f[i] = pack.Item[2 + i].Value;
                }
                pPoints.Add(dateTime, f);
                if (pPoints.Length > numberOfPoints)
                {
                    pPoints.RemoveFirst();
                    //break;
                }
                if (pPoints.Length == numberOfPoints)
                {
                    break;
                }
                if (!Stream.CanSeek) break;
            }

            return (PPoints)pPoints.Clone();


            PSP.Decode(Stream, ref pack);

            if (pack == null && pPoints.Length == 0) return null;
            else
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(pack.Item[0].Value);
                dateTime = dateTime.AddMilliseconds(pack.Item[1].Value);

                float[] f = new float[10];
                for (int i = 0; i < 10; i++)
                {
                    f[i] = pack.Item[2 + i].Value;
                }
                pPoints.Add(dateTime, f);
                if (pPoints.Length > numberOfPoints)
                {
                    pPoints.RemoveFirst();
                }
            }

            return (PPoints)pPoints.Clone();
        }

        //public PPoints Compression(int countPoints)
        //{
        //    long pos = 0;
            
        //    PPoints outPPoints = new PPoints(10);
        //    PPoint minPoint = new PPoint(10);
        //    PPoint maxPoint = new PPoint(10);
        //    for (int i = 0; i < maxPoint.Y.Length; i++)
        //    {
        //        maxPoint.Y[i] = psp.packageStructure. 

        //    }

        //    var pPoints = Decoding(countPoints, pos);

        //    for (int i = 0; i < countPoints; i++)
        //    {

        //    }
        //}

        public DecodingStreamPSP(Stream stream, PackagePSPStructure packagePSPStructure)
        {
            Stream = stream;
            psp = new PSP(packagePSPStructure);
        }
    }
}
