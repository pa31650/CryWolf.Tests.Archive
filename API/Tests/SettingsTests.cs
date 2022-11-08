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
    class SettingsTests : Settings
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "Account Invoices API Test";
        private string res;
        private static string primeKey;

        private Auth auth = new Auth();

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

        [TestCase("Dallas","<Default>",HttpStatusCode.OK)]
        [TestCase("Dallas", "<Default>", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "Frisco", HttpStatusCode.OK)]
        [TestCase("Dallas", "Frisco", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", "<Default>", HttpStatusCode.OK)]
        [TestCase("HighPoint", "<Default>", HttpStatusCode.Unauthorized)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario051_Get_PaymentSettings(string jurisdiction, string agency, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;

            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetPaymentSettings(token, agency);

            var settings = Models.v1.Settings.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }
}
