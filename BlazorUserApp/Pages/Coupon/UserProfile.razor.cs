using BlazorUserApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Pages.Coupon
{
    public partial class UserProfile
    {
        string spinner = string.Empty, UserName = string.Empty, UserPhone = string.Empty, UserEmail = string.Empty, message = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var userState = authenticationState.Result;
                UserName = userState.User.FindFirst("Name").Value;
                UserPhone = userState.User.FindFirst("Phone").Value;
                UserEmail = userState.User.FindFirst("Email").Value;
                spinner = "d-none";
            }
            catch (Exception)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
                spinner = "d-none";
            }
        }

        public async Task Logout()
        {
            var localStateProvider = (LocalAuthenticationStateProvider)authenticationStateProvider;
            await localStateProvider.LogoutAsync();
            navigationManager.NavigateTo("/");
        }
    }
}
