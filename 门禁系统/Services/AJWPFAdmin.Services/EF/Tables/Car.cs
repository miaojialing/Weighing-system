using AJWPFAdmin.Core.CommonEntity;
using AJWPFAdmin.Core.Models;
using AJWPFAdmin.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services.EF.Tables
{
    /// <summary>
    /// 车辆记录表
    /// </summary>
    [Comment("车辆记录表")]
    public partial class Car : CommonTableEntity
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        [Comment("车牌号")]
        [Required]
        [MaxLength(120)]
        public string CarNo { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        [Comment("卡号")]
        [MaxLength(120)]
        public string IDCardNo { get; set; }

        /// <summary>
        /// 车辆类型Id, CarType.Id
        /// </summary>
        [Comment("车辆类型Id")]
        public long TypeId { get; set; }

        /// <summary>
        /// 车辆类型名称,  CarType.Name
        /// </summary>
        [Comment("车辆类型名称")]
        [Required]
        public string TypeName { get; set; }

        /// <summary>
        /// 有效期, 过了有效期自动变为临时车
        /// </summary>
        [Comment("有效期")]
        public DateTime? ExpireDate { get; set; }

        /// <summary>
        /// 排放阶段
        /// </summary>
        [MaxLength(50)]
        [Comment("排放阶段")]
        public string PaiFangJieDuan { get; set; }

        /// <summary>
        /// 发动机号
        /// </summary>
        [Comment("发动机号")]
        [MaxLength(50)]
        public string EngineNo { get; set; }

        /// <summary>
        /// VIN
        /// </summary>
        [MaxLength(50)]
        [Comment("VIN")]
        public string VIN { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        [Comment("注册日期")]
        public DateTime? RegDate { get; set; }

        /// <summary>
        /// 车队名称
        /// </summary>
        [Comment("车队名称")]
        [MaxLength(50)]
        public string TeamName { get; set; }

        /// <summary>
        /// 货物名称
        /// </summary>
        [Comment("货物名称")]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "{0}超长:{1}")]
        public string MaterialName { get; set; }

        /// <summary>
        /// 重量(KG) 
        /// </summary>
        [Comment("重量(KG)")]
        [Display(Name = "重量")]
        [Precision(18, 2)]
        public decimal CarNetWeight { get; set; }

        /// <summary>
        /// 行驶证图片路径json数组字符串
        /// </summary>
        [Comment("行驶证图片路径json数组字符串")]
        public string VehicleLicense { get; set; }

        /// <summary>
        /// 随车清单路径json数组字符串
        /// </summary>
        [Comment("随车清单路径json数组字符串")]
        public string Attachments { get; set; }


        /// <summary>
        /// 根据第三方车辆信息数据更新本机数据,内部调用了 db.savechanges, 可能会有并发问题,加了try
        /// </summary>
        /// <param name="db"></param>
        /// <param name="carRes"></param>
        public void UpdateFromThirdPartyAPI(ref DbService db, ThirdPartyCarInfoAPIResponse carRes)
        {
            var updated = false;
            if (EngineNo != carRes.data.engineNumber)
            {
                EngineNo = carRes.data.engineNumber;
                updated = true;
            }

            if (MaterialName != carRes.data.goods)
            {
                MaterialName = carRes.data.goods;
                updated = true;
            }

            if (PaiFangJieDuan != carRes.data.emissionstage)
            {
                PaiFangJieDuan = carRes.data.emissionstage;
                updated = true;
            }

            if (TeamName != carRes.data.fleet)
            {
                TeamName = carRes.data.fleet;
                updated = true;
            }

            if (VIN != carRes.data.vin)
            {
                VIN = carRes.data.vin;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(carRes.data.driving_image))
            {
                var img = CommonUtil
                .AJSerializeObject(new string[] { carRes.data.driving_image });
                if (VehicleLicense != img)
                {
                    VehicleLicense = img;
                    updated = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(carRes.data.list_image))
            {
                var img = CommonUtil
                .AJSerializeObject(new string[] { carRes.data.list_image });
                if (Attachments != img)
                {
                    Attachments = img;
                    updated = true;
                }
            }

            if (updated)
            {
                db.Entry(this).State = EntityState .Modified;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }

        }
    }
}
