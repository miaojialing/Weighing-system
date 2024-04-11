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
    /// 岗亭表
    /// </summary>
    [Comment("岗亭表")]
    public partial class Watchhouse : CommonTableEntity
    {
        public Watchhouse()
        {
            Passageways = new List<Passageway>();
        }

        /// <summary>
        /// 导航属性,关联通道
        /// </summary>
        public virtual IList<Passageway> Passageways { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Comment("名称")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [Comment("IP地址")]
        [MaxLength(50)]
        public string IP { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [MaxLength(100)]
        public string Remark { get; set; }
    }
}
