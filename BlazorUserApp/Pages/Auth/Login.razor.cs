using Encryption;
using Microsoft.JSInterop;
using BlazorUserApp.Models;
using BlazorUserApp.Models.DbModels;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlazorUserApp.Pages.Auth
{
    public partial class Login
    {
        Models.DbModels.Login model = new Models.DbModels.Login();
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
                var serializedValue = JsonConvert.SerializeObject(sendOtpModel);
                var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                var result = await Http.PostAsync("/api/sendotp", stringContent).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<Response>(responseData);
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

        public async Task LoginUser()
        {
            try
            {
                EncryptionClass encryption = new EncryptionClass();
                string IV = "Qz-N!p#ATb9_2MkL";
                string PASSWORD = "ledV\\K\"zRaNF]WXki,RMtLLZ{Cyr_1";
                if (isEmail)
                {
                    string EncryptedPassword = await encryption.EncryptAndEncode(model.Password, IV, PASSWORD);
                    SignInModel loginUser = new SignInModel()
                    {
                        UserName = model.UserName,
                        Password = EncryptedPassword,
                    };

                    var serializedValue = JsonConvert.SerializeObject(loginUser);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/qrsignin", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<SignInResponse>(responseData);
                    if (response.status == true)
                    {
                        var userInfo = new LocalUserInfo()
                        {
                            UserId = response.user.UserId,
                            Name = response.user.Name,
                            Email = response.user.Email,
                            Phone = response.user.Phone,
                            Token = response.user.Token
                        };
                        await storageService.SetItemAsync("User", userInfo);
                        await authenticationStateProvider.GetAuthenticationStateAsync();
                        displaySpinner = "d-none";
                        navigationManager.NavigateTo("/coupon");
                    }
                    else
                    {
                        message = response.message;
                        messageType = AlertMessageType.Error;
                    }
                }
                else if (isPhone)
                {
                    VerifySignInOTPModel otpUser = new VerifySignInOTPModel()
                    {
                        Phone = model.UserName,
                        Code = model.Otp,
                    };
                    var serializedValue = JsonConvert.SerializeObject(otpUser);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/qrverifysigninotp", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<SignInResponse>(responseData);
                    if (response.status == true)
                    {
                        var userInfo = new LocalUserInfo()
                        {
                            UserId = response.user.UserId,
                            Name = response.user.Name,
                            Email = response.user.Email,
                            Phone = response.user.Phone,
                            Token = response.user.Token
                        };
                        await storageService.SetItemAsync("User", userInfo);
                        await authenticationStateProvider.GetAuthenticationStateAsync();
                        displaySpinner = "d-none";
                        navigationManager.NavigateTo("/coupon");
                    }
                    else
                    {
                        message = response.message;
                        messageType = AlertMessageType.Error;
                    }
                }
                displaySpinner = "d-none";
            }
            catch (Exception ex)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
        }
    }
}
