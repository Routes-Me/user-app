using BlazorUserApp.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Models
{
    public class LocalUserInfo
    {
        public string Token { get; set; }
        public string OfficerId { get; set; }
        public bool isOfficer { get; set; }
        public LoginUser loginUser { get; set; }
    }
}
