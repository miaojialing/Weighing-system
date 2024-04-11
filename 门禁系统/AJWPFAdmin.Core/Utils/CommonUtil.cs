using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJWPFAdmin.Core.Validation;
using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Hosting.Server;
using AJWPFAdmin.Core.Logger;
using System.Net;
using System.Collections.ObjectModel;
using AngleSharp.Text;
using AJWPFAdmin.Core.ExtensionMethods;
using System.Diagnostics;
using System.Net.Http;

namespace AJWPFAdmin.Core.Utils
{
    public static class CommonUtil
    {

        public static readonly JsonSerializerSettings JSONSERIALIZERSETTINGS = new()
        {
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),//new DefaultContractResolver()
            NullValueHandling = NullValueHandling.Include,
            //Converters = new List<JsonConverter> { new Int64ToStringConverter() },
        };

        public static readonly ObservableCollection<string> CARNOPREFIX = new ObservableCollection<string>(new string[]
        {
            "京",
            "津",
            "沪",
            "渝",
            "冀",
            "豫",
            "云",
            "辽",
            "黑",
            "湘",
            "皖",
            "鲁",
            "新",
            "苏",
            "浙",
            "赣",
            "鄂",
            "桂",
            "甘",
            "晋",
            "蒙",
            "陕",
            "吉",
            "闽",
            "贵",
            "粤",
            "青",
            "藏",
            "川",
            "宁",
            "琼",
            "使",
            "领"
        });

        public static Process CreateCommand(string exeFile, string arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = exeFile;
            process.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            process.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            process.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            process.StartInfo.CreateNoWindow = true;//不显示程序窗口
            process.StartInfo.Arguments = arguments;

            return process;
        }

        public static (string host, int port, string user, string database, string password) DecryptMySqlConnStr(string connStr)
        {
            var array = connStr.Split(';');

            var fields = new string[] { "server", "port", "user", "database", "password" };

            var dic = new Dictionary<string, string>();
            foreach (var item in array)
            {
                var parts = item.Split('=');
                var key = (parts.ElementAt(0) ?? string.Empty).ToLower();
                if (fields.Contains(key))
                {
                    dic.Add(key, parts.ElementAt(1));
                }
            }

            if (!dic.ContainsKey("port"))
            {
                dic.Add("port", "3306");
            }

            var port = dic[fields[1]].TryGetInt();

            return (dic[fields[0]], port, dic[fields[2]], dic[fields[3]], dic[fields[4]]);
        }

        public static string GetLocalIPV4()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        }

        /// <summary>
        /// 添加环境变量
        /// </summary>
        /// <param name="paths">路径列表</param>
        public static void AddEnvironmentPaths(IEnumerable<string> paths)
        {
            var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };
            string newPath = string.Join(Path.PathSeparator.ToString(), path.Concat(paths));
            Environment.SetEnvironmentVariable("PATH", newPath);   // 这种方式只会修改当前进程的环境变量
        }

        private static readonly string[] _sizeUnits = new string[] { "B", "K", "M", "G", "TB" };

        public static string FormatFileSize(long size)
        {
            var index = 0;
            double sizeVal = size;
            if (size > 1024)
            {
                for (index = 0; (size / 1024L) > 0;index++,size /= 1024L)
                {
                    sizeVal = size / 1024.0;
                }
            }
            return string.Format("{0:0.##}{1}", sizeVal, _sizeUnits[index]);
        }

        public static void TryDeleteFiles(IEnumerable<string> files)
        {
            try
            {
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
            }
        }

        public static void TryCopyFile(string source, string target)
        {
            try
            {
                if (!File.Exists(source))
                {
                    return;
                }

                var dic = Path.GetDirectoryName(target);

                if (!Directory.Exists(dic))
                {
                    Directory.CreateDirectory(dic);
                }

                File.Copy(source, target, true);
            }
            catch
            {
            }
        }


        /// <summary>
        /// 解压ZIP 
        /// </summary>
        /// <param name="zipFileName">zip文件 C:/data/a.zip</param>
        /// <param name="targetDirectory">解压到的目标路径 C:/data/temp</param>
        /// <returns></returns>
        public static bool ZIPUnpack(string zipFileName, string targetDirectory,
            Action<string, Exception> logHandler = null)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                (new FastZip()).ExtractZip(zipFileName, targetDirectory, "");
                return true;
            }
            catch (Exception e)
            {
                logHandler?.Invoke(e.Message, e);
                return false;
            }

        }

        /// <summary>
        /// 压缩指定目录的所有文件,不包含嵌套的文件夹
        /// </summary>
        /// <param name="sourceDirectory">源文件目录, 如 C:/uplodas/images</param>
        /// <param name="zipFilePathName">zip文件路径 如 C:/uploads/files/pack.zip</param>
        /// <param name="level">压缩级别, 0 - 9, 越高压缩文件越小,但是速度越慢</param>
        /// <param name="fileNamePattern">要压缩的文件类型 如 *.jpg</param>
        /// <param name="deleteOriginalFile">压缩完毕是否删除源文件</param>
        public static bool ZIPPack(string sourceDirectory,
            string zipFilePathName,
            int level = 0,
            string fileNamePattern = "*.xlsx",
            bool deleteOriginalFile = true,
            Action<string, Exception> logHandler = null)
        {
            if (level < 0)
            {
                level = 0;
            }
            if (level > 9)
            {
                level = 9;
            }
            try
            {


                var files = Directory
                        .GetFiles(sourceDirectory, fileNamePattern)
                        .Select(p => new FileInfo(p)).ToList();

                if (files == null || files.Count == 0)
                {
                    return false;
                }

                using (var zipStream = new ZipOutputStream(File.Open(zipFilePathName, FileMode.OpenOrCreate)))
                {
                    zipStream.SetLevel(level);

                    foreach (var file in files)
                    {
                        using (var fs = File.OpenRead(file.FullName))
                        {
                            var buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);

                            zipStream.PutNextEntry(new ZipEntry(file.Name));
                            zipStream.Write(buffer, 0, buffer.Length);
                        }
                    }
                }

                if (deleteOriginalFile)
                {
                    foreach (var item in files)
                    {
                        item.Delete();
                    }
                }
            }
            catch (Exception e)
            {
                logHandler?.Invoke(e.Message, e);
                return false;
            }
            return true;
        }

        public static string AJSerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, JSONSERIALIZERSETTINGS);
        }

        public static BitmapImage GetImageFromLocalOrHttp(string file)
        {
            var result = new BitmapImage();
            if (!CommonRegex.IMAGEREGEX.IsMatch(Path.GetExtension(file)))
            {
                return GetImageFromLocalOrHttp(Path.Combine(AppContext.BaseDirectory, "Images\\media-empty.png"));
            }
            try
            {
                Stream fs = null;
                if (CommonRegex.HTTPURLREGEX.IsMatch(file))
                {
                    using var client = new HttpClient();
                    var task = client.GetStreamAsync(file);

                    task.Wait();

                    fs = task.Result;
                }
                else
                {
                    fs = new FileStream(file, FileMode.Open);
                }

                using var reader = new BinaryReader(fs);

                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = new MemoryStream(reader.ReadBytes(Convert.ToInt32(fs.Length)));
                result.EndInit();

                fs.Dispose();

            }
            catch
            {
                result = GetImageFromLocalOrHttp(Path.Combine(AppContext.BaseDirectory, "Images\\media-empty.png"));
            }

            return result;
        }

        public static bool CopyFile(string source, string target)
        {

            if (!File.Exists(source))
            {
                return false;
            }

            try
            {
                var dic = Path.GetDirectoryName(target);

                if (!Directory.Exists(dic))
                {
                    Directory.CreateDirectory(dic);
                }
                File.Copy(source, target, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Dictionary<string, T> EnumToDictionary<T>(Func<T, string> keyFormatHandler) where T : Enum
        {
            var result = new Dictionary<string, T>();
            var array = Enum.GetValues(typeof(T));
            foreach (T item in array)
            {
                result.Add(keyFormatHandler?.Invoke(item) ?? item.ToString(), item);
            }
            return result;
        }

        /// <summary>
        /// 将 source 的属性值复制到 org 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="org"></param>
        /// <param name="source"></param>
        public static void CopyPropertyValues<T>(T org, T source)
        {
            var props = org.GetType().GetRuntimeProperties();
            var sourceProps = source.GetType().GetRuntimeProperties();

            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<AJNotCopyFieldAttribute>();
                if (attr != null)
                {
                    continue;
                }
                var sourceProp = sourceProps.FirstOrDefault(p => p.Name == prop.Name);

                prop.SetValue(org, sourceProp.GetValue(source));
            }
        }

        /// <summary>
        /// 尝试转换json字符串为 T,如果转换失败,则直接返回 默认 T --阿吉 2021年6月18日17点02分
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T TryGetJSONObject<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, JSONSERIALIZERSETTINGS);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 尝试转换json字符串为 T
        /// </summary>
        /// <param name="json"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool TryGetJSONObject<T>(string json, out T obj)
        {
            var result = true;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(json, JSONSERIALIZERSETTINGS);
            }
            catch
            {
                obj = default;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 尝试转换json字符串为 object
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool TryGetJSONObject(string json, Type type, out object data)
        {
            var result = true;
            try
            {
                data = JsonConvert.DeserializeObject(json, type, JSONSERIALIZERSETTINGS);
            }
            catch
            {
                data = null;
                result = false;
            }
            return result;
        }


        public static Task ShowAlertDialogAsync(AlertDialogArguments alertDialogArguments,
            string dialogHostId = "root")
        {
            if (!DialogHost.IsDialogOpen(dialogHostId))
            {
                return AlertDialog.ShowDialogAsync(dialogHostId, alertDialogArguments);
            }

            return Task.CompletedTask;
        }

        public static Task<bool> ShowConfirmDialogAsync(ConfirmationDialogArguments confirmDialogArguments,
            string dialogHostId = "root")
        {
            if (!DialogHost.IsDialogOpen(dialogHostId))
            {
                return ConfirmationDialog.ShowDialogAsync(dialogHostId, confirmDialogArguments);
            }

            return Task.FromResult(false);
        }
    }
}
