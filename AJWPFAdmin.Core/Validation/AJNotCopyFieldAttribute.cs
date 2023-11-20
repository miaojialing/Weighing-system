using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Validation
{
    /// <summary>
    /// 用于  Utils.CopyPropertyValues 不复制某些属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AJNotCopyFieldAttribute : Attribute
    {
    }
}
