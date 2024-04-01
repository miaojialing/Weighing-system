using System.Windows;
using System.Windows.Interop;

namespace Monitor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {       
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == IPCHelper.CLOSE_APP)
            {
                Current.Shutdown();
            }
        }
    }
}
