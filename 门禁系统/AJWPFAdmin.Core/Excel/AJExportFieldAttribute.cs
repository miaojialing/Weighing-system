using System;

namespace AJWPFAdmin.Core.Excel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AJExportFieldAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public int ColumnIndex { get; set; }
        public bool Dynamic { get; set; }

        public bool IsImage { get; set; }

        public AJExportFieldAttribute()
        {

        }
    }
}
