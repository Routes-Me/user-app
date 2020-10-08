using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Models.DbModels
{
    public class Officer
    {
        public string OfficerId { get; set; }
        public string UserId { get; set; }
        public string InstitutionId { get; set; }
    }
}
