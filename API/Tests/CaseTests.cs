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
    class CaseTests : Case
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "Account API Test";
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

        [TestCase("Dallas", "19-0111868", "*", HttpStatusCode.OK),Property("TestCase ID", "398212")]
        [TestCase("Dallas", "19-0111868", "accountStatus", HttpStatusCode.OK), Property("TestCase ID", "398201")] 
        //[TestCase("Dallas", "19-0111868", "monitoringCompany", HttpStatusCode.OK)]
        [TestCase("Dallas", "201962812941","*", HttpStatusCode.NotFound)]
        [TestCase("HighPoint", "2016131216", "*", HttpStatusCode.OK), Property("TestCase ID", "398212")]
        [TestCase("HighPoint", "2016131216", "accountStatus", HttpStatusCode.OK), Property("TestCase ID", "398201")]
        //[TestCase("HighPoint", "19-0111868", "monitoringCompany", HttpStatusCode.OK)]
        [TestCase("HighPoint", "201962812941", "*", HttpStatusCode.NotFound)]
        [Category("19.3_Regression")]
        public void Scenario082_Get_Case_ById(string jurisdiction, string id, string fields, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetCaseById(token, id, fields);
            
            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            var alarmCase = Models.v1.Case.FromJson(restResponse.Content);

            //if (fields.Equals("monitoringCompany"))
            //{
            //    Assert.NotNull(alarmCase.MonitoringCompany);
            //}
        }

        [TestCase("Dallas", "1036937","*", HttpStatusCode.OK), Property("TestCase ID","398206")]
        [TestCase("Dallas", "1036937", "id", HttpStatusCode.OK), Property("TestCase ID", "398205")]
        [TestCase("Dallas", "1036937", "accountStatus", HttpStatusCode.OK), Property("TestCase ID", "398201")]
        [TestCase("Dallas", "1036937a", "*", HttpStatusCode.NotFound)]
        [Category("19.3_Regression")]
        public void Scenario083_Get_Cases_ByAccount(string jurisdiction, string accountNumber, string fields, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetCasesByAccount(token, accountNumber, fields);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            var alarmCases = Models.v1.Cases.FromJson(restResponse.Content);

            //if (fields.Equals("monitoringCompany"))
            //{
            //    foreach (var alarmCase in alarmCases)
            //    {
            //        Assert.NotNull(alarmCase.MonitoringCompany);
            //    }
            //}
        }
    }
}
