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
        [Parameter]
        public string CouponId { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        int counter = 0, totalCount = 0;
        string spinner = "", message = string.Empty, Name = string.Empty, userId = string.Empty, tokenInstitutionId = string.Empty;
        List<CouponListData> model = new List<CouponListData>();
        AlertMessageType messageType = AlertMessageType.Success;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                tokenInstitutionId = userState.User.FindFirst("InstitutionId").Value;
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

                if (!string.IsNullOrEmpty(CouponId))
                {
                    var couponResult = await Http.GetAsync("/api/coupons/" + CouponId + "?include=promotion");
                    var couponResponseData = await couponResult.Content.ReadAsStringAsync();
                    var couponResponse = JsonConvert.DeserializeObject<CouponGetResponse>(couponResponseData);
                    if (couponResponse.status == true)
                    {
                        string promotionInstitutionId = couponResponse.included.promotions.Select(x => x.InstitutionId).FirstOrDefault();
                        if (tokenInstitutionId == promotionInstitutionId)
                        {
                            navigationManager.NavigateTo("/redeem?id=" + CouponId + "");
                        }
                        else
                        {
                            message = "Opps!! You are not authorize to redeem coupon.";
                            messageType = AlertMessageType.Error;
                        }
                    }
                }
                else
                {
                    var result = await Http.GetAsync("/api/coupons?userId=" + userId + "&include=promotion");
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<CouponResponse>(responseData);
                    if (response.status == true)
                    {
                        totalCount = response.data.Count();
                        if(totalCount > 0)
                        {
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
                            message = "No coupons available.";
                            messageType = AlertMessageType.Error;
                        }
                    }
                    else
                    {
                        message = response.message;
                        messageType = AlertMessageType.Error;
                    }
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
