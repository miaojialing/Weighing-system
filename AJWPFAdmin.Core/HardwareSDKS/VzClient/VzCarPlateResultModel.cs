using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.VzClient
{
    public class VzCarPlateResultModel
    {
        public class Car_brand
        {
            /// <summary>
            /// 
            /// </summary>
            public int brand { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int year { get; set; }
        }

        public class RECT
        {
            /// <summary>
            /// 
            /// </summary>
            public int bottom { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int left { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int right { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int top { get; set; }
        }

        public class Car_location
        {
            /// <summary>
            /// 
            /// </summary>
            public RECT RECT { get; set; }
        }

        public class Location
        {
            /// <summary>
            /// 
            /// </summary>
            public RECT RECT { get; set; }
        }

        public class Timeval
        {
            /// <summary>
            /// 
            /// </summary>
            public int sec { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int usec { get; set; }
        }

        public class TimeStamp
        {
            /// <summary>
            /// 
            /// </summary>
            public Timeval Timeval { get; set; }
        }

        public class PlateResultData
        {
            /// <summary>
            /// 
            /// </summary>
            public int bright { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int carBright { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int carColor { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Car_brand car_brand { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Car_location car_location { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int colorType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int colorValue { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int confidence { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int direction { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int enable_encrypt { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int fake_plate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string featureCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string license { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int license_ext_type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Location location { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int plate_distance { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int plate_true_width { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public TimeStamp timeStamp { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int timeUsed { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int triggerType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int trigger_time_end { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int trigger_time_start { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int type { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlateResultData PlateResult { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int active_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int clipImgSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cmd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fullImgSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string imageformat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string timeString { get; set; }

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
