using Encryption;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RoutesApp.Models;
using RoutesApp.Models.DbModels;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RoutesApp.Pages.Auth
{
    public partial class ResetPassword
    {
        SetNewPassword model = new SetNewPassword();
        string message = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        string spinner = "d-none";
        [Parameter]
        public string UserId { get; set; }

        public async Task CreateNewPassword()
        {
            try
            {
                EncryptionClass encryption = new EncryptionClass();
                string IVForAndroid = "Qz-N!p#ATb9_2MkL";
                string KeyForAndroid = "ledV\\K\"zRaNF]WXki,RMtLLZ{Cyr_1";
                string IVForDashboard = "7w'}DkAkO!A&mLyL";
                string KeyForDashboard = "Wf6cXM10cj_7B)V,";
                string EncryptedPassword = string.Empty;

                if (encryption.IndexOfBSign(model.Password) != -1)
                {
                    EncryptedPassword = await encryption.EncryptAndEncode(model.Password, IVForDashboard, KeyForDashboard);
                }
                else
                {
                    EncryptedPassword = await encryption.EncryptAndEncode(model.Password, IVForAndroid, KeyForAndroid);
                }

                ChangePasswordModel changePasswordModel = new ChangePasswordModel()
                {
                    UserId = UserId,
                    NewPassword = EncryptedPassword
                };
                spinner = "";
                var serializedValue = JsonConvert.SerializeObject(changePasswordModel);
                var stringContent = new StringContent(serializedValue, Encoding.UTF8, "application/json");
                var result = await Http.PutAsync("/api/account/password", stringContent).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                Response response = new Response();
                response = JsonConvert.DeserializeObject<Response>(responseData);
                if (response.status == true)
                {
                    message = response.message;
                    messageType = AlertMessageType.Success;
                    navigationManager.NavigateTo("/");
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
    }
}
