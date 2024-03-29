﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using RoutesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using RoutesApp.Models.DbModels;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace RoutesApp.Pages.Coupon
{
    public partial class Coupon
    {
#pragma warning disable
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        int counter = 0, totalCount = 0;
        string spinner = "", message = string.Empty, Name = string.Empty, userId = string.Empty, tokenInstitutionId = string.Empty, OfficerId = string.Empty, popupMessage = string.Empty;
        List<CouponListData> model = new List<CouponListData>();
        AlertMessageType messageType = AlertMessageType.Success;
        AlertMessageTypePopup popupMessageType = AlertMessageTypePopup.Success;
        PromotionCode promotionModel = new PromotionCode();
        string modelSpinner = "d-none";
        bool isError = false;
        string displayCoupon = string.Empty, displayEmpty = "d-none", localStorageKey = "Coupons", localStorageKeyCount = "CouponsCount", localStorageUserId = "CouponsUserId", isNewCoupon = string.Empty;
#pragma warning restore
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("new", out var _isNew))
                {
                    isNewCoupon = _isNew;
                }

                var userState = authenticationState.Result;
                userId = userState.User.FindFirst("UserId").Value;
                tokenInstitutionId = userState.User.FindFirst("InstitutionId").Value;
                string UserName = userState.User.FindFirst("Name").Value;
                string Token = userState.User.FindFirst("AccessToken").Value;
                Http.DefaultRequestHeaders.Clear();
                Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                string[] arrUserName = UserName.Split(' ');
                if (arrUserName.Length > 1)
                {
                    Name = arrUserName[0].Substring(0, 1) + arrUserName[1].Substring(0, 1);
                }
                else
                {
                    Name = UserName.Substring(0, 2);
                }
                string localStorageUser = await localStore.GetItemAsync<string>(localStorageUserId);
                if (localStorageUser != userId || string.IsNullOrEmpty(isNewCoupon) || isNewCoupon.ToLower() == "true")
                {
                    await localStore.RemoveItemAsync(localStorageKey);
                    await localStore.RemoveItemAsync(localStorageKeyCount);
                    await localStore.RemoveItemAsync(localStorageUserId);
                }
                var couponLocalStorage = await localStore.GetItemAsync<List<CouponListData>>(localStorageKey);
                if (couponLocalStorage == null)
                {
                    var result = await Http.GetAsync("/api/coupons?userId=" + userId + "&include=promotion");
                    var responseData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<CouponResponse>(responseData);
                    if (response.status == true)
                    {
                        totalCount = response.data.Count();
                        if (totalCount > 0)
                        {
                            message = string.Empty;
                            foreach (var item in response.data.OrderByDescending(s => s.createdAt).Take(4))
                            {
                                CouponListData couponList = new CouponListData();
                                couponList.Id = item.couponId;
                                couponList.Promotion = response.included.promotions.Where(x => x.PromotionId == item.promotionId).FirstOrDefault();
                                couponList.QrCodeImage = await JSRuntime.InvokeAsync<string>("GenerateQRCode", "http://app.routesme.com/redeem/" + item.couponId + "");
                                couponList.Count = totalCount;
                                model.Add(couponList);
                            }
                            await localStore.SetItemAsync(localStorageKey, model);
                            await localStore.SetItemAsync(localStorageKeyCount, totalCount);
                            await localStore.SetItemAsync(localStorageUserId, userId);
                            displayCoupon = string.Empty;
                            displayEmpty = "d-none";
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
                }
                else
                {
                    model = couponLocalStorage;
                    totalCount = await localStore.GetItemAsync<int>(localStorageKeyCount);
                }

                spinner = "d-none";
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again. ";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
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
                            popupMessage = "Enter valid promotion.";
                            popupMessageType = AlertMessageTypePopup.Error;
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
                            popupMessage = "Enter valid promotion.";
                            popupMessageType = AlertMessageTypePopup.Error;
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
                                popupMessage = response.message;
                                popupMessageType = AlertMessageTypePopup.Error;
                            }
                        }

                    }
                }
                else
                {
                    popupMessage = "Promotions not found.";
                    popupMessageType = AlertMessageTypePopup.Error;
                }
                modelSpinner = "d-none";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Input string was not in a correct format."))
                {
                    popupMessage = "Enter valid promotion.";
                }
                else
                {
                    popupMessage = "Something went wrong!! Please try again. ";
                }
                popupMessageType = AlertMessageTypePopup.Error;
                modelSpinner = "d-none";
            }
        }
    }
}
