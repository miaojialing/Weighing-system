using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LED
{
    public interface ILED
    {
        string Name { set; get; }
        void GET_PLATE();
        void SendTxtToLed(string path);
        void READY_TO_WEIGH();
        void WEIGHING_COMPLETE();
        void IS_STABLE();
        void IS_WEIGHING();
    }
}
