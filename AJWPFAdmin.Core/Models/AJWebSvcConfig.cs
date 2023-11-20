using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Models
{
    public class AJWebSvcConfig
    {
        public string IP { get; set; }

        public int MQTTPort { get; set; }

        public int HttpPort { get; set; }

        public void Init()
        {
            IP = "127.0.0.1";
            MQTTPort = 23483;
            HttpPort = 5000;
        }
    }
}
