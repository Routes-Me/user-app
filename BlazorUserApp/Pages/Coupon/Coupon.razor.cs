using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using BlazorUserApp.Models;
using BlazorUserApp.Models.DbModels;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.JSInterop;
using System.Runtime.InteropServices;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorUserApp.Pages.Coupon
{
    public partial class Coupon
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        int counter = 0, totalCount = 0;
        string spinner = "", message = string.Empty;
        List<CouponListData> model = new List<CouponListData>();
        AlertMessageType messageType = AlertMessageType.Success;
        string promotionId = string.Empty, couponId = string.Empty, userId = string.Empty, token = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    promotionId = _id;
                }
                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                token = userState.User.FindFirst("AccessToken").Value;
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
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
                        await GetCouponByUser(userId);
                    }
                    else
                    {
                        if (response.message.Contains("Coupons already redeemed"))
                            await JSRuntime.InvokeVoidAsync("displayPopupModel", response.message);
                        else
                            message = response.message;

                        messageType = AlertMessageType.Error;
                        await GetCouponByUser(userId);
                    }
                }
                else
                {
                    await GetCouponByUser(userId);
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
            spinner = "d-none";
        }


        public async Task GetCouponByUser(string userId)
        {
            try
            {
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var result = await Http.GetAsync("/api/coupons?userId=" + userId + "&include=promotion");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CouponResponse>(responseData);
                if (response.status == true)
                {
                    totalCount = response.data.Count();
                    foreach (var item in response.data.OrderByDescending(s => s.createdAt).Take(4))
                    {
                        CouponListData couponList = new CouponListData();
                        couponList.Id = item.couponId;
                        couponList.Promotion = response.included.promotions.Where(x => x.PromotionId == Convert.ToInt32(item.promotionId)).FirstOrDefault();
                        couponList.QrCodeImage = await JSRuntime.InvokeAsync<string>("GenerateQRCode", "https://userapp.routesme.com/coupons/" + item.couponId + "");
                        couponList.Count = totalCount;
                        model.Add(couponList);
                    }
                }
                else
                {
                    message = response.message;
                    messageType = AlertMessageType.Error;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
