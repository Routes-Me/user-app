using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Models.DbModels
{
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Application { get; set; }
    }

    public class TokenPayload
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public List<UserRoleForToken> Roles { get; set; }
        public string InstitutionId { get; set; }
    }
    public class UserRoleForToken
    {
        public string Application { get; set; }
        public string Privilege { get; set; }
    }
}
