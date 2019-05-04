using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRW.Model;
using VRW.Model.ControlStream;
using VRW.Model.ChartingControl;

namespace VRW.View
{
    interface IMainView
    {
        event EventHandler StopDecodingSerialPort;
        event EventHandler ComPortConnection;
        event EventHandler OpenDataFile;
        event EventHandler CopyPointValuesToArray;
        event EventHandler UpdateChart; //Событие обновления графика
        event EventHandler ExitProgramm;
        event EventHandler ComPortNamesUpdate;
        event EventHandler SynchronizationTime;

        //int VisiblePointsOnChart { get; set; }
        SynchronizationContext SyncContext { get; }
        string ComPortName { get; set; }
        List<string> ComPortNames { set;  }
        int MaxValueScrollChart { set; }
        int ValueScrollChart { get; set; }
        PPoints MainChart { set; }
        PPoints PreviewChart { set; }
        Zoom ZoomChart { set; get; }
    }
}
