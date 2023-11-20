using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Models
{
    /// <summary>
    /// 车次统计记录模型
    /// </summary>
    public class WarehousePassageRecordsGroupRecord
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string IDCardNo { get; set; }
        /// <summary>
        /// 仓库id
        /// </summary>
        public long WarehouseId { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 车次
        /// </summary>
        public int Count { get; set; }
    }
}
