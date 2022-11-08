using API.Models.pResponse;
using Desktop.PageObjects.CryWolf;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Utils;
using Utils.PersonFactory;
using Web.PageObjects;
using Web.PageObjects.CitizenPortal;
using Web.PageObjects.Web;

namespace Tests
{
    public class Tests
    {
        protected WindowsDriver<WindowsElement> session;
        protected WindowsDriver<WindowsElement> childSession;
        protected IWebDriver driver;

        private CommonElements commonElements = new CommonElements();
        private MaintenanceHelper MaintenanceHelper = new MaintenanceHelper(CommonTestSettings.Dallas.dbName);

        private static Process pWinAppDriver;
        private static readonly DatabaseReader data = new DatabaseReader();
        private static readonly ReportBuilder report = new ReportBuilder();
        //exit status to determine if test was terminated due to pass or failure
        private static bool exitStatus = false;
        private static bool status = false;
        private static string primeKey;
        private static string arguments;
        private static string res;

        private readonly SetUp setUp = new SetUp();
        private readonly List<string> scenariosTested = new List<string>();

        public TestContext TestContext { get; set; }

        [SetUp]
        public void TestInit()
        {
            ReportBuilder.getStartTime();

            if (CommonTestSettings.WriteToSQL)
            {
                data.AddToScenarioList(primeKey, TestContext.CurrentContext.Test.MethodName);
                scenariosTested.Add(TestContext.CurrentContext.Test.MethodName);
            }

            if (CommonTestSettings.RunLocation == "local")
            {
                exitStatus = (pWinAppDriver != null);
                ReportBuilder.ArrayBuilder("Start WinAppDriver", exitStatus, "Test Initialize/Setup");
                Assert.IsTrue(exitStatus);
            }
        }

        [TearDown]
        public void TestCleanup()
        {
            TestContext.WriteLine($"Starting Test Cleanup on Test: {Environment.NewLine}{TestContext.CurrentContext.Test.MethodName}");
            ResultState resultState = TestContext.CurrentContext.Result.Outcome;

            if ((exitStatus) && (resultState == ResultState.Success))
                ReportBuilder.ArrayBuilder("Test exited due to end of scenario", true, "Test Cleanup");
            else
            {
                if (driver != null)
                {
                    Library.TakeScreenShot(driver, "Browser in Fail state", out string filepath);
                }

                if (session != null)
                {
                    Library.TakeScreenShot(session, "App in Fail state [main session]", out string filepath);
                }

                if (childSession != null)
                {
                    Library.TakeScreenShot(childSession, "App in Fail state [child session]", out string filepath);
                }

                ReportBuilder.ArrayBuilder(TestContext.CurrentContext.Result.Message, false, "Result Message");
                ReportBuilder.ArrayBuilder(TestContext.CurrentContext.Result.StackTrace, false, "Stack Trace");
            }

            setUp.WinAppTeardown(session, exitStatus);
            setUp.Teardown(driver, exitStatus);

            Library.ReportResults(TestContext.CurrentContext.Test.Name, TestContext.CurrentContext.Test.MethodName, out res, primeKey, scenariosTested);
            data.UpdateEndTimeAndBuildVersion(primeKey, CommonTestSettings.BuildVersionDesktop);

            CryWolfUtil.KillChrome();
            CryWolfUtil.KillAllCryWolfProcesses();

            TestContext.WriteLine("Wrapping up Cleanup");
        }

        [TestCase("Dallas")]
        [Author("Paul Atkins")]
        [Category("19.2_Regression")]
        public void Scenario097_AdminResetsPassword_CitizenLogsIn(string agency)
        {
            #region Variable Assignment
            string newPassword = "Passw0rd!";
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveResident);
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region Desktop Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);

            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            Desktop.Scenarios.ReportOnAdminLogin(exitStatus, session);

            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);

            registrations.OpenAccountRegistration(alarmNumber);

            registrations.OpenResetPassword();
            exitStatus = registrations.ResetPassword(newPassword);
            ReportBuilder.ArrayBuilder("Reset Users Password", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);

            registrations.CloseRegistrationsWindow();
            main.Close();
            session.Quit();
            #endregion

            #region Web Process
            driver = setUp.Setup(CommonTestSettings.RunLocation);
            driver.Url = URL;

            Web.Scenarios.HomePageLogin(alarmNumber, newPassword, driver);

            ChangePassword changePassword = new ChangePassword(driver);

            exitStatus = changePassword.ChangeUserPassword(newPassword);
            ReportBuilder.ArrayBuilder("User changes password after admin resets", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);

            driver.Quit();
            #endregion
        }

        [TestCase("Dallas")]
        [Author("Paul Atkins")]
        [Category("19.2_Regression")]
        public void Scenario098_AlarmRegistrationCreatedDuringFaProcess(string agency)
        {
            #region Variable Assignment
            Address address = new Address(true);
            Person person = new Person();

            string streetAddress = $"{address.StreetNumber},{address.StreetName}";
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region Desktop Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);

            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            Desktop.Scenarios.ReportOnAdminLogin(exitStatus, session);

            main.OpenProcessAlarms();

            ProcessAlarms processAlarms = new ProcessAlarms(session);

            processAlarms.EnterFalseAlarm(streetAddress);
            processAlarms.AddAccount();

            processAlarms.CompleteAddAccount(person, address);
            exitStatus = processAlarms.ValidateAlarmNumber(out string AlarmNo);
            Library.TakeScreenShot(session, "Account Added to FA Processing window", out path);
            ReportBuilder.ArrayBuilder($"Account [{AlarmNo}] Created and added to FA Processing window", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);

            processAlarms.CloseAlarmProcessing();
            main.Close();

            session.Quit();
            #endregion

            #region Web Process
            string password = CryWolfUtil.GetInvoiceByAlarmNo(AlarmNo);
            driver = setUp.Setup(CommonTestSettings.RunLocation);
            driver.Url = URL;

            Web.Scenarios.HomePageLogin(AlarmNo, password, driver);

            driver.Quit();
            #endregion
        }

        [TestCase("Dallas")]
        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        public void Scenario099_AddAC_ACLogsIn_ChangePassword(string agency)
        {
            #region Variable Assignment
            Person person = new Person();
            Address address = new Address(true);
            person.LastName = CryWolfUtil.GetUniqueCaseNumber();
            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyAlarmCoUrl(agency);
            #endregion

            #region Desktop Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);

            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            Desktop.Scenarios.ReportOnAdminLogin(exitStatus, session);

            main.OpenProcessAlarmCompanies();

            AlarmCompanies alarmCompanies = new AlarmCompanies(session);

            alarmCompanies.EnterNewAccountInfo(person, address);
            alarmCompanies.AddRelatedPerson();

            Library.TakeScreenShot(session, "Enter Alarm Company Information", out path);

            alarmCompanies.UpdateAccount();
            exitStatus = alarmCompanies.ValidateNewAccount(out string accountNum);
            ReportBuilder.ArrayBuilder($"New Account created: [{accountNum}]", exitStatus, "UI Validation");

            //string accountNum = "AC226";
            alarmCompanies.OpenAlarmCompany(accountNum);
            alarmCompanies.OpenResetPassword();
            alarmCompanies.ResetPassword(password);
            alarmCompanies.Close();

            main.Close();

            session.Quit();
            #endregion

            #region Web Proces
            driver = setUp.Setup(CommonTestSettings.RunLocation);
            driver.Url = URL;

            AcDefault acDefault = new AcDefault(driver);

            acDefault.Login(accountNum, password);

            AcDefaultMain acDefaultMain = new AcDefaultMain(driver);

            Library.TakeScreenShot(driver, "AC Login and Reset Password", out path);
            exitStatus = acDefaultMain.ResetPassword(password, password);
            ReportBuilder.ArrayBuilder("AC User Logs in and is forced to reset Password", exitStatus, "UI Validation");

            driver.Quit();
            #endregion
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("HighPoint")]
        public void Scenario101_Post_AuthorizeNet_Success(string agency)
        {
            #region Variable Assignment
            string password = CommonTestSettings.pw;
            string cpURL = CryWolfUtil.GetAgencyUrl(agency);
            string responseCode = "1";
            string description = "test";
            driver = setUp.Setup(CommonTestSettings.RunLocation);
            #endregion

            #region Web Process
            CryWolfUtil.SetDBName(agency);

            Person person = new Person();
            Address address = new Address(true);

            driver.Url = cpURL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);

            onlineRegistration.EnterResidentialLocation(person, address);

            Web.Scenarios.CompleteRegistration(person, address, password, driver);
            Web.Scenarios.ValidateNewRegistration(out string accountNumber, driver);

            UserMain userMain = new UserMain(driver);
            userMain.NavigatetoPayOnlinePage();

            PayOnline payOnline = new PayOnline(driver);

            string invoiceNo = CryWolfUtil.GetInvoiceByAlarmNo(accountNumber);

            payOnline.SelectOpenInvoice(invoiceNo);
            payOnline.SubmitInvoices();

            AuthorizeNetJump authorizeNetJump = new AuthorizeNetJump(driver);
            authorizeNetJump.JumpToAuthorizeNet();

            driver.Quit();
            #endregion

            #region Variable Assignment
            string invoice = CryWolfUtil.GetInvoiceByAlarmNo(accountNumber);
            string cart = CryWolfUtil.GetCartByInvoice(invoice);
            string amount = CryWolfUtil.GetInvoiceAmount(invoice);

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            #endregion

            IRestResponse restResponse = AuthorizeNet.MockPaymentResponse(responseCode, cart, description, amount, accountNumber, description);

            actualStatusCode = restResponse.StatusCode;

            #region Reporting & Validation
            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            ReportBuilder.ArrayBuilder("Content of the response is blank", restResponse.Content.Equals(string.Empty), "Validate Response Content");
            Assert.IsEmpty(restResponse.Content);

            string note = CryWolfUtil.GetNoteByAccount(accountNumber);
            exitStatus = !note.Equals(null);
            ReportBuilder.ArrayBuilder($"Found Online Payment Notification for Account: [{accountNumber}] {Environment.NewLine}{note}", exitStatus, "Database Check");
            Assert.IsTrue(exitStatus, $"Note for Account [{accountNumber}] was not found");
            #endregion
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("HighPoint")]
        public void Scenario103_CitizenSelfRegisters_PayFee_ChangePassword(string agency)
        {
            driver = setUp.Setup(CommonTestSettings.RunLocation);
            string responseCode = "1";
            string description = "test";
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);

            Person person = new Person();
            Address address = new Address(true);

            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);

            Web.Scenarios.CompleteRegistration(person, address, password,driver);
            Web.Scenarios.ValidateNewRegistration(out string accountNumber,driver);

            UserMain userMain = new UserMain(driver);
            userMain.NavigatetoPayOnlinePage();

            PayOnline payOnline = new PayOnline(driver);

            string invoiceNo = CryWolfUtil.GetInvoiceByAlarmNo(accountNumber);

            payOnline.SelectOpenInvoice(invoiceNo);
            payOnline.SubmitInvoices();

            AuthorizeNetJump authorizeNetJump = new AuthorizeNetJump(driver);
            authorizeNetJump.JumpToAuthorizeNet();

            #region Variable Assignment
            string invoice = CryWolfUtil.GetInvoiceByAlarmNo(accountNumber);
            string cart = CryWolfUtil.GetCartByInvoice(invoice);
            string amount = CryWolfUtil.GetInvoiceAmount(invoice);

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            #endregion

            IRestResponse restResponse = AuthorizeNet.MockPaymentResponse(responseCode, cart, description, amount, accountNumber, description);

            actualStatusCode = restResponse.StatusCode;

            #region Reporting & Validation
            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            ReportBuilder.ArrayBuilder("Content of the response is blank", restResponse.Content.Equals(string.Empty), "Validate Response Content");
            Assert.IsEmpty(restResponse.Content);
            #endregion

            driver.Url = URL;

            homePage = new HomePage(driver);
            homePage.Login(accountNumber, password);

            userMain = new UserMain(driver);
            userMain.ClickChangePassword();

            ChangePassword changePassword = new ChangePassword(driver);
            exitStatus = changePassword.ChangeUserPassword(password);

            ReportBuilder.ArrayBuilder($"New User Changes Password", exitStatus, "Change Password");
            Assert.IsTrue(exitStatus, $"New User Changes Password failed");

            driver.Quit();
        }

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            Console.WriteLine("Entering Class Initialize");
            if (CommonTestSettings.WriteToSQL)
            {
                primeKey = DatabaseReader.CreateParentRecord();
            }

            if (CommonTestSettings.RunLocation == "local")
            {
                pWinAppDriver = Utility.StartWinAppDriver();
                exitStatus = (pWinAppDriver != null);
                Assert.IsTrue(exitStatus);
            }

            CryWolfUtil.KillChrome();
            CryWolfUtil.KillAllCryWolfProcesses();
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            if (CommonTestSettings.RunLocation == "local")
            {
                Utility.StopWinAppDriver();
            }

            Utility.KillExcelProcesses();
        }
    }
}