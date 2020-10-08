using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorUserApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorUserApp
{
    public class LocalAuthenticationStateProvider : AuthenticationStateProvider
    {

        private readonly ILocalStorageService _storageService;

        public LocalAuthenticationStateProvider(ILocalStorageService storageService)
        {
            _storageService = storageService;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (await _storageService.ContainKeyAsync("User"))
            {
                var userInfo = await _storageService.GetItemAsync<LocalUserInfo>("User");

                var claims = new[]
                {
                    new Claim("AccessToken", userInfo.Token),
                    new Claim("Name", userInfo.tokenPayload.Name),
                    new Claim("Email", userInfo.tokenPayload.Email),
                    new Claim("Phone", userInfo.tokenPayload.PhoneNumber),
                    new Claim("UserId", userInfo.tokenPayload.UserId),
                    new Claim("isOfficer", userInfo.isOfficer.ToString()),
                };

                var identity = new ClaimsIdentity(claims, "BearerToken");
                var user = new ClaimsPrincipal(identity);
                var state = new AuthenticationState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }

            return new AuthenticationState(new ClaimsPrincipal());
        }

        public async Task LogoutAsync()
        {
            await _storageService.RemoveItemAsync("User");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
        }
    }
}
