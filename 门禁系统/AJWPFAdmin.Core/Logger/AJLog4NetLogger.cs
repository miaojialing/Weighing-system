using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Logger
{
    public class AJLog4NetLogger
    {
        private ILog _logger;
        public AJLog4NetLogger()
        {
            log4net.Config.XmlConfigurator
                .ConfigureAndWatch(new FileInfo(Path.Combine(AppContext.BaseDirectory, "Logger\\log4net.config")));
            _logger = LogManager.GetLogger("ajfilelog");
        }

        public void Info(string message, Exception ex = null)
        {
            _logger.Info(message, ex);
        }

        public void Error(string message, Exception ex = null)
        {
            _logger.Error(message, ex);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }
    }
}
