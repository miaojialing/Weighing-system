using AJWPFAdmin.Core;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using Masuit.Tools.Security;
using MaterialDesignExtensions.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services
{
    public class SystemUserService
    {
        private DbService db;
        private AJConfigService _ajConfigService;
        private bool _databaseCreated = false;

        public SystemUser CurrnetUser { get; private set; }

        public SystemUserService(AJConfigService agCfgSvc, DbService dbIns)
        {
            db = dbIns;
            _ajConfigService = agCfgSvc;
        }

        public void RefreshDb()
        {
            db.Database.SetConnectionString(_ajConfigService.Config.GetConnectionString("MYSQL"));
        }

        public void Login(string account, string password)
        {
            if (!_databaseCreated)
            {
                db.Database.Migrate();
                _databaseCreated = true;
            }

            password = password.AESEncrypt(Core.Properties.Resources.AESKey);

            CurrnetUser = db.SystemUsers
                .Where(x => (x.AccountName == account || x.Phone == account)
                && x.Password == password).FirstOrDefault();

            if (!CurrnetUser.Enable)
            {
                ExceptionTool.FriendlyError("账号已被禁用", "登录失败", true);
            }

            if (CurrnetUser == null)
            {
                ExceptionTool.FriendlyError("账号或密码错误", "登录失败", true);
            }
        }
    }
}
