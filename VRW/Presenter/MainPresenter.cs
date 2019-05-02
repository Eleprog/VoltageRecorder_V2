using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRW.Model;
using VRW.View;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using VRW.Model.ControlStream;
using PSP1N;

namespace VRW.Presenter
{
    class MainPresenter
    {
        IMainView view;
        int progressUpdate = 0;

        public SerialPort serialPort = new SerialPort(
            Program.settings.SerialPort.PortName,
            Program.settings.SerialPort.BaudRate,
            Program.settings.SerialPort.ParityBit,
            Program.settings.SerialPort.DataBits,
            Program.settings.SerialPort.StopBit);
        //TODO: private
        //public TempStreamDecoding streamDecoding = new TempStreamDecoding();
        FileStream fileStreamRead;
        //StreamDecoding decoding;
        string PathFileNameBvr;

        //Decoding decoding;

        bool isFileLoad = false;
        //public StreamControl streamControl = new StreamControl();
        public DecodingStream decodingStream = new DecodingStream();
        public DecodingStreamPSP DecodStreamPSP;
        MemoryStream ms;
        public MainPresenter(IMainView view)
        {
            this.view = view;
            //view.RunDecodingSerialPort += RunDecodingSerialPort;
            view.StopDecodingSerialPort += StopDecodingSerialPortEvent;
            view.ComPortConnection += ComPortConnectionEvent;
            view.OpenDataFile += OpenDataFileEvent;
            ComPortNamesUpdate();
            view.ComPortName = Program.settings.SerialPort.PortName;
            view.CopyPointValuesToArray += CopyPointValuesToArrayEvent;
            view.UpdateChart += UpdateChartMainEvent;
            view.ExitProgramm += ExitProgrammEvent;
            view.ComPortNamesUpdate += ComPortNamesUpdateEvent;

            serialPort.DataReceived += DataReceivesEvent;
            //streamDecoding.NewPackageEvent += NewPackageEvent;
            //streamDecoding.EndDecodingStreamEvent += EndDecodingStreamEvent;
            fs = File.OpenWrite("xxx.bin");


            //streamControl.AddStream("COM_Port", serialPort.BaseStream);
            if (serialPort.IsOpen)
                DecodStreamPSP = new DecodingStreamPSP(serialPort.BaseStream, new PackagePSPStructure(StartBit.ZERO, 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12));
            //streamControl.SeleсtStream(serialPort.BaseStream);
        }

        private void ComPortNamesUpdateEvent(object sender, EventArgs e)
        {
            ComPortNamesUpdate();
        }

        //private void ComPortNamesUpdateEvent(object sender, EventArgs e) => ComPortNamesUpdate();


        private void ComPortNamesUpdate()
        {
            view.ComPortNames = SerialPort.GetPortNames().ToList();
        }

        private void ExitProgrammEvent(object sender, EventArgs e)
        {
            Program.settings.Save();
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        private async void UpdateChartPreview()
        {
            //ChartDisplay cd = new ChartDisplay(PathFileNameBvr);
            //MPoints PreviewChart = await cd.DecreaseAsync(0, decoding.NumberOfPackageInStream, 1000);
            //view.PreviewChart = PreviewChart;
            //ChartDisplay cd = new ChartDisplay(PathFileNameBvr);
            // MPoints PreviewChart = await cd.DecreaseAsync(0, (int)(streamControl.LenghtStream), 1000);
            // view.PreviewChart = PreviewChart;
        }

        private void UpdateChartMainEvent(object sender, EventArgs e)
        {
            // PPoints MainChart = decodingStream.Decode(streamControl.ReadStream(Program.settings.ChartDisplay.VisiblePointsOnChart, view.ValueScrollChart));
            PPoints MainChart;
            //do
            {
                MainChart = DecodStreamPSP.Decoding(Program.settings.ChartDisplay.VisiblePointsOnChart, view.ValueScrollChart);
            }
            //while (MainChart != null);

            view.MaxValueScrollChart = (int)(DecodStreamPSP.Stream.Length );
            view.MainChart = MainChart;

            //view.SyncContext.Post((obj) => view.MainChart = (PPoints)obj, MainChart);

            //if (decoding != null)
            {
                //Debug.Print("view.ValueScrollChart: {0}", view.ValueScrollChart.ToString());
                //MPoints MainChart = decoding.DecodingBlock(Program.settings.ChartDisplay.VisiblePointsOnChart, view.ValueScrollChart);

                //view.MaxValueScrollChart = decoding.NumberOfPointsForScrollBar;
                //view.MainChart = MainChart;
            }
        }

        FileStream fs;
        private void WriteToFile(Stream stream)
        {
            stream.CopyTo(fs);
        }

        private void EndDecodingStreamEvent(object sender, EventArgs e)
        {
            //Debug.Print("END");
        }

        private void CopyPointValuesToArrayEvent(object sender, EventArgs e)
        {
            //PointValues[] pv = model.CopyPointValuesToArray();
            //model.cl
            //Debug.Print("CountPoint {0}", pv.Count.ToString());
        }

        //private void NewPackageEvent(object sender, StreamDecodingEventArgs e)
        //{

        //}

        int countReceivesByte = 0;

        private void DataReceivesEvent(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (sender as SerialPort);
            countReceivesByte++;

            if (sp.IsOpen)
            {
                try
                {
                    PPoints p = DecodStreamPSP.Decoding(Program.settings.ChartDisplay.VisiblePointsOnChart, view.ValueScrollChart);
                    if (countReceivesByte >= 24)
                    {
                        countReceivesByte = 0;
                        view.SyncContext.Post((obj) => view.MainChart = (PPoints)obj, p);

                    }
                    //DecodStreamPSP.Decoding(Program.settings.ChartDisplay.VisiblePointsOnChart, view.ValueScrollChart);
                }
                catch (Exception)
                {

                }

                //Debug.Print(sp.BytesToRead.ToString());
                //while (sp.BytesToRead>0)
                //{
                //    ms.WriteByte((byte)sp.ReadByte());
                //    countReceivesByte++;
                //}
                //Debug.Print(countReceivesByte.ToString());
                //countReceivesByte = 0;


                //view.SyncContext.Post((obj) => view.MainChart = (MPoints)obj, decodingStream.Decode(streamControl.ReadStream(Program.settings.ChartDisplay.VisiblePointsOnChart)));
                //ms = new MemoryStream();
                //if (countReceivesByte == 24)
                //{
                //    //view.SyncContext.Post((obj) => view.MainChart = (MPoints)obj, decoding.DecodingBlock(Program.settings.ChartDisplay.VisiblePointsOnChart));

                //    //view.SyncContext.Post((obj) => view.MainChart = (MPoints)obj, decodingStream.Decode(streamControl.ReadStream(Program.settings.ChartDisplay.VisiblePointsOnChart))); 


                //    countReceivesByte = 0;
                //}
                //else
                //{
                //    //decoding.DecodingBlock(Program.settings.ChartDisplay.VisiblePointsOnChart);
                //   // decodingStream.Decode(streamControl.ReadStream(Program.settings.ChartDisplay.VisiblePointsOnChart));
                //}


            }

            countReceivesByte++;

            // IsComPortConnUpdate();
        }

        private void OpenDataFileEvent(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                openFileDialog.Filter = "Binary voltage recoder file (*.bvr)|*.bvr";
                if (openFileDialog.ShowDialog() == DialogResult.Cancel) return;
                isFileLoad = true;
                //serialPort.DataReceived -= DataReceivesEvent;
                PathFileNameBvr = openFileDialog.FileName;
                //streamDecoding.ClearCollectionPointValues();
                view.ValueScrollChart = 0;
                fileStreamRead = File.OpenRead(PathFileNameBvr);
                //decoding = new StreamDecoding(fileStreamRead);

                DecodStreamPSP = new DecodingStreamPSP(fileStreamRead, new PackagePSPStructure(StartBit.ZERO, 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12));

                UpdateChartMainEvent(sender, e);
                UpdateChartPreview();

            }
        }

        private void ComPortConnectionEvent(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
                serialPort.Close();

            serialPort.PortName = view.ComPortName;
            try
            {
                serialPort.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Не верный COM порт!");
                return;
            }
            Program.settings.SerialPort.PortName = serialPort.PortName;
            //decoding = new SerialPortDecoding(serialPort.BaseStream);
            //streamControl = new StreamControl();


            //view.SyncContext.Post((obj) => view.MainChart = (MPoints)obj, decodingStream.Decode(streamControl.ReadStream(Program.settings.ChartDisplay.VisiblePointsOnChart)));
        }


        public void LoadSettings()
        {
            Program.settings = Settings.Load();
        }

        public void SaveSettings()
        {
            Program.settings.Save();
        }

        public void StopDecodingSerialPortEvent(object sender, EventArgs e)
        {
            //streamDecoding.Stop();
        }
    }
}
