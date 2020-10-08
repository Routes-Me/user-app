using BlazorUserApp.Models;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Pages.Redeem
{
    public partial class RedeemSuccess
    {
        string spinner = string.Empty, message = string.Empty, trasactionId = string.Empty, officerId = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var uri = navManager.ToAbsoluteUri(navManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var _id))
                {
                    trasactionId = _id;
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

        public void Redirect()
        {
            spinner = string.Empty;
            officerId = "2";
            navManager.NavigateTo("/history?id=" + officerId + "");
            spinner = "d-none";
        }
    }
}
