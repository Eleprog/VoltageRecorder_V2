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

        public SerialPort serialPort = new SerialPort(
            Program.settings.SerialPort.PortName,
            Program.settings.SerialPort.BaudRate,
            Program.settings.SerialPort.ParityBit,
            Program.settings.SerialPort.DataBits,
            Program.settings.SerialPort.StopBit);

        FileStream fileStreamRead;
        string PathFileNameBvr;

        public StreamDecoding DecodStreamPSP;

        public MainPresenter(IMainView view)
        {
            this.view = view;
            view.StopDecodingSerialPort += StopDecodingSerialPortEvent;
            view.ComPortConnection += ComPortConnectionEvent;
            view.OpenDataFile += OpenDataFileEvent;
            ComPortNamesUpdate();
            view.ComPortName = Program.settings.SerialPort.PortName;
            view.UpdateChart += UpdateChartMainEvent;
            view.ExitProgramm += ExitProgrammEvent;
            view.ComPortNamesUpdate += ComPortNamesUpdateEvent;
            view.SynchronizationTime += SynchronizationTimeEvent;

            serialPort.DataReceived += DataReceivesEvent;

            if (serialPort.IsOpen)
                DecodStreamPSP = new StreamDecoding(serialPort.BaseStream, new PackagePSP1NStructure(StartBit.ZERO, 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12));

        }

        private void SynchronizationTimeEvent(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                SynchroTime synchroTime = new SynchroTime();
                synchroTime.SendTime(serialPort); 
            }
        }

        private void StopDecodingSerialPortEvent(object sender, EventArgs e)
        {
            serialPort.Close();
        }

        private void ComPortNamesUpdateEvent(object sender, EventArgs e)
        {
            ComPortNamesUpdate();
        }

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

        }

        private void UpdateChartMainEvent(object sender, EventArgs e)
        {
            Program.settings.ChartDisplay.VisiblePointsOnChart = view.VisiblePointsOnChart;
            if (DecodStreamPSP!=null && DecodStreamPSP.Stream.CanSeek)
            {
                PPoints MainChart;
                {
                    MainChart = DecodStreamPSP.Decoding(Program.settings.ChartDisplay.VisiblePointsOnChart, view.ValueScrollChart);
                    //Debug.Print("Points: " + Program.settings.ChartDisplay.VisiblePointsOnChart + " position: " + view.ValueScrollChart);
                }
                view.MaxValueScrollChart = (int)(DecodStreamPSP.Lenght);
                view.MainChart = MainChart; 
            }
        }

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
                    if (countReceivesByte >= 96)
                    {
                        countReceivesByte = 0;
                        view.SyncContext.Post((obj) => view.MainChart = (PPoints)obj, p);
                    }
                }
                catch (Exception)
                {

                }
            }

            countReceivesByte++;
        }

        private void OpenDataFileEvent(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                openFileDialog.Filter = "Binary voltage recoder file (*.bvr)|*.bvr";
                if (openFileDialog.ShowDialog() == DialogResult.Cancel) return;
                PathFileNameBvr = openFileDialog.FileName;
                view.ValueScrollChart = 0;
                fileStreamRead = File.OpenRead(PathFileNameBvr);
                DecodStreamPSP = new StreamDecoding(fileStreamRead, new PackagePSP1NStructure(StartBit.ZERO, 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12));

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
                if (serialPort.IsOpen)
                    DecodStreamPSP = new StreamDecoding(serialPort.BaseStream, new PackagePSP1NStructure(StartBit.ZERO, 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12));
            }
            catch (Exception)
            {
                MessageBox.Show("Не верный COM порт!");
                return;
            }
            Program.settings.SerialPort.PortName = serialPort.PortName;
        }


        public void LoadSettings()
        {
            Program.settings = Settings.Load();
        }

        public void SaveSettings()
        {
            Program.settings.Save();
        }
    }
}
