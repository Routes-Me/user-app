using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Models.DbModels
{
    public class Login
    {
        [Required(ErrorMessage = " Email or phone number field is required.")]
        [StringLength(50, ErrorMessage = "Email or phone number should not be more than 50 characters long.")]
        [UsernameValidation]
        public string UserName { get; set; }

        [PasswordValidation]
        public string Password { get; set; }

        [OTPValidation]

        public string Otp { get; set; }
    }

    public class SignInModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class VerifySignInOTPModel
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }

    public class LoginUser
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public List<UserRoleForToken> Roles { get; set; }
        public string InstitutionId { get; set; }
        public bool isOfficer { get; set; }
        public string OfficerId { get; set; }
    }
}
