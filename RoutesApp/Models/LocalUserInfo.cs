using RoutesApp.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Models
{
    public class LocalUserInfo
    {
        public string Token { get; set; }
        public string OfficerId { get; set; }
        public bool isOfficer { get; set; }
        public TokenPayload tokenPayload { get; set; }
    }
}
