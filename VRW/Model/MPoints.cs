using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRW.Model
{

    public class MPoint
    {
        public DateTime X { get; set; }
        public int[] Y { get; set; } = new int[Program.settings.Decoding.NumberOfChannelsADC];
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"X={X}, ");
            for (int i = 0; i < Y.Length; i++)
            {
                sb.Append($"Y{i}={Y[i]}, ");
            }
            return sb.ToString();
        }
    }

    public class MPoints
    {
        public class PointY
        {
            public int[] Value { get; set; }
            public PointY(int size)
            {
                Value = new int[size];
            }
        }
        public DateTime[] X { get; set; }
        public PointY[] Y { get; set; } = new PointY[Program.settings.Decoding.NumberOfChannelsADC];
        public MPoint this[int index]
        {
            get
            {
                MPoint mPoint = new MPoint();
                mPoint.X = X[index];
                for (int i = 0; i < Y.Length; i++)
                {
                    mPoint.Y[i] = Y[i].Value[index];
                }
                return mPoint;
            }
            set
            {
                X[index] = value.X;
                for (int i = 0; i < Y.Length; i++)
                {
                    Y[i].Value[index] = value.Y[i];
                }
            }
        }
        public MPoints(int size)
        {
            X = new DateTime[size];
            for (int i = 0; i < Y.Length; i++)
            {
                Y[i] = new PointY(size);
            }
        }
        public void Insert(int index, MPoints mPoints)
        {
            mPoints.X.CopyTo(X, index);
            for (int i = 0; i < Y.Length; i++)
            {
                mPoints.Y[i].Value.CopyTo(Y[i].Value, index);
            }
        }
        public int Length { get => X.Length; }
    }
}