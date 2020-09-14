using QRCode.Models.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QRCode.Models
{
    public class UsernameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value.ToString();
            if (Regex.IsMatch(email, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", RegexOptions.IgnoreCase))
            {
                return ValidationResult.Success;
            }
            else if (Regex.IsMatch(email, @"(\d*-)?\d{10}", RegexOptions.IgnoreCase))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Invalid EmailID or Mobile Number.");
            }
        }
    }

    public class PasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var loginModel = validationContext.ObjectInstance as LoginModel;
            if (loginModel != null)
            {
                if (string.IsNullOrEmpty(loginModel.UserName) && string.IsNullOrEmpty(loginModel.Password))
                {
                    return new ValidationResult("Password is required.");
                }
                else if (!string.IsNullOrEmpty(loginModel.UserName))
                {
                    if (Regex.IsMatch(loginModel.UserName, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", RegexOptions.IgnoreCase))
                    {
                        string password = value.ToString().Trim();
                        if (string.IsNullOrEmpty(password))
                        {
                            return new ValidationResult("Password is required.");
                        }

                        if (password.Length < 6)
                        {
                            return new ValidationResult("Password must be at least 6 characters long.");
                        }

                        if (password.Length > 50)
                        {
                            return new ValidationResult("Password should not be more then 50 characters long.");
                        }
                        return ValidationResult.Success;
                    }
                    else if (Regex.IsMatch(loginModel.UserName, @"(\d*-)?\d{10}", RegexOptions.IgnoreCase))
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return ValidationResult.Success;
        }
    }

    public class OTPValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var loginModel = validationContext.ObjectInstance as LoginModel;
            if (loginModel != null)
            {
                if (!string.IsNullOrEmpty(loginModel.Password) && value == null)
                {
                    return ValidationResult.Success;
                }
                if (!string.IsNullOrEmpty(loginModel.UserName))
                {
                    string otpValue = value.ToString().Trim();
                    if (Regex.IsMatch(loginModel.UserName, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", RegexOptions.IgnoreCase))
                    {
                        return ValidationResult.Success;
                    }
                    else if (Regex.IsMatch(loginModel.UserName, @"(\d*-)?\d{10}", RegexOptions.IgnoreCase))
                    {
                        if (string.IsNullOrEmpty(otpValue))
                        {
                            return new ValidationResult("OTP is required.");
                        }
                        if (otpValue.Length != 6)
                        {
                            return new ValidationResult("OTP must be 6 digit long.");
                        }
                        return ValidationResult.Success;
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
