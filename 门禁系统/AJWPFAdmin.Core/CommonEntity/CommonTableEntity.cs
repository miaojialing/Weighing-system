using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJWPFAdmin.Core.Validation;

namespace AJWPFAdmin.Core.CommonEntity
{
    /// <summary>
    /// 所有表共有字段模型类
    /// </summary>
    public class CommonTableEntity
    {
        /// <summary>
        /// 主键id,新增0,编辑必填
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [AJNotCopyField]
        public long Id { get; set; }

        /// <summary>
        /// 平台客户id,后端自动赋值
        /// </summary>
        [AJNotCopyField]
        public int PId { get; set; }

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
