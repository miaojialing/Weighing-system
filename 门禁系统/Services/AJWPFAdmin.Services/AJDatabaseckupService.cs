using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Core.Utils;
using AJWPFAdmin.Services.EF;
using AJWPFAdmin.Services.EF.Tables;
using Masuit.Tools;
using Masuit.Tools.Files;
using Masuit.Tools.Systems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Quartz.Impl.AdoJobStore;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace AJWPFAdmin.Services
{
    public class AJDatabaseckupService
    {
        public class BackupOrRestoreProgress
        {
            public string Message { get; set; }
            public int Current { get; set; }
            public int Total { get; set; }
            public bool Success { get; set; }
        }

        private DbService db;
        private AJLog4NetLogger _logger;
        private AJConfigService _cfgSvc;

        public AJDatabaseckupService(DbService dbIns, AJLog4NetLogger logger, AJConfigService cfgSvc)
        {
            db = dbIns;
            _logger = logger;
        }

        public void Backup(Action<BackupOrRestoreProgress> progressReport, string backupDir)
        {
            var progress = new BackupOrRestoreProgress();

            if (string.IsNullOrWhiteSpace(backupDir))
            {
                progress.Success = false;
                progress.Message = $"备份路径未设置";
                progressReport?.Invoke(progress);
                return;
            }

            var mysqlBaseDir = db.Database
                .SqlQueryRaw<string>("select @@basedir as basePath from dual;").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mysqlBaseDir)
                || !Directory.Exists(mysqlBaseDir))
            {
                progress.Success = false;
                progress.Message = $"本机未安装MYSQL或MYSQL配置有误";
                progressReport?.Invoke(progress);
                return;
            }

            progress.Success = true;
            progress.Message = $"已获取到MYSQL信息";
            progress.Current = 10;
            progress.Total = 100;
            progressReport?.Invoke(progress);

            Thread.Sleep(1000);

            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);

                progress.Success = true;
                progress.Message = $"正在创建备份目录";
                progress.Current = 15;
                progress.Total = 100;
                progressReport?.Invoke(progress);

                Thread.Sleep (1000);
            }

            var now = DateTime.Now;

            var rad = $"{now.ToString("yyyyMMdd_HHmmss")}_{SnowFlake.GetInstance().GetUniqueId()}";

            var sqlFileName = $"{rad}.sql";

            var connDic = CommonUtil.DecryptMySqlConnStr(_cfgSvc.Config.GetConnectionString("MYSQL"));

            var record = new DatabaseBackupRecord
            {
                Id = SnowFlake.GetInstance().GetLongId(),
                FileName = sqlFileName,
                FilePath = Path.Combine(backupDir, sqlFileName),
                CreateDate = now
            };

            var exe = $"{mysqlBaseDir}bin\\mysqldump.exe";
            var args = $"--user={connDic.user} --host={connDic.host} --password={connDic.password} --hex-blob --default-character-set=utf8 --result-file={record.FilePath} -E -R --databases --master-data=2 --quick {connDic.database}";
            var cmd = CommonUtil.CreateCommand(exe, args);

            progress.Success = true;
            progress.Message = $"生成备份命令";
            progress.Current = 80;
            progress.Total = 100;
            progressReport?.Invoke(progress);

            Thread.Sleep(1000);

            File.WriteAllText(record.FilePath, string.Empty, Encoding.UTF8);

            progress.Success = true;
            progress.Message = $"创建备份文件";
            progress.Current = 40;
            progress.Total = 100;
            progressReport?.Invoke(progress);

            Thread.Sleep(1000);

            cmd.Start();

            cmd.WaitForExit();

            db.DatabaseBackupRecords.Add(record);

            db.SaveChanges();

            progress.Success = true;
            progress.Message = $"备份成功";
            progress.Current = 100;
            progress.Total = 100;
            progressReport?.Invoke(progress);

        }

        public void Restore(Action<BackupOrRestoreProgress> progressReport, string backupFile)
        {
            var progress = new BackupOrRestoreProgress();

            if (!File.Exists(backupFile))
            {
                progress.Success = false;
                progress.Message = "备份文件不存在";
                progressReport.Invoke(progress);
                return;
            }


            var mysqlBaseDir = db.Database
                .SqlQueryRaw<string>("select @@basedir as basePath from dual;").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mysqlBaseDir)
                || !Directory.Exists(mysqlBaseDir))
            {
                progress.Success = false;
                progress.Message = $"本机未安装MYSQL或MYSQL配置有误";
                progressReport?.Invoke(progress);
                return;
            }

            progress.Success = true;
            progress.Message = "检测到MYSQL信息";
            progress.Current = 40;
            progress.Total = 100;
            progressReport.Invoke(progress);

            Thread.Sleep(1000);

            var exe = "cmd.exe";// $"{SQLDataAccess.QueryMySqlBaseDir()}bin\\mysql.exe";
            var connDic = CommonUtil.DecryptMySqlConnStr(_cfgSvc.Config.GetConnectionString("MYSQL"));
            /*Running: mysql.exe  --protocol=tcp --host=localhost --user=root --port=3306 --default-character-set=utf8 --comments --database=main  < "C:\\Users\\admin\\Desktop\\20230703092048.sql"*/
            var args = $"--user={connDic.user} --host={connDic.host} --password={connDic.password} --default-character-set=utf8 --comments --database={connDic.database} < \"{backupFile}\"";
            var cmd = CommonUtil.CreateCommand(exe, string.Empty);
            cmd.StartInfo.WorkingDirectory = $"{mysqlBaseDir}bin";

            progress.Success = true;
            progress.Message = "生成还原命令";
            progress.Current = 60;
            progress.Total = 100;
            progressReport.Invoke(progress);

            Thread.Sleep(1000);

            cmd.Start();

            cmd.StandardInput.WriteLine($"mysql {args}");
            cmd.StandardInput.WriteLine($"exit");

            cmd.WaitForExit();

            progress.Success = true;
            progress.Message = "还原成功!";
            progress.Current = 100;
            progress.Total = 100;
            progressReport.Invoke(progress);
        }
    }
}
