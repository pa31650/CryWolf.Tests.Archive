using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace API.Models.v1
{
    partial class PasswordPolicy : CryWolf
    {
        new public static string URL = $"{v1.Auth.URL}passwordPolicy";

        public IRestResponse GetPasswordPolicy()
        {
            RestClient restClient = new RestClient(URL);

            ReportBuilder.ArrayBuilder($"URL: [{URL}] used for GetPasswordPolicy", true, "URL/Environment Reporting");

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            return restClient.Execute(restRequest);
        }
    }
}
