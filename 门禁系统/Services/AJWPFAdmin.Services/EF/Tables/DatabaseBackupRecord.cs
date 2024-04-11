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
    public partial class DatabaseBackupRecord : CommonTableEntity
    {
        /// <summary>
        /// 备份文件名
        /// </summary>
        [Comment("备份文件名")]
        [MaxLength(200)]
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// 备份文件所在路径
        /// </summary>
        [Comment("备份文件所在路径")]
        [Required]
        public string FilePath { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Comment("文件大小")]
        public long FileSize { get; set; }
    }
}
