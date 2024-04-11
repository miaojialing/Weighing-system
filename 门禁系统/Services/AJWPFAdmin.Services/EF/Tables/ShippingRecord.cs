using AJWPFAdmin.Core.CommonEntity;
using AJWPFAdmin.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AJWPFAdmin.Services.EF.Tables
{
    /// <summary>
    /// 运输台账记录表
    /// </summary>
    [Comment("运输台账记录表")]
    public partial class ShippingRecord : CommonTableEntity
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        [Comment("车牌号")]
        [Display(Name = "车牌号")]
        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(120)]
        [StringLength(120,ErrorMessage = "{0}超长:{1}")]
        public string CarNo { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        [Comment("卡号")]
        [Display(Name = "卡号")]
        [MaxLength(120)]
        [StringLength(120, ErrorMessage = "{0}超长:{1}")]
        public string IDCardNo { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>
        [Comment("物料种类")]
        [Display(Name = "物料种类")]
        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string MaterialCategory { get; set; }

        /// <summary>
        /// 货物名称
        /// </summary>
        [Comment("货物名称")]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string MaterialName { get; set; }

        /// <summary>
        /// 到港时间
        /// </summary>
        [Comment("到港时间")]
        [Display(Name = "到港时间")]
        public DateTime ArriveDate { get; set; }

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
        /// 运单净重 
        /// </summary>
        [Comment("运单净重")]
        [Display(Name = "运单净重")]
        [Precision(18,2)]
        public decimal OrderNetWeight { get; set; }

        /// <summary>
        /// 汽车衡净重 
        /// </summary>
        [Comment("汽车衡净重")]
        [Display(Name = "汽车衡净重")]
        [Precision(18, 2)]
        public decimal CarNetWeight { get; set; }

        /// <summary>
        /// 发货单位
        /// </summary>
        [Comment("发货单位")]
        [Display(Name = "发货单位")]
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "{0}超长:{1}")]
        public string SenderName { get; set; }

        /// <summary>
        /// 收货单位
        /// </summary>
        [Comment("收货单位")]
        [Display(Name = "收货单位")]
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "{0}超长:{1}")]
        public string ReceiverName { get; set; }

        /// <summary>
        /// 装/卸货地点泊位
        /// </summary>
        [Comment("装/卸货地点泊位")]
        [Display(Name = "装/卸货地点泊位")]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string Berth { get; set; }

        /// <summary>
        /// 进厂岗亭id
        /// </summary>
        [Comment("进厂岗亭id")]
        public long WatchhouseId { get; set; }

        /// <summary>
        /// 进厂岗亭名称
        /// </summary>
        [Comment("进厂岗亭名称")]
        [MaxLength(50)]
        public string WatchhouseName { get; set; }

        /// <summary>
        /// 进厂通道id
        /// </summary>
        [Comment("进厂通道id")]
        public long PassagewayId { get; set; }

        /// <summary>
        /// 进厂通道名称
        /// </summary>
        [Comment("进厂通道名称")]
        [MaxLength(50)]
        public string PassagewayName { get; set; }

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
        /// 入口车牌识别抓拍图文件路径
        /// </summary>
        [Comment("入口车牌识别抓拍图文件路径")]
        public string EntranceIdentifiedCaptureFile { get; set; }

        /// <summary>
        /// 入口监控抓拍图文件路径
        /// </summary>
        [Comment("入口监控抓拍图文件路径")]
        public string EntranceCameraCaptureFile { get; set; }

        /// <summary>
        /// 出口车牌识别抓拍图文件路径
        /// </summary>
        [Comment("出口车牌识别抓拍图文件路径")]
        public string ExitIdentifiedCaptureFile { get; set; }

        /// <summary>
        /// 出口监控抓拍图文件路径
        /// </summary>
        [Comment("出口监控抓拍图文件路径")]
        public string ExitCameraCaptureFile { get; set; }

        /// <summary>
        /// 进出方向/状态
        /// </summary>
        [Comment("进出方向/状态")]
        public PassagewayDirection Direction { get; set; }

        /// <summary>
        /// 排放阶段
        /// </summary>
        [MaxLength(50)]
        [Comment("排放阶段")]
        public string PaiFangJieDuan { get; set; }

        /// <summary>
        /// 发动机号
        /// </summary>
        [Comment("发动机号")]
        [MaxLength(50)]
        public string EngineNo { get; set; }

        /// <summary>
        /// VIN
        /// </summary>
        [MaxLength(50)]
        [Comment("VIN")]
        public string VIN { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        [Comment("注册日期")]
        public DateTime? RegDate { get; set; }

        /// <summary>
        /// 车队名称
        /// </summary>
        [Comment("车队名称")]
        [MaxLength(50)]
        public string TeamName { get; set; }

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
        /// 是否自动开闸,车牌识别后会赋值
        /// </summary>
        [Comment("是否是自动开闸")]
        public bool AutoPass { get; set; }

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
        /// 关联车类型是否启用了车辆统计, 车牌识别到达后, 动态赋值
        /// </summary>
        [NotMapped]
        public bool EnablePassagewayStatistic { get; set; }
    }
}
