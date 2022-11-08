using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace API.Models.v1
{
    public partial class Settings : CryWolf
    {
        new private readonly string URL = $"{URLv1}payment/settings";

        public IRestResponse GetPaymentSettings(string token,string agency)
        {
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.GET);
            restRequest.AddQueryParameter("agency", agency);
            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }
    }
}
