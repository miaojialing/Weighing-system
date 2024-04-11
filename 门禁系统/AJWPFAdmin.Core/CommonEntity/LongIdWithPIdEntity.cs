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
    /// 所有表共用字段模型类,自带长整型id和pid(租户id)
    /// </summary>
    public partial class LongIdWithPIdEntity
    {
        /// <summary>
        /// 表单id,新增0,编辑必填
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
    }
}
