using RoutesApp.Models;
using RoutesApp.Models.DbModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Pages.Redeem
{
    public partial class TransactionDetails
    {
        #pragma warning disable
        string spinner = string.Empty, UserName = string.Empty, message = string.Empty, redemptionId = string.Empty, officerId = string.Empty, token = string.Empty;
        List<RedeemDetailModel> model = new List<RedeemDetailModel>();
        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        bool IsAccess = true;
        #pragma warning restore

        protected override async Task OnInitializedAsync()
        {
            try
            {
                spinner = string.Empty;
                var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    redemptionId = _id;
                }
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("officer", out var _officerid))
                {
                    officerId = _officerid;
                }
                var userState = authenticationState.Result;
                string isOfficer = userState.User.FindFirst("isOfficer").Value;
                UserName = userState.User.FindFirst("Name").Value;
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
                    var result = await Http.GetAsync("/api/coupons/redeem/" + redemptionId + "?include=coupon,user");
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<RedemptionGetResponse>(responseData);
                    if (response.status == true)
                    {
                        foreach (var item in response.data)
                        {
                            RedeemDetailModel redeemModel = new RedeemDetailModel();
                            redeemModel.RedemptionId = item.RedemptionId;
                            redeemModel.CreatedAt = item.CreatedAt;
                            redeemModel.CouponId = item.CouponId;
                            redeemModel.coupons = response.included.coupons.Where(x => x.couponId == item.CouponId).FirstOrDefault();
                            redeemModel.Users = response.included.users.Where(x => x.UserId == redeemModel.coupons.userId).FirstOrDefault();
                            model.Add(redeemModel);
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
