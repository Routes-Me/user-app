﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using RoutesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Pages.Promotions
{
    public partial class Links
    {
        [Parameter]
        public string PromotionId { get; set; }
        string spinner = string.Empty, message = string.Empty, token = string.Empty;

        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                spinner = string.Empty;
                var result = await Http.GetAsync("/api/links?promotionId=" + PromotionId + "");
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<LinkResponse>(responseData);
                if (response != null && response.status == true)
                {
                    if (response.data.Count > 0)
                    {
                        string deviceName = await JSRuntime.InvokeAsync<string>("GetDevice");
                        foreach (var item in response.data)
                        {
                            if (deviceName == "web")
                            {
                                if (string.IsNullOrEmpty(item.Web))
                                {
                                    navigationManager.NavigateTo("/promotions/" + PromotionId + "");
                                }
                                else
                                {
                                    navigationManager.NavigateTo(item.Web);
                                }
                            }
                            else if (deviceName == "ios")
                            {
                                if (string.IsNullOrEmpty(item.Ios))
                                {
                                    navigationManager.NavigateTo("/promotions/" + PromotionId + "");
                                }
                                else
                                {
                                    navigationManager.NavigateTo(item.Ios);
                                }
                            }
                            else if (deviceName == "android")
                            {
                                if (string.IsNullOrEmpty(item.Android))
                                {
                                    navigationManager.NavigateTo("/promotions/" + PromotionId + "");
                                }
                                else
                                {
                                    navigationManager.NavigateTo(item.Android);
                                }
                            }
                        }
                    }
                    else
                    {
                        navigationManager.NavigateTo("/promotions/" + PromotionId + "");
                    }
                }
                else
                {
                    navigationManager.NavigateTo("/promotions/" + PromotionId + "");
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
