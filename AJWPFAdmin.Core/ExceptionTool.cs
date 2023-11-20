using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Core.Utils;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core
{
    public class ExceptionTool
    {
        public class ExceptionCode
        {
            public string Code { get; set; }
            public Type ExType { get; set; }
            public string Desc { get; set; }

            public override string ToString()
            {
                return $"{Code}:{Desc}";
            }
        }

        public class AJGlobalExceptionResolvedEvent : PubSubEvent
        {

        }

        public class FriendlyException : Exception
        {
            public string Title { get; set; }
            public bool Silent { get; set; }
            public string DialogId { get; set; }
            public FriendlyException(string message, string title = "提示", 
                bool silent = false, 
                string dialogId = DialogIds.Root) : base(message)
            {
                Title = title;
                Silent = silent;
                DialogId = dialogId;

            }
        }

        private AJLog4NetLogger _logger;
        private IEventAggregator _eventAggregator;

        public ExceptionTool(AJLog4NetLogger logger, IEventAggregator eventAggregator)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;

        }

        /// <summary>
        /// 抛出 友好异常
        /// </summary>
        /// <param name="message">弹窗提示信息</param>
        /// <param name="title">弹窗提示标题</param>
        /// <param name="silent">如果在后台线程,建议传true</param>
        /// <exception cref="FriendlyException"></exception>
        public static void FriendlyError(string message, string title = "提示", 
            bool silent = false, 
            string dialogId = DialogIds.Root)
        {
            throw new FriendlyException(message, title, silent, dialogId);
        }

        private static readonly List<ExceptionCode> _errCodes = new()
        {
            new ExceptionCode {ExType=typeof(NullReferenceException), Code ="1000", Desc="空引用" },
            new ExceptionCode {ExType=typeof(IndexOutOfRangeException), Code = "2000", Desc="索引超出可用范围" },
            new ExceptionCode {ExType=typeof(NotImplementedException), Code = "3000", Desc="尚未实现处理逻辑" },
            new ExceptionCode {ExType=typeof(FileNotFoundException), Code = "4000", Desc="未找到指定资源文件" },
            new ExceptionCode {ExType=typeof(ArgumentException), Code = "5000", Desc="参数错误" },
            new ExceptionCode {ExType=typeof(DbUpdateException), Code = "6000", Desc="数据库操作失败" },
            new ExceptionCode {ExType=typeof(DbUpdateConcurrencyException ), Code = "6001", Desc="数据已被修改:请刷新获取最新数据" },
            new ExceptionCode {ExType=typeof(InvalidOperationException), Code = "7000", Desc="无效操作" },
            new ExceptionCode {ExType=typeof(SqlException), Code = "8000", Desc="数据库异常" },
            new ExceptionCode {ExType=typeof(OutOfMemoryException), Code = "9000", Desc="内存溢出" },
            new ExceptionCode {ExType=typeof(RetryLimitExceededException), Code = "8001", Desc="数据库连接失败:请检查服务器地址" },
            new ExceptionCode {ExType=typeof(MySqlException), Code = "8002", Desc="数据库连接失败:账户密码错误或其他问题" },
        };

        private static readonly ExceptionCode _unknownExpCode = new()
        {
            Code = "-1",
            Desc = "未知错误"
        };

        public async Task LogExceptionAsync(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            if (exception is FriendlyException fex)
            {
                _eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Publish();
                if (!fex.Silent && !DialogHost.IsDialogOpen(fex.DialogId))
                {
                    await CommonUtil.ShowAlertDialogAsync(new AlertDialogArguments
                    {
                        Title = fex.Title,
                        Message = fex.Message
                    }, fex.DialogId);
                }
                return;
            }

            var msgText = exception.Message;
            var exType = exception.GetType();
            var errCode = _errCodes.FirstOrDefault(p => exType == p.ExType) ?? _unknownExpCode;

            if (exception is DbUpdateException dbEx)
            {
                msgText += $"DbErrors:{string.Join(";\r\n", dbEx.Entries.Select(e => e.ToString()))}";
            }

            if (!DialogHost.IsDialogOpen(DialogIds.Root))
            {
                await AlertDialog.ShowDialogAsync(DialogIds.Root, new AlertDialogArguments
                {
                    Title = "糟糕!发生了一些错误",
                    Message = errCode.ToString()
                });
            }

            _eventAggregator.GetEvent<AJGlobalExceptionResolvedEvent>().Publish();

            _logger.Error(msgText, exception);
        }
    }
}
