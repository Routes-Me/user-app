using BlazorUserApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Pages.Coupon
{
    public partial class CouponList
    {
        string spinner = "";
        List<QRCodeAll> model = new List<QRCodeAll>();
        string message = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                string userId = string.Empty;
                Http.BaseAddress = null;
                Http.BaseAddress = new Uri("http://localhost:63746");
                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                var result = await Http.GetAsync("/api/coupons?userId=" + userId + "&include=promotion");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CouponResponse>(responseData);
                if (response.status == true)
                {
                    foreach (var item in response.data)
                    {
                        QRCodeAll qrModel = new QRCodeAll();
                        qrModel.Id = item.couponId;
                        qrModel.Promotion = response.included.promotions.Where(x => x.PromotionId == Convert.ToInt32(item.promotionId)).FirstOrDefault();
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
