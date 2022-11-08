using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class Invoice : CryWolf
    {
        new public static string URL = $"{URLv1}invoice/";

        public IRestResponse GetInvoice(string token,string id,string fields)
        {

            RestClient restClient = new RestClient($"{URL}{id}");

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);
            
            restRequest.AddQueryParameter("fields", fields);
            
            return restClient.Execute(restRequest);
        }

        public IRestResponse GetInvoicesByAccount(string token, string id, int skip=0, int take=100, string isOutstanding = null, string fields = "*")
        {
            string accountNumber = id;
            string URL = $"{Account.URL}{accountNumber}/invoices";

            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);
            restRequest.AddQueryParameter("skip", skip.ToString());
            restRequest.AddQueryParameter("take", take.ToString());
            restRequest.AddQueryParameter("fields", fields);
            if (isOutstanding != null)
            {
                restRequest.AddQueryParameter("isOutstanding", isOutstanding.ToString());
            }

            return restClient.Execute(restRequest);
        }

        public List<int> GetInvoiceIdsByAccount(string token, string id,string isOutstanding)
        {
            IRestResponse getInvoices = GetInvoicesByAccount(token, id, 0, 100, "true", "id");

            var invoices = Models.v1.Invoices.FromJson(getInvoices.Content);

            List<int> invoiceIds = new List<int>();

            foreach (var invoice in invoices)
            {
                invoiceIds.Add(invoice.Id);
            }

            return invoiceIds;
        }
    }
}
