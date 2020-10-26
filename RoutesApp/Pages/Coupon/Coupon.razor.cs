using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using RoutesApp.Models;
using RoutesApp.Models.DbModels;
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

namespace RoutesApp.Pages.Coupon
{
    public partial class Coupon
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        int counter = 0, totalCount = 0;
        string spinner = "", message = string.Empty, Name = string.Empty;
        List<CouponListData> model = new List<CouponListData>();
        AlertMessageType messageType = AlertMessageType.Success;
        string promotionId = string.Empty, couponId = string.Empty, userId = string.Empty, token = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                string UserName = userState.User.FindFirst("Name").Value;
                string[] arrUserName = UserName.Split(' ');
                if (arrUserName.Length > 1)
                {
                    Name = arrUserName[0].Substring(0, 1) + arrUserName[1].Substring(0, 1);
                }
                else
                {
                    Name = UserName.Substring(0, 2);
                }
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
                        couponList.Promotion = response.included.promotions.Where(x => x.PromotionId == item.promotionId).FirstOrDefault();
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
                spinner = "d-none";
            }
            catch (Exception ex)
            {
                message = "Something went wrong!! Please try again. " + ex.Message;
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }
        }
    }
}
