using Newtonsoft.Json;
using QRCode.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRCode.Models
{
    public class Response
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
    }

    public class QRUsersResponse : Response
    {
        public int userId { get; set; }
        public string email { get; set; }
    }

    public class SignInResponse : Response
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string token { get; set; }
    }

    public class Pagination
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
    }
 
    public class PromotionsResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<PromotionsModel> data { get; set; }
    }

    public class CouponResponse : Response
    {
        public string couponId { get; set; }
    }
}
