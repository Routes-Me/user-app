using Encryption;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RoutesApp.Models;
using RoutesApp.Models.CommonModels;
using RoutesApp.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RoutesApp.Pages.Auth
{
    public partial class Registration
    {
        Models.DbModels.Registration model = new Models.DbModels.Registration();
        bool isPhone = false;
        bool isEmail = true;
        string otpSentSuccess = "d-none";
        string otpSentProgress = "d-none";
        string spinner = "d-none";
        string message = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;

        public async Task CheckEmailOrPhone()
        {
            if (Regex.IsMatch(model.UserName, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", RegexOptions.IgnoreCase))
            {
                isPhone = false;
                isEmail = true;
            }
            else if (Regex.IsMatch(model.UserName, @"(\d*-)?\d{10}", RegexOptions.IgnoreCase))
            {
                isPhone = true;
                isEmail = false;
                otpSentProgress = "";
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    await JSRuntime.InvokeVoidAsync("stopTimer");
                    await SendOtp(model.UserName);
                }
            }
        }

        public async Task SendOtp(string PhoneNumber)
        {
            try
            {
                SendOTPModel sendOtpModel = new SendOTPModel()
                {
                    Phone = PhoneNumber
                };
                var serializedValue = JsonConvert.SerializeObject(sendOtpModel);
                var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                var result = await Http.PostAsync("/api/qr/otp", stringContent).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<Response>(responseData);
                if (response.status == true)
                {
                    message = string.Empty;
                    otpSentProgress = "d-none";
                    otpSentSuccess = "";
                    await JSRuntime.InvokeVoidAsync("timer", 240);
                }
                else
                {
                    otpSentProgress = "d-none";
                    message = response.message;
                    messageType = AlertMessageType.Error;
                }
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
        }

        public async Task ResendOTP()
        {
            model.Otp = string.Empty;
            await SendOtp(model.UserName);
        }

        public async Task UserSignup()
        {
            try
            {
                string Phone = string.Empty;
                string EncryptedPassword = string.Empty;
                string EmailAddress = string.Empty;
                List<RolesModel> Roles = new List<RolesModel>();
                RolesModel privilege = new RolesModel();
                EncryptionClass encryption = new EncryptionClass();
                string IVForAndroid = "Qz-N!p#ATb9_2MkL";
                string KeyForAndroid = "ledV\\K\"zRaNF]WXki,RMtLLZ{Cyr_1";
                string IVForDashboard = "7w'}DkAkO!A&mLyL";
                string KeyForDashboard = "Wf6cXM10cj_7B)V,";
                privilege.ApplicationId = "445123223";
                privilege.PrivilegeId = "1012576698";
                Roles.Add(privilege);

                if (isEmail)
                {
                    EmailAddress = model.UserName;
                    if (encryption.IndexOfBSign(model.Password) != -1)
                    {
                        EncryptedPassword = await encryption.EncryptAndEncode(model.Password, IVForDashboard, KeyForDashboard);
                    }
                    else
                    {
                        EncryptedPassword = await encryption.EncryptAndEncode(model.Password, IVForAndroid, KeyForAndroid);
                    }
                }
                else if (isPhone)
                {
                    Phone = model.UserName;
                    EncryptedPassword = model.Password;
                }

                SignUpModel registration = new SignUpModel()
                {
                    Name = model.Name,
                    Email = EmailAddress,
                    PhoneNumber = Phone,
                    Password = EncryptedPassword,
                    Roles = Roles
                };
                var serializedValue = JsonConvert.SerializeObject(registration);
                var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                var result = await Http.PostAsync("/api/signup", stringContent).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<QRUsersResponse>(responseData);
                if (response.status == true)
                {
                    message = string.Empty;
                    navigationManager.NavigateTo("/");
                }
                else
                {
                    message = response.message;
                    messageType = AlertMessageType.Error;
                }
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }


        public async Task RegisterUser()
        {
            try
            {
                spinner = string.Empty;
                await Task.Delay(1);
                if (isEmail)
                {
                    await UserSignup();
                }
                else if (isPhone)
                {
                    VerifyOTPModel verifyOtpModel = new VerifyOTPModel()
                    {
                        Phone = model.UserName,
                        Code = model.Otp
                    };
                    var serializedValue = JsonConvert.SerializeObject(verifyOtpModel);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/qr/otp/verify", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Response>(responseData);
                    if (response.status == true)
                    {
                        message = string.Empty;
                        await UserSignup();
                    }
                    else
                    {
                        message = response.message;
                        messageType = AlertMessageType.Error;
                    }
                }
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
            spinner = "d-none";
            await Task.Delay(1);
        }
    }
}
