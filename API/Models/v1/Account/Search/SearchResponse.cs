using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class Search
    {
        public static List<Account> FromJson(string json) => JsonConvert.DeserializeObject<List<Account>>(json, API.Models.v1.Converter.Settings);
    }
}
