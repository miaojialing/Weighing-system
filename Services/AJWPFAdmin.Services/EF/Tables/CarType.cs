using AJWPFAdmin.Core.CommonEntity;
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
    /// 车辆类型记录表
    /// </summary>
    [Comment("车辆类型记录表")]
    public partial class CarType : CommonTableEntity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [Comment("类型名称")]
        [Required]
        [MaxLength(10)]
        public string Name { get; set; }

        /// <summary>
        /// 是否系统必须,true 表示不能修改和删除
        /// </summary>
        [Comment("是否系统必须")]
        public bool SysRequired { get; set; }

        /// <summary>
        /// 是否自动开闸
        /// </summary>
        [Comment("是否自动开闸")]
        public bool AutoPass { get; set; }

        /// <summary>
        /// 是否参与车辆统计
        /// </summary>
        [Comment("是否参与车辆统计")]
        public bool EnablePassagewayStatistic { get; set; }
    }
}
