using RoutesApp.Models;
using RoutesApp.Models.DbModels;
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

namespace RoutesApp.Pages.Redeem
{
    public partial class RedeemCoupon
    {
        string spinner = string.Empty, UserName = string.Empty, message = string.Empty, couponId = string.Empty, expiredCode = string.Empty, token = string.Empty;
        List<CouponDetailData> couponListModel = new List<CouponDetailData>();
        Redemption model = new Redemption();

        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = navManager.ToAbsoluteUri(navManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    couponId = _id;
                }
                var userState = authenticationState.Result;
                UserName = userState.User.FindFirst("Name").Value;
                string Token = userState.User.FindFirst("Token").Value;

                if (string.IsNullOrEmpty(Http.DefaultRequestHeaders.Authorization.ToString()))
                {
                    if (!string.IsNullOrEmpty(Token))
                        Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                }

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
                            couponModel.Promotion = response.included.promotions.Where(x => x.PromotionId == item.promotionId).FirstOrDefault();
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
                spinner = string.Empty;
                var userState = authenticationState.Result;
                bool isOfficer = Convert.ToBoolean(userState.User.FindFirst("isOfficer").Value);

                string OfficerId = string.Empty;
                string InstitutionId = string.Empty;
                if (isOfficer == true)
                {
                    OfficerId = userState.User.FindFirst("OfficerId").Value;
                    InstitutionId = userState.User.FindFirst("InstitutionId").Value;
                }
                Redemption redemption = new Redemption()
                {
                    CouponId = couponId,
                    OfficerId = OfficerId,
                    InstitutionId = InstitutionId,
                    Pin = model.Pin
                };
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
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }
        }
    }
}
