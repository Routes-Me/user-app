using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesApp.Pages.Redeem
{
    public partial class Search
    {
        #pragma warning disable
        public string SearchTerm { get; set; }
        [Parameter]
        public EventCallback<string> OnSearchChanged { get; set; }
        #pragma warning restore
    }
}
