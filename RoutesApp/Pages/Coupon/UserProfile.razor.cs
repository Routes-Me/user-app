using RoutesApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutesApp.Models.DbModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace RoutesApp.Pages.Coupon
{
    public partial class UserProfile
    {
        string spinner = string.Empty, UserName = string.Empty, UserPhone = string.Empty, UserEmail = string.Empty, message = string.Empty;
        string modelSpinner = "d-none";
        AlertMessageType messageType = AlertMessageType.Success;
        PromotionCode model = new PromotionCode();

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override void OnInitialized()
        {
            try
            {
                var userState = authenticationState.Result;
                UserName = userState.User.FindFirst("Name").Value;
                UserPhone = userState.User.FindFirst("Phone").Value;
                UserEmail = userState.User.FindFirst("Email").Value;
                spinner = "d-none";
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }
        }

        public async Task Logout()
        {
            var localStateProvider = (LocalAuthenticationStateProvider)authenticationStateProvider;
            await localStateProvider.LogoutAsync();
            navigationManager.NavigateTo("/");
        }

        public async Task SubmitPromotionCode()
        {
            try
            {
                if (!string.IsNullOrEmpty(model.PromotionId))
                {
                    modelSpinner = "";
                    var userState = authenticationState.Result;
                    string userId = userState.User.FindFirst("UserId").Value;
                    string Token = userState.User.FindFirst("AccessToken").Value;
                    Http.DefaultRequestHeaders.Clear();
                    Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                    var promotionResult = await Http.GetAsync("/api/promotions/" + model.PromotionId);
                    var promotionResponseData = await promotionResult.Content.ReadAsStringAsync();
                    var promotionRresponse = JsonConvert.DeserializeObject<PromotionsGetResponse>(promotionResponseData);
                    if (promotionRresponse.status == true)
                    {
                        if (promotionRresponse.data.Count == 0)
                        {
                            message = "promotion not found.";
                            messageType = AlertMessageType.Error;
                        }
                    }
                    else
                    {
                        if (promotionRresponse.message.Contains("Authentication failed."))
                        {
                            navigationManager.NavigateTo("/");
                        }
                        else
                        {
                            message = "promotion not found.";
                            messageType = AlertMessageType.Error;
                        }
                    }


                    Models.DbModels.Coupon coupon = new Models.DbModels.Coupon()
                    {
                        promotionId = model.PromotionId,
                        userId = userId
                    };
                    Http.DefaultRequestHeaders.Clear();
                    Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                    var serializedValue = JsonConvert.SerializeObject(coupon);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/coupons", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Response>(responseData);
                    if (response.status == true)
                    {
                        navigationManager.NavigateTo("/promotion-details?id=" + model.PromotionId + "");
                    }
                    else
                    {
                        if (response.message.Contains("Authentication failed."))
                        {
                            navigationManager.NavigateTo("/");
                        }
                        else
                        {
                            message = response.message;
                        }
                        messageType = AlertMessageType.Error;
                    }
                }
                else
                {
                    message = "Promotions not found.";
                    messageType = AlertMessageType.Error;
                }
                modelSpinner = "d-none";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Input string was not in a correct format."))
                {
                    message = "Enter valid promotion.";
                }
                else
                {
                    message = "Something went wrong!! Please try again. ";
                }
                messageType = AlertMessageType.Error;
                modelSpinner = "d-none";
            }
        }
    }
}
