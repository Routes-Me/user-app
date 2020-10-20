using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using RoutesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RoutesApp
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
                    new Claim("Name", userInfo.loginUser.Name),
                    new Claim("Email", userInfo.loginUser.Email),
                    new Claim("Phone", userInfo.loginUser.PhoneNumber),
                    new Claim("UserId", userInfo.loginUser.UserId),
                    new Claim("isOfficer", userInfo.loginUser.isOfficer.ToString()),
                    new Claim("OfficerId", userInfo.loginUser.OfficerId.ToString()),
                      new Claim("InstitutionId", userInfo.loginUser.InstitutionId.ToString()),
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
