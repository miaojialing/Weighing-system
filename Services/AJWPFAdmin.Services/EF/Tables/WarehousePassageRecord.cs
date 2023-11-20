using AJWPFAdmin.Core.CommonEntity;
using AJWPFAdmin.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Comment("车牌号")]
        [Display(Name = "车牌号")]
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
        /// 关联通行记录id
        /// </summary>
        [Comment("关联通行记录id")]
        public long SippingRecordId { get; set; }
    }
}
