using Encryption;
using Microsoft.JSInterop;
using RoutesApp.Models;
using RoutesApp.Models.DbModels;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace RoutesApp.Pages.Auth
{
    public partial class Login
    {
        Models.DbModels.Login model = new Models.DbModels.Login();
        bool isPhone = false, isEmail = true;
        string otpSentSuccess = "d-none", otpSentProgress = "d-none", spinner = "d-none", message = string.Empty;
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
                var result = await Http.PostAsync("/api/otp", stringContent).ConfigureAwait(false);
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
                spinner = "";
                await Task.Delay(1);
                string returnUrl = WebUtility.UrlDecode(new Uri(navigationManager.Uri).PathAndQuery);
                if(returnUrl == "/")
                {
                    returnUrl = string.Empty;
                }
                else
                {
                    returnUrl = returnUrl.Replace("/?", "");
                }
                EncryptionClass encryption = new EncryptionClass();
                string IVForAndroid = "Qz-N!p#ATb9_2MkL";
                string KeyForAndroid = "ledV\\K\"zRaNF]WXki,RMtLLZ{Cyr_1";
                string IVForDashboard = "7w'}DkAkO!A&mLyL";
                string KeyForDashboard = "Wf6cXM10cj_7B)V,";
                string EncryptedPassword = string.Empty;
                if (isEmail)
                {
                    if (encryption.IndexOfBSign(model.Password) != -1)
                    {
                        EncryptedPassword = await encryption.EncryptAndEncode(model.Password, IVForDashboard, KeyForDashboard);
                    }
                    else
                    {
                        EncryptedPassword = await encryption.EncryptAndEncode(model.Password, IVForAndroid, KeyForAndroid);
                    }

                    SignInModel loginUser = new SignInModel()
                    {
                        UserName = model.UserName,
                        Password = EncryptedPassword,
                    };

                    var serializedValue = JsonConvert.SerializeObject(loginUser);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/qr/signin", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<QrSignInResponse>(responseData);
                    if (response.status == true)
                    {

                        var userInfo = new LocalUserInfo()
                        {
                            Token = response.Token,
                            loginUser = response.user
                        };
                        await storageService.SetItemAsync("User", userInfo);
                        await authenticationStateProvider.GetAuthenticationStateAsync();
                        Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {response.Token}");
                        if (string.IsNullOrEmpty(returnUrl) || returnUrl== null || returnUrl == "" || returnUrl.Trim().Length == 0)
                        {
                            navigationManager.NavigateTo("/coupon");
                        }
                        else
                        {
                            navigationManager.NavigateTo(returnUrl);
                        }
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
                    var result = await Http.PostAsync("/api/qr/signin/otp/verify", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<QrSignInResponse>(responseData);
                    if (response.status == true)
                    {
                        var userInfo = new LocalUserInfo()
                        {
                            Token = response.Token,
                            loginUser = response.user
                        };
                        await storageService.SetItemAsync("User", userInfo);
                        await authenticationStateProvider.GetAuthenticationStateAsync();
                        if (string.IsNullOrEmpty(returnUrl) || returnUrl == null || returnUrl == "")
                        {
                            navigationManager.NavigateTo("/coupon");
                        }
                        else
                        {
                            string url = returnUrl.Replace("/?", "");
                            navigationManager.NavigateTo(url);
                        }
                    }
                    else
                    {
                        message = response.message;
                        messageType = AlertMessageType.Error;
                    }
                }
                spinner = "d-none";
                await Task.Delay(1);
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
