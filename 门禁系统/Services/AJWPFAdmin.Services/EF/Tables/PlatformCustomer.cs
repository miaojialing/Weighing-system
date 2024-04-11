using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AJWPFAdmin.Services.EF.Tables
{
    /// <summary>
    /// 平台客户表
    /// </summary>
    public partial class PlatformCustomer
    {
        /// <summary>
        /// 平台客户id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        /// 
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 负责人名称
        /// </summary>
        [MaxLength(50)]
        [StringLength(50)]
        public string Director { get; set; }
        /// <summary>
        /// 负责人手机号
        /// </summary>
        [Required]
        [MaxLength(20)]
        [StringLength(20)]
        public string DirectorPhone { get; set; }

        /// <summary>
        /// 客户热线(联系方式)
        /// </summary>
        [MaxLength(50)]
        [StringLength(50)]
        public string Contact { get; set; }

        /// <summary>
        /// 客户详细地址
        /// </summary>
        [MaxLength(500)]
        [StringLength(500)]
        public string Address { get; set; }

        /// <summary>
        /// 客户logo Url
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [Required]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 到期日期
        /// </summary>
        [Required]
        public DateTime ExpireDate { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [Required]
        public bool Enable { get; set; }
    }
}
