using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace API.Models.v1
{
    public partial class Login : CryWolf
    {
        Auth auth = new Auth();

        new public static string URL = $"{v1.Auth.URL}login/";

        
        public IRestResponse PatchLogin(string jurisdiction, string username, string password, string loginType)
        {
            string token = auth.GetAuthToken(jurisdiction);
            LoginBodyPatch loginBodyPatch = new LoginBodyPatch(username, password);

            RestClient restClient = new RestClient($"{URL}{loginType}/");

            RestRequest restRequest = new RestRequest(Method.PATCH)
            {
                RequestFormat = DataFormat.Json
            };
            
            restRequest.AddJsonBody(loginBodyPatch);
            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }

        public IRestResponse PatchCitizenLogin(string jurisdiction, string username, string password)
        {
            return PatchLogin(jurisdiction, username, password, "citizen");
        }

        public IRestResponse PatchAlarmCompanyLogin(string jurisdiction, string username, string password)
        {
            return PatchLogin(jurisdiction, username, password, "alarmCompany");
        }

        public IRestResponse PostLogin(string jurisdiction, string username, string password, string allowLoginByInvoiceNumber, string loginType)
        {
            string xapikey = CryWolfUtil.GetAgencyKey(jurisdiction);

            RestClient restClient = new RestClient($"{URL}{loginType}/");

            RestRequest restRequest = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddParameter("jurisdiction", jurisdiction);
            restRequest.AddParameter("userName", username);
            restRequest.AddParameter("password", password);
            restRequest.AddParameter("allowLoginByInvoiceNumber", allowLoginByInvoiceNumber);
            restRequest.AddHeader("x-api-key", xapikey);

            return restClient.Execute(restRequest);
        }


        public IRestResponse PostCitizenLogin(string jurisdiction, string username, string password, string allowLoginByInvoiceNumber)
        {
            return PostLogin(jurisdiction, username, password, allowLoginByInvoiceNumber, "citizen");
        }


        public IRestResponse PostAlarmCompanyLogin(string jurisdiction, string username, string password, string allowLoginByInvoiceNumber)
        {
            return PostLogin(jurisdiction, username, password, allowLoginByInvoiceNumber, "alarmCompany");
        }
    }
}
