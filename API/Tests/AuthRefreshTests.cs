using API.Models.v1;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace API.Tests
{
    class AuthRefreshTests : Refresh
    {
        private ReportBuilder reportBuilder = new ReportBuilder();
        private Library library = new Library(CommonTestSettings.AppName);

        private DatabaseReader databaseReader = new DatabaseReader();

        private TestContext TestContext;
        private List<String> scenariosTested = new List<String>();

        private bool status;

        private string categoryName = "Auth Refresh API Test";
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
        
        [Test,Combinatorial]
        [Category("19.2_Regression")]
        public void Scenario034_Post_AuthRefresh_OK(
            [Values("Dallas","HighPoint")] string jurisdiction, 
            [Values("admin")] string username)
        {
            string password = CommonTestSettings.DesktopPw;
            string token = auth.GetAuthToken(jurisdiction, username, password, out string refreshToken);
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;

            IRestResponse restResponse = PostAuthRefreshToken(jurisdiction, token, refreshToken);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [Test,Combinatorial]
        [Category("19.2_Regression")]
        public void Scenario035_Post_AuthRefresh_Unauthorized(
            [Values("Used Token", "Refresh Token")] string testScenario, 
            [Values("Dallas","HighPoint")]string jurisdiction,
            [Values("admin")]string username)
        {
            string password = CommonTestSettings.DesktopPw;
            string token = auth.GetAuthToken(jurisdiction, username, password, out string refreshToken);
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            HttpStatusCode actualStatusCode;

            switch (testScenario)
            {
                case "Used Token":
                    _ = GetAuthRefreshToken(jurisdiction, token, refreshToken, out _);
                    break;
                case "Refresh Token":
                    token = refreshToken;
                    break;
                default:
                    break;
            }

            IRestResponse restResponse = PostAuthRefreshToken(jurisdiction, token, refreshToken);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }
}
