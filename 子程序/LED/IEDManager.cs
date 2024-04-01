using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LED
{
    public class IEDManager
    {
        private static IEDManager _IEDManager;
        private static List<ILED> _LEDS;

        public static List<ILED> LEDS
        {
            get
            {
                return _LEDS;
            }
        }

        public static IEDManager Instance
        {
            get
            {
                if (_IEDManager == null)
                {
                    _IEDManager = new IEDManager();
                }

                return _IEDManager;
            }
        }

        private IEDManager()
        {
            _LEDS = new List<ILED>();
        }

        /// <summary>
        /// 根据IP集合来批量创建LED
        /// </summary>
        /// <param name="ips"></param>
        public void GreateLEDS(List<LEDConfig> configs)
        {
            foreach (var i in configs)
            {
                if (!string.IsNullOrWhiteSpace(i.IP))
                {
                    _LEDS.Add(new LEDControl(i));
                }
            }
        }

        public void GET_PLATE()
        {
            RunAction((led) => { led.GET_PLATE(); });
        }
        public void READY_TO_WEIGH()
        {
            RunAction((led) => { led.READY_TO_WEIGH(); });
        }
        public void WEIGHING_COMPLETE()
        {
            RunAction((led) => { led.WEIGHING_COMPLETE(); });
        }
        public void IS_STABLE()
        {
            RunAction((led) => { led.IS_STABLE(); });
        }
        public void IS_WEIGHING()
        {
            RunAction((led) => { led.IS_WEIGHING(); });
        }
        public void SendTxtToLed(string path)
        {
            RunAction((led) => { led.SendTxtToLed(path); });
        }
        private void RunAction(Action<ILED> action, int delay = 200)
        {
            //Task.Run(() =>
            //{
            foreach (var led in _LEDS)
            {
                action(led);
                //Task.Delay(delay).Wait();
                Thread.Sleep(delay);
            }
            //});
        }

    }

    public class LEDConfig
    {
        public string IP { set; get; }
        public int AreaWidth { get; set; }
        public int AreaHeight { get; set; }
    }


}
