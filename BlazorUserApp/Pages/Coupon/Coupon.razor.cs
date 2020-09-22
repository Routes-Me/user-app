using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using BlazorUserApp.Models;
using BlazorUserApp.Models.DbModels;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.JSInterop;

namespace BlazorUserApp.Pages.Coupon
{
    public partial class Coupon
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        int counter = 0;
        string spinner = "";
        List<Promotion> model = new List<Promotion>();
        string message = string.Empty;
        AlertMessageType messageType = AlertMessageType.Success;
    }
}
