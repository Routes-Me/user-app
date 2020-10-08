using BlazorUserApp.Models;
using BlazorUserApp.Models.DbModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlazorUserApp.Pages.Redeem
{
    public partial class RedeemCoupon
    {
        string spinner = string.Empty, UserName = string.Empty, message = string.Empty, couponId = string.Empty, expiredCode = string.Empty;
        List<CouponDetailData> couponListModel = new List<CouponDetailData>();
        Redemption model = new Redemption();

        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var userState = authenticationState.Result;
                UserName = userState.User.FindFirst("Name").Value;
                Http.BaseAddress = null;
                Http.BaseAddress = new Uri("http://localhost:63746");
                var uri = navManager.ToAbsoluteUri(navManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    couponId = _id;
                }
                couponId = "3";
                var result = await Http.GetAsync("/api/coupons/" + couponId + "?include=promotion,user");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CouponResponse>(responseData);
                if (response.status == true)
                {
                    if (response.data.Count > 0)
                    {
                        foreach (var item in response.data)
                        {
                            CouponDetailData couponModel = new CouponDetailData();
                            couponModel.Id = item.couponId;
                            couponModel.Promotion = response.included.promotions.Where(x => x.PromotionId == Convert.ToInt32(item.promotionId)).FirstOrDefault();
                            couponModel.User = response.included.users.Where(x => x.UserId == item.userId).FirstOrDefault();
                            couponListModel.Add(couponModel);
                        }
                    }
                    else
                    {
                        message = "No coupon found for redemption. Please try again.";
                        messageType = AlertMessageType.Error;
                    }
                }
                else
                {
                    message = response.message;
                    messageType = AlertMessageType.Error;
                }
                spinner = "d-none";
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }
        }

        public async Task InsertRedemptions()
        {
            try
            {
                var userState = authenticationState.Result;
                bool isOfficer = Convert.ToBoolean(userState.User.FindFirst("isOfficer").Value);
                string OfficerId = string.Empty;
                if (isOfficer == false)
                {
                    //Http.BaseAddress = "http://localhost:62574";
                    Http.BaseAddress = new Uri("http://localhost:62574");
                    string UserId = userState.User.FindFirst("UserId").Value;
                    var officerResult = await Http.GetAsync("/api/officers?userId=" + UserId + "");
                    var officerResponseData = await officerResult.Content.ReadAsStringAsync();
                    var officerResponse = JsonConvert.DeserializeObject<OfficersResponse>(officerResponseData);
                    if (officerResponse.status == true)
                    {
                        if (officerResponse.data.Count > 0)
                        {
                            foreach (var item in officerResponse.data)
                            {
                                OfficerId = item.OfficerId;
                            }
                        }
                    }
                }

                spinner = string.Empty;
                Redemption redemption = new Redemption()
                {
                    CouponId = couponId,
                    OfficerId = OfficerId,
                    Pin = model.Pin
                };
                Http.BaseAddress = null;
                Http.BaseAddress = new Uri("http://localhost:63746");
                var serializedValue = JsonConvert.SerializeObject(redemption);
                var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                var result = await Http.PostAsync("/api/coupons/redeem", stringContent).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RedemptionResponse>(responseData);
                if (response.status == true)
                {
                    navManager.NavigateTo("/success?id=" + response.RedemptionId + "");
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
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }
        }
    }
}
