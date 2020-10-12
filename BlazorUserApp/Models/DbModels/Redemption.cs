using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Models.DbModels
{
    public class Redemption
    {
        public string CouponId { get; set; }
        public string OfficerId { get; set; }
        public string InstitutionId { get; set; }
        public string Pin { get; set; }
    }

    public class RedemptionGetModel
    {
        public string RedemptionId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CouponId { get; set; }
        public string OfficerId { get; set; }
    }
}   
