using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class Case : CryWolf
    {
        public static string CaseURL = $"{CryWolf.URLv1}case/";

        public IRestResponse GetCaseById(string token, string caseNumber, string fields)
        {
            RestClient restClient = new RestClient(CaseURL + caseNumber);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddQueryParameter("fields", fields);
            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }

        public IRestResponse GetCasesByAccount(string token, string accountNumber, string fields, int skip = 0, int take = 5)
        {
            RestClient restClient = new RestClient($"{v1.Account.URL}/{accountNumber}/cases");

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddQueryParameter("skip", skip.ToString());
            restRequest.AddQueryParameter("take", take.ToString());

            restRequest.AddQueryParameter("fields", fields);
            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }
    }
}
