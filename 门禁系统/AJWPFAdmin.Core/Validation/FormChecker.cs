using AJWPFAdmin.Core.Utils;
using Masuit.Tools;
using Masuit.Tools.Reflection;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

namespace AJWPFAdmin.Core.Validation
{
    public class FormChecker
    {

        public class FormValidateMessage
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }


        public static (bool Success, string ErrorMsg) IsEnum(object val, Type enumType,
            string fieldName = "类型")
        {
            if (!Enum.IsDefined(enumType, val))
            {
                return (false, $"未定义的{fieldName}");
            }
            return (true, string.Empty);
        }

        public static FormValidateMessage Json<T>(string val,
            string fieldName = "该项")
        {
            var result = new FormValidateMessage { Success = true };
            if (!CommonUtil.TryGetJSONObject(val, out T obj))
            {
                result.Success = false;
                result.Message = $"{fieldName}格式有误";
            }
            result.Data = obj;
            return result;
        }

        public static FormValidateMessage Json(string val, Type type,
            string fieldName = "该项")
        {
            var result = new FormValidateMessage { Success = true };
            if (!CommonUtil.TryGetJSONObject(val, type, out var obj))
            {
                result.Success = false;
                result.Message = $"{fieldName}格式有误";
            }
            result.Data = obj;
            return result;
        }

        public static FormValidateMessage Rate(decimal val, string fieldName = "该项")
        {
            var result = new FormValidateMessage { Success = true };
            if (val < 0 || val > 1)
            {
                result.Success = false;
                result.Message = $"{fieldName}须在0-1之间";
            }
            return result;
        }

        public static FormValidateMessage RequireAndLength(string val, int max,
            string fieldName = "该项")
        {
            var result = new FormValidateMessage { Success = true };
            if (string.IsNullOrWhiteSpace(val) || val.Length > max)
            {
                result.Success = false;
                result.Message = $"{fieldName}未填写或超长:{max}";
            }
            return result;
        }

        public static FormValidateMessage Require(string val, string fieldName = "该项")
        {
            var result = new FormValidateMessage { Success = true };
            if (string.IsNullOrWhiteSpace(val))
            {
                result.Success = false;
                result.Message = $"{fieldName}未填写";
            }
            return result;
        }

        public static FormValidateMessage Length(string val, int max,
            string fieldName = "该项")
        {
            var result = new FormValidateMessage { Success = true };
            if (!string.IsNullOrWhiteSpace(val) && val.Length > max)
            {
                result.Success = false;
                result.Message = $"{fieldName}超长:{max}";
            }
            return result;
        }

        public static FormValidateMessage Regex(string val, Regex reg,
            string fieldName = "该项")
        {
            var result = new FormValidateMessage { Success = true };

            if (!val.RegexMatch(reg))
            {
                result.Success = false;
                result.Message = $"{fieldName}格式有误";
            }
            return result;
        }

        public static FormValidateMessage MatchPhone(string val,
            string fieldName = "手机号")
        {
            var result = new FormValidateMessage { Success = true };
            if (!CommonRegex.PHONEREGEX.IsMatch(val))
            {
                result.Success = false;
                result.Message = $"{fieldName}格式有误";
            }
            return result;
        }

        public static FormValidateMessage MatchIDCardNo(string val,
            string fieldName = "该项")
        {
            var result = new FormValidateMessage { Success = true };
            if (!CommonRegex.IDCARDREGEX.IsMatch(val))
            {
                result.Success = false;
                result.Message = $"{fieldName}格式有误";
            }
            return result;
        }

        public static FormValidateMessage MatchPassword(string val,
            string fieldName = "账户密码")
        {
            var result = new FormValidateMessage { Success = true };
            if (!Regex(val, CommonRegex.PASSWORDREGEX, fieldName).Success)
            {
                result.Message = $"{fieldName}格式有误";
                result.Success = false;
            }
            return result;
        }

        public static (bool Success, string ErrorMsg) MatchUrl(string val,
            string fieldName = "链接")
        {
            if (!Regex(val, CommonRegex.HTTPURLREGEX, fieldName).Success)
            {
                return (false, $"{fieldName}格式有误");
            }
            return (true, string.Empty);
        }

        public static (bool Success, string ErrorMsg) MatchImageUrl(string val,
            string fieldName = "图片")
        {
            if (!Regex(val, CommonRegex.HTTPURLREGEX, fieldName).Success)
            {
                return (false, $"{fieldName}链接错误");
            }

            var name = val.Split('/').LastOrDefault();

            if (!Regex(name, CommonRegex.IMAGEREGEX, fieldName).Success)
            {
                return (false, $"{fieldName}格式错误");
            }

            return (true, string.Empty);
        }

        public static FormValidateMessage MatchVideoUrl(string val,
            string fieldName = "视频")
        {
            var result = new FormValidateMessage { Success = true };
            if (!Regex(val, CommonRegex.HTTPURLREGEX, fieldName).Success)
            {
                result.Success = false;
                result.Message = $"{fieldName}链接错误";
                return result;
            }


            var name = val.Split('/').LastOrDefault();

            if (!Regex(name, CommonRegex.VIDEOREGEX, fieldName).Success)
            {
                result.Success = false;
                result.Message = $"{fieldName}格式错误";
                return result;
            }

            return result;
        }

        public static FormValidateMessage MatchAudioUrl(string val,
            string fieldName = "音频")
        {
            var result = new FormValidateMessage { Success = true };
            if (!Regex(val, CommonRegex.HTTPURLREGEX, fieldName).Success)
            {
                result.Success = false;
                result.Message = $"{fieldName}链接错误";
                return result;
            }


            var name = val.Split('/').LastOrDefault();

            if (!Regex(name, CommonRegex.AUDIOREGEX, fieldName).Success)
            {
                result.Success = false;
                result.Message = $"{fieldName}格式错误";
                return result;
            }

            return result;
        }

        public static FormValidateMessage MatchEmailUrl(string val,
            string fieldName = "邮箱")
        {
            var result = new FormValidateMessage { Success = true };
            if (!Regex(val, CommonRegex.EMAILREGEX, fieldName).Success)
            {
                result.Success = false;
                result.Message = $"{fieldName}格式错误";
                return result;
            }

            return result;
        }

        public static FormValidateMessage MatchIP(string val,
            string fieldName = "IP地址")
        {
            var result = new FormValidateMessage { Success = true };
            if (!Regex(val, CommonRegex.IP, fieldName).Success)
            {
                result.Success = false;
                result.Message = $"{fieldName}格式错误";
                return result;
            }

            return result;
        }
    }
}
