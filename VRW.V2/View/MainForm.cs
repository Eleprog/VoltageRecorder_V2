using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
//TODO: delete VRW.Model
using VRW.Model;
using VRW.Model.ControlStream;
using VRW.View;


namespace VRW.View
{
    public partial class MainForm : Form, IMainView
    {
        public string ComPortName
        {
            get
            {
                return comboBox1.Text;
            }
            set
            {
                comboBox1.Text = value;
            }
        }
        public List<string> ComPortNames
        {
            set
            {
                comboBox1.Items.AddRange(value.ToArray());
            }
          
        }
        //public int FileLoadingProgress { set => progressBar1.Value = value; }
        public SynchronizationContext SyncContext { get; }
        public int MaxValueScrollChart { set => hScrollBar1.Maximum = value; }
        public int ValueScrollChart { get => hScrollBar1.Value; set => hScrollBar1.Value = value; }
        public PPoints MainChart
        {
            set
            {
                if (value!=null)
                {
                    for (int i = 0; i < value.Y.Length; i++)
                    {
                        //chart1.Series[i].Points.DataBindXY(value.X, value.Y[i].Value);
                        chart1.Series[i].Points.DataBindXY(value.X, value.Y[i]);
                    }

                    chart1.DataBind();
                }
            }
        }

        //public MPoints PreviewChart
        //{
        //    set
        //    {
        //        for (int i = 0; i < value.Y.Length; i++)
        //        {
        //            chart1.Series[i + value.Y.Length].Points.DataBindXY(value.X, value.Y[i].Value);
        //        }
        //    }
        //}

        PPoints IMainView.PreviewChart { set => throw new NotImplementedException(); }
        public int VisiblePointsOnChart { get =>  hScrollBar2.Value; set => hScrollBar2.Value = value; }

        public event EventHandler StopDecodingSerialPort;
        public event EventHandler ComPortConnection;
        public event EventHandler OpenDataFile;
        public event EventHandler CopyPointValuesToArray;
        public event EventHandler UpdateChart;
        public event EventHandler ExitProgramm;
        public event EventHandler ComPortNamesUpdate;
        public event EventHandler SynchronizationTime;

        public MainForm()
        {
            InitializeComponent();
            SyncContext = SynchronizationContext.Current;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopDecodingSerialPort(sender, e);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitProgramm(this, e);         
            System.Environment.Exit(0);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComPortConnection(sender, e);
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ComPortConnection(sender, e);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenDataFile(sender, e);
            checkedListBox1.Items.Clear();
            for (int i = 0; i < Program.settings.Decoding.NumberOfChannelsADC; i++)
            {
                checkedListBox1.Items.Add("Канал " + i, true);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();

            for (int i = 0; i < Program.settings.Decoding.NumberOfChannelsADC; i++)
                chart1.Series.Add("Канал " + i);

            for (int i = 0; i < Program.settings.Decoding.NumberOfChannelsADC; i++)
                chart1.Series.Add("Превью канал " + i);

            for (int i = 0; i < chart1.Series.Count / 2; i++)
            {
                chart1.Series[i].ChartArea = chart1.ChartAreas[0].Name; 
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[i].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            }
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd.MM.yy HH:mm:ss.fff";

            for (int i = chart1.Series.Count / 2; i < chart1.Series.Count; i++)
            {
                chart1.Series[i].ChartArea = chart1.ChartAreas[1].Name;
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[i].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;

            }
            chart1.ChartAreas[1].AxisX.LabelStyle.Format = "dd.MM.yy HH:mm";


            checkedListBox1.Items.Clear();
            for (int i = 0; i < Program.settings.Decoding.NumberOfChannelsADC; i++)
            {
                checkedListBox1.Items.Add("Канал " + i, true);
            }

            Text = "Монитор параметров ЖАТ  -  Version " + Application.ProductVersion;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

            UpdateChart(sender, e);
            //Debug.Print(ValueScrollChart.ToString() + " " + hScrollBar1.Value);

        }

        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            int scroll = Program.settings.ChartDisplay.MouseScrollSpeed;
            if (e.Delta > 0 && hScrollBar1.Value >= hScrollBar1.Minimum && hScrollBar1.Value < hScrollBar1.Maximum - scroll)
                hScrollBar1.Value += scroll;
            else if (e.Delta < 0 && hScrollBar1.Value > hScrollBar1.Minimum + scroll && hScrollBar1.Value <= hScrollBar1.Maximum)
                hScrollBar1.Value -= scroll;

            UpdateChart(sender, e);
        }


        private void checkedListBox1_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    chart1.Series[i].Enabled = checkedListBox1.GetItemChecked(i);
                }
                UpdateChart(sender, e);
            }
           ), null);
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }


        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            ComPortNamesUpdate(sender,e);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SynchronizationTime(sender, e);
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            //Program.settings.ChartDisplay.VisiblePointsOnChart = hScrollBar2.Value;
            UpdateChart(sender, e);
        }
    }
}
