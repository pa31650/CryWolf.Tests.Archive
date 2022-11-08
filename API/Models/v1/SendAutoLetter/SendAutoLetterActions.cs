using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class SendAutoLetter : CryWolf
    {
        new public static string URL = $"{Payment.URL}sendAutoLetter";

        public IRestResponse PostApproveSendAutoLetter(string token, SendAutoLetterBody sendAutoLetterBody)
        {
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            restRequest.AddJsonBody(sendAutoLetterBody);

            return restClient.Execute(restRequest);
        }
    }
}
