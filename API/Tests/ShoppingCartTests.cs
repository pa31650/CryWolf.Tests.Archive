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
    class ShoppingCartTests : ShoppingCart
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

        [TestCase("Dallas", "69697",HttpStatusCode.OK)]
        [TestCase("Dallas", "0", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "-1", HttpStatusCode.BadRequest)]
        [TestCase("Dallas", "69697", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "999999999", HttpStatusCode.NotFound)]
        [TestCase("HighPoint", "328", HttpStatusCode.OK)]
        [TestCase("HighPoint", "0", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "-1", HttpStatusCode.BadRequest)]
        [TestCase("HighPoint", "69697", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", "999999999", HttpStatusCode.NotFound)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario050_Get_ShoppingCart(string jurisdiction,string id,HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;

            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = GetShoppingCart(token,id);

            var shoppingCart = ShoppingCart.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }
}
