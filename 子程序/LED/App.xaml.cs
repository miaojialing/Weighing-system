using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace LED
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger("LED");

        private static readonly int CLOSE_APP = 0x0801;
        private static readonly int READY_TO_WEIGH = 0x0804;
        private static readonly int GET_PLATE = 0x0806;
        private static readonly int WEIGHING_COMPLETE = 0x0807;
        private static readonly int IS_WEIGHING = 0x0808;
        private static readonly int IS_STABLE = 0x080A;



        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LED.IEDManager.Instance.GreateLEDS(new List<LEDConfig>()
            {
                 new LEDConfig(){
                     IP=ConfigurationManager.AppSettings["LEDIP"],
                     AreaWidth=Convert.ToInt32(ConfigurationManager.AppSettings["LEDWIDTH"]),
                     AreaHeight=Convert.ToInt32(ConfigurationManager.AppSettings["LEDHEIGHT"])},
                   new LEDConfig(){
                     IP=ConfigurationManager.AppSettings["LED1IP"],
                     AreaWidth=Convert.ToInt32(ConfigurationManager.AppSettings["LED1WIDTH"]),
                     AreaHeight=Convert.ToInt32(ConfigurationManager.AppSettings["LED1HEIGHT"])}
            });

            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        //接收其他APP传来的消息
        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == CLOSE_APP)
            {
                Current.Shutdown();
            }
            if (msg.message == GET_PLATE)
            {
                LED.IEDManager.Instance.GET_PLATE();
            }
            if (msg.message == IS_WEIGHING)
            {
                LED.IEDManager.Instance.IS_WEIGHING();
            }
            if (msg.message == IS_STABLE)
            {
                LED.IEDManager.Instance.IS_STABLE();
            }
            if (msg.message == WEIGHING_COMPLETE)
            {
                LED.IEDManager.Instance.WEIGHING_COMPLETE();
            }
            if (msg.message == READY_TO_WEIGH)
            {
                LED.IEDManager.Instance.READY_TO_WEIGH();
            }
        }

        private void SendTxtToLed(string path)
        {
            LED.IEDManager.Instance.SendTxtToLed(path);
        }
    }
}
