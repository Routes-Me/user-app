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
using System.IdentityModel.Tokens.Jwt;

namespace RoutesApp.Pages.Auth
{
    public partial class Login
    {
        Models.DbModels.Login model = new Models.DbModels.Login();
        bool isPhone = false, isEmail = true;
        string otpSentSuccess = "d-none", otpSentProgress = "d-none", spinner = "d-none", message = string.Empty, returnUrl = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        bool officer = false;
        string OfficerId = string.Empty;

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

        public async Task LoginUser()
        {
            try
            {
                spinner = string.Empty;
                await Task.Delay(1);
                bool IsRedeem = false;
                returnUrl = WebUtility.UrlDecode(new Uri(navigationManager.Uri).PathAndQuery);
                if (returnUrl == "/")
                {
                    returnUrl = string.Empty;
                }
                else
                {
                    returnUrl = returnUrl.Replace("/?", "");
                }

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    string[] arrReturnUrl = returnUrl.Split('/');
                    if (arrReturnUrl.Length > 0)
                    {
                        if (arrReturnUrl[1].ToLower() == "redeem")
                        {
                            IsRedeem = true;
                        }
                    }
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
                    var result = await Http.PostAsync("/api/signin", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<SignInResponse>(responseData);

                    if (response.status == true)
                    {
                        message = string.Empty;
                        Http.DefaultRequestHeaders.Clear();
                        Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {response.token}");
                        var jwtPayload = await parseJwtAsync(response.token);
                        await GetOfficerAsync(jwtPayload);
                        var userInfo = new LocalUserInfo()
                        {
                            Token = response.token,
                            OfficerId = OfficerId,
                            isOfficer = officer,
                            tokenPayload = jwtPayload
                        };
                        await storageService.SetItemAsync("User", userInfo);
                        await authenticationStateProvider.GetAuthenticationStateAsync();
                        if (officer == true)
                        {
                            if (!string.IsNullOrEmpty(OfficerId))
                            {
                                if (IsRedeem == true)
                                {
                                    navigationManager.NavigateTo(returnUrl);
                                }
                                else
                                {
                                    navigationManager.NavigateTo("/history?id=" + OfficerId + "");
                                }

                            }
                            else
                            {
                                message = "Officer not found.";
                                messageType = AlertMessageType.Error;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(returnUrl) || returnUrl == null || returnUrl == "" || returnUrl.Trim().Length == 0)
                            {
                                navigationManager.NavigateTo("/coupons");
                            }
                            else
                            {
                                navigationManager.NavigateTo(returnUrl);
                            }
                        }
                    }
                    else
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseData);
                        foreach (var item in errorResponse.errors)
                        {
                            message = item.detail;
                            messageType = AlertMessageType.Error;
                        }
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
                    var result = await Http.PostAsync("/api/signin/otp/verify", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<SignInResponse>(responseData);
                    if (response.status == true)
                    {
                        message = string.Empty;
                        Http.DefaultRequestHeaders.Clear();
                        Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {response.token}");
                        var jwtPayload = await parseJwtAsync(response.token);
                        await GetOfficerAsync(jwtPayload);
                        var userInfo = new LocalUserInfo()
                        {
                            Token = response.token,
                            OfficerId = OfficerId,
                            isOfficer = officer,
                            tokenPayload = jwtPayload
                        };
                        await storageService.SetItemAsync("User", userInfo);
                        await authenticationStateProvider.GetAuthenticationStateAsync();
                        if (officer == true)
                        {
                            if (!string.IsNullOrEmpty(OfficerId))
                            {
                                if (IsRedeem == true)
                                {
                                    navigationManager.NavigateTo(returnUrl);
                                }
                                else
                                {
                                    navigationManager.NavigateTo("/history?id=" + OfficerId + "");
                                }

                            }
                            else
                            {
                                message = "Officer not found.";
                                messageType = AlertMessageType.Error;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(returnUrl) || returnUrl == null || returnUrl == "" || returnUrl.Trim().Length == 0)
                            {
                                navigationManager.NavigateTo("/coupons");
                            }
                            else
                            {
                                navigationManager.NavigateTo(returnUrl);
                            }
                        }
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
                message = "Something went wrong!! Please try again. ";
                messageType = AlertMessageType.Error;
            }
            spinner = "d-none";
            await Task.Delay(1);
        }

        private async Task GetOfficerAsync(TokenPayload jwtPayload)
        {
            foreach (var item in jwtPayload.Roles)
            {
                if (item.Privilege.ToLower() == "employee")
                {
                    officer = true;
                }
            }

            if (officer == true)
            {
                var officerResult = await Http.GetAsync("/api/officers?userId=" + jwtPayload.UserId + "");
                var officerResponseData = await officerResult.Content.ReadAsStringAsync();
                var officerResponse = JsonConvert.DeserializeObject<OfficersResponse>(officerResponseData);
                if (officerResponse.status == true)
                {
                    if (officerResponse.data.Count > 0)
                    {
                        foreach (var itemData in officerResponse.data)
                        {
                            OfficerId = itemData.OfficerId;
                        }
                    }
                }
            }
        }

        public async Task<TokenPayload> parseJwtAsync(string token)
        {
            string decodePayload = await JSRuntime.InvokeAsync<string>("ParseJWT", token);
            decodePayload = decodePayload.Replace(@"\", "").Replace("\"[", "[").Replace("]\"", "]");
            return JsonConvert.DeserializeObject<TokenPayload>(decodePayload);
        }
    }
}
