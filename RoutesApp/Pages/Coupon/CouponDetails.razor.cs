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
    public partial class CouponDetails
    {
        string spinner = "", couponId = string.Empty, token = string.Empty, message = string.Empty;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        List<CouponDetailData> model = new List<CouponDetailData>();
        AlertMessageType messageType = AlertMessageType.Success;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    couponId = _id;
                }
                var result = await Http.GetAsync("/api/coupons/" + couponId + "?include=promotion");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CouponResponse>(responseData);
                if (response.status == true)
                {
                    foreach (var item in response.data)
                    {
                        CouponDetailData qrModel = new CouponDetailData();
                        qrModel.Id = item.couponId;
                        qrModel.Promotion = response.included.promotions.Where(x => x.PromotionId == item.promotionId).FirstOrDefault();
                        qrModel.QrCodeImage = await JSRuntime.InvokeAsync<string>("GenerateQRCode", "https://userapp.routesme.com/coupons/" + item.couponId + "");
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
