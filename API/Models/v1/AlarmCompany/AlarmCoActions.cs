using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1.AlarmCompany
{
    partial class AlarmCompany : CryWolf
    {
        new private readonly string URL = $"{URLv1}alarmcompany/";

        public IRestResponse GetAlarmCompany(string token,string id,string fields = "*")
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
    }
}
