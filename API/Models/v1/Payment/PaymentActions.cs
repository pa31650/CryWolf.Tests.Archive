using RestSharp;
using API.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{
    public partial class Payment : CryWolf
    {
        new public static string URL = $"{URLv1}payment/";

        public IRestResponse PutPaymentInvoice(string token, List<int> invoiceIds, string paymentType, double amount, 
            string paidByAccountType = null, string paidByAccountNumber = null, string escrowTransferredToAccountNumber = null,
            string paymentDate = null,string checkNumber = null,string transactionId = null,string overpaymentGoesToEscrow = null,
            string category = null,string subCategory = null, string setStatus = null,string setPermitIssueDate = null,string setPermitExpirationDate = null,
            string letterToSend = null, bool autoApproveSpecialActions = false)
        {
            PaymentBody paymentBody = new PaymentBody(
                invoiceIds, paymentType, amount, paidByAccountType, paidByAccountNumber, escrowTransferredToAccountNumber,
                paymentDate, checkNumber, transactionId, overpaymentGoesToEscrow, category, subCategory, setStatus, setPermitIssueDate, setPermitExpirationDate,
                letterToSend, autoApproveSpecialActions);

            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            restRequest.AddJsonBody(paymentBody);

            return restClient.Execute(restRequest);
        }

        public IRestResponse PutPaymentShoppingCart(string token, string shoppingCart, string paymentType, double amount,
            string paidByAccountType = null, string paidByAccountNumber = null, string escrowTransferredToAccountNumber = null,
            string paymentDate = null, string checkNumber = null, string transactionId = null, string overpaymentGoesToEscrow = null,
            string category = null, string subCategory = null, string setStatus = null, string setPermitIssueDate = null, string setPermitExpirationDate = null,
            string letterToSend = null, bool autoApproveSpecialActions = false)
        {
            PaymentBody paymentBody = new PaymentBody(
                shoppingCart, paymentType, amount, paidByAccountType, paidByAccountNumber, escrowTransferredToAccountNumber,
                paymentDate, checkNumber, transactionId, overpaymentGoesToEscrow, category, subCategory, setStatus, setPermitIssueDate, setPermitExpirationDate,
                letterToSend, autoApproveSpecialActions);

            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            restRequest.AddJsonBody(paymentBody);

            return restClient.Execute(restRequest);
        }

        public IRestResponse DeletePayment(
            string token, string id, string voidReason = "Testing",
            string category = null, string subCategory = null,
            string sendLetter = null, string setStatus = null, string setPermitIssueDate = null,string setPermitExpirationDate = null,
            string placeVoidedAmountInEscrow = null)
        {
            URL += id;

            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            restRequest.AddQueryParameter("voidReason", voidReason);

            restRequest.AddQueryWhenNotNull("category", category);
            restRequest.AddQueryWhenNotNull("subCategory", subCategory);

            restRequest.AddQueryWhenNotNull("sendLetter", sendLetter);
            restRequest.AddQueryWhenNotNull("setStatus", setStatus);

            restRequest.AddQueryWhenNotNull("setPermitIssueDate", setPermitIssueDate);
            restRequest.AddQueryWhenNotNull("setPermitExpirationDate", setPermitExpirationDate);

            restRequest.AddQueryWhenNotNull("placeVoidedAmountInEscrow", placeVoidedAmountInEscrow);

            return restClient.Execute(restRequest);
        }

        
    }
}
