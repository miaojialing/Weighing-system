using AJWPFAdmin.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Models
{
    /// <summary>
    /// 第三方车辆信息接口配置
    /// </summary>
    public class ThirdPartyCarInfoAPIConfig
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
    }
}
