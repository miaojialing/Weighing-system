using AJWPFAdmin.Core.CommonEntity;
using AJWPFAdmin.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services.EF.Tables
{
    /// <summary>
    /// 仓库通行记录表
    /// </summary>
    public partial class WarehousePassageRecord : CommonTableEntity
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        [Comment("车牌号")]
        [Display(Name = "车牌号")]
        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(120)]
        public string CarNo { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        [Comment("卡号")]
        [Display(Name = "卡号")]
        [MaxLength(120)]
        public string IDCardNo { get; set; }

        /// <summary>
        /// 车辆类型名称
        /// </summary>
        [Comment("车辆类型名称")]
        [MaxLength(10)]
        public string TypeName { get; set; }

        /// <summary>
        /// 车辆类型Id
        /// </summary>
        [Comment("车辆类型Id")]
        public long TypeId { get; set; }

        /// <summary>
        /// 岗亭id
        /// </summary>
        [Comment("岗亭id")]
        public long WatchhouseId { get; set; }

        /// <summary>
        /// 岗亭名称
        /// </summary>
        [Comment("岗亭名称")]
        [MaxLength(50)]
        public string WatchhouseName { get; set; }

        /// <summary>
        /// 通道id
        /// </summary>
        [Comment("通道id")]
        public long PassagewayId { get; set; }

        /// <summary>
        /// 通道名称
        /// </summary>
        [Comment("通道名称")]
        [MaxLength(50)]
        public string PassagewayName { get; set; }

        /// <summary>
        /// 进出方向/状态
        /// </summary>
        [Comment("进出方向/状态")]
        public PassagewayDirection Direction { get; set; }

        /// <summary>
        /// 通道关联的仓库id
        /// </summary>
        [Comment("通道关联的仓库id")]
        public long? WarehouseId { get; set; }

        /// <summary>
        /// 通道关联的仓库名称
        /// </summary>
        [Comment("通道关联的仓库名称")]
        public string WarehouseName { get; set; }

        /// <summary>
        /// 车牌识别抓拍全图文件路径
        /// </summary>
        [Comment("车牌识别抓拍全图文件路径")]
        public string IdentifiedCaptureFullFile { get; set; }

        /// <summary>
        /// 车牌识别抓拍小图文件路径
        /// </summary>
        [Comment("车牌识别抓拍图文件路径")]
        public string IdentifiedCaptureSmallFile  { get; set; } 
        /// <summary>
        /// 监控抓拍图文件路径
        /// </summary>
        [Comment("监控抓拍图文件路径")]
        public string CameraCaptureFile { get; set; }

        /// <summary>
        /// 进厂时间
        /// </summary>
        [Comment("进厂时间")]
        [Display(Name = "进厂时间")]
        public DateTime? ShipStartDate { get; set; }

        /// <summary>
        /// 出厂时间 
        /// </summary>
        [Comment("出厂时间 ")]
        [Display(Name = "出厂时间 ")]
        public DateTime? ShipEndDate { get; set; }

        /// <summary>
        /// 关联车类型是否启用了车辆统计, 车牌识别到达后, 动态赋值
        /// </summary>
        [NotMapped]
        public bool EnablePassagewayStatistic { get; set; }

        /// <summary>
        /// 出厂岗亭id
        /// </summary>
        [Comment("出厂岗亭id")]
        public long? ExitWatchhouseId { get; set; }

        /// <summary>
        /// 出厂岗亭名称
        /// </summary>
        [Comment("出厂岗亭名称")]
        [MaxLength(50)]
        public string ExitWatchhouseName { get; set; }

        /// <summary>
        /// 出厂通道id
        /// </summary>
        [Comment("出厂通道id")]
        public long? ExitPassagewayId { get; set; }

        /// <summary>
        /// 出厂通道名称
        /// </summary>
        [Comment("出厂通道名称")]
        [MaxLength(50)]
        public string ExitPassagewayName { get; set; }

        /// <summary>
        /// 出厂设备Id
        /// </summary>
        [Comment("出厂设备Id")]
        public long? ExitDeviceId { get; set; }

        /// <summary>
        /// 出厂设备编号
        /// </summary>
        [Comment("出厂设备编号")]
        [MaxLength(50)]
        public string ExitDeviceCode { get; set; }

        /// <summary>
        /// 进厂设备Id
        /// </summary>
        [Comment("进厂设备Id")]
        public long DeviceId { get; set; }

        /// <summary>
        /// 进厂设备编号
        /// </summary>
        [Comment("进厂设备编号")]
        [MaxLength(50)]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 车牌识别小图图片路径数组, 由具体逻辑手动赋值
        /// </summary>
        [NotMapped]
        public string[] SmallImages { get; set; }

        /// <summary>
        /// 车牌识别全图片路径数组, 由具体逻辑手动赋值
        /// </summary>
        [NotMapped]
        public string[] FullImages { get; set; }

        /// <summary>
        /// 监控抓拍图片路径数组, 由具体逻辑手动赋值
        /// </summary>
        [NotMapped]
        public string[] CaptureImages { get; set; }

    }
}
