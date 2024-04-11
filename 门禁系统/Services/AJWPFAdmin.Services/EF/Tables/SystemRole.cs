using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AJWPFAdmin.Services.EF.Tables
{
    /// <summary>
    /// 系统角色表
    /// </summary>
    public partial class SystemRole
    {
        /// <summary>
        /// 角色id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        /// <summary>
        /// 平台客户id
        /// </summary>
        public int PId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "角色名称必填")]
        [MaxLength(20, ErrorMessage = "角色名称超长:{1}")]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "角色描述超长:{1}")]
        public string Description { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 权限json字符串数据
        /// </summary>
        public string Permission { get; set; }

        public ICollection<SystemUser> SystemUsers { get; set; }

        #region 方法扩展

        //public Task<int> UpdateOrAddAsync(MSSQLDb db, int pcid)
        //{
        //    PId = pcid;

        //    var id = Id;
        //    var name = Name;

        //    var dbdata = db.SystemRoles.FirstOrDefault(p => p.Id == id);

        //    if (dbdata == null)
        //    {
        //        Id = SnowFlake.GetInstance().GetLongId();
        //        UpdateDate = CreateDate = DateTime.Now;

        //        db.SystemRoles.Add(this);
        //    }
        //    else
        //    {
        //        Util.CopyPropertyValues(dbdata, this);

        //        db.Entry(dbdata).State = EntityState.Modified;
        //    }

        //    return db.SaveChangesAsync();
        //}

        #endregion

    }
}
