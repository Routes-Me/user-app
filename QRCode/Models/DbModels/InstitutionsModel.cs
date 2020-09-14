using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRCode.Models.DbModels
{
    public class InstitutionsModel
    {
        public int InstitutionId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryIso { get; set; }
    }
}
