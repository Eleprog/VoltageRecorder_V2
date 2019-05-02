using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRW.Model;
using VRW.Model.ControlStream;

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
        //event EventHandler LoadChart; //Событие возникающие при загрузке график

        SynchronizationContext SyncContext { get; }
        string ComPortName { get; set; }
        List<string> ComPortNames { set; }
        //int FileLoadingProgress { set; }
        int MaxValueScrollChart { set; }
        int ValueScrollChart { get; set; }
        PPoints MainChart { set; }
        MPoints PreviewChart { set; }
    }
}
