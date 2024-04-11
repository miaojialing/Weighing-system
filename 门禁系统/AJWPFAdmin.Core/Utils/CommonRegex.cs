using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Utils
{
    public class CommonRegex
    {
        public static readonly Regex PHONEREGEX = new Regex(@"^1[1-9][0-9]\d{8}$");
        public static readonly Regex PASSWORDREGEX = new Regex(@"^[\S]{6,12}$");
        public static readonly Regex CODEREGEX = new Regex(@"^[A-Za-z0-9_-]+$");
        public static readonly Regex IDCARDREGEX = new Regex(@"^\d{6}\d{4}(0[1-9]|1[012])(0[1-9]|[12]\d|3[01])\d{3}(\d|[xX])$");
        public static readonly Regex DECIMALREGEX = new Regex(@"^\d+(\.\d+)?$");
        public static readonly Regex IMAGEREGEX = new Regex(@"\.(jpg|jpeg|png|ico|bmp|gif|tif|tga)", RegexOptions.IgnoreCase);
        public static readonly Regex EMAILREGEX = new Regex(@"^([a-zA-Z]|[0-9])(\w|\-)+@[a-zA-Z0-9]+\.([a-zA-Z]{2,4})$");
        public static readonly Regex HTTPURLREGEX = new Regex(@"^(http|https):\/\/.+");
        public static readonly Regex AUDIOREGEX = new Regex(@"\.(mp3|wma|ape|flac|aac|amr|m4a|m4r|ogg|wav)", RegexOptions.IgnoreCase);

        public static readonly Regex VIDEOREGEX = new Regex(@"\.(flv|3gp|rmvb|rm|swf|mp4|mkv|avi|mov|wmv|mpg)", RegexOptions.IgnoreCase);

        public static readonly Regex WINDOWILLEGALFILENAME = new Regex(@"/[\\\\/:*?\""<>|]/");

        public static readonly Regex SPECIALCHAR = new Regex(@"[`~!@#$%^&*()\-+=<>?:""{}|,.\/;'\\[\]·~！@#￥%……&*（）——\-+={}|《》？：“”【】、；‘'，。、]");

        public static readonly Regex NUMBERSTRING = new Regex(@"^(-|\+)?\d+(.\d+)?$");

        public static readonly Regex INTEGERSTRING = new Regex(@"^-?\d+$");

        public static readonly Regex IP = new Regex(@"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        public static readonly Regex CARNO = new Regex(@"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[A-HJ-NP-Z]{1}[A-HJ-NP-Z0-9]{5}[A-HJ-NP-Z0-9挂学警港澳]?");

    }
}
