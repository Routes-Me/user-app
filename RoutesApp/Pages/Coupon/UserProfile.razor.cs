using RoutesApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutesApp.Models.DbModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace RoutesApp.Pages.Coupon
{
    public partial class UserProfile
    {
        #pragma warning disable
        string spinner = string.Empty, UserName = string.Empty, UserPhone = string.Empty, UserEmail = string.Empty, message = string.Empty;
        string modelSpinner = "d-none";
        AlertMessageType messageType = AlertMessageType.Success;
        PromotionCode model = new PromotionCode();
        bool IsError = false;

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        #pragma warning disable
        protected override void OnInitialized()
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
