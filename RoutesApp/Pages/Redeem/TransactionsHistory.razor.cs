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

namespace RoutesApp.Pages.Redeem
{
    public partial class TransactionsHistory
    {
        string spinner = string.Empty, message = string.Empty, officerId = string.Empty, token = string.Empty, Name = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        List<RedeemHistory> model = new List<RedeemHistory>();
        public string SearchTerm { get; set; }
        [Parameter]
        public EventCallback<string> OnSearchChanged { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        bool IsSearch = false, IsAccess = true;
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
                        message = "No redeem coupons available.";
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
