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
    public class AccountSearchTests : Search
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

        [Test,Combinatorial,Parallelizable]
        [Category("19.2_Regression")]
        public void Scenario042_Get_AccountsByLastName_OK(
            [Values("Dallas","HighPoint")] string jurisdiction, 
            [Values("smith")] string lastName,
            [Values(0)]int skip, 
            [Values(5)] int take, 
            [Values("*","id")]string fields)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = SearchAccounts(token,skip,take,fields,lastName);

            var search = Models.v1.Search.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [Test,Combinatorial,Parallelizable]
        [Category("19.2_Regression")]
        public void Scenario043_Get_AccountsByLastName_NotFound(
            [Values("Dallas", "HighPoint")] string jurisdiction,
            [Values("poppins")] string lastName,
            [Values(0)]int skip,
            [Values(5)] int take,
            [Values("*", "id")]string fields)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.NotFound;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = SearchAccounts(token, skip, take, fields, lastName);

            var search = Models.v1.Search.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [Test,Combinatorial,Parallelizable]
        [Category("19.2_Regression")]
        public void Scenario044_Get_AccountsByLastName_Unauthorized(
            [Values("Dallas", "HighPoint")] string jurisdiction,
            [Values("smith")] string lastName,
            [Values(0)]int skip,
            [Values(5)] int take,
            [Values("*")]string fields)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = CommonTestSettings.badToken;

            IRestResponse restResponse = SearchAccounts(token, skip, take, fields, lastName);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas", 0, 5,"*", "7/12/2019", HttpStatusCode.OK)]
        [Category("19.3_Regression")]
        [Parallelizable]
        public void Scenario105_Get_AccountsByUpdatedDate(string jurisdiction, int skip, int take, string fields, string dateTime, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string accountType = string.Empty;
            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : CryWolfUtil.GetAgencyToken(jurisdiction);
            CryWolfUtil.SetDBName(jurisdiction);

            IRestResponse restResponse = SearchAccounts(token, skip, take, fields, DateTime.Parse(dateTime));

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request. Account Type: {accountType}", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }

}
