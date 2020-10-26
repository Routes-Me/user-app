using RoutesApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Pages.Redeem
{
    public partial class RedeemSuccess
    {
        string spinner = string.Empty, message = string.Empty, trasactionId = string.Empty, officerId = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override void OnInitialized()
        {
            try
            {
                var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
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
            var userState = authenticationState.Result;
            officerId = userState.User.FindFirst("OfficerId").Value;
            navigationManager.NavigateTo("/history?id=" + officerId + "");
            spinner = "d-none";
        }
    }
}
