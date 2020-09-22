using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Models.DbModels
{
    public class Coupon
    {
        public string CouponId { get; set; }
        public string PromotionId { get; set; }
        public string UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Uri Uri { get; set; } 
        public virtual Promotion Promotion { get; set; }
    }
}
    