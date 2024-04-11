using AJWPFAdmin.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.CommonEntity
{
    /// <summary>
    /// 所有表共用日期字段实体类
    /// </summary>
    public partial class DatePropertyEntity
    {
        /// <summary>
        /// 创建日期,前端提交的话, 要么不要包含这个字段, 要么传递日期格式字符串
        /// </summary>
        [AJNotCopyField]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 并发控制字段, 无需提交
        /// </summary>
        [Timestamp]
        public byte[] RowVersionTimestamp { get; set; }
    }
}
