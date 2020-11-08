using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using RoutesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace RoutesApp.Pages.Coupon
{
    public partial class Coupon
    {
        #pragma warning disable
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        int counter = 0, totalCount = 0;
        string spinner = "", message = string.Empty, Name = string.Empty, userId = string.Empty, tokenInstitutionId = string.Empty, OfficerId = string.Empty;
        List<CouponListData> model = new List<CouponListData>();
        AlertMessageType messageType = AlertMessageType.Success;
        #pragma warning restore
        protected override async Task OnInitializedAsync()
        {
            try
            {

                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                tokenInstitutionId = userState.User.FindFirst("InstitutionId").Value;
                string UserName = userState.User.FindFirst("Name").Value;
                string Token = userState.User.FindFirst("AccessToken").Value;
                Http.DefaultRequestHeaders.Clear();
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
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
                    if (totalCount > 0)
                    {
                        message = string.Empty;
                        foreach (var item in response.data.OrderByDescending(s => s.createdAt).Take(4))
                        {
                            CouponListData couponList = new CouponListData();
                            couponList.Id = item.couponId;
                            couponList.Promotion = response.included.promotions.Where(x => x.PromotionId == item.promotionId).FirstOrDefault();
                            couponList.QrCodeImage = await JSRuntime.InvokeAsync<string>("GenerateQRCode", "http://vmtprojectstage.uaenorth.cloudapp.azure.com:5050/redeem/" + item.couponId + "");
                            couponList.Count = totalCount;
                            model.Add(couponList);
                        }
                    }
                    else
                    {
                        message = "No coupons available.";
                        messageType = AlertMessageType.Error;
                    }
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
                        messageType = AlertMessageType.Error;
                    }
                }
                spinner = "d-none";
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again. ";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }
        }
    }
}
