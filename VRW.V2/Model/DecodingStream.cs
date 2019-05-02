using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSP1N;

namespace VRW.Model
{
    class DecodingStream
    {

        PackagePSP package = new PackagePSP(new PackagePSPStructure(StartBit.ZERO, 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12));

        public MPoints Decode(MemoryStream memoryStream)
        {            
            List<MPoint> mPoints = new List<MPoint>();

            while (true)
            {
                if (PSP.Decode(memoryStream, ref package)==true) break;
                MPoint mPoint = new MPoint();
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(package.Item[0].Value);
                dateTime = dateTime.AddMilliseconds(package.Item[1].Value);
                mPoint.X = dateTime;
                for (int i = 0; i < mPoint.Y.Length; i++)
                {
                    mPoint.Y[i] = (int)package.Item[i+2].Value;
                };
                mPoints.Add(mPoint);
                
            }
            //Debug.Print(mPoints.Count.ToString());
            if (mPoints.Count == 0) return null;

            MPoints points = new MPoints(mPoints.Count);
            for (int i = 0; i < mPoints.Count; i++)
            {
                points[i] = mPoints[i];
               
            }
            return points;
        }
    }
}
