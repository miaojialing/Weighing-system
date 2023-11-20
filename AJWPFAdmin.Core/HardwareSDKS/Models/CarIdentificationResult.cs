using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.Models
{
    public class CarIdentificationResult
    {
        /// <summary>
        /// 车牌号或卡号, 如果是高频读头, 这里应该是卡号,要注意判断
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 识别全图图片文件路径, 车牌识别后,处理的
        /// </summary>
        public string FullImgFile { get; set; }

        /// <summary>
        /// 识别车牌区域图片文件路径, 车牌识别后,处理的
        /// </summary>
        public string ClipImgFile { get; set; }
    }
}
