using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class Account : CryWolf
    {
        new public static string URL = $"{URLv1}account/";

        public IRestResponse GetAccount(string token, string id, string fields = "*")
        {
            RestClient restClient = new RestClient($"{URL}{id}");

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddQueryParameter("fields", fields);
            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }

        public IRestResponse GetAccountNoId(string token, string id=null, string fields = "*")
        {
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddQueryParameter("fields", fields);
            restRequest.AddHeader("token", token);

            return restClient.Execute(restRequest);
        }

        public string GetAccountStatus(string token, string id)
        {
            IRestResponse restResponse = GetAccount(token, id, "status");

            var account = v1.Account.FromJson(restResponse.Content);

            return account.Status;
        }

        public DateTime GetAccountIssueDate(string token, string id)
        {
            IRestResponse restResponse = GetAccount(token, id, "permit");

            var account = v1.Account.FromJson(restResponse.Content);

            return account.Permit.Issued;
        }

        public DateTime GetAccountExpDate(string token, string id)
        {
            IRestResponse restResponse = GetAccount(token, id, "permit");

            var account = v1.Account.FromJson(restResponse.Content);

            return account.Permit.Expires;
        }

        public double GetAccountEscrow(string token,string id)
        {
            IRestResponse restResponse = GetAccount(token, id, "escrow");

            var account = v1.Account.FromJson(restResponse.Content);

            return account.EscrowBalance;
        }
    }
}
