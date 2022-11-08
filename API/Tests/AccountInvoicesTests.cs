using API.Models.v1;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using Utils;

namespace API.Tests
{
    public class AccountInvoicesTests : Invoice
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "Account Invoices API Test";
        private string res;
        private static string primeKey;

        private Auth auth = new Auth();
        private Account account = new Account();

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

        [TestCase("Dallas", "714941", 0, 10)]
        [TestCase("Dallas", "625479", 0, 10, "true")]
        [TestCase("Dallas", "714941", 0, 10, "false")]
        [TestCase("Dallas", "714941", 0, 10, null,"id")]
        [TestCase("Dallas", "625479", 0, 10, "true","id")]
        [TestCase("Dallas", "714941", 0, 10, "false","id")]
        [TestCase("Dallas", "ADT", 0, 10)]
        [TestCase("HighPoint", "16781", 0, 10)]
        [TestCase("HighPoint", "16781", 0, 10, "true")]
        [TestCase("HighPoint", "16781", 0, 10, "false")]
        [TestCase("HighPoint", "16781", 0, 10, null, "id")]
        [TestCase("HighPoint", "16781", 0, 10, "true", "id")]
        [TestCase("HighPoint", "16781", 0, 10, "false", "id")]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario039_Get_InvoiceByAccount_OK(string jurisdiction, string id, int skip, int take, string isOutstanding = null, string fields = "*")
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetInvoicesByAccount(token, id, skip, take, isOutstanding, fields);

            var invoice = Invoices.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;
            
            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas", "714941", 0, 10)]
        [TestCase("HighPoint", "16781", 0, 10)]
        [Category("19.2_Regression")]
        [Parallelizable]
        
        public void Scenario046_Get_InvoiceByAccount_Unauthorized(string jurisdiction,string id,int skip,int take,string isOutstanding = null, string fields = "*")
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = CommonTestSettings.badToken;

            IRestResponse restResponse = GetInvoicesByAccount(token, id, skip, take, isOutstanding, fields);

            var invoice = Invoices.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }
}
