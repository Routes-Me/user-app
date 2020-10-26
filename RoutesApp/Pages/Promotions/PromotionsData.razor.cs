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

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        string spinner = "", message = string.Empty, couponLoader = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        string promotionId = string.Empty, couponId = string.Empty, userId = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                spinner = "";
                var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    promotionId = _id;
                }
                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                if (!string.IsNullOrEmpty(promotionId) && Convert.ToInt32(promotionId) > 0)
                {
                    Models.DbModels.Coupon coupon = new Models.DbModels.Coupon()
                    {
                        promotionId = promotionId,
                        userId = userId
                    };
                    var serializedValue = JsonConvert.SerializeObject(coupon);
                    var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                    var result = await Http.PostAsync("/api/coupons", stringContent).ConfigureAwait(false);
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Response>(responseData);
                    if (response.status == true)
                    {
                        NavManager.NavigateTo("/promotion-details?id=" + promotionId + "");
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
            NavManager.NavigateTo("/");
        }
        public async Task RedirectToCoupon()
        {
            await JSRuntime.InvokeVoidAsync("removeModelBackdrop");
            NavManager.NavigateTo("/coupon");
        }
    }
}
