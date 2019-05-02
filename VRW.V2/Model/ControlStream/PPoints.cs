using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRW.Model.ControlStream
{
    public class PointCreater
    {
        public int NumberOfY { get; }

        public PointCreater(int numberOfY)
        {
            NumberOfY = numberOfY;
        }

        public PPoint NewPoint()
        {
            return new PPoint(NumberOfY);
        }

        public PPoints NewPoints()
        {
            return new PPoints(NumberOfY);
        }
    }

    public class PPoint
    {
        public static int NumberOfY { get; set; } = 0;
        public DateTime X { get; set; } 
        public float[] Y { get; set; }

        public PPoint(int countY)
        {
            X = new DateTime(0);
            Y = new float[countY];
            for (int i = 0; i < countY; i++)
            {
                Y[i] = 0;
            }
        }
    }


    public class PPoints : ICloneable
    {
        public List<DateTime> X { get; set; } = new List<DateTime>();
        public List<float>[] Y{ get; set; }
        public int Length { get => X.Count; }

        public void Add(PPoint point)
        {
            Add(point.X, point.Y);
        }

        public void Add(DateTime X, float[] Y)
        {
            this.X.Add(X);
            for (int i = 0; i < Y.Length; i++)
            {
                this.Y[i].Add(Y[i]);
            }            
        }
         
        public void RemoveFirst()
        {
            X.Remove(X.First());
            for (int i = 0; i < Y.Length; i++)
            {
                Y[i].Remove(Y[i].First()); 
            }
        }

        public object Clone()
        {
            var p = new PPoints(Y.Length);
            p.X = X.ToList();
            for (int i = 0; i < Y.Length; i++)
            {
                p.Y[i] = Y[i].ToList();
            }
            return p;
        }

        public PPoints(int countY)
        {
            Y = new List<float>[countY];
            for (int i = 0; i < countY; i++)
            {
                Y[i] = new List<float>();
            }
        }
    }
}
