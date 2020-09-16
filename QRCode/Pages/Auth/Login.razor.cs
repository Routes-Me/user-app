using Encryption;
using Microsoft.JSInterop;
using QRCode.Models;
using QRCode.Models.DbModels;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QRCode.Pages.Auth
{
    public partial class Login
    {
        LoginModel model = new LoginModel();
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
                var result = await Http.PostAsync("/api/sendotp", stringContent).ConfigureAwait(false);
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
            catch (Exception ex)
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
                System.Threading.Thread.Sleep(1000);
                EncryptionClass encryption = new EncryptionClass();
                string IV = "Qz-N!p#ATb9_2MkL";
                string PASSWORD = "ledV\\K\"zRaNF]WXki,RMtLLZ{Cyr_1";
                if (isEmail)
                {
                    SignInModel loginUser = new SignInModel()
                    {
                        UserName = model.UserName,
                        Password = await encryption.EncryptAndEncode(model.Password, IV, PASSWORD),
                    };

                    var serializedValue = JsonSerializer.Serialize(loginUser);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/signin", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    Response response = new Response();
                    response = JsonSerializer.Deserialize<SignInResponse>(responseData);
                    if (response.status == true)
                    {
                        displaySpinner = "d-none";
                        navigationManager.NavigateTo("/index");
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
                    var serializedValue = JsonSerializer.Serialize(otpUser);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/verifysigninotp", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    Response response = new Response();
                    response = JsonSerializer.Deserialize<Response>(responseData);
                    if (response.status == true)
                    {
                        displaySpinner = "d-none";
                        navigationManager.NavigateTo("/index");
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
