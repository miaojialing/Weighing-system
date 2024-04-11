using AJWPFAdmin.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJWPFAdmin.Services.EF.Tables;

namespace AJWPFAdmin.Services.Models
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

        public List<WarehousePassageRecord> Records { get; set; }
    }
}
