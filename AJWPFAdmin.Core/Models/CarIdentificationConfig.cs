using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Models
{
    /// <summary>
    /// 车辆识别配置
    /// </summary>
    public class CarIdentificationConfig
    {
        /// <summary>
        /// 图片存放路径
        /// </summary>
        public string ImageSavePath { get; set; }

        public static string GetDefaultSavePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "CarNoImages");
        }
    }
}
