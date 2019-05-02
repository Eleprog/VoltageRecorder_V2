using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PSP1N;

namespace VRW
{
    public class Settings
    {
        public class DecodingSetting
        {
            readonly int[] packageDefault = { 32, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 };
            public StartBit StartBit { get; set; } = StartBit.ZERO;
            public int NumberOfChannelsADC { get; set; } = 10;
            public int[] PSP_Items { get; set; }
            public PackagePSPStructure PSPStructurePackage { get => new PackagePSPStructure(StartBit, PSP_Items); }

            public DecodingSetting()
            {
                PSP_Items = packageDefault;
            }
        }

        public class SerialPortSetting
        {
            public string PortName { get; set; } = "COM3";
            public int BaudRate { get; set; } = 250000;
            public Parity ParityBit { get; set; } = Parity.None;
            public int DataBits { get; set; } = 8;
            public StopBits StopBit { get; set; } = StopBits.One;
        }

        public class ChartDisplaySetting
        {
            public int VisiblePointsOnChart { get; set; } = 300;
            public int MouseScrollSpeed { get; set; } = 101;
        }

        public DecodingSetting Decoding { get; set; } = new DecodingSetting();
        public SerialPortSetting SerialPort { get; set; } = new SerialPortSetting();
        public ChartDisplaySetting ChartDisplay { get; set; } = new ChartDisplaySetting();

        static XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

        public void Save()
        {
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
