using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using RoutesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RoutesApp.Pages.Promotions
{
    public partial class PromotionsData
    {
        [Parameter]
        public string PromotionId { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        string spinner = "", message = string.Empty, couponLoader = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        string couponId = string.Empty, userId = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                spinner = "";
                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                if (!string.IsNullOrEmpty(PromotionId) && Convert.ToInt32(PromotionId) > 0)
                {
                    Models.DbModels.Coupon coupon = new Models.DbModels.Coupon()
                    {
                        promotionId = PromotionId,
                        userId = userId
                    };
                    var serializedValue = JsonConvert.SerializeObject(coupon);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/coupons", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Response>(responseData);
                    if (response.status == true)
                    {
                        navigationManager.NavigateTo("/promotion-details?id=" + PromotionId + "");
                    }
                    else
                    {
                        if (response.message.Contains("Coupons already redeemed"))
                        {
                            await JSRuntime.InvokeVoidAsync("displayPopupModel", response.message);
                        }
                        else
                        {
                            message = response.message;
                        }
                        couponLoader = "d-none";
                        messageType = AlertMessageType.Error;
                    }
                }
                else
                {
                    message = "Promotions not found.";
                    messageType = AlertMessageType.Error;
                }
                spinner = "d-none";
                couponLoader = "d-none";
            }
            catch (Exception ex)
            {
                message = "Something went wrong!! Please try again. " + ex.Message;
                messageType = AlertMessageType.Error;
                spinner = "d-none";
                couponLoader = "d-none";
            }
        }
        public async Task RedirectToLogin()
        {
            await JSRuntime.InvokeVoidAsync("removeModelBackdrop");
            navigationManager.NavigateTo("/");
        }
        public async Task RedirectToCoupon()
        {
            await JSRuntime.InvokeVoidAsync("removeModelBackdrop");
            navigationManager.NavigateTo("/coupons");
        }
    }
}
