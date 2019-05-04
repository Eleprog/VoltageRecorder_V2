using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PSP1N;

namespace VRW.Model
{
    public class Settings
    {

        public class DecodingSetting
        {
            readonly int[] packageDefault = { 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 };
            public StartBit StartBit { get; set; } = StartBit.ZERO;
            public int NumberOfChannelsADC { get; set; } = 10;
            public int[] PSP_Items { get; set; }
            public PackagePSP1NStructure PSPStructurePackage { get => new PackagePSP1NStructure(StartBit, PSP_Items); }
            readonly double[] calibrationDefault = { 0.0008059, 0.0008059, 0.0008059, 0.0008059, 0.000805, 0.0008059, 0.0008059, 0.0008059, 0.0008059, 0.0008059 };
            public double[] Calibration { get; set; }

            public DecodingSetting()
            {
                PSP_Items = packageDefault;
                Calibration = calibrationDefault;
            }
        }

        public class SerialPortSetting
        {
            public string PortName { get; set; } = "COM3";
            public int BaudRate { get; set; } = 250000;
            public Parity ParityBit { get; set; } = Parity.Odd;
            public int DataBits { get; set; } = 8;
            public StopBits StopBit { get; set; } = StopBits.One;
        }

        public class ChartDisplaySetting
        {
            const int FIXED_MAX_ZOOM = 3000;
            const int FIXED_MIN_ZOOM = 50;
            public int VisiblePointsOnChart { get; set; } = 300;
            public int MouseScrollSpeed { get; set; } = 100;

            int minZoom = FIXED_MIN_ZOOM;
            public int MinZoom
            {
                get => minZoom;
                set => minZoom = value < FIXED_MIN_ZOOM ? FIXED_MIN_ZOOM : value;
            }
            int maxZoom = FIXED_MAX_ZOOM;
            public int MaxZoom
            {
                get => maxZoom;
                set => maxZoom = value > FIXED_MAX_ZOOM ? FIXED_MAX_ZOOM : value;
            }
        }

        public DecodingSetting Decoding { get; set; } = new DecodingSetting();
        public SerialPortSetting SerialPort { get; set; } = new SerialPortSetting();
        public ChartDisplaySetting ChartDisplay { get; set; } = new ChartDisplaySetting();

        static XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

        public void Save()
        {
            if (File.Exists(Properties.Settings.Default.SettingsFileName))
            {
                File.Delete(Properties.Settings.Default.SettingsFileName);
            }
            using (FileStream fs = File.OpenWrite(Properties.Settings.Default.SettingsFileName))
            {
                xmlSerializer.Serialize(fs, this);
            }
        }

        public static Settings Load()
        {
            if (File.Exists(Properties.Settings.Default.SettingsFileName))
                using (FileStream fs = File.OpenRead(Properties.Settings.Default.SettingsFileName))
                {
                    return xmlSerializer.Deserialize(fs) as Settings;
                }
            return new Settings();
        }
    }
}
