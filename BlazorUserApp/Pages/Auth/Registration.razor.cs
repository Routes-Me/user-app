using Encryption;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorUserApp.Models;
using BlazorUserApp.Models.CommonModels;
using BlazorUserApp.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlazorUserApp.Pages.Auth
{
    public partial class Registration
    {
        Models.DbModels.Registration model = new Models.DbModels.Registration();
        bool isPhone = false;
        bool isEmail = true;
        string otpSentSuccess = "d-none";
        string otpSentProgress = "d-none";
        string displaySpinner = "d-none";
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
                var serializedValue = JsonSerializer.Serialize(sendOtpModel);
                var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                var result = await Http.PostAsync("/api/qrsendotp", stringContent).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                Response response = new Response();
                response = JsonSerializer.Deserialize<Response>(responseData);
                if (response.status == true)
                {
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
                string EncyptedPassword = string.Empty;
                string EmailAddress = string.Empty;
                List<int> staticRole = new List<int>();
                EncryptionClass encryption = new EncryptionClass();
                
                string IV = "Qz-N!p#ATb9_2MkL";
                string PASSWORD = "ledV\\K\"zRaNF]WXki,RMtLLZ{Cyr_1";
                staticRole.Add(1);

                if (isEmail)
                {
                    EmailAddress = model.UserName;
                    EncyptedPassword = await encryption.EncryptAndEncode(model.Password, IV, PASSWORD);
                }
                else if (isPhone)
                {
                    Phone = model.UserName;
                    EncyptedPassword = model.Password;
                }

                SignUpModel registration = new SignUpModel()
                {
                    Name = model.Name,
                    Email = EmailAddress,
                    PhoneNumber = Phone,
                    Password = EncyptedPassword,
                    Roles = staticRole
                };
                var serializedValue = JsonSerializer.Serialize(registration);
                var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                var result = await Http.PostAsync("/api/signup", stringContent).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                QRUsersResponse response = new QRUsersResponse();
                response = JsonSerializer.Deserialize<QRUsersResponse>(responseData);
                if (response.status == true)
                {
                    displaySpinner = "d-none";
                    navigationManager.NavigateTo("/");
                }
                else
                {
                    message = response.message;
                    messageType = AlertMessageType.Error;
                }
                displaySpinner = "d-none";
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
        }


        public async Task RegisterUser()
        {
            try
            {
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
                    var serializedValue = JsonSerializer.Serialize(verifyOtpModel);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/qrverifyotp", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    Response response = new Response();
                    response = JsonSerializer.Deserialize<Response>(responseData);
                    if (response.status == true)
                    {
                        await UserSignup();
                    }
                    else
                    {
                        message = response.message;
                        messageType = AlertMessageType.Error;
                    }
                }
                displaySpinner = "d-none";
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
        }
    }
}
