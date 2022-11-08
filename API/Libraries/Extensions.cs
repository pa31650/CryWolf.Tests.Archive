using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Libraries
{
    static class Extensions
    {
        public static IRestRequest AddQueryWhenNotNull(this IRestRequest restRequest, string paramName, string paramValue)
        {
            if (paramValue != null)
            {
                restRequest.AddQueryParameter(paramName, paramValue);
            }

            return restRequest;
        }
    }
}