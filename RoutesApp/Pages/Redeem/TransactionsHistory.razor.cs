using RoutesApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using RoutesApp.Models.DbModels;
using System.Net.Http;
using System.Text;

namespace RoutesApp.Pages.Redeem
{
    public partial class TransactionsHistory
    {
        #pragma warning disable
        string spinner = string.Empty, message = string.Empty, officerId = string.Empty, token = string.Empty, Name = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        List<RedeemHistory> model = new List<RedeemHistory>();
        public string SearchTerm { get; set; }
        [Parameter]
        public EventCallback<string> OnSearchChanged { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        bool IsSearch = false, IsAccess = true;
        PromotionCode promotionModel = new PromotionCode();
        string modelSpinner = "d-none";
        bool isError = false;
        string displayCoupon = string.Empty, displayEmpty = "d-none";

#pragma warning restore
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    officerId = _id;
                }
                var userState = authenticationState.Result;
                string isOfficer = userState.User.FindFirst("isOfficer").Value;
                string Token = userState.User.FindFirst("AccessToken").Value;
                Http.DefaultRequestHeaders.Clear();
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                if (isOfficer == "True")
                {
                    string tokenInstitutionId = userState.User.FindFirst("InstitutionId").Value;
                    if (!string.IsNullOrEmpty(officerId))
                    {
                        var officerResult = await Http.GetAsync("/api/officers/" + officerId + "");
                        var officerResponseData = await officerResult.Content.ReadAsStringAsync();
                        var officerResponse = JsonConvert.DeserializeObject<OfficersResponse>(officerResponseData);
                        if (officerResponse.status == true)
                        {
                            string officerInstitutionId = string.Empty;
                            foreach (var item in officerResponse.data)
                            {
                                officerInstitutionId = item.InstitutionId;
                            }
                            if (tokenInstitutionId != officerInstitutionId)
                            {
                                IsAccess = false;
                                message = "You don't have permission to access this page.";
                                messageType = AlertMessageType.Error;
                            }
                        }
                        else
                        {
                            if (officerResponse.message.Contains("Authentication failed."))
                            {
                                navigationManager.NavigateTo("/");
                            }
                            else
                            {
                                IsAccess = false;
                                message = officerResponse.message;
                                messageType = AlertMessageType.Error;
                            }
                        }
                    }
                }
                else
                {
                    IsAccess = false;
                    message = "You don't have permission to access this page.";
                    messageType = AlertMessageType.Error;
                }

                if (IsAccess == true)
                {
                    string searchTerm = string.Empty;
                    await GetRedeemHistory(searchTerm);
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

        private async Task SearchChanged(string searchTerm)
        {
            try
            {
                spinner = string.Empty;
                await GetRedeemHistory(searchTerm);
                spinner = "d-none";
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }

        }
        private async Task GetRedeemHistory(string searchTerm)
        {
            try
            {
                var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    officerId = _id;
                }
                var userState = authenticationState.Result;
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

                RedemptionGetResponse response = new RedemptionGetResponse();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    IsSearch = false;
                    var result = await Http.GetAsync("/api/coupons/redeem?officerId=" + officerId + "&include=coupon");
                    var responseData = await result.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<RedemptionGetResponse>(responseData);
                }
                else
                {
                    IsSearch = true;
                    var result = await Http.GetAsync("/api/coupons/redeem/search?officerId=" + officerId + "&q=" + searchTerm + "&include=coupon");
                    var responseData = await result.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<RedemptionGetResponse>(responseData);
                }
                model = new List<RedeemHistory>();
                if (response.status == true)
                {
                    if (response.data != null && response.data.Count > 0)
                    {
                        message = string.Empty;
                        foreach (var item in response.data)
                        {
                            RedeemHistory redeemModel = new RedeemHistory();
                            redeemModel.RedemptionId = item.RedemptionId;
                            redeemModel.CreatedAt = item.CreatedAt;
                            redeemModel.OfficerId = item.OfficerId;
                            redeemModel.Title = response.included.coupons.Where(x => x.couponId == item.CouponId).Select(x => x.Promotion.Title).FirstOrDefault();
                            redeemModel.Subtitle = response.included.coupons.Where(x => x.couponId == item.CouponId).Select(x => x.Promotion.Subtitle).FirstOrDefault();
                            redeemModel.LogoUrl = response.included.coupons.Where(x => x.couponId == item.CouponId).Select(x => x.Promotion.LogoUrl).FirstOrDefault();
                            model.Add(redeemModel);
                        }
                    }
                    else
                    {
                        displayCoupon = "d-none";
                        displayEmpty = string.Empty;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SubmitPromotionCode()
        {
            try
            {
                if (!string.IsNullOrEmpty(promotionModel.PromotionId))
                {
                    modelSpinner = "";
                    var userState = authenticationState.Result;
                    string userId = userState.User.FindFirst("UserId").Value;
                    string Token = userState.User.FindFirst("AccessToken").Value;
                    Http.DefaultRequestHeaders.Clear();
                    Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                    var promotionResult = await Http.GetAsync("/api/promotions/" + promotionModel.PromotionId);
                    var promotionResponseData = await promotionResult.Content.ReadAsStringAsync();
                    var promotionRresponse = JsonConvert.DeserializeObject<PromotionsGetResponse>(promotionResponseData);
                    if (promotionRresponse.status == true)
                    {
                        if (promotionRresponse.data.Count == 0)
                        {
                            message = "Enter valid promotion.";
                            messageType = AlertMessageType.Error;
                            isError = true;
                        }
                    }
                    else
                    {
                        if (promotionRresponse.message.Contains("Authentication failed."))
                        {
                            navigationManager.NavigateTo("/");
                        }
                        else
                        {
                            message = "Enter valid promotion.";
                            messageType = AlertMessageType.Error;
                            isError = true;
                        }
                    }

                    if (isError == false)
                    {
                        Models.DbModels.Coupon coupon = new Models.DbModels.Coupon()
                        {
                            promotionId = promotionModel.PromotionId,
                            userId = userId
                        };
                        Http.DefaultRequestHeaders.Clear();
                        Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                        var serializedValue = JsonConvert.SerializeObject(coupon);
                        var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                        var result = await Http.PostAsync("/api/coupons", stringContent).ConfigureAwait(false);
                        var responseData = await result.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<Response>(responseData);
                        if (response.status == true)
                        {
                            navigationManager.NavigateTo("/promotion-details?id=" + promotionModel.PromotionId + "");
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

                    }
                }
                else
                {
                    message = "Promotions not found.";
                    messageType = AlertMessageType.Error;
                }
                modelSpinner = "d-none";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Input string was not in a correct format."))
                {
                    message = "Enter valid promotion.";
                }
                else
                {
                    message = "Something went wrong!! Please try again. ";
                }
                messageType = AlertMessageType.Error;
                modelSpinner = "d-none";
            }
        }
    }
}
