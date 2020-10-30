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
        [Parameter]
        public string CouponId { get; set; }

        string spinner = string.Empty, UserName = string.Empty, message = string.Empty, expiredCode = string.Empty, token = string.Empty, officerId = string.Empty;
        List<CouponDetailData> couponListModel = new List<CouponDetailData>();
        Redemption model = new Redemption();

        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("OfficerId", out var _id))
                {
                    officerId = _id;
                }
                var userState = authenticationState.Result;
                string isOfficer = userState.User.FindFirst("isOfficer").Value;
                if (isOfficer == "true")
                {
                    string tokenOfficerId = userState.User.FindFirst("OfficerId").Value;
                    if (tokenOfficerId != officerId)
                    {
                        message = "You don't have permission to access this page.";
                        messageType = AlertMessageType.Error;
                    }
                }
                else
                {
                    message = "You don't have permission to access this page.";
                    messageType = AlertMessageType.Error;
                }

                UserName = userState.User.FindFirst("Name").Value;
                string Token = userState.User.FindFirst("AccessToken").Value;
                Http.DefaultRequestHeaders.Clear();
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                var result = await Http.GetAsync("/api/coupons/" + CouponId + "?include=promotion,user");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CouponResponse>(responseData);
                if (response != null && response.status == true)
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
                    CouponId = CouponId,
                    OfficerId = OfficerId,
                    InstitutionId = InstitutionId,
                    Pin = model.Pin
                };
                string Token = userState.User.FindFirst("AccessToken").Value;
                Http.DefaultRequestHeaders.Clear();
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                var serializedValue = JsonConvert.SerializeObject(redemption);
                var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                var result = await Http.PostAsync("/api/coupons/redeem", stringContent).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RedemptionResponse>(responseData);
                if (response.status == true)
                {
                    navigationManager.NavigateTo("/success?id=" + response.RedemptionId + "");
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
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }
        }
    }
}
