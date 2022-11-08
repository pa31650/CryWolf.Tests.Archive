using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class Invoices : Invoice
    {
        public static List<Invoice> FromJson(string json) => JsonConvert.DeserializeObject<List<Invoice>>(json, API.Models.v1.Converter.Settings);
    }
}
