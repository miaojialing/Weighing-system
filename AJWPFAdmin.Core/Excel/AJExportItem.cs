namespace AJWPFAdmin.Core.Excel
{
    public class AJExportItem
    {
        public string Title { get; set; }
        public object Value { get; set; }
        public int Index { get; set; }
        public bool Dynamic { get; set; }
        public bool IsImage { get; set; }
    }
}
