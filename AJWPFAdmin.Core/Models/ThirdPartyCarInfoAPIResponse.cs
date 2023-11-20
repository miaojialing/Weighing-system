using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Models
{
    public class ThirdPartyCarInfoAPIResponseData
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 张安帮
        /// </summary>
        public string companyname { get; set; }
        /// <summary>
        /// 豫HR8697
        /// </summary>
        public string plateNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string vin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string engineNumber { get; set; }
        /// <summary>
        /// 黄色
        /// </summary>
        public string carColor { get; set; }
        /// <summary>
        /// 大运牌CGC4250D5ECCE
        /// </summary>
        public string carModel { get; set; }
        /// <summary>
        /// 重型半挂牵引车
        /// </summary>
        public string carType { get; set; }
        /// <summary>
        /// B柴油
        /// </summary>
        public string useNNature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string registerTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string issueTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createtime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updatetime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string driving_image { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string list_image { get; set; }
        /// <summary>
        /// 山东天铭重工科技股份有限公司
        /// </summary>
        public string danwei { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string comment { get; set; }
        /// <summary>
        /// 国5
        /// </summary>
        public string emissionstage { get; set; }
        /// <summary>
        /// 个体
        /// </summary>
        public string fleet { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string temperature { get; set; }
        /// <summary>
        /// 耐火砖
        /// </summary>
        public string goods { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string goodsUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string carrierUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string inputPersonMobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ownerAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string driverName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string driverMobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string carImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string envLabelImage { get; set; }
        /// <summary>
        /// 货运
        /// </summary>
        public string operationLicenseImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expire_at { get; set; }
    }

    public class ThirdPartyCarInfoAPIResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回成功
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ThirdPartyCarInfoAPIResponseData data { get; set; }

        public bool success
        {
            get { return code == 1; }
            set {  code = value ? 1 : 0;}
        }
    }

}
