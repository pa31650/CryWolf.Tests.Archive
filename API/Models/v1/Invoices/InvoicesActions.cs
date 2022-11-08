using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class Invoices : Invoice
    {
        new public static string URL = $"{URLv1}invoices/";

        public IRestResponse GetInvoices(string token, string invoiceIds, string fields)
        {
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            restRequest.AddQueryParameter("invoiceIds",invoiceIds);
            restRequest.AddQueryParameter("fields", fields);

            return restClient.Execute(restRequest);
        }
    }
}
