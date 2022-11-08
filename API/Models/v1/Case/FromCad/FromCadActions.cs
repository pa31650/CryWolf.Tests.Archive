using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class FromCad : CryWolf
    {
        public static string FromCadURL = $"{URLv1}case/fromCad/";

        public IRestResponse GetCasesFromCad(string token, string fields, string caseNumber, int skip = 0, int take = 10)
        {
            RestClient restClient = new RestClient(FromCadURL);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddQueryParameter("skip", skip.ToString());
            restRequest.AddQueryParameter("take", take.ToString());
            restRequest.AddQueryParameter("fields", fields);
            restRequest.AddQueryParameter("caseNumber", caseNumber);
            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }

        public IRestResponse GetCaseFromCadByPath(string token, string fields, string caseNumber)
        {
            RestClient restClient = new RestClient(FromCadURL + caseNumber);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddQueryParameter("fields", fields);
            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }

        public IRestResponse PutCasesFromCad(string token, List<FromCad> calls)
        {
            RestClient restClient = new RestClient(FromCadURL);

            RestRequest restRequest = new RestRequest(Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddJsonBody(calls);

            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }

        public IRestResponse PutCasesFromCad(string token, FromCad call)
        {
            List<FromCad> calls = new List<FromCad>();
            calls.Add(call);

            return PutCasesFromCad(token, calls);
        }

        public IRestResponse UpdateCasesFromCad(string token, FromCad existingCall)
        {
            RestClient restClient = new RestClient(FromCadURL);

            RestRequest restRequest = new RestRequest(Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddJsonBody(existingCall);

            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }

        public IRestResponse DeleteCaseFromCad(string token, string caseNumber)
        {
            RestClient restClient = new RestClient(FromCadURL + caseNumber);

            RestRequest restRequest = new RestRequest(Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }
    }
}
