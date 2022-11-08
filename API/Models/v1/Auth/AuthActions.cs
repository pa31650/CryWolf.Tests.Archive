using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using API.Models.v1;
using Utils;

namespace API.Models.v1
{
    public partial class Auth : CryWolf
    {
        new public static string URL = $"{URLv1}auth/";
        
        public IRestResponse PostAuthToken(string jurisdiction, string username, string password,bool key = true)
        {
            string xapikey = CryWolfUtil.GetAgencyKey(jurisdiction);
            
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddParameter("jurisdiction", jurisdiction);
            restRequest.AddParameter("userName", username);
            restRequest.AddParameter("password", password);
            restRequest.AddHeader("x-api-key", xapikey);

            return restClient.Execute(restRequest);
        }

        public string GetAuthToken(string jurisdiction,string username,string password,out string refreshToken)
        {
            IRestResponse restResponse = PostAuthToken(jurisdiction, username, password);

            var Auth = JsonConvert.DeserializeObject<Auth>(restResponse.Content);

            refreshToken = Auth.refreshToken;

            return Auth.token;
        }
        public string GetAuthToken(string jurisdiction)
        {
            return GetAuthToken(jurisdiction, "admin", CommonTestSettings.DesktopPw, out string refreshToken);
        }
    }
}
