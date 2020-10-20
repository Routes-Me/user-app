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
        string spinner = string.Empty, UserName = string.Empty, message = string.Empty, redemptionId = string.Empty, officerId = string.Empty, token = string.Empty;
        List<RedeemDetailModel> model = new List<RedeemDetailModel>();
        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var userState = authenticationState.Result;
                UserName = userState.User.FindFirst("Name").Value;
                var uri = navManager.ToAbsoluteUri(navManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    redemptionId = _id;
                }
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("officer", out var _officerid))
                {
                    officerId = _officerid;
                }
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
