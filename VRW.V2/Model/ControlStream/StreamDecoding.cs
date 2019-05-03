using PSP1N;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VRW.Model.ControlStream
{
    public class StreamDecoding : IDecodingStream
    {
        //PSP psp;
        PackagePSP1N package;
        ByteBuffer buffer;
        int sizeBuffer;
        PPoints pPoints;
        int numberOfPoints = 0;

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

        public StreamDecoding(Stream stream, PackagePSP1NStructure structure)
        {
            Stream = stream;
            package = new PackagePSP1N(structure);
        }


        int countErrors = 0;
        
        public PPoints Decoding(int numberOfPoints, long position = 0)
        {
            if (Stream == null) throw new NullReferenceException("No stream selected for reading");
            if (!Stream.CanRead) throw new Exception("Unable to read stream");

            //var pack = psp.CreateNewPackage();
            package.Clear();

            if (numberOfPoints != this.numberOfPoints || Stream.CanSeek)
            {
                pPoints = new PPoints(10);
                this.numberOfPoints = numberOfPoints;
            }

            if (Stream.CanSeek)
            {
                Stream.Position = position;
            }
            
            while (true)
            {
                var resultDecode = package.Decode(Stream);
                if (resultDecode.EndStream==true)
                {
                    break;
                }
                countErrors += (resultDecode.ErrorsEncoding);
                //Debug.Print(countErrors.ToString());
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(package.Item[0].Value);
                dateTime = dateTime.AddMilliseconds(package.Item[1].Value);

                float[] f = new float[10];
                for (int i = 0; i < 10; i++)
                {
                    f[i] = (float)(package.Item[2 + i].Value * Program.settings.Decoding.Calibration[i]);
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
                package.Decode(Stream);
            }

            return (PPoints)pPoints.Clone();
        }               
    }
}
