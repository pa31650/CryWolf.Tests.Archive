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
    public class InvoiceTests : Invoice
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "Account Invoices API Test";
        private string res;
        private static string primeKey;

        private Auth auth = new Auth();
        private Payments payments = new Payments();

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

        [TestCase("Dallas", "773678", "accountnumber", HttpStatusCode.OK)]
        [TestCase("Dallas", "773678", "*", HttpStatusCode.OK)]
        [TestCase("Dallas", "773678", "account", HttpStatusCode.OK)]
        [TestCase("Dallas", "773678", "accountnumber", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "0", "accountnumber", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "-1", "accountnumber", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "999999999", "accountnumber", HttpStatusCode.NotFound)]
        [TestCase("HighPoint", "173600", "accountnumber", HttpStatusCode.OK)]
        [TestCase("HighPoint", "173600", "*", HttpStatusCode.OK)]
        [TestCase("HighPoint", "173600", "account", HttpStatusCode.OK)]
        [TestCase("HighPoint", "173600", "accountnumber", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", "0", "accountnumber", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "-1", "accountnumber", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "999999999", "accountnumber", HttpStatusCode.NotFound)]
        [Category("19.2_Regression")]
        public void Scenario047_Get_Invoice(string jurisdiction,string id,string fields,HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;

            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetInvoice(token, id,fields);

            var invoice = Models.v1.Invoice.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}. Request was: {restResponse.Request.ToString()}");
        }

        [TestCase("Dallas", "1557194", 0, 10, "*", HttpStatusCode.OK)]
        [TestCase("Dallas", "1557194", 0, 10, "amount", HttpStatusCode.OK)]
        [TestCase("Dallas", "1557194", 0, 10, "accounttype", HttpStatusCode.OK)]
        [TestCase("Dallas", "1557194", 0, 10, "id", HttpStatusCode.OK)]
        [TestCase("Dallas", "169581", 0, 10, "id", HttpStatusCode.OK)] //invoice with multiple payments
        [TestCase("Dallas", "1", 0, 10, "*", HttpStatusCode.OK)]
        [TestCase("Dallas", "0", 0, 10, "*", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "-1", 0, 10, "*", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "1557194", 0, 10, "*", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "999999999", 0, 10, "*", HttpStatusCode.NotFound)]
        [TestCase("HighPoint", "172471", 0, 10, "*", HttpStatusCode.OK)]
        [TestCase("HighPoint", "172471", 0, 10, "amount", HttpStatusCode.OK)]
        [TestCase("HighPoint", "172471", 0, 10, "accounttype", HttpStatusCode.OK)]
        [TestCase("HighPoint", "172471", 0, 10, "id", HttpStatusCode.OK)]
        [TestCase("HighPoint", "38948", 0, 10, "id", HttpStatusCode.OK)] //invoice with multiple payments
        [TestCase("HighPoint", "0", 0, 10, "*", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "-1", 0, 10, "*", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "172471", 0, 10, "*", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", "999999999", 0, 10, "*", HttpStatusCode.NotFound)]
        [Category("19.2_Regression")]
        public void Scenario048_Get_PaymentsByInvoice(string jurisdiction, string id, int skip, int take, string fields, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;

            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = payments.GetPaymentsByInvoice(token, id, skip, take, fields);

            var payment = expectedStatusCode == HttpStatusCode.OK ? Models.v1.Payments.FromJson(restResponse.Content) : null;

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }
}
