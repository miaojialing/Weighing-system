using AJWPFAdmin.Core.CommonEntity;
using AJWPFAdmin.Core.Enums;
using Microsoft.EntityFrameworkCore;
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
    /// 通道表
    /// </summary>
    [Comment("通道表")]
    public partial class Passageway : CommonTableEntity
    {
        public Passageway()
        {
            Devices = new List<Device>();
        }

        /// <summary>
        /// 导航属性,关联设备列表
        /// </summary>
        public virtual IList<Device> Devices { get; set; }

        /// <summary>
        /// 关联岗亭Id
        /// </summary>
        public long WatchhouseId { get; set; }

        /// <summary>
        /// 导航属性,关联岗亭数据
        /// </summary>
        [ForeignKey("WatchhouseId")]
        public virtual Watchhouse Watchhouse { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Comment("名称")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [MaxLength(100)]
        public string Remark { get; set; }

        /// <summary>
        /// 进出方向
        /// </summary>
        public PassagewayDirection Direction { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [Comment("编号")]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 设备网关
        /// </summary>
        [Comment("设备网关")]
        public string DeviceGateway { get; set; }

        /// <summary>
        /// 关联仓库id
        /// </summary>
        [Comment("关联仓库id")]
        public long?  WarehouseId { get; set; }

        /// <summary>
        /// 关联仓库名称
        /// </summary>
        [Comment("关联仓库名称")]
        [MaxLength(50)]
        public string WarehouseName { get; set; }

        /// <summary>
        /// 是否启用统计车辆,启用后, 关联仓库必填
        /// </summary>
        [Comment("是否统计车辆")]
        public bool CountCarEnable { get; set; }

        /// <summary>
        /// 关联岗亭名称,动态赋值
        /// </summary>
        [NotMapped]
        public string WatchhouseName { get; set; }
    }
}
