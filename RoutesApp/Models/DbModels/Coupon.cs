using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Models.DbModels
{
    public class Coupon
    {
        public string couponId { get; set; }
        public string promotionId { get; set; }
        public string userId { get; set; }
        public DateTime? createdAt { get; set; }
    }

    public class CouponModel
    {
        public string couponId { get; set; }
        public string promotionId { get; set; }
        public string userId { get; set; }
        public DateTime? createdAt { get; set; }
        public Promotion Promotion { get; set; }
    }
}
    