using RestSharp;
using API.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class Search : CryWolf
    {
        
        new private readonly string URL = $"{v1.Account.URL}search";

        public IRestResponse SearchAccounts(
            string token, int skip, int take, string fields, string firstName, string lastName,
            string streetName = null, string streetNumber = null, string suite = null, string streetNameExactMatch = null, string streetNumberTolerance = null,
            string alarmType = null, string updatedOn = null)
        {
            
            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            restRequest.AddQueryParameter("skip", skip.ToString());
            restRequest.AddQueryParameter("take", take.ToString());
            restRequest.AddQueryParameter("fields", fields);
            restRequest.AddQueryParameter("lastName", lastName);

            restRequest.AddQueryWhenNotNull("firstName", firstName);
            restRequest.AddQueryWhenNotNull("streetName", streetName);
            restRequest.AddQueryWhenNotNull("streetNumber", streetNumber);
            restRequest.AddQueryWhenNotNull("suite", suite);
            restRequest.AddQueryWhenNotNull("streetNameExactMatch", streetNameExactMatch);
            restRequest.AddQueryWhenNotNull("streetNumberTolerance", streetNumberTolerance);

            restRequest.AddQueryWhenNotNull("alarmType", alarmType);

            return restClient.Execute(restRequest);
        }
        public IRestResponse SearchAccounts(string token, int skip, int take, string fields, string lastName)
        {
            return SearchAccounts(token, skip, take, fields, null, lastName);
        }
        public IRestResponse SearchAccounts(string token, int skip, int take, string fields, DateTime updatedOn)
        {
            return SearchAccounts(token, skip, take, fields, null, null, null, null, null, null, null, null, updatedOn.ToShortDateString());
        }
    }
}
