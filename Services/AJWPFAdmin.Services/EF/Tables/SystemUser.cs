using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AJWPFAdmin.Services.EF.Tables
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    public partial class SystemUser
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        /// <summary>
        /// 平台客户id
        /// </summary>
        public int PId { get; set; }

        /// <summary>
        /// 登录账户名称
        /// </summary>
        [Required(ErrorMessage = "账户名称必填")]
        [MaxLength(20, ErrorMessage = "账户名称超长:{1}")]
        [StringLength(20, ErrorMessage = "账户名称超长:{1}")]
        public string AccountName { get; set; }

        /// <summary>
        /// 用户手机号
        /// </summary>
        [Required(ErrorMessage = "手机号必填")]
        [MaxLength(20, ErrorMessage = "账户名称超长:{1}")]
        public string Phone { get; set; }

        /// <summary>
        /// 账户密码
        /// </summary>
        /// 
        [Required(ErrorMessage = "登录密码必填")]
        public string Password { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [MaxLength(50)]
        public string NickName { get; set; }

        /// <summary>
        /// 用户头像Url
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        [Required(ErrorMessage = "未指定角色")]
        public long RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        [ForeignKey("RoleId")]
        public SystemRole Role { get; set; }

        #region 方法扩展

        //public Task<int> UpdateOrAddAsync(MSSQLDb db, int pcid)
        //{
        //    PId = pcid;

        //    var roleId = RoleId;

        //    RoleName = db.SystemRoles.Where(p => p.Id == roleId).Select(p => p.Name).FirstOrDefault();

        //    if (string.IsNullOrWhiteSpace(RoleName))
        //    {
        //        Util.FriendlyError("指定的角色不存在");
        //    }

        //    var id = Id;
        //    var actName = AccountName;
        //    var phone = Phone;

        //    var dbdata = db.SystemUsers.FirstOrDefault(p => p.Id == id);

        //    if (dbdata == null)
        //    {
        //        if (db.SystemUsers.Any(p => (p.AccountName == actName
        //        || p.Phone == phone) && p.PId == pcid))
        //        {
        //            Util.FriendlyError("账号/手机号已存在,请用该账号登录");
        //        }
        //        Id = Sequence.TryNextSequence(SequenceType.System, true);
        //        Password = Password.AESEncrypt(Resources.AesKey);
        //        UpdateDate = CreateDate = DateTime.Now;

        //        db.SystemUsers.Add(this);
        //    }
        //    else
        //    {
        //        if ((dbdata.AccountName != AccountName || dbdata.Phone != phone)
        //            && db.SystemUsers.Any(p => (p.AccountName == actName
        //            || p.Phone == phone) && p.PId == pcid))
        //        {
        //            Util.FriendlyError("账号/手机号已存在");
        //        }

        //        if (!dbdata.Password.MDString2().Substring(0, 12).Equals(Password))
        //        {
        //            Password = Password.AESEncrypt(Resources.AesKey);
        //        }

        //        UpdateDate = DateTime.Now;

        //        Util.CopyPropertyValues(dbdata, this);

        //        db.Entry(dbdata).State = EntityState.Modified;
        //    }

        //    return db.SaveChangesAsync();
        //}

        #endregion

    }
}
