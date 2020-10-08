﻿using BlazorUserApp.Models;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Pages.Redeem
{
    public partial class TransactionsHistory
    {
        string spinner = string.Empty, message = string.Empty, officerId = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        List<RedeemHistory> model = new List<RedeemHistory>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = navManager.ToAbsoluteUri(navManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    officerId = _id;
                }
                Http.BaseAddress = null;
                Http.BaseAddress = new Uri("http://localhost:63746");
                var result = await Http.GetAsync("/api/coupons/redeem?officerId=" + officerId + "&include=coupon");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RedemptionGetResponse>(responseData);
                if (response.status == true)
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