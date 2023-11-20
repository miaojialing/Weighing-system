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
    /// 系统配置字典记录表
    /// </summary>
    [Comment("系统配置字典记录表")]
    public partial class SystemConfigDictionary : DatePropertyEntity
    {
        /// <summary>
        /// 配置Key
        /// </summary>
        [Comment("配置Key")]
        [Display(Name = "配置Key")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public SystemConfigKey Key { get; set; }

        /// <summary>
        /// 整数配置值
        /// </summary>
        [Comment("整数配置值")]
        public int IntValue { get; set; }

        /// <summary>
        /// 数值配置值
        /// </summary>
        [Comment("数值配置值")]
        [Precision(18, 4)]
        public decimal DecimalValue { get; set; }

        /// <summary>
        /// 字符串配置值
        /// </summary>
        [Comment("字符串配置值")]
        public string StringValue { get; set; }


    }
}
