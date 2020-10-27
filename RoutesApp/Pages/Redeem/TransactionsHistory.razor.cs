using RoutesApp.Models;
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
    public partial class TransactionsHistory
    {
        string spinner = string.Empty, message = string.Empty, officerId = string.Empty, token = string.Empty, Name = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        List<RedeemHistory> model = new List<RedeemHistory>();
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
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

                var result = await Http.GetAsync("/api/coupons/redeem?officerId=" + officerId + "&include=coupon");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RedemptionGetResponse>(responseData);
                if (response.status == true)
                {
                    if(response.data.Count > 0)
                    {
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
