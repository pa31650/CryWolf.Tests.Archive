using API.Models.v1;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using Utils;

namespace API.Tests
{
    [TestFixture]
    public class AuthTests : Auth
    {
        private ReportBuilder reportBuilder = new ReportBuilder();
        private Library library = new Library(CommonTestSettings.AppName);
        
        private DatabaseReader databaseReader = new DatabaseReader();

        private TestContext TestContext;
        private List<String> scenariosTested = new List<String>();

        private bool status;

        private string categoryName = "Auth API Test";
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
                databaseReader.UpdateEndTimeAndBuildVersion(primeKey,"version");
            }
        }

        [TestCase("Dallas", "admin")]
        [TestCase("HighPoint", "admin")]
        [Category("19.2_Regression")]
        public void Scenario031_Post_Auth_OK(string jurisdiction, string username)
        {
            string password = CommonTestSettings.DesktopPw;
            IRestResponse restResponse = PostAuthToken(jurisdiction,username,password.ToString());
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("HighPoint", "admin","welcome1")]
        [TestCase("HighPoint", "admin2","welcome")]
        [TestCase("Dallas", "admin", "welcome1")]
        [TestCase("Dallas", "admin2","welcome")]
        [Category("19.2_Regression")]
        public void Scenario032_Post_Auth_Unauthorized(string jurisdiction, string username, string password = null, bool key = true)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            HttpStatusCode actualStatusCode;
            IRestResponse restResponse = PostAuthToken(jurisdiction, username, password.ToString(), key);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [Test,Combinatorial]
        [Category("19.2_Regression")]
        public void Scenario033_Post_Auth_BadRequest(
            [Values("HighPoint!","Dallas!","HighPoint","Dallas")] string jurisdiction,
            [Values("admin")] string username, 
            [Values("")] string password)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
            HttpStatusCode actualStatusCode;
            IRestResponse restResponse = PostAuthToken(jurisdiction, username, password);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
        
        
    }
}