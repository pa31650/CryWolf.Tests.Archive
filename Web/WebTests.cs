using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Utils;
using Utils.PersonFactory;
using Web.PageObjects;
using Web.PageObjects.CitizenPortal;
using Web.PageObjects.Web;
using Web.PageObjects.WebExternal;
using Web.PageObjects.WebMobile;
using static Utils.CommonTestSettings;

namespace Web
{
    [TestFixture]
    public partial class Scenarios
    {
        protected static IWebDriver driver;
        CommonElements commonElements = new CommonElements();

        private static DatabaseReader data = new DatabaseReader();
        private static SetUp setUp = new SetUp();

        private List<String> scenariosTested = new List<String>();
        private string res;

        //exit status to determine if test was terminated due to pass or failure
        private static bool exitStatus = false;
        private static bool status = false;
        private static string primeKey;
        private string arguments;
        public TestContext TestContext { get; set; }

        [SetUp]
        public void TestInit()
        {
            if (CommonTestSettings.WriteToSQL)
            {
                data.AddToScenarioList(primeKey, TestContext.CurrentContext.Test.MethodName);
                scenariosTested.Add(TestContext.CurrentContext.Test.MethodName);
            }

            driver = setUp.Setup(CommonTestSettings.RunLocation);

            ReportBuilder.getStartTime();
        }

        [TearDown]
        public void TestCleanup()
        {
            ResultState resultState = TestContext.CurrentContext.Result.Outcome;
            if (resultState != ResultState.Success)
            {
                Library.TakeScreenShot(driver, "Browser in Fail state", out string filepath);
                ReportBuilder.ArrayBuilder($"Current test outcome: {resultState.Status.ToString()}", false, "Result Outcome");
            }

            TestContext.WriteLine($"Starting Test Cleanup on Test: {TestContext.CurrentContext.Test.MethodName}");

            setUp.Teardown(driver, exitStatus);

            ReportBuilder.getEndTime();
            Library.ReportResults(TestContext.CurrentContext.Test.Name, TestContext.CurrentContext.Test.MethodName, out res, primeKey, scenariosTested);
            data.UpdateEndTimeAndBuildVersion(primeKey,CommonTestSettings.BuildVersionWeb);
        }

        [Category("18.5_Regression")]
        [Category("Smoke")]
        [Property("test_id","225890")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario014_UpdateCitizenInformation(string agency)
        {
            #region Variable Assignment
            string userName = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string password = CryWolfUtil.GetCitizenPassword(userName);
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region TestData
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);
            
            exitStatus = homePage.Login(userName, password);

            Library.TakeScreenShot(driver, "Login", out string path);
            ReportBuilder.ArrayBuilder($"Login to {driver.Url} as {userName}", exitStatus, "Login Validation");
            Assert.IsTrue(exitStatus);

            //Click Update
            UserMain userMain = new UserMain(driver);

            userMain.NavigateToUpdatePage();
            Library.TakeScreenShot(driver, "Navigate to Update Page", out path);
            //TestContext.AddTestAttachment(path);

            Update update = new Update(driver);
            CommonFields commonFields = new CommonFields(driver);

            exitStatus = update.ValidateAlarmedLocationState();
            ReportBuilder.ArrayBuilder("Alarmed Location State field is dropdown & disabled", exitStatus, update.categoryName);
            Assert.IsTrue(exitStatus, "Alarmed Location State field is dropdown & disabled");

            //update.ChangeAlarmedLocationPhone1
            //update.ChangeAlarmedLocationPhone2
            //update.ChangeAlarmedLocationEmail
            //update.SetEmailCorrespondence

            //update.ChangeMailingName

            //update.ChangeMainingStreet
            //update.ChangeMailingCity

            exitStatus = update.ValidateMailingState();
            ReportBuilder.ArrayBuilder("Mailing State field is dropdown & contains expected state abbreviations", exitStatus, update.categoryName);
            Assert.IsTrue(exitStatus);

            //update.ChangeMailingState
            //update.ChangeMailingZip

            //update.ChangeMailingPhone1
            //update.ChangeMailingPhone2
            //update.ChangeMailingPhone3
            //update.ChangeMailingPhone4
            //update.ChangeMailingEmail

            //update.ChangeContact1
            exitStatus = commonFields.ValidateContact1State();
            Library.TakeScreenShot(driver, "Contact 1 State", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder("Contact 1 State field is dropdown & contains expected state abbreviations", exitStatus, update.categoryName);
            Assert.IsTrue(exitStatus);

            //update.ChangeContact2
            exitStatus = commonFields.ValidateContact2State();
            Library.TakeScreenShot(driver, "Contact 2 State", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder("Contact 2 State field is dropdown & contains expected state abbreviations", exitStatus, update.categoryName);
            Assert.IsTrue(exitStatus);

            //update.ChangeAlarmCompaniesMonitored
            //update.ChangeAlarmCompaniesSold
            //update.ChangeAlarmCompaniesServiced
            //update.ChangeAlarmCompaniesInstalled

            //update.ChangeSpecialNotes
            #endregion

            #region Validation
            #endregion

            driver.Quit();
        }

        [Category("18.5_Regression")]
        [Property("test_id", "226301,226299")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario015_ValidateDobField(string agency)
        {
            #region Variable Assignment
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string userName = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string password = CryWolfUtil.GetCitizenPassword(userName);
            string nonNumberInput = "test!()";

            DateTime minorDOB = DateTime.Today.AddYears(-17);
            DateTime methuselah = DateTime.Today.AddYears(-141);
            #endregion

            #region TestData
            TestContext.WriteLine($"User: {userName} was selected for testing");
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            exitStatus = homePage.Login(userName, password);

            Library.TakeScreenShot(driver, "Login", out string path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Login to {driver.Url} as {userName}", exitStatus, "Login Validation");
            
            Assert.IsTrue(exitStatus);

            //Click Update
            UserMain userMain = new UserMain(driver);

            userMain.NavigateToUpdatePage();

            Update update = new Update(driver);
            CommonFields commonFields = new CommonFields(driver);

            exitStatus = commonFields.ValidateDobField(nonNumberInput);

            Library.TakeScreenShot(driver, "Update -- DOB Letters-Special Characters", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Update -- D.O.B. field does not accept letters/special characters Input: {nonNumberInput}", exitStatus, update.categoryName);
            Assert.IsTrue(exitStatus);

            driver.Navigate().Refresh();

            exitStatus = commonFields.ValidateDobField(minorDOB.ToShortDateString());

            Library.TakeScreenShot(driver, "Update -- Minor Birthdate", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Update -- D.O.B. field does not accept minor birthdates Input: {minorDOB}", exitStatus, update.categoryName);
            Assert.IsTrue(exitStatus);

            driver.Navigate().Refresh();

            exitStatus = commonFields.ValidateDobField(methuselah.ToShortDateString());

            Library.TakeScreenShot(driver, "Update -- Age over 140", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Update -- D.O.B. field does not accept birthdates older than 140 Input: {methuselah}", exitStatus, update.categoryName);
            Assert.IsTrue(exitStatus);

            driver.Url = URL;

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);

            exitStatus = commonFields.ValidateDobField(nonNumberInput);

            Library.TakeScreenShot(driver, "Register -- DOB Letters-Special Characters", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Register -- D.O.B. field does not accept letters/special characters Input: {nonNumberInput}", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);

            driver.Navigate().Refresh();

            exitStatus = commonFields.ValidateDobField(minorDOB.ToShortDateString());

            Library.TakeScreenShot(driver, "Register -- Minor Birthdate", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Register -- D.O.B. field does not accept minor birthdates Input: {minorDOB}", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);

            driver.Navigate().Refresh();

            exitStatus = commonFields.ValidateDobField(methuselah.ToShortDateString());

            Library.TakeScreenShot(driver, "Register -- Age over 140", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Register -- D.O.B. field does not accept birthdates older than 140 Input: {methuselah}", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            #region Validation
            #endregion

            driver.Quit();
        }

        [Category("18.5_Regression")]
        [Property("test_id", "225890")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario016_RegisterNewCitizen(string agency)
        {
            #region Variable Assignment
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string defaultState = CryWolfUtil.GetDefaultState();
            #endregion

            #region TestData
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            exitStatus = commonFields.ValidateAlarmedLocationState(defaultState);
            Library.TakeScreenShot(driver, "Alarmed Location State", out string path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Alarmed Location State field is dropdown and default value: {defaultState} is selected", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = commonFields.ValidateMailingState(defaultState);
            Library.TakeScreenShot(driver, "Mailing Information State", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Mailing State field is dropdown and default value: {defaultState} is selected", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = commonFields.ValidateContact1State();
            Library.TakeScreenShot(driver, "Contact 1 State", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Contact 1 State field is dropdown and contains expected state values", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = commonFields.ValidateContact1State();
            Library.TakeScreenShot(driver, "Contact 2 State", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Contact 2 State field is dropdown and contains expected state values", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            #region Validation
            #endregion

            driver.Quit();
        }

        [Category("18.5_Regression")]
        [Property("test_id", "225890")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario017_ValidateDlFields(string agency)
        {
            #region Variable Assignment
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string password = CommonTestSettings.pw;
            #endregion

            #region TestData
            Person person = new Person();
            Address address = new Address();
            if (address.City == "")
            {
                address.SetCityAsDefault();
            }
            #endregion
            
            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);
            onlineRegistration.UseAlarmedLocationInformation(true);
            onlineRegistration.CreatePassword(password);
            onlineRegistration.AcceptTermsAndConditions();
            onlineRegistration.SubmitRegistrationInformation();
            #endregion

            #region Validation
            exitStatus = onlineRegistration.ValidateDlStateReq();
            Library.TakeScreenShot(driver, "DL State Required", out string path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Drivers License State is Required", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);

            commonFields.ChooseDLState(address.State);

            onlineRegistration.CreatePassword(password);
            onlineRegistration.AcceptTermsAndConditions();
            onlineRegistration.SubmitRegistrationInformation();

            exitStatus = onlineRegistration.ValidateDlNumberReq();
            Library.TakeScreenShot(driver, "DL Number Required", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Drivers License Number is Required", exitStatus, onlineRegistration.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            driver.Quit();
        }

        [Category("19.1_Regression")]
        [Property("test_id", "342909")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario018_PayOpenInvoiceFIS(string agency)
        {
            #region Variable Assignment
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region TestData
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            string password = CryWolfUtil.GetCitizenPassword(alarmNumber);
            string invoiceNo = CryWolfUtil.GetInvoiceNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            string address = CryWolfUtil.GetStreetAddress(alarmNumber);
            string state = CryWolfUtil.GetDefaultState();
            string city = CryWolfUtil.GetCity(alarmNumber);
            string zip = CryWolfUtil.GetZipCode(alarmNumber);
            string name = CryWolfUtil.GetNameOnAccount(alarmNumber);
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            exitStatus = homePage.Login(alarmNumber, password);
            Library.TakeScreenShot(driver, "Login", out string path);
            ReportBuilder.ArrayBuilder($"Login to {driver.Url} as {alarmNumber}", exitStatus, "Login Validation");
            Assert.IsTrue(exitStatus);

            UserMain userMain = new UserMain(driver);
            CommonFields commonFields = new CommonFields(driver);

            userMain.NavigatetoPayOnlinePage();

            PayOnline payOnline = new PayOnline(driver);
            payOnline.SelectOpenInvoice(invoiceNo);
            payOnline.SubmitInvoices();

            FISJump fISJump = new FISJump(driver);
            fISJump.JumpToFIS();

            FISPaymentEntry fISPaymentEntry = new FISPaymentEntry(driver);

            exitStatus = fISPaymentEntry.VerifyLanding();
            Library.TakeScreenShot(driver, "Payment Provider landing page", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Payment Provider landing page displayed", exitStatus, "3rd Party Redirect");
            Assert.IsTrue(exitStatus);

            fISPaymentEntry.ProcessPayment(name, address, city, state, zip);
            #endregion

            #region Validation
            #endregion

            driver.Quit();
        }

        [Category("19.1_Regression")]
        [Property("test_id", "317590")]
        [Author("Paul Atkins")]
        [TestCase("NV-Washoe")]
        public void Scenario019_ValidateJumpPageText(string agency)
        {
            #region Variable Assignment
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region TestData
            string userName = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            string password = CryWolfUtil.GetCitizenPassword(userName);
            string invoiceNo = CryWolfUtil.GetInvoiceNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            string expectedMessage = CryWolfUtil.GetAgencyMessage(agency);
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            exitStatus = homePage.Login(userName, password);
            Library.TakeScreenShot(driver, "Login", out string path);
            ReportBuilder.ArrayBuilder($"Login to {driver.Url} as {userName}", exitStatus, "Login Validation");
            Assert.IsTrue(exitStatus);

            UserMain userMain = new UserMain(driver);
            CommonFields commonFields = new CommonFields(driver);

            userMain.NavigatetoPayOnlinePage();

            PayOnline payOnline = new PayOnline(driver);
            payOnline.SelectOpenInvoice(invoiceNo);
            payOnline.SubmitInvoices();

            FISJump FisJump = new FISJump(driver);
            #endregion

            #region Validation
            exitStatus = FisJump.ValidateAgencyMessage(expectedMessage, out string actualMessage);
            Library.TakeScreenShot(driver, "Agency Jump Page Message", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Agency Jump Page message {Environment.NewLine}" +
                $"Expected: {expectedMessage}. {Environment.NewLine}" +
                $"Actual: {actualMessage}", exitStatus, FisJump.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            driver.Quit();
        }

        [Category("19.1_Regression")]
        [Property("test_id", "316451")]
        [Author("Paul Atkins")]
        //[TestCase("TX-Dallas", "iPhone X")]
        [TestCase("TX-Dallas", "iPhone X",true)]
        public void Scenario020_ProcessPayment_mobile(string agency,string device,bool isEncrypt)
        {
            #region Variable Assignment
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            CryWolfUtil.SetDBName(agency);
            #endregion

            #region TestData
            string userName = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            string invoiceNo = CryWolfUtil.GetInvoiceNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            string password = CryWolfUtil.GetCitizenPassword(userName);
            
            string expectedMessage = CryWolfUtil.GetAgencyMessage(agency);
            #endregion

            #region Process
            driver.Url = URL;

            cwMobile cwMobile = new cwMobile(driver);

            exitStatus = cwMobile.LoginAsCitizen(userName, password);
            Library.TakeScreenShot(driver, "Login as a citizen", out string path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Login to {driver.Url} as {userName}", exitStatus, "Login Validation");
            Assert.IsTrue(exitStatus);

            Account account = new Account(driver);
            account.NavigateToOutstanding();

            Outstanding outstanding = new Outstanding(driver);
            outstanding.PayInvoice(invoiceNo);

            PaymentRedirect paymentRedirect = new PaymentRedirect(driver);
            paymentRedirect.NavigateToPaymentProvider();

            FISPaymentEntry fISPaymentEntry = new FISPaymentEntry(driver);
            #endregion

            #region Validation
            exitStatus = fISPaymentEntry.VerifyLanding();
            Library.TakeScreenShot(driver, "Payment Provider landing page", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Payment Provider landing page displayed", exitStatus, "3rd Party Redirect");
            Assert.IsTrue(exitStatus);

            driver.Quit();
            #endregion
        }

        [Category("19.1_Regression")]
        [Property("test_id", "316452")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas","iPhone X",true)]
        public void Scenario021_ChangePasswordProcessPayment_mobile(string agency,string device,bool isEncrypt)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region TestData
            string userName = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            string invoiceNo = CryWolfUtil.GetInvoiceNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            string password = CryWolfUtil.GetCitizenPassword(userName);

            string expectedMessage = CryWolfUtil.GetAgencyMessage(agency);
            #endregion

            #region Process
            driver.Url = URL;

            cwMobile cwMobile = new cwMobile(driver);
            exitStatus = cwMobile.LoginAsCitizen(userName, password);
            Library.TakeScreenShot(driver, "Login", out string path);

            Account account = new Account(driver);
            account.NavigateToChangePassword();

            Library.TakeScreenShot(driver, "Change Password", out path);
            cwChangePassword cwChangePassword = new cwChangePassword(driver);

            exitStatus = cwChangePassword.ChangePassword(password, password);
            Library.TakeScreenShot(driver, "Change Password result", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Password Changed successfully", exitStatus, "Account page");
            Assert.IsTrue(exitStatus);

            account.NavigateToOutstanding();

            Outstanding outstanding = new Outstanding(driver);
            outstanding.PayInvoice(invoiceNo);

            PaymentRedirect paymentRedirect = new PaymentRedirect(driver);
            paymentRedirect.NavigateToPaymentProvider();

            FISPaymentEntry fISPaymentEntry = new FISPaymentEntry(driver);
            #endregion

            #region Validation
            exitStatus = fISPaymentEntry.VerifyLanding();
            Library.TakeScreenShot(driver, "Payment Provider landing page", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Payment Provider landing page displayed", exitStatus, "3rd Party Redirect");
            Assert.IsTrue(exitStatus);
            #endregion
        }

        [Category("19.1_Regression")]
        [Property("test_id", "316453")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas", "iPhone X",true)]
        public void Scenario022_NewUserProcessPayment_mobile(string agency, string device,bool isEncrypt)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);
            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region TestData
            Person person = new Person();
            Address address = new Address();
            

            if (address.City == "")
            {
                address.SetCityAsDefault();
            }

            if (address.ZipCode == "")
            {
                address.GetNewAddress();
            }

            //if (isEncrypt)
            //{
            //    CryWolfUtil.GetEncryptSiteData(out URL, out password);
            //}
            //else
            //{
                
            //    URL = CryWolfUtil.GetAgencyUrl(agency);
            //}
            #endregion

            #region Process
            driver.Url = URL;

            cwMobile cwMobile = new cwMobile(driver);
            cwMobile.CreateNewUser();

            NewAccount newAccount = new NewAccount(driver);
            exitStatus = newAccount.CompleteNewUserForm("Residential", password, person, address, out string AlarmNo);
            Library.TakeScreenShot(driver, "New User", out string path);
            ReportBuilder.ArrayBuilder($"New Account {AlarmNo} created successfully", exitStatus, "3rd Party Redirect");
            Assert.IsTrue(exitStatus);

            string invoiceNo = CryWolfUtil.GetInvoiceByAlarmNo(AlarmNo);

            Account account = new Account(driver);

            account.NavigateToOutstanding();

            Outstanding outstanding = new Outstanding(driver);
            outstanding.PayInvoice(invoiceNo);

            PaymentRedirect paymentRedirect = new PaymentRedirect(driver);
            paymentRedirect.NavigateToPaymentProvider();

            FISPaymentEntry fISPaymentEntry = new FISPaymentEntry(driver);
            #endregion

            #region Validation
            exitStatus = fISPaymentEntry.VerifyLanding();
            Library.TakeScreenShot(driver, "Payment Provider landing page", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Payment Provider landing page displayed", exitStatus, "3rd Party Redirect");
            Assert.IsTrue(exitStatus);
            #endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "355618")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario063_CitizenResetsPassword(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);
            string accountNum = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);
            homePage.Login(accountNum, password);
            Library.TakeScreenShot(driver, "Login", out string path);

            UserMain main = new UserMain(driver);
            main.ClickChangePassword();

            ChangePassword changePassword = new ChangePassword(driver);
            Library.TakeScreenShot(driver, "Change Password", out path);

            #endregion

            #region Validation
            exitStatus = changePassword.ChangeUserPassword(password);
            Library.TakeScreenShot(driver, "Change Password result", out path);
            ReportBuilder.ArrayBuilder($"Password Changed successfully", exitStatus, "Account page");
            Assert.IsTrue(exitStatus);
            #endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "368207")]
        [Author("Paul Atkins")]
        [TestCase("CA-Winnipeg")]
        public void Scenario064_CreateNewLicenseTwoContacts(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);

            Person person = new Person();
            Address address = new Address(true);
            //address.GetNewAddress();

            Thread.Sleep(500);
            Person contact1 = new Person();

            Thread.Sleep(500);
            Person contact2 = new Person();
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);
            Library.TakeScreenShot(driver, "Location Information", out string path);

            onlineRegistration.UseAlarmedLocationInformation(true);
            commonFields.ChooseDLState(address.State);
            commonFields.EnterDLNumber(person.DlNumber);
            Library.TakeScreenShot(driver, "Mailing & Billing Information", out path);

            onlineRegistration.EnterContact1Information(contact1);
            Library.TakeScreenShot(driver, "Contact 1 Information", out path);

            onlineRegistration.EnterContact2Information(contact2);
            Library.TakeScreenShot(driver, "Contact 2 Information", out path);

            onlineRegistration.CreatePassword(password);
            onlineRegistration.AcceptTermsAndConditions();
            onlineRegistration.SubmitRegistrationInformation();

            ValidateNewRegistration(out string accountNumber);
            #endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "368205")]
        [Author("Paul Atkins")]
        [TestCase("CA-Winnipeg")]
        public void Scenario065_CreateNewLicenseMonitoringCompany(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string monitor = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);

            Person person = new Person();
            Address address = new Address(true);
            Thread.Sleep(500);
            Person contact1 = new Person();
            Thread.Sleep(500);
            Person contact2 = new Person();
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);
            Library.TakeScreenShot(driver, "Location Information", out string path);

            onlineRegistration.UseAlarmedLocationInformation(true);
            commonFields.ChooseDLState(address.State);
            commonFields.EnterDLNumber(person.DlNumber);
            Library.TakeScreenShot(driver, "Mailing & Billing Information", out path);

            commonFields.SelectMonitoringCompany(monitor);

            onlineRegistration.CreatePassword(password);
            onlineRegistration.AcceptTermsAndConditions();
            onlineRegistration.SubmitRegistrationInformation();

            ValidateNewRegistration(out string accountNumber);
            # endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "368221")]
        [Author("Paul Atkins")]
        [TestCase("CA-Winnipeg")]
        public void Scenario066_CreateNewLicenseNoContactMonitor(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string monitor = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);

            Person person = new Person();
            Address address = new Address(true);
            //address.GetNewAddress();
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);
            Library.TakeScreenShot(driver, "Location Information", out string path);

            CompleteRegistration(person, address, password);

            exitStatus = onlineRegistration.ValidateMonitorAndContacts();
            ReportBuilder.ArrayBuilder($"New Account not created due to restrictions", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);
            # endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "368208")]
        [Author("Paul Atkins")]
        [TestCase("CA-Winnipeg")]
        public void Scenario067_CreateNewLicenseOneContact(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);


            Person person = new Person();
            Address address = new Address(true);
            //address.GetNewAddress();
            Thread.Sleep(500);
            Person contact1 = new Person();
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);
            Library.TakeScreenShot(driver, "Location Information", out string path);

            CompleteRegistration(person, address, password);

            exitStatus = onlineRegistration.ValidateMonitorAndContacts();
            ReportBuilder.ArrayBuilder($"New Account not created due to restrictions", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);
            # endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "355593")]
        [Author("Paul Atkins")]
        [TestCase("CA-Winnipeg")]
        public void Scenario077_DefaultLocaleControlsAddressLabels(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);

            string defaultLocale = agency.Equals("CA-Winnipeg") ? "en-ca" : "en-us";
            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string acURL = URL + "/ac";

            string accountNum = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string acNum = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            string path;

            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            exitStatus = onlineRegistration.ValidateAddressLabels(defaultLocale);
            Library.TakeScreenShot(driver, "Address Labels", out path);
            ReportBuilder.ArrayBuilder($"New Account address labels are Canadian", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);

            driver.Url = URL;
            homePage.Login(accountNum, password);

            UserMain userMain = new UserMain(driver);
            userMain.NavigateToUpdatePage();
            Update update = new Update(driver);

            exitStatus = update.ValidateAddressLabels(defaultLocale);
            Library.TakeScreenShot(driver, "Address Labels", out path);
            ReportBuilder.ArrayBuilder($"Existing Account address labels are Canadian", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);

            //Navigate to AC Add Account Page
            driver.Url = acURL;

            AcDefault acDefault = new AcDefault(driver);

            acDefault.Login(acNum, password);

            AcDefaultMain acDefaultMain = new AcDefaultMain(driver);

            acDefaultMain.RegisterNewAccount();

            AcAddArAccount acAddArAccount = new AcAddArAccount(driver);

            exitStatus = acAddArAccount.ValidateAddressLabels(defaultLocale);
            ReportBuilder.ArrayBuilder($"New Account address labels are Canadian", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);
            #endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "375667")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario079_NewUserEmailAllDefault(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);

            Person person = new Person();
            Address address = new Address(true);
            //address.GetNewAddress();
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);
            Library.TakeScreenShot(driver, "Location Information", out string path);

            CompleteRegistration(person, address, password);
            ValidateNewRegistration(out string accountNumber);

            string EmailMyLetters = SQLHandler.GetDatabaseValue($"SELECT sValue FROM ALARM_ARAC_EXTENDED WHERE AlarmNo = {accountNumber} AND sAction = 'EmailMyLetters'", "sValue", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            exitStatus = EmailMyLetters.Equals("Yes");
            ReportBuilder.ArrayBuilder($"New Account created with 'EmailMyLetters' set to {EmailMyLetters}", exitStatus, "Database Checkpoint");
            # endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "375710,375722")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario080_InactiveAlarmCompaniesExcluded(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string acURL = URL + "/ac";
            string acActive = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            string acInactive = CryWolfUtil.GetACNo(SQLStrings.InactiveAlarmCo);
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            //OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            status = !commonFields.SelectMonitoringCompany(acInactive);
            exitStatus = status;
            ReportBuilder.ArrayBuilder($"Inactive Monitoring Company [{acInactive}] excluded in Citizen Portal", status, "UI Validation");

            //Navigate to AC Add Account Page
            driver.Url = acURL;

            AcDefault acDefault = new AcDefault(driver);

            acDefault.Login(acActive, password);

            AcDefaultMain acDefaultMain = new AcDefaultMain(driver);

            acDefaultMain.RegisterNewAccount();

            AcAddArAccount acAddArAccount = new AcAddArAccount(driver);

            status = !acAddArAccount.SelectMonitoringCompany(acInactive);
            exitStatus = exitStatus && status;
            ReportBuilder.ArrayBuilder($"Inactive Monitoring Company [{acInactive}] excluded in Alarm Company Registration site", status, "UI Validation");
            Assert.IsTrue(exitStatus);
            #endregion
        }

        [Category("19.2_Regression")]
        [Property("test_id", "375712")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario081_ActivatedAlarmCompanyIncluded(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);
            MaintenanceHelper MaintenanceHelper = new MaintenanceHelper(CommonTestSettings.dbName);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string acURL = URL + "/ac";
            string acActive = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            string acInactive = CryWolfUtil.GetACNo(SQLStrings.InactiveAlarmCo);
            #endregion

            #region Process
            ReportBuilder.ArrayBuilder($"Alarm Company: [{acInactive}] is marked as Closed", true, "Test Data");

            MaintenanceHelper.SetAcStatusActive(acInactive);
            ReportBuilder.ArrayBuilder($"Alarm Company: [{acInactive}] has been marked as Active", true, "Test Data");

            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            CommonFields commonFields = new CommonFields(driver);

            status = commonFields.SelectMonitoringCompany(acInactive);
            exitStatus = status;
            ReportBuilder.ArrayBuilder($"Now Active Monitoring Company [{acInactive}] included in Citizen Portal", status, "UI Validation");

            //Navigate to AC Add Account Page
            driver.Url = acURL;

            AcDefault acDefault = new AcDefault(driver);

            acDefault.Login(acActive, password);

            AcDefaultMain acDefaultMain = new AcDefaultMain(driver);

            acDefaultMain.RegisterNewAccount();

            AcAddArAccount acAddArAccount = new AcAddArAccount(driver);

            status = acAddArAccount.SelectMonitoringCompany(acInactive);
            exitStatus = exitStatus && status;
            ReportBuilder.ArrayBuilder($"Now Active Monitoring Company [{acInactive}] included in Alarm Company Registration site", status, "UI Validation");
            Assert.IsTrue(exitStatus);

            MaintenanceHelper.SetAcStatusClosed(acInactive);
            ReportBuilder.ArrayBuilder($"Alarm Company: [{acInactive}] has been marked as Closed", true, "Test Data");
            #endregion
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Dallas"), Property("test_id", "372737")]
        public void Scenario084_SQL_Injection(string agency)
        {
            #region Variable Assignment
            CryWolfUtil.SetDBName(agency);
            MaintenanceHelper MaintenanceHelper = new MaintenanceHelper(CommonTestSettings.dbName);

            //string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string acURL = URL + "/ac";
            string acNum = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            string sqlInject = "' or 1=1;--";
            string accountNum = "714913";
            #endregion

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            //attempt illegal login
            exitStatus = homePage.Login(sqlInject, sqlInject);
            ReportBuilder.ArrayBuilder("Attempt to login illegally by injecting SQL into the Account Number field", !exitStatus, "SQL Injection");

            driver.Navigate().Refresh();

            exitStatus = homePage.Login(accountNum, sqlInject);
            ReportBuilder.ArrayBuilder("Attempt to login illegally by injecting SQL into the Password field", !exitStatus, "SQL Injection");

            //Navigate to AC Add Account Page
            driver.Url = acURL;

            AcDefault acDefault = new AcDefault(driver);

            exitStatus = acDefault.Login(sqlInject, sqlInject);
            ReportBuilder.ArrayBuilder("Attempt to login illegally by injecting SQL into the Account Number field", !exitStatus, "SQL Injection");

            driver.Navigate().Refresh();

            exitStatus = acDefault.Login(acNum, sqlInject);
            ReportBuilder.ArrayBuilder("Attempt to login illegally by injecting SQL into the Password field", !exitStatus, "SQL Injection");
            exitStatus = !exitStatus;
            #endregion
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("TX-Dallas")]
        public void Scenario088_User_LoginWithOtherInvoice(string agency)
        {
            #region MyRegion
            string password = CryWolfUtil.GetInvoiceByAlarmNo(CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen));
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string accountNum = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            #endregion

            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            //attempt illegal login
            exitStatus = !homePage.Login(accountNum, password);
            Library.TakeScreenShot(driver, "Login Attempt", out string path);
            ReportBuilder.ArrayBuilder("Attempt to login with another user's invoice number", exitStatus, "Login Validation");
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [Test]
        public void Scenario090_DesktopUser_LogsIn_AdminPortal()
        {
            string agency = Dallas.agencyCode;
            string signon = "admin";
            string password = DesktopPw;

            driver.Url = CommonTestSettings.AdminPortal;
            Library.TakeScreenShot(driver, "Clock Home Page", out string path);

            AdminPortalLogin(agency, signon, password);
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Dallas")]
        public void Scenario091_NoLocation_CausesAlert(string agency)
        {
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);

            Person person = new Person();
            Address address = new Address(true);
            //address.GetNewAddress();

            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterAlarmed_No_Location(person, address);
            Library.TakeScreenShot(driver, "Location Information", out string path);

            CompleteRegistration(person, address, password);

            try
            {
                string message = driver.SwitchTo().Alert().Text;
                driver.SwitchTo().Alert().Accept();
                Library.TakeScreenShot(driver, "No Location Alert", out path);
                ReportBuilder.ArrayBuilder($"Alert encountered with message: [{message}]", true, "Alert Validation");
            }
            catch (NoAlertPresentException)
            {
                ReportBuilder.ArrayBuilder("Expected Alert message did not appear", false, "Alert Validation");
                Library.TakeScreenShot(driver, "No Location Alert", out path);
                throw;
            }
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Dallas")]
        public void Scenario092_InactiveAlarmCompanyOnProfile(string agency)
        {
            #region Variable Assignment
            string acInactive = CryWolfUtil.GetACNo(SQLStrings.InactiveAlarmCo);
            string userName = CryWolfUtil.GetAlarmNoByAC(acInactive);
            string password = CryWolfUtil.GetCitizenPassword(userName);
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            #endregion

            #region TestData
            #endregion

            #region Process
            driver.Url = URL;

            HomePageLogin(userName, password);

            //Click Update
            UserMain userMain = new UserMain(driver);

            userMain.NavigateToUpdatePage();
            Library.TakeScreenShot(driver, "Navigate to Update Page", out string path);

            CommonFields commonFields = new CommonFields(driver);

            exitStatus = commonFields.ValidateMonitoringCompany(acInactive);
            Library.TakeScreenShot(driver, "Validate Monitoring Company", out path);
            ReportBuilder.ArrayBuilder($"Inactive Monitoring Company found [{acInactive}]", true, "UI Validation");

            exitStatus = commonFields.SelectSoldByCompany(acInactive);
            Library.TakeScreenShot(driver, "Select Sold By Company", out path);
            ReportBuilder.ArrayBuilder($"Inactive Sold By Company selected [{acInactive}]", true, "UI Validation");

            exitStatus = commonFields.SelectServicedByCompany(acInactive);
            Library.TakeScreenShot(driver, "Select Serviced By Company", out path);
            ReportBuilder.ArrayBuilder($"Inactive Serviced By Company selected [{acInactive}]", true, "UI Validation");

            exitStatus = commonFields.SelectInstalledByCompany(acInactive);
            Library.TakeScreenShot(driver, "Select Installed By Company", out path);
            ReportBuilder.ArrayBuilder($"Inactive Installed By Company selected [{acInactive}]", true, "UI Validation");
            #endregion
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Dallas")]
        public void Scenario093_AlarmCompany_Login_ChangePassword(string agency)
        {
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string acURL = URL + "/ac";
            string acNum = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            string password = CommonTestSettings.pw;

            driver.Url = acURL;

            AcDefault acDefault = new AcDefault(driver);

            exitStatus = acDefault.Login(acNum, password);
            Library.TakeScreenShot(driver, "Alarm Company Login", out string path);

            ReportBuilder.ArrayBuilder($"Login to {driver.Url} as {acNum}", exitStatus, "Login Validation");
            Assert.IsTrue(exitStatus);

            AcDefaultMain acDefaultMain = new AcDefaultMain(driver);

            exitStatus = acDefaultMain.ChangePassword(password, password);
            ReportBuilder.ArrayBuilder($"Alarm Company [{acNum}] changes password", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Dallas")]
        public void Scenario094_User_NeighborhoodEyes_EnrollUnenroll(string agency)
        {
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);

            Person person = new Person();
            Address address = new Address(true);
            //address.GetNewAddress();

            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);
            commonFields.EnrollInNeighborhoodEyes();
            Library.TakeScreenShot(driver, "Citizen Enrolled in Neighborhood Eyes", out string path);

            CompleteRegistration(person, address, password);
            ValidateNewRegistration(out string accountNumber);

            UserMain userMain = new UserMain(driver);
            userMain.NavigateToUpdatePage();

            Update update = new Update(driver);
            commonFields = new CommonFields(driver);

            commonFields.UnenrollInNeighborhoodEyes();
            Library.TakeScreenShot(driver, "Citizen Unenrolled in Neighborhood Eyes", out path);
            update.SubmitRegistrationInformation();

            driver.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Dallas")]
        public void Scenario095_User_NeighborhoodEyes_Enroll(string agency)
        {
            CryWolfUtil.SetDBName(agency);

            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);

            Person person = new Person();
            Address address = new Address(true);
            //address.GetNewAddress();

            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.EnterResidentialLocation(person, address);
            commonFields.EnrollInNeighborhoodEyes();
            Library.TakeScreenShot(driver, "Citizen Enrolled in Neighborhood Eyes", out string path);

            CompleteRegistration(person, address, password);
            ValidateNewRegistration(out string accountNumber);

            driver.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Dallas")]
        public void Scenario096_User_HasCamera_Registrations(string agency)
        {
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
            commonFields.NotifyCamera();
            Library.TakeScreenShot(driver, "Citizen has a camera", out string path);

            CompleteRegistration(person, address, password);
            ValidateNewRegistration(out string accountNumber);

            driver.Quit();
        }

        

        [Category("19.3_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Winnipeg",false)]
        [TestCase("Dallas", true)]
        public void Scenario111_DisabledACFields_DoNotDisplay(string agency, bool isDisplayed)
        {
            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string acURL = URL + "/ac";

            string accountNum = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string acNum = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            string path;

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            CommonFields commonFields = new CommonFields(driver);

            exitStatus = commonFields.ValidateInstalledByField(isDisplayed);
            Library.TakeScreenShot(driver, "Installed By User Register", out path);
            ReportBuilder.ArrayBuilder($"Installed By field displayed on Registration Page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Installed By field displayed on Registration Page");

            exitStatus = commonFields.ValidateServicedByField(isDisplayed);
            Library.TakeScreenShot(driver, "Serviced By User Register", out path);
            ReportBuilder.ArrayBuilder($"Serviced By field displayed on Registration Page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Serviced By field displayed on Registration Page");

            driver.Url = URL;
            homePage.Login(accountNum, password);

            UserMain userMain = new UserMain(driver);
            userMain.NavigateToUpdatePage();
            Update update = new Update(driver);

            exitStatus = update.ValidateInstalledByField(isDisplayed);
            Library.TakeScreenShot(driver, "Installed By User Update", out path);
            ReportBuilder.ArrayBuilder($"Installed By field displayed on Update Page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Installed By field displayed on Update Page");

            exitStatus = update.ValidateServicedByField(isDisplayed);
            Library.TakeScreenShot(driver, "Serviced By User Update", out path);
            ReportBuilder.ArrayBuilder($"Serviced By field displayed on Update Page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Serviced By field displayed on Update Page");

            //Navigate to AC Add Account Page
            driver.Url = acURL;

            AcDefault acDefault = new AcDefault(driver);

            acDefault.Login(acNum, password);

            AcDefaultMain acDefaultMain = new AcDefaultMain(driver);

            acDefaultMain.RegisterNewAccount(); 

            AcAddArAccount acAddArAccount = new AcAddArAccount(driver);

            exitStatus = acAddArAccount.ValidateInstalledByField(isDisplayed);
            Library.TakeScreenShot(driver, "Installed By AC Register", out path);
            ReportBuilder.ArrayBuilder($"Installed By field displayed on Update Page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Installed By field displayed on Update Page");

            exitStatus = acAddArAccount.ValidateServicedByField(isDisplayed);
            Library.TakeScreenShot(driver, "Serviced By AC Register", out path);
            ReportBuilder.ArrayBuilder($"Serviced By field displayed on Update Page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Serviced By field displayed on Update Page");

            driver.Quit();
            #endregion
        }

        [TestCase("Winnipeg")]
        [Category("19.3_Regression")]
        [Author("Paul Atkins")]
        public void Scenario112_DisabledACFields_UpdateDoesNotOverwrite(string agency)
        {
            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);

            string accountNum = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizenWAcServicerInstaller);
            string servicedBy = CryWolfUtil.GetServicedByAlarmNo(accountNum);
            string installedBy = CryWolfUtil.GetInstalledByAlarmNo(accountNum);
            string path;

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.Login(accountNum, password);

            UserMain userMain = new UserMain(driver);
            userMain.NavigateToUpdatePage();
            CommonFields commonFields = new CommonFields(driver);

            Update update = new Update(driver);

            commonFields.EnterDLNumber("35726162");
            Library.TakeScreenShot(driver, "DL Number Update", out path);
            ReportBuilder.ArrayBuilder($"DL Number was added/updated", true, "UI Validation");
            
            commonFields.ChooseDLState(CryWolfUtil.GetDefaultState());
            Library.TakeScreenShot(driver, "DL State Update", out path);
            ReportBuilder.ArrayBuilder($"DL State was added/updated", false, "UI Validation");
            
            update.SubmitRegistrationInformation();

            string currentServicedBy = CryWolfUtil.GetServicedByAlarmNo(accountNum);
            exitStatus = servicedBy.Equals(currentServicedBy);
            ReportBuilder.ArrayBuilder($"Compare Serviced By before saving [{servicedBy}] to Current value [{currentServicedBy}]", exitStatus, "DB Validation");
            Assert.IsTrue(exitStatus, $"Compare Serviced By before saving to Current value");

            string currentInstalledBy = CryWolfUtil.GetServicedByAlarmNo(accountNum);
            exitStatus = installedBy.Equals(currentInstalledBy);
            ReportBuilder.ArrayBuilder($"Compare Installed By before saving [{installedBy}] to Current value [{currentInstalledBy}]", exitStatus, "DB Validation");
            Assert.IsTrue(exitStatus, $"Compare Serviced By before saving to Current value");
            #endregion
            driver.Quit();
        }

        [TestCase("Dallas")]
        [Category("19.3_Regression")]
        [Author("Paul Atkins")]
        public void Scenario113_Validate_PhoneFields(string agency)
        {
            string password = CommonTestSettings.pw;
            string URL = CryWolfUtil.GetAgencyUrl(agency);
            string acURL = URL + "/ac";

            string accountNum = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string acNum = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            string path;

            #region Process
            driver.Url = URL;

            HomePage homePage = new HomePage(driver);

            homePage.ClickRegisterOnline();

            CommonFields commonFields = new CommonFields(driver);

            exitStatus = commonFields.ValidatePhoneInputs("Registration Page");
            //Library.TakeScreenShot(driver, "Installed By User Register", out path);
            ReportBuilder.ArrayBuilder($"Validate Phone inputs on Registration page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Validate Phone inputs on Registration page");

            driver.Url = URL;
            homePage.Login(accountNum, password);

            UserMain userMain = new UserMain(driver);
            userMain.NavigateToUpdatePage();
            Update update = new Update(driver);

            exitStatus = commonFields.ValidatePhoneInputs("User Update Page");
            //Library.TakeScreenShot(driver, "Installed By User Register", out path);
            ReportBuilder.ArrayBuilder($"Validate Phone inputs on User Update page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Validate Phone inputs on User Update page");

            //Navigate to AC Add Account Page
            driver.Url = acURL;

            AcDefault acDefault = new AcDefault(driver);

            acDefault.Login(acNum, password);

            AcDefaultMain acDefaultMain = new AcDefaultMain(driver);

            acDefaultMain.RegisterNewAccount();

            AcAddArAccount acAddArAccount = new AcAddArAccount(driver);

            exitStatus = commonFields.ValidatePhoneInputs("AC User Registration Page");
            //Library.TakeScreenShot(driver, "Installed By User Register", out path);
            ReportBuilder.ArrayBuilder($"Validate Phone inputs on AC User Registration page", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, "Validate Phone inputs on AC User Registration page");

            driver.Quit();

            #endregion
        }

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            Console.WriteLine("Entering Class Initialize");
            if (CommonTestSettings.WriteToSQL)
            {
                primeKey = DatabaseReader.CreateParentRecord();
            }

            CryWolfUtil.KillAllCryWolfProcesses();
            CryWolfUtil.KillChrome();
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            Utility.KillExcelProcesses();
        }
    }
}
