using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUserApp.Models.CommonModels
{
    public class AppSettings
    {
        public string IV { get; set; }
        public string PASSWORD { get; set; }
        public string UserEndpointUrl { get; set; }
    }
}
