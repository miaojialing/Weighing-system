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
    public partial class MaterialDictionary : CommonTableEntity
    {

        /// <summary>
        /// 名称
        /// </summary>
        [Comment("名称")]
        [Display(Name = "名称")]
        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string Name { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Comment("序号")]
        [Display(Name = "序号")]
        public int SortNo { get; set; }

        /// <summary>
        /// 物料字典类型
        /// </summary>
        [Comment("物料字典类型")]
        [Display(Name = "物料字典类型")]
        public MaterialType Type { get; set; }
    }
}
