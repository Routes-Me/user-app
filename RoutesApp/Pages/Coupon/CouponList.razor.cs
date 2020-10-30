using RoutesApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Pages.Coupon
{
    public partial class CouponList
    {
        string spinner = "", message = string.Empty, userId = string.Empty, token = string.Empty;
        List<QRCodeAll> model = new List<QRCodeAll>();
        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                string Token = userState.User.FindFirst("AccessToken").Value;
                Http.DefaultRequestHeaders.Clear();
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                var result = await Http.GetAsync("/api/coupons?userId=" + userId + "&include=promotion");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CouponResponse>(responseData);
                if (response.status == true)
                {
                    foreach (var item in response.data)
                    {
                        QRCodeAll qrModel = new QRCodeAll();
                        qrModel.Id = item.couponId;
                        qrModel.Promotion = response.included.promotions.Where(x => x.PromotionId == item.promotionId).FirstOrDefault();
                        model.Add(qrModel);
                    }
                }
                else
                {
                    message = response.message;
                    messageType = AlertMessageType.Error;
                }
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
            spinner = "d-none";
        }
    }
}
