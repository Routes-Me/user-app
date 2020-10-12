using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Models.DbModels
{
    public class Promotion
    {
        public string PromotionId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int? UsageLimit { get; set; }
        public string AdvertisementId { get; set; }
        public string InstitutionId { get; set; }
        public bool? IsSharable { get; set; }
        public string LogoUrl { get; set; }
    }
}
