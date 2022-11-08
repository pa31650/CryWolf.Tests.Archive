using API.Models.v1;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using Utils;

namespace API.Tests
{
    class AccountTests : Account
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "Account API Test";
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

        [TestCase("Dallas", "714941", "*")]
        [TestCase("Dallas", "714954", "*")]
        [TestCase("Dallas", "714941", "lastName")]
        [TestCase("Dallas", "714954", "lastName")]
        [TestCase("Dallas", "714941", "Address")]
        [TestCase("Dallas", "714954", "Address")]
        [TestCase("Dallas", "714954", "updatedOn"), Category("19.3_Regression")]
        [TestCase("Dallas", "714954", "createdOn"), Category("19.3_Regression")]
        [TestCase("HighPoint", "16810", "*")]
        [TestCase("HighPoint", "16781", "*")]
        [TestCase("HighPoint", "16810", "lastName")]
        [TestCase("HighPoint", "16781", "lastName")]
        [TestCase("HighPoint", "16810", "Address")]
        [TestCase("HighPoint", "16781", "Address")]
        [TestCase("HighPoint", "16781", "updatedOn"), Category("19.3_Regression")]
        [TestCase("HighPoint", "16781", "createdOn"), Category("19.3_Regression")]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario038_Get_Account_OK(string jurisdiction, string id, string fields)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetAccount(token, id, fields);

            var account = FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;
            if (id == "*")
            {
                accountType = account.LocationType.ToString();
            }

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [Test, Combinatorial, Parallelizable]
        [Category("19.2_Regression")]
        public void Scenario040_Get_Account_NotFound(
            [Values("Dallas", "HighPoint")] string jurisdiction,
            [Values(null, "1")]string id,
            [Values("*", "lastName")] string fields)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.NotFound;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetAccountNoId(token, id, fields);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [Test, Combinatorial, Parallelizable]
        [Category("19.2_Regression")]
        public void Scenario041_Get_Account_Unauthorized(
            [Values("Dallas", "HighPoint")] string jurisdiction,
            [Values("714941", "714954", "16810", "16781")] string id,
            [Values("*", "lastName")] string fields)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = CommonTestSettings.badToken;

            IRestResponse restResponse = GetAccount(token, id, fields);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas", "714941")]
        [TestCase("HighPoint", "16810")]
        [Parallelizable]
        public void Scenario078_Get_Account_Escrow(string jurisdiction, string id)
        {
            string token = auth.GetAuthToken(jurisdiction);

            double escrowAmount = GetAccountEscrow(token, id);

            //ReportBuilder.ReportResponseContent(restResponse);
            bool exitStatus = escrowAmount > -1;
            ReportBuilder.ArrayBuilder($"Escrow Amount found for Account: {id} = ${escrowAmount}", exitStatus, "Validate Amount Returned");
            Assert.IsTrue(exitStatus, $"Escrow Amount is a double value greater than -1");
        }
    }
}
