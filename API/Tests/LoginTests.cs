using API.Models.v1;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using Utils;
using static Utils.CommonTestSettings;

namespace API.Tests
{
    [TestFixture]
    public class LoginTests : Login
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

        [TestCase("Dallas", "Password", true, HttpStatusCode.OK)]
        [TestCase("Dallas", "Invoice", true, HttpStatusCode.OK)]
        [TestCase("Dallas", "Invoice", false, HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "Password", false, HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "Password", false, HttpStatusCode.BadRequest)]
        //[TestCase("HighPoint", "Password", true, HttpStatusCode.OK)]
        //[TestCase("HighPoint", "Invoice", true, HttpStatusCode.OK)]
        //[TestCase("HighPoint", "Invoice", false, HttpStatusCode.Unauthorized)]
        //[TestCase("HighPoint", "Password", false, HttpStatusCode.Unauthorized)]
        //[TestCase("HighPoint", "Password", false, HttpStatusCode.BadRequest)]
        [Category("19.3_Regression")]
        public void Scenario073_Post_Login_Citizen(string jurisdiction, string authMethod, bool allowLoginByInvoiceNumber, HttpStatusCode httpStatusCode)
        {
            
            CryWolfUtil.SetDBName(jurisdiction);

            string userName = httpStatusCode.Equals(HttpStatusCode.BadRequest) ? "" : CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string password = authMethod.Equals("Password") ? httpStatusCode.Equals(HttpStatusCode.Unauthorized) ? CommonTestSettings.badToken : CommonTestSettings.pw : CryWolfUtil.GetInvoiceByAlarmNo(userName);

            IRestResponse restResponse = PostCitizenLogin(jurisdiction, userName, password, allowLoginByInvoiceNumber.ToString());
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas", "Password", true, HttpStatusCode.OK)]
        [TestCase("Dallas", "Invoice", true, HttpStatusCode.OK)]
        [TestCase("Dallas", "Invoice", false, HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "Password", false, HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "Password", false, HttpStatusCode.BadRequest)]
        //[TestCase("HighPoint", "Password", true, HttpStatusCode.OK)]
        //[TestCase("HighPoint", "Invoice", true, HttpStatusCode.OK)]
        //[TestCase("HighPoint", "Invoice", false, HttpStatusCode.Unauthorized)]
        //[TestCase("HighPoint", "Password", false, HttpStatusCode.Unauthorized)]
        //[TestCase("HighPoint", "Password", false, HttpStatusCode.BadRequest)]
        [Category("19.3_Regression")]
        public void Scenario072_Post_Login_AlarmCompany(string jurisdiction, string authMethod, bool allowLoginByInvoiceNumber, HttpStatusCode httpStatusCode)
        {
            
            CryWolfUtil.SetDBName(jurisdiction);

            string userName = httpStatusCode.Equals(HttpStatusCode.BadRequest) ? "" : CryWolfUtil.GetAlarmNo(SQLStrings.ActiveAlarmCo);
            string password = authMethod == "Password" ? httpStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : CommonTestSettings.pw : CryWolfUtil.GetInvoiceByAlarmNo(userName);

            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            IRestResponse restResponse = PostAlarmCompanyLogin(jurisdiction, userName, password, allowLoginByInvoiceNumber.ToString());

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas", HttpStatusCode.OK)]
        [TestCase("Dallas", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", HttpStatusCode.OK)]
        [TestCase("HighPoint", HttpStatusCode.BadRequest)]
        [Category("19.3_Regression")]
        public void Scenario075_Change_Login_Citizen(string jurisdiction, HttpStatusCode httpStatusCode)
        {
            
            CryWolfUtil.SetDBName(jurisdiction);

            string userName = httpStatusCode.Equals(HttpStatusCode.BadRequest) ? "" : CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen, dbName);
            string password = CommonTestSettings.pw;

            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            IRestResponse restResponse = PatchCitizenLogin(jurisdiction, userName, password);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

        }

        [TestCase("Dallas", HttpStatusCode.OK)]
        [TestCase("Dallas", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", HttpStatusCode.OK)]
        [TestCase("HighPoint", HttpStatusCode.BadRequest)]
        [Category("19.3_Regression")]
        public void Scenario076_Change_Login_AlarmCompany(string jurisdiction, HttpStatusCode httpStatusCode)
        {
            
            CryWolfUtil.SetDBName(jurisdiction);

            string userName = httpStatusCode.Equals(HttpStatusCode.BadRequest) ? "" : CryWolfUtil.GetAlarmNo(SQLStrings.ActiveAlarmCo, dbName);
            string password = CommonTestSettings.pw;

            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            IRestResponse restResponse = PatchAlarmCompanyLogin(jurisdiction, userName, password);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), categoryName);
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

        }
    }
}