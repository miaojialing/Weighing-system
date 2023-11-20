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
    /// 仓库表
    /// </summary>
    [Comment("仓库表")]
    public partial class Warehouse : CommonTableEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Comment("名称")]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 车次
        /// </summary>
        [Comment("车次")]
        public int CarLimit { get; set; }

        /// <summary>
        /// 剩余车次
        /// </summary>
        [Comment("剩余车次")]
        public int ResidualCarLimit { get; set; }
    }
}
