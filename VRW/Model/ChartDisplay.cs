using PSP1N;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRW.Model.ControlStream;

namespace VRW.Model
{
    class ChartDisplay
    {
        private string fileName;
        //private int numberOfParallelTasks = 10;

        public ChartDisplay(string fileName)
        {
            this.fileName = fileName;

        }

        //public async Task<MPoints> ZoomOutAsync(int position, int quantityPointsInput, int quantityPointsOutput)
        //{
        //    if (quantityPointsOutput % 2 != 0) throw new Exception("Количество точек на выходе должно быть четным");
        //    int blockSize = quantityPointsInput / quantityPointsOutput; //размер блока
        //    int quantityBlocks = quantityPointsOutput; //количество блоков
        //    //TODO:добавить конечный блок
        //    //if (quantityPointsInput % quantityPointsOutput != 0) quantityBlocks += 1;

        //    var tasks = new Task<MPoints>[quantityBlocks];

        //    for (int i = 0; i < quantityBlocks; i++)
        //    {
        //        tasks[i] = Task.Run(() => Decrease(position + (i * blockSize), quantityBlocks, factor));
        //    }
        //    var result = await Task.WhenAll(tasks);

        //    MPoints mPointsOut = new MPoints(quantityPointsOutput);
        //    for (int i = 0; i < result.Length; i++)
        //    {
        //        //TODO: если размеры результата разные то будет ошибка
        //        mPointsOut.Add(i * result[i].Length, result[i]);
        //    }
        //    return mPointsOut;
        //}

        public async Task<MPoints> DecreaseAsync(int position, int quantityPointsInput, int quantityPointsOutput)
        {
            if (quantityPointsOutput % 2 != 0) throw new Exception("Количество точек на выходе должно быть четным");
            int blockSize = quantityPointsInput / quantityPointsOutput; //размер блока
            int quantityBlocks = quantityPointsOutput; //количество блоков
            //TODO:добавить конечный блок
            //if (quantityPointsInput % quantityPointsOutput != 0) quantityBlocks += 1;
            int quantityPairPointsOut = quantityBlocks / 2; //количество пар точек
            MPoints mPOut = new MPoints(quantityBlocks);
            var tasks = new Task<MPoints>[quantityPairPointsOut];
            for (int i = 0; i < quantityPairPointsOut; i++)
            {
                tasks[i] = BlockDecreaseAsync(position + i * blockSize, blockSize);
            }
            MPoints[] result = await Task.WhenAll(tasks);

            for (int i = 0; i < quantityPairPointsOut; i++)
            {
                mPOut.Insert(i * 2, result[i]);
            }
            //GC.Collect();
            return mPOut;            
        }

        private async Task<MPoints> BlockDecreaseAsync(int position, int blockSize)
        {
            return await Task.Run(() =>
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    //StreamDecoding sd = new StreamDecoding(fs);

                    MPoints mPointsOut = new MPoints(2);
                    MPoints mPointsBlock;

                    //Создание блока из потока
                    //mPointsBlock = sd.DecodingBlock(blockSize, position);
                    //StreamControl sc = new StreamControl();
                    //sc.SeleсtStream(fs);
                    //var s = sc.ReadStream(blockSize, position);
                    PackagePSP package = new PackagePSP(new PackagePSPStructure(StartBit.ZERO, 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12));

                    List<MPoint> mPoints = new List<MPoint>();
                    while (true)
                    {
                       // if (PSP.Decode(s, ref package) == true) break;
                        MPoint mPoint = new MPoint();
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        dateTime = dateTime.AddSeconds(package.Item[0].Value);
                        dateTime = dateTime.AddMilliseconds(package.Item[1].Value);
                        mPoint.X = dateTime;
                        for (int i = 0; i < mPoint.Y.Length; i++)
                        {
                            mPoint.Y[i] = (int)package.Item[i + 2].Value;
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



                    //Выборка первой и последней даты в блоке
                    mPointsOut.X[0] = points.X.First();
                    mPointsOut.X[1] = points.X.Last();

                    //Выборка минимальных и максимальных Y значений
                    for (int g = 0; g < mPointsOut.Y.Length; g++)
                    {
                        mPointsOut.Y[g].Value[0] = points.Y[g].Value.Min();
                        mPointsOut.Y[g].Value[1] = points.Y[g].Value.Max();
                    }
                    return mPointsOut;
                }
            }
            );
        }

        //static public MPoints ZoomOutAll(StreamDecoding fileDecoding, int reduceToQuantity)
        //{
        //    //Вычисления...
        //    int block = fileDecoding.NumberOfPackageInStream / reduceToQuantity; //Размер блока
        //    int blockEnd = fileDecoding.NumberOfPackageInStream % reduceToQuantity; //Размер конечного блока
        //    int numberOfBlocks; //Всего блоков на выходе
        //    int temp = 0;
        //    if (blockEnd != 0) temp = 1;
        //    //TODO: добавить конечные блоки
        //    numberOfBlocks = (fileDecoding.NumberOfPackageInStream / block);// + temp;
        //    //Точки на выходе
        //    MPoints mPointsOut = new MPoints(numberOfBlocks);
        //    //Блок точек
        //    MPoints mPointsBlock;

        //    for (int i = 0; i < (numberOfBlocks / 2); i++)
        //    {
        //        //Создание блока из потока
        //        mPointsBlock = fileDecoding.DecodingBlock(i * block, block);

        //        //Выборка первой и последней даты в блоке
        //        mPointsOut.X[i * 2] = mPointsBlock.X.First();
        //        mPointsOut.X[i * 2 + 1] = mPointsBlock.X.Last();

        //        //Выборка минимальных и максимальных Y значений
        //        for (int g = 0; g < mPointsOut.Y.Length; g++)
        //        {
        //            mPointsOut.Y[g].Value[i * 2] = mPointsBlock.Y[g].Value.Min();
        //            mPointsOut.Y[g].Value[i * 2 + 1] = mPointsBlock.Y[g].Value.Max();
        //        }
        //    }
        //    GC.Collect();
        //    return mPointsOut;
        //}
    }
}
