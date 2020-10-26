using RoutesApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutesApp.Models.DbModels;

namespace RoutesApp.Pages.Promotions
{
    public partial class PromotionDetails
    {
        string spinner = "", promotionId = string.Empty, token = string.Empty, message = string.Empty;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        List<PromotionDetailsData> model = new List<PromotionDetailsData>();
        AlertMessageType messageType = AlertMessageType.Success;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    promotionId = _id;
                }
                var result = await Http.GetAsync("/api/promotions/" + promotionId);
                var responseData = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<PromotionsGetResponse>(responseData);
                if (response.status == true)
                {
                    foreach (var item in response.data)
                    {
                        PromotionDetailsData promotionDetails = new PromotionDetailsData();
                        Promotion promotion = new Promotion();
                        promotion.PromotionId = item.PromotionId;
                        promotion.Title = item.Title;
                        promotion.Subtitle = item.Subtitle;
                        promotion.LogoUrl = item.LogoUrl;
                        promotion.EndAt = item.EndAt;
                        promotion.StartAt = item.StartAt;
                        promotion.UsageLimit = item.UsageLimit;
                        promotion.IsSharable = item.IsSharable;
                        promotionDetails.promotion = promotion;
                        promotionDetails.QrCodeImage = await JSRuntime.InvokeAsync<string>("GenerateQRCode", "https://userapp.routesme.com/promotions/" + item.PromotionId + "");
                        model.Add(promotionDetails);
                    }
                }
                else
                {
                    message = response.message;
                    messageType = AlertMessageType.Error;
                }
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
            spinner = "d-none";
        }

        public void RedirectToCoupons()
        {
            navigationManager.NavigateTo("/coupons");
        }
    }
}
