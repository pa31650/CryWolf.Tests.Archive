using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace API.Models.pResponse
{
    public partial class AuthorizeNet
    {
        public static string URL = CommonTestSettings.HighPoint.url + @"/pResponse/authorizeNetResponse.ashx";

        public static IRestResponse MockPaymentResponse(string response_code, string invoice_num, string description, string amount, string cust_id, string trans_id)
        {
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.POST)
            {
                AlwaysMultipartFormData = true
            };

            restRequest.AddParameter("x_response_code", response_code, ParameterType.GetOrPost);
            restRequest.AddParameter("x_invoice_num", invoice_num, ParameterType.GetOrPost);
            restRequest.AddParameter("x_description", description, ParameterType.GetOrPost);
            restRequest.AddParameter("x_amount", amount, ParameterType.GetOrPost);
            restRequest.AddParameter("x_cust_id", cust_id, ParameterType.GetOrPost);
            restRequest.AddParameter("x_trans_id", trans_id, ParameterType.GetOrPost);

            return restClient.Execute(restRequest);
        }
    }
}
