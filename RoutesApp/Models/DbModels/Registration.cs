﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Models.DbModels
{
    public class Registration
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        [UsernameValidation]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [StringLength(50)]
        public string Password { get; set; }
        [StringLength(6)]
        public string Otp { get; set; }
    }

    public class SignUpModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public List<RolesModel> Roles { get; set; }
    }

    public class EmailModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class SendOTPModel
    {
        public string Phone { get; set; }
    }

    public class VerifyOTPModel
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }

    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class SetNewPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password must match.")]
        public string ConfirmPassword { get; set; }
    }
    public class ChangePasswordModel
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
