using API.Models.pResponse;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using Utils;

namespace API.Tests
{
    public class AuthorizeNetTests : AuthorizeNet
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "Authorize.Net API Test";
        private string res;
        private static string primeKey;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            if (CommonTestSettings.WriteToSQL)
            {
                primeKey = DatabaseReader.CreateParentRecord();
            }
        }

        [SetUp]
        public void Setup()
        {
            if (CommonTestSettings.WriteToSQL)
            {
                databaseReader.AddToScenarioList(primeKey, TestContext.CurrentContext.Test.MethodName);
                scenariosTested.Add(TestContext.CurrentContext.Test.Name);
                ReportBuilder.getStartTime();
            }
        }

        [TearDown]
        public void Cleanup()
        {
            if (CommonTestSettings.WriteToSQL)
            {
                ReportBuilder.getEndTime();
                Library.ReportResults(TestContext.CurrentContext.Test.MethodName, TestContext.CurrentContext.Test.MethodName, out res, primeKey, scenariosTested);
                databaseReader.UpdateEndTimeAndBuildVersion(primeKey, "version");
            }
        }

        [TestCase("HighPoint", "18861", "2")]
        [TestCase("HighPoint", "18861", "3")]
        [TestCase("HighPoint", "18861", "4")]
        [TestCase("HighPoint", "18861", "5")]
        public void Scenario102_Post_AuthorizeNet_Fail(string agency, string accountNumber, string responseCode)
        {
            CryWolfUtil.SetDBName(agency);

            string description = "test";
            string invoice = CryWolfUtil.GetInvoiceByAlarmNo(accountNumber);
            string cart = CryWolfUtil.GetCartByInvoice(invoice);
            string amount = CryWolfUtil.GetInvoiceAmount(invoice);
            string expectedNotification = string.Empty;

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;

            CryWolfUtil.DeleteOnlinePaymentNotes();

            IRestResponse restResponse = MockPaymentResponse(responseCode, cart, description, amount, accountNumber, description);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            switch (responseCode)
            {
                case "2":
                    expectedNotification = Notification.ERROR_2;
                    break;
                case "3":
                    expectedNotification = Notification.ERROR_3;
                    break;
                case "4":
                    expectedNotification = Notification.ERROR_4;
                    break;
                case "5":
                    expectedNotification = Notification.ERROR_5;
                    break;
                default:
                    break;
            }

            expectedNotification += accountNumber;

            string actualNotification = SQLHandler.GetDatabaseValue(
                "SELECT Notes FROM ALARM_NOTES WHERE LetterType = 'Online Payment'",
                "Notes",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);

            ReportBuilder.ArrayBuilder($"Validate Note added to Notifications: {actualNotification}", expectedNotification.Equals(actualNotification), "DB Validation");
            Assert.AreEqual(expectedNotification, actualNotification);

            CryWolfUtil.DeleteOnlinePaymentNotes();
        }

        private class Notification
        {
            public const string ERROR_2 =
                "Citizen portal: The payment provider returned a response code of 2, which means the payment was declined.  " +
                "A payment was not tendered.  AuthNet_AR Account: ";
            public const string ERROR_3 =
                "Citizen portal: The payment provider returned a response code of 3, which means the payment provider had an error when processing the payment.  " +
                "A payment was not tendered.  AuthNet_AR Account: ";
            public const string ERROR_4 =
                "Citizen portal: The payment provider returned a response code of 4.  " +
                "A payment was not tendered.  AuthNet_AR Account: ";
            public const string ERROR_5 =
                "Citizen portal: The payment provider returned a response code of 5.  " +
                "A payment was not tendered.  AuthNet_AR Account: ";
        }
    }
}
