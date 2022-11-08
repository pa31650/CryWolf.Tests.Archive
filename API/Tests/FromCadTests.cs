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
using static Utils.CommonTestSettings;

namespace API.Tests
{
    class FromCadTests : FromCad
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "From Cad API Test";
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

        [TestCase("Dallas", "*", HttpStatusCode.OK)]
        [TestCase("Dallas", "ClearanceCode", HttpStatusCode.OK)]
        [TestCase("Dallas", "*", HttpStatusCode.NotFound)]
        [TestCase("Dallas", "*", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "*", HttpStatusCode.OK)]
        [TestCase("HighPoint", "ClearanceCode", HttpStatusCode.OK)]
        [TestCase("HighPoint", "*", HttpStatusCode.NotFound)]
        [TestCase("HighPoint", "*", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", "", HttpStatusCode.BadRequest)]
        [Category("19.3_Regression")]
        public void Scenario109_Get_CasesFromCad_ByCaseNumber(string jurisdiction, string fields, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : CryWolfUtil.GetAgencyToken(jurisdiction);
            CryWolfUtil.SetDBName(jurisdiction);
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            FromCad call = CreateCall(jurisdiction,alarmNumber);
            string id = String.Empty;

            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Unauthorized:
                    id = call.CaseNumber;
                    break;
                case HttpStatusCode.NotFound:
                    id = "201962812941";
                    break;
                default:
                    break;
            }

            PutCasesFromCad(token, call);

            IRestResponse restResponse = GetCasesFromCad(token, fields, id);
            
            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            var fromCad = httpStatusCode != HttpStatusCode.BadRequest ? Models.v1.FromCad.FromJson(restResponse.Content) : null;
        }
        [TestCase("Dallas", "ClearanceCode", HttpStatusCode.OK)]
        [TestCase("Dallas", "*", HttpStatusCode.OK)]
        [TestCase("HighPoint", "ClearanceCode", HttpStatusCode.OK)]
        [TestCase("HighPoint", "*", HttpStatusCode.OK)]
        [Category("19.3_Regression")]
        public void Scenario108_Get_CasesFromCad(string jurisdiction, string fields, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : CryWolfUtil.GetAgencyToken(jurisdiction);
            CryWolfUtil.SetDBName(jurisdiction);

            IRestResponse restResponse = GetCasesFromCad(token, fields, "");

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            var fromCad = Models.v1.FromCad.FromJson(restResponse.Content);
        }

        [TestCase("Dallas", "*", HttpStatusCode.OK)]
        [TestCase("Dallas", "ClearanceCode", HttpStatusCode.OK)]
        [TestCase("Dallas", "*", HttpStatusCode.NotFound)]
        [TestCase("Dallas", "*", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "*", HttpStatusCode.OK)]
        [TestCase("HighPoint", "ClearanceCode", HttpStatusCode.OK)]
        [TestCase("HighPoint", "*", HttpStatusCode.NotFound)]
        [TestCase("HighPoint", "*", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", "", HttpStatusCode.BadRequest)]
        [Category("19.3_Regression")]
        public void Scenario107_Get_CaseFromCad_ByPath(string jurisdiction, string fields, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : CryWolfUtil.GetAgencyToken(jurisdiction);
            CryWolfUtil.SetDBName(jurisdiction);
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            FromCad call = CreateCall(jurisdiction, alarmNumber);
            string id = String.Empty;

            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Unauthorized:
                    id = call.CaseNumber;
                    break;
                case HttpStatusCode.NotFound:
                    id = "201962812941";
                    break;
                default:
                    break;
            }

            PutCasesFromCad(token, call);

            IRestResponse restResponse = GetCasesFromCad(token, fields, id);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            var fromCad = httpStatusCode != HttpStatusCode.BadRequest ? Models.v1.FromCad.FromJson(restResponse.Content) : null;
        }

        [TestCase("Dallas", 1, HttpStatusCode.OK)]
        [TestCase("Dallas", 2, HttpStatusCode.OK)]
        [TestCase("Dallas", 0, HttpStatusCode.BadRequest)]
        [TestCase("Dallas", 2, HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", 1, HttpStatusCode.OK)]
        [TestCase("HighPoint", 2, HttpStatusCode.OK)]
        [TestCase("HighPoint", 0, HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", 2, HttpStatusCode.Unauthorized)]
        [Category("19.3_Regression")]
        public void Scenario110_Put_CasesFromCad(string jurisdiction, int callCount, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : CryWolfUtil.GetAgencyToken(jurisdiction);
            CryWolfUtil.SetDBName(jurisdiction);

            List<FromCad> calls = new List<FromCad>();

            for (int i = 0; i < callCount; i++)
            {
                string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
                calls.Add(CreateCall(jurisdiction, alarmNumber));
            }

            IRestResponse restResponse = PutCasesFromCad(token, calls);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas",HttpStatusCode.OK)]
        [TestCase("Dallas", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", HttpStatusCode.NotFound)]
        //[TestCase("Dallas", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", HttpStatusCode.OK)]
        [TestCase("HighPoint", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", HttpStatusCode.NotFound)]
        //[TestCase("HighPoint", HttpStatusCode.BadRequest)]
        [Category("19.3_Regression")]
        public void Scenario106_Delete_CasesFromCad(string jurisdiction, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : CryWolfUtil.GetAgencyToken(jurisdiction);
            CryWolfUtil.SetDBName(jurisdiction);
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            FromCad call = CreateCall(jurisdiction, alarmNumber);
            string id = String.Empty;

            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Unauthorized:
                    id = call.CaseNumber;
                    break;
                case HttpStatusCode.NotFound:
                    id = "201962812941";
                    break;
                case HttpStatusCode.BadRequest:
                    id = string.Empty;
                    break;
                default:
                    break;
            }

            PutCasesFromCad(token, call);

            IRestResponse restResponse = DeleteCaseFromCad(token, id);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        private FromCad CreateCall(string jurisdiction, string alarmNumber)
        {
            return new FromCad
            {
                CaseNumber = CryWolfUtil.GetUniqueCaseNumber(),
                StreetNumber = CryWolfUtil.GetStreetNum(alarmNumber),
                StreetName = CryWolfUtil.GetStreetName(alarmNumber),
                IncidentDateTime = DateTime.Now.AddHours(-3),
                TimeDispatched = DateTime.Now.AddHours(-2.5),
                TimeOnScene = DateTime.Now.AddHours(-2),
                TimeCleared = DateTime.Now.AddHours(-1.5),
                CallTakerInformation = ProcessAlarmValues.CallTakerInfo,
                ClearanceCode = "5F",
                OfficerComments = "FALSE NO ONE THERE",

                DispatchCode = jurisdiction.ToLower() != "highpoint" ? CryWolfUtil.GetValidDispatch() : null
            };
        }

    }
}
