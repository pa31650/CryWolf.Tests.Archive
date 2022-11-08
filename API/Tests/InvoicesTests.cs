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
    public class InvoicesTests : Invoices
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

        [TestCase("Dallas", "773678", "*", HttpStatusCode.OK)]
        [TestCase("Dallas", "773678,773678", "*", HttpStatusCode.OK)]
        [TestCase("Dallas", "773678,773679", "*", HttpStatusCode.OK)]
        [TestCase("Dallas", "773678,773679", "accountType", HttpStatusCode.OK)]
        [TestCase("Dallas", "773678,773679", "accountNumber", HttpStatusCode.OK)]
        [TestCase("Dallas", "-1", "*", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "0", "*", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "", "*", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "773678", "*", HttpStatusCode.Unauthorized)]
        //[TestCase("Dallas", "999999999", "accountnumber", HttpStatusCode.NotFound)]
        [TestCase("HighPoint", "173600", "*", HttpStatusCode.OK)]
        [TestCase("HighPoint", "173600,173600", "*", HttpStatusCode.OK)]
        [TestCase("HighPoint", "173600,173601", "*", HttpStatusCode.OK)]
        [TestCase("HighPoint", "173600,173601", "accountType", HttpStatusCode.OK)]
        [TestCase("HighPoint", "173600,173601", "accountNumber", HttpStatusCode.OK)]
        [TestCase("HighPoint", "-1", "*", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "0", "*", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "", "*", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "173600", "*", HttpStatusCode.Unauthorized)]
        //[TestCase("HighPoint", "999999999", "accountnumber", HttpStatusCode.NotFound)]
        [Category("19.3_Regression")]
        public void Scenario104_Get_Invoices(string jurisdiction,string ids,string fields,HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;

            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetInvoices(token, ids,fields);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}. Request was: {restResponse.Request.ToString()}");

            var invoices = Models.v1.Invoice.FromJson(restResponse.Content);
        }
    }
}
