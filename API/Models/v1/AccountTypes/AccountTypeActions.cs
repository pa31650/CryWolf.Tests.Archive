using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class AccountTypes : CryWolf
    {
        new public static string URL = $"{URLv1}accountTypes/";

        public IRestResponse GetAccountTypes()
        {
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            return restClient.Execute(restRequest);
        }
    }
}
