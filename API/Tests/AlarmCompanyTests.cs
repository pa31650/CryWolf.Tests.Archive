using API.Models.v1;
using API.Models.v1.AlarmCompany;
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
    class AlarmCompanyTests : AlarmCompany
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

        [TestCase("Dallas","ADT","*")]
        [TestCase("Dallas", "ADT", "name")]
        [TestCase("Dallas", "ADT", "permit")]
        [TestCase("HighPoint", "100", "*")]
        [TestCase("HighPoint", "100", "name")]
        [TestCase("HighPoint", "100", "permit")]
        [Category("19.2_Regression")]
        public void Scenario036_Get_AlarmCompany_OK(string jurisdiction,string id,string fields)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetAlarmCompany(token, id, fields);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [Test]
        [Category("19.2_Regression")]
        public void Scenario037_Get_AlarmCompany_Unauthorized(
            [Values("Dallas","HighPoint")]string jurisdiction, 
            [Values("ADT","100")]string id, 
            [Values("*")]string fields)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            string token = CommonTestSettings.badToken;
            HttpStatusCode actualStatusCode;

            IRestResponse restResponse = GetAlarmCompany(token, id, fields);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }
}
