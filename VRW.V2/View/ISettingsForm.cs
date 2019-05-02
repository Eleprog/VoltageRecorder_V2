using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRW.View
{
    interface ISettingsForm
    {
        event EventHandler Save;
        event EventHandler Load;


    }
}
