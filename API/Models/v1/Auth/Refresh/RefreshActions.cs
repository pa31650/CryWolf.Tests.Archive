using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using API.Models.v1;
using Utils;

namespace API.Models.v1
{
    public partial class Refresh : CryWolf
    {
        new private readonly string URL = $"{v1.Auth.URL}refresh";

        public IRestResponse PostAuthRefreshToken(string jurisdiction,string token, string refreshToken)
        {
            IRestResponse restResponse;
            string xapikey = string.Empty;
            xapikey = CryWolfUtil.GetAgencyKey(jurisdiction);
            
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.POST);

            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddHeader("token", token);
            restRequest.AddHeader("refreshToken", refreshToken);
            restRequest.AddHeader("x-api-key", xapikey);

            return restResponse = restClient.Execute(restRequest);
        }

        public string GetAuthRefreshToken(string jurisdiction,string token,string refreshToken,out string newRefreshToken)
        {
            IRestResponse restResponse = PostAuthRefreshToken(jurisdiction, token, refreshToken);

            var Refresh = JsonConvert.DeserializeObject<Refresh>(restResponse.Content);

            newRefreshToken = Refresh.refreshToken;

            return Refresh.token;
        }
    }
}
