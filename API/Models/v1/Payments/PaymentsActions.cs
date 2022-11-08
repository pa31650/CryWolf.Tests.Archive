using RestSharp;
using API.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Utils;
using System.Net;

namespace API.Models.v1
{
    public partial class Payments : CryWolf
    {
        new public static string URL = $"{URLv1}payment/";

        public IRestResponse DeletePayment(
            string token, string id, string voidReason = "Testing",
            string category = null, string subCategory = null,
            string sendLetter = null, string setStatus = null, string setPermitIssueDate = null, string setPermitExpirationDate = null,
            string placeVoidedAmountInEscrow = null)
        {
            RestClient restClient = new RestClient(URL + id);

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

        public string GetPaymentIdByInvoice(string token,string invoiceId)
        {
            string paymentId;
            IRestResponse getResponse = GetPaymentsByInvoice(token, invoiceId);

            ReportBuilder.ReportResponseContent(getResponse);

            if (!getResponse.StatusCode.Equals(HttpStatusCode.NotFound))
            {
                var payment = Models.v1.Payments.FromJson(getResponse.Content);
                paymentId = payment[0].Id.ToString();
            }
            else
            {
                paymentId = "NotFound";
            }

            return paymentId;
        }

        public IRestResponse GetPaymentsByInvoice(string token, string id, int skip = 0, int take = 10, string fields = "*")
        {
            string URL = $"{v1.Invoice.URL}{id}/payments/";

            RestClient restClient = new RestClient(URL);

            RestRequest restRequest = new RestRequest(Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            restRequest.AddHeader("token", token);

            restRequest.AddQueryParameter("skip", skip.ToString());
            restRequest.AddQueryParameter("take", take.ToString());
            restRequest.AddQueryParameter("fields", fields);

            return restClient.Execute(restRequest);
        }

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

            //paymentBody = null;

            return restClient.Execute(restRequest);
        }

        public IRestResponse PutPaymentInvoiceSpecialActions(string token, List<int> invoiceIds, string paymentType, double amount, bool autoApproveSpecialActions)
        {
            return PutPaymentInvoice(token, invoiceIds, paymentType, amount, null, null, null, null, null, null, null, null, null, null, null, null, null, autoApproveSpecialActions);
        }
        public IRestResponse PutPaymentInvoice(string token, List<int> invoiceIds, string paymentType, double amount,
            string paidByAccountType, string paidByAccountNumber)
        {
            return PutPaymentInvoice(token, invoiceIds, paymentType, amount, paidByAccountType, paidByAccountNumber,null);
        }

        public IRestResponse PutPaymentInvoiceCheck(string token, List<int> invoiceIds, string paymentType, double amount, string checkNumber, string category)
        {
            return PutPaymentInvoice(token, invoiceIds, paymentType, amount, null, null, null, null, checkNumber, null, null, category);
        }

        public IRestResponse PutPaymentInvoiceOverPay(string token, List<int> invoiceIds, string paymentType, double amount, string overpaymentGoesToEscrow)
        {
            return PutPaymentInvoice(token, invoiceIds, paymentType, amount, null, null,null,null,null,null, overpaymentGoesToEscrow);
        }

        public IRestResponse PutPaymentInvoiceWithAction(string token, List<int> invoiceIds, string paymentType, double amount,
            string letter, string setStatus, string setPermitIssueDate,string setPermitExpirationDate,string overpaymentGoesToEscrow)
        {
            return PutPaymentInvoice(token, invoiceIds, paymentType, amount,
                null,null,null,null,null,null,
                overpaymentGoesToEscrow, null, null, setStatus, setPermitIssueDate, setPermitExpirationDate, letter);
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

        
        public IRestResponse PutPaymentTransferEscrow(
            string token, string sourceAccountType, string sourceAccountNumber, 
            string destinationAccountType, string destinationAccountNumber, int amountToTransfer)
        {
            RestClient restClient = new RestClient(URL + "escrowTransfer");

            RestRequest restRequest = new RestRequest(Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };

            TransferBody transferBody = new TransferBody(
                sourceAccountType, sourceAccountNumber,
                destinationAccountType, destinationAccountNumber,
                amountToTransfer);

            restRequest.AddHeader("token", token);

            restRequest.AddJsonBody(transferBody);

            return restClient.Execute(restRequest);
        }

        public IRestResponse PutPaymentWithDate(string token, List<int> invoiceIds, string paymentType, double amount, string paymentDate)
        {
            return PutPaymentInvoice(token, invoiceIds, paymentType, amount, null, null, null, paymentDate);
        }
    }
}
