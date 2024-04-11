using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.ExtensionMethods
{
    public static class StringExt
    {
        public static DateTime TryGetDate(this string str)
        {
            if (DateTime.TryParse(str, out var val))
            {
                return val;
            }
            return DateTime.MinValue;
        }

        public static long TryGetLong(this string str)
        {
            if (long.TryParse(str, out var val))
            {
                return val;
            }
            return 0;
        }

        public static int TryGetInt(this string str)
        {
            if (int.TryParse(str, out var val))
            {
                return val;
            }
            return 0;
        }

        public static double TryGetDouble(this string str)
        {
            if (double.TryParse(str, out var val))
            {
                return val;
            }
            return 0;
        }

        public static float TryGetFloat(this string str)
        {
            if (float.TryParse(str, out var val))
            {
                return val;
            }
            return 0;
        }

        public static decimal TryGetDecimal(this string str)
        {
            if (decimal.TryParse(str, out var val))
            {
                return val;
            }
            return 0;
        }

        /// <summary>
        /// 过滤html标签
        /// </summary>
        /// <param name="htmlContent">html字符串内容</param>
        /// <returns>返回没有html标签的字符串内容</returns>
        public static string NoHtml(this string htmlContent)
        {
            var noHtmlString = "";
            noHtmlString = RegexReplace(htmlContent, @"<script[^>]*?>.*?</script>", "");
            noHtmlString = RegexReplace(noHtmlString, @"<(.[^>]*)>", "");
            noHtmlString = RegexReplace(noHtmlString, @"([/r/n])[/s]+", "");
            noHtmlString = RegexReplace(noHtmlString, @"&(nbsp|#160);", "");
            noHtmlString = RegexReplace(noHtmlString, @"-->", "");
            noHtmlString = RegexReplace(noHtmlString, @"<!--.*", "");
            noHtmlString = RegexReplace(noHtmlString, @" +", " ");
            return noHtmlString;
        }

        /// <summary>
        /// 获取html文档中 img 标签中的 src 地址
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static string[] GetHtmlImageUrlArray(this string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                return new string[0];
            }
            // 定义正则表达式用来匹配 img 标签   
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串   
            MatchCollection matches = regImg.Matches(htmlContent);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表   
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;
        }

        private static string RegexReplace(string sourceText, string regExpression, string replaceValue)
        {
            return Regex.Replace(sourceText, regExpression, replaceValue, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 截断字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="displayMax">最大显示文本长度,超过该长度则会截断</param>
        /// <returns></returns>
        public static string Overflow(this string source, int displayMax = 10)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return source;
            }
            return source.Length > displayMax ? $"{source[..displayMax]}..." : source;
        }
    }
}
