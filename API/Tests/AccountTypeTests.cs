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
    public class AccountTypeTests : AccountTypes
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "Account Types API Test";
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

        [Test]
        [Category("19.2_Regression")]
        public void Scenario045_Get_AccountTypes()
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            
            IRestResponse restResponse = GetAccountTypes();

            var accountTypes = Models.v1.AccountTypes.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }
}
