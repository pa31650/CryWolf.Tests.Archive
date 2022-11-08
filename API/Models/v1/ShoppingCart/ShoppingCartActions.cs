using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class ShoppingCart : CryWolf
    {
        new public static string URL = $"{URLv1}shoppingCart/";

        public IRestResponse GetShoppingCart(string token, string id)
        {
            RestClient restClient = new RestClient(URL + id);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            //restRequest.AddQueryParameter("skip", skip.ToString());
            //restRequest.AddQueryParameter("take", take.ToString());
            //restRequest.AddQueryParameter("fields", fields);

            return restClient.Execute(restRequest);
        }
    }
}
