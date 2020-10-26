using RoutesApp.Models;
using RoutesApp.Models.DbModels;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace RoutesApp.Pages.Auth
{
    public partial class RecoverPassword
    {
        ResetPassword model = new ResetPassword();
        string message = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        string spinner = "d-none";

        public async Task ResetPassword()
        {
            try
            {
                spinner ="";
                string Url = "/api/account/password?email=" + model.Email;
                var result = await Http.PostAsync(Url, null).ConfigureAwait(false);
                var responseData = await result.Content.ReadAsStringAsync();
                Response response = new Response();
                response = JsonSerializer.Deserialize<Response>(responseData);
                if (response.status == true)
                {
                    message = response.message;
                    messageType = AlertMessageType.Success;
                }
                else
                {
                    message = response.message;
                    messageType = AlertMessageType.Error;
                }
            }
            catch (Exception ex)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
            spinner = "d-none";
        }
    }
}
