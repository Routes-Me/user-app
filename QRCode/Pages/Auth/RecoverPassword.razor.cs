﻿using QRCode.Models;
using QRCode.Models.DbModels;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace QRCode.Pages.Auth
{
    public partial class RecoverPassword
    {
        ResetPassword model = new ResetPassword();
        string message = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
        string displaySpinner = "d-none";

        public async Task ResetPassword()
        {
            try
            {
               
                string Url = "api/forgotpassword?email=" + model.Email;
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
                displaySpinner = "d-none";
            }
            catch (Exception ex)
            {
                message = "Something went wrong!! Please try again.";
                messageType = AlertMessageType.Error;
            }
        }
    }
}
