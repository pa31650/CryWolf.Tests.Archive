using Desktop.PageObjects.AutoProcess;
using Desktop.PageObjects.CryWolf;
using Desktop.PageObjects.Maintenance;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using Utils;
using Utils.PersonFactory;

namespace Desktop
{
    [TestFixture]
    public class Scenarios
    {
        protected static WindowsDriver<WindowsElement> session;
        protected static WindowsDriver<WindowsElement> childSession;

        private static Process pWinAppDriver;
        private static readonly DatabaseReader data = new DatabaseReader();
        private static readonly ReportBuilder report = new ReportBuilder();
        private MaintenanceHelper MaintenanceHelper = new MaintenanceHelper(CommonTestSettings.Dallas.dbName);

        private readonly SetUp setUp = new SetUp();
        private readonly List<string> scenariosTested = new List<String>();
        private string res;

        //exit status to determine if test was terminated due to pass or failure
        private static bool exitStatus = false;
        private static string primeKey;
        private string arguments;

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
            TestContext.WriteLine($"Starting Test Cleanup on Test: {TestContext.CurrentContext.Test.MethodName}");
            ResultState resultState = TestContext.CurrentContext.Result.Outcome;
            if ((exitStatus) && (resultState == ResultState.Success))
                ReportBuilder.ArrayBuilder("Driver exited due to end of scenario", true, "Test Cleanup");
            else
            {
                Library.TakeScreenShot(session, "App in Fail state", out string filepath);
                ReportBuilder.ArrayBuilder("Test was exited due to failure", false, "Test Cleanup");
            }

            ReportBuilder.getEndTime();

            Library.ReportResults(TestContext.CurrentContext.Test.Name, TestContext.CurrentContext.Test.MethodName, out res, primeKey, scenariosTested);
            data.UpdateEndTimeAndBuildVersion(primeKey,CommonTestSettings.BuildVersionDesktop);

            session.Quit();
            CryWolfUtil.KillAllCryWolfProcesses();
            TestContext.WriteLine("Wrapping up Cleanup");
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [Category("Smoke")]
        [Test]
        public void Scenario001_ConfigureHarrisCoDates(
            [Values("First Issue Last Expire")] string issueExpSetting,
            [Values("<Default>")]string agency)
        {
            #region Variable Assignment
            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);

            //
            Main main = new Main(session);

            exitStatus = main.ChooseDbLoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();
            Maintenance maintenance = new Maintenance(childSession);

            maintenance.OpenAccountRelatedSettings();

            AccountRelated accountRelated = new AccountRelated(childSession);
            exitStatus = accountRelated.SetIssueExpirationSettings(issueExpSetting, agency); //<--methods take screenshots
            accountRelated.CommitAccountRelatedSettings();
            #endregion

            #region Validation
            ReportBuilder.ArrayBuilder($"Locate and select Issue/Expiration option: [{issueExpSetting}]", exitStatus, "Configure Issue/Expiration dates");
            Assert.IsTrue(exitStatus);

            accountRelated.Close();
            maintenance.Close();

            main.Close();
            #endregion

            childSession.Quit();
            session.Quit();
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [TestCase("First Issue Last Expire", "Payment Received","Renew Permit","Active","<Default>")]
        public void Scenario002_ConfigurePaymentReceivedLetter(
            string issueExpSetting, string letterName, string letterUpdateAction, string expectedStatus,string agency)
        {
            #region Variable Assignment
                
            #endregion

            #region Test Data
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ExpiringCitizenOutstandingInvoice);

            DateTime issueInitial = CryWolfUtil.GetIssueDate(alarmNumber);
            DateTime expInitial = CryWolfUtil.GetExpDate(alarmNumber);
            DateTime issueToCheck = CryWolfUtil.ApplyRenewalRuleIssueDate(expInitial, issueExpSetting);
            DateTime expToCheck = CryWolfUtil.ApplyRenewalRuleExpirationDate(issueToCheck, issueExpSetting);

            List<KeyValuePair<string, string>> statusPairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Expired","Active"),
                new KeyValuePair<string, string>("Expiring","Active"),
                new KeyValuePair<string, string>("Expiring Manual","Active"),
                new KeyValuePair<string, string>("Pending","Active"),
                new KeyValuePair<string, string>("Web-Pending","Active"),
                new KeyValuePair<string, string>("Unregistered","Active")
            };

            MaintenanceHelper.SetIssueExpiration(issueExpSetting, agency);
            #endregion

            #region Process 
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);

            //Open CryWolf
            Main main = new Main(session);

            main.ChooseDatabase();

            //Login as Admin
            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin(); //<--Takes screenshot

            //Open CryWolfMaint
            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();
            Maintenance maintenance = new Maintenance(childSession);

            //Navigate to Form Letters
            maintenance.OpenFormLetters();

            AddEditLetters addEditLetters = new AddEditLetters(childSession);

            addEditLetters.SelectAgency(agency);
            //Check if letter exists
            if (addEditLetters.IsLetterPresent(letterName))
            {
                TestContext.WriteLine($"Letter {letterName} already exists");
            }
            else //create letter if it doesn't exist
            {
                addEditLetters.CreateNewLetter(
                    "<Default>", 
                    letterName, 
                    "Billing/Payments", 
                    "Responsible Party [Default]", 
                    "DefaultLetter", 
                    letterUpdateAction, 
                    statusPairs);
            }

            //Choose Letter for validation
            addEditLetters.SelectLetterName(letterName);

            //Validate the Actions tab for Payment Received letter
            exitStatus = addEditLetters.ValidateLetterActions(letterUpdateAction);

            Library.TakeScreenShot(childSession, "Actions Tab of Payment Received", out  path);
            ReportBuilder.ArrayBuilder("Validate the Actions tab for Payment Received letter", exitStatus, "Form Letters Window");
            Assert.IsTrue(exitStatus);

            //Validate the Status tab for Payment Received letter
            exitStatus = addEditLetters.ValidateLetterStatus(statusPairs);

            Library.TakeScreenShot(childSession, "Status Tab of Payment Received", out path);
            ReportBuilder.ArrayBuilder("Validate the Status tab for Payment Received letter", exitStatus, "Form Letters Window");
            Assert.IsTrue(exitStatus);

            //maintenance.OpenAccountRelatedSettings();

            //AccountRelated accountRelated = new AccountRelated(childSession);

            //exitStatus = accountRelated.SetIssueExpirationSettings(issueExpSetting, agency);//<--Takes screenshot

            //accountRelated.CommitAccountRelatedSettings();

            //Close CryWolfMaint
            maintenance.Close();
            childSession.Quit();

            //Navigate to Payments screen
            main.OpenProcessPayments();

            //Process the payment
            Payments payments = new Payments(session);

            payments.ChooseAllInvoices(alarmNumber);
            payments.CompletePaymentSendOptLetter(letterName);
            payments.Close();

            OutstandCorresp outstandingCorresp = new OutstandCorresp(session);

            outstandingCorresp.PrintCreatedLetters();
                
            //Open the Account window for processed payment
            main.OpenProcessRegistrations();

            //Enter the alarmNumber into the Account text box
            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);
            #endregion

            #region Validation
            //Validate Account info
            exitStatus = registrations.CheckAccountStatus(expectedStatus, out string actualStatus);

            Library.TakeScreenShot(session, $"Account Window for {alarmNumber}", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Validate the Account status is {expectedStatus} Status found: {actualStatus}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = registrations.CheckAccountIssueDate(issueToCheck, out string actualIssue);
            
            ReportBuilder.ArrayBuilder($"Validate the Account issue date is {issueToCheck} ; Date found: {actualIssue}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = registrations.CheckAccountExpDate(expToCheck, out string actualExp);
            ReportBuilder.ArrayBuilder($"Validate the Account expiration date is {expToCheck}; Date found: {actualExp}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);
            
            main.Close();
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Expiring","Payment Received","Active")]
        public void Scenario003_PermitRenewalwFalseAlarmBeforeRenewal(string outstandingCorrespondence,string letterName,string expectedStatus)
        {
            #region Variable Assignment
            #endregion

            #region TestData
            List<KeyValuePair<string, string>> statusPairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Active","Expiring"),
            };

            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ExpiringCitizenOutstandingInvoice);
            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //Open CryWolf
            Main main = new Main(session);

            main.ChooseDatabase();

            //Login as Admin
            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            OutstandCorresp outstanding = new OutstandCorresp(session);
            if (alarmNumber == null)
            {
                #region AutoProcess
                //Run AutoProcess
                main.OpenAutoProcess();

                childSession = CryWolfUtil.AttachToAutoProcess();
                AutoProcess autoProcess = new AutoProcess(childSession);

                //Validate Active to Expiring 
                autoProcess.ValidateAlarmedLocationRenewals(statusPairs);

                //Click Process (Count Only)
                autoProcess.ExecuteAutoProcess();

                childSession.Quit();
                #endregion

                #region Outstanding Correspondance
                main.OpenOutstandingCorrespondance();

                outstanding.ExecuteOustandingCorrespondence(outstandingCorrespondence);
                #endregion

                alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ExpiringCitizenOutstandingInvoice);
            }

            main.OpenProcessPayments();

            Payments payments = new Payments(session);

            payments.ChooseAllInvoices(alarmNumber);
            payments.CompletePaymentSendOptLetter(letterName);
            payments.Close();

            outstanding.PrintCreatedLetters();

            //Open the Account window for processed payment
            main.OpenProcessRegistrations();

            //Enter the alarmNumber into the Account text box
            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);

            //Validate Account info
            exitStatus = registrations.CheckAccountStatus(expectedStatus, out string actualStatus);
            Library.TakeScreenShot(session, $"Account Window for {alarmNumber}", out path);
            ReportBuilder.ArrayBuilder($"Validate the Account status is {expectedStatus}. Actual Status is {actualStatus}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);

            //Get FA count
            int faCount = registrations.GetFalseAlarmCount();

            //Get Alarm Registration Name
            string alarmName = registrations.GetNameOnAccount();

            //Get Alarm Location Address
            string alarmAddress = registrations.GetStreetAddressOnAccount();

            registrations.CloseRegistrationsWindow();

            //Process Alarms
            main.OpenProcessAlarms();

            ProcessAlarms cryWolfProcessAlarms = new ProcessAlarms(session);

            cryWolfProcessAlarms.EnterFalseAlarm(alarmAddress);
            cryWolfProcessAlarms.MatchAccount(alarmName);
            cryWolfProcessAlarms.ChargeFA();
            cryWolfProcessAlarms.CloseAlarmProcessing();

            //Validate Account's FA Count after Charging FA
            main.OpenProcessRegistrations();

            registrations.OpenAccountRegistration(alarmNumber);
            #endregion

            #region Validation
            int faCountToCheck = faCount + 1;
            exitStatus = faCountToCheck == registrations.GetFalseAlarmCount();

            Library.TakeScreenShot(session, $"Account Window for {alarmNumber}", out path);
            //TestContext.AddTestAttachment(path);
            ReportBuilder.ArrayBuilder($"Validate Account's FA Count ({faCountToCheck}) after Charging FA", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [TestCase("First Issue Last Expire", "PaymentAutoActions", "Expiring", "Expiring", "Payment Received", "No Warning","Active","<Default>")]

        public void Scenario004_SpecialActionsPaymentProcessing(
            string renewalRule, 
            string actionType, 
            string currentStatus, 
            string paidLetterType,
            string prepareLetter,
            string options,
            string expectedStatus,
            string agency)
        {
            #region Variable Assignment
            #endregion

            #region Test Data
            //Get Expiring Citizen
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ExpiringCitizenOutstandingInvoice);

            DateTime issueInitial = CryWolfUtil.GetIssueDate(alarmNumber);
            DateTime expInitial = CryWolfUtil.GetExpDate(alarmNumber);
            DateTime issueToCheck = CryWolfUtil.ApplyRenewalRuleIssueDate(expInitial, renewalRule);
            DateTime expToCheck = CryWolfUtil.ApplyRenewalRuleExpirationDate(issueToCheck,renewalRule);
            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //Open CryWolf
            Main main = new Main(session);

            main.ChooseDatabase();

            //Login as Admin
            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            MaintenanceHelper.SetSpecialAction(actionType, currentStatus, paidLetterType, prepareLetter, options);

            MaintenanceHelper.SetIssueExpiration(renewalRule, agency);

            //Launch CryWolfMaint
            //main.OpenCryWolfMaint();

            //childSession = CryWolfUtil.AttachToCryWolfMaint();
            //Maintenance maintenance = new Maintenance(childSession);

            //maintenance.OpenAccountRelatedSettings();

            //AccountRelated accountRelated = new AccountRelated(childSession);
            //exitStatus = accountRelated.SetIssueExpirationSettings(renewalRule,agency);

            //accountRelated.CommitAccountRelatedSettings();

            //maintenance.Close();
            //childSession.Quit();

            //Process renewal payment
            main.OpenProcessPayments();

            Payments payments = new Payments(session);

            payments.ChooseAllInvoices(alarmNumber);
            payments.CompletePaymentForm();
            payments.Close();

            main.OpenProcessRegistrations();

            //Validate Active, Updated Expiration
            //Enter the alarmNumber into the Account text box
            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);
            #endregion

            #region Validation
            //Validate Account info
            Library.TakeScreenShot(session, $"Account Window for {alarmNumber}", out path);
            exitStatus = registrations.CheckAccountStatus(expectedStatus, out string actualStatus);
            ReportBuilder.ArrayBuilder($"Validate Account Status is {expectedStatus} Actual Status: {actualStatus}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = registrations.CheckAccountIssueDate(issueToCheck, out string actualIssue);
            ReportBuilder.ArrayBuilder($"Validate Account issue date is {issueToCheck} Actual Date: {actualIssue}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = registrations.CheckAccountExpDate(expToCheck, out string actualExp);
            ReportBuilder.ArrayBuilder($"Validate Account expiration date is {expToCheck} Actual Date: {actualExp}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            #region Cleanup
            //Close Account window
            registrations.CloseRegistrationsWindow();

            MaintenanceHelper.DeleteSpecialAction(actionType, currentStatus, paidLetterType, prepareLetter, options);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Issue Mnth/Day", "N/A", 365, false, "<Default>")]
        public void Scenario008_ValidatePermitTermAlert(string timePeriod, string month, int days, bool dispatchCode, string agency)
        {
            #region Variable Assignment
            DateTime issueDate;
            DateTime newIssueDate;
            string alarmNumber;
            #endregion

            #region Test Data
            do
            {
                alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.CitizenWithOneFA);
                ReportBuilder.ArrayBuilder($"Account Number: [{alarmNumber}] found", true, "Test Data");

                issueDate = CryWolfUtil.GetIssueDate(alarmNumber);
                ReportBuilder.ArrayBuilder($"Initial Issue Date: [{issueDate}]", true, "Test Data");

                string incidentDate = CryWolfUtil.GetIncDateByAlarmNumberLetter(alarmNumber, "FA 1");
                ReportBuilder.ArrayBuilder($"Incident Date: [{incidentDate}]", true, "Test Data");

                newIssueDate = DateTime.Parse(incidentDate).AddDays(1);
                ReportBuilder.ArrayBuilder($"New Issue Date: [{newIssueDate}]", true, "Test Data");
            } while (issueDate.Equals(newIssueDate));

            MaintenanceHelper.SetAlarmCountingPeriod(agency, "<General>", timePeriod, month, days, dispatchCode);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Process
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);

            registrations.OpenAccountRegistration(alarmNumber);
            registrations.EditAccountIssueDate(newIssueDate.ToShortDateString());
            registrations.UpdateAccount();

            //reporting
            exitStatus = registrations.ValidatePermitChangeDialog(); //<--takes screenshot
            ReportBuilder.ArrayBuilder(
                $"Confirm Permit Period Change dialog displays when false alarm counts are impacted by period changes",
                exitStatus,
                registrations.categoryName);
            Assert.IsTrue(exitStatus);

            //change date back validate the other way
            registrations.OpenAccountRegistration(alarmNumber);
            registrations.EditAccountIssueDate(issueDate.ToShortDateString());
            registrations.UpdateAccount();
            #endregion

            #region Validation
            Library.TakeScreenShot(session, TestContext.CurrentContext.Test.MethodName, out path);
            //TestContext.AddTestAttachment(path);
            exitStatus = registrations.ValidatePermitChangeDialog();
            ReportBuilder.ArrayBuilder(
                $"Confirm Permit Period Change dialog displays when false alarm counts are impacted by period changes",
                exitStatus,
                registrations.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [TestCase("10012017")]
        [Author("Paul Atkins")]
        //[TestCase("HP_OSSI")]
        public void Scenario006_ProcessAlarmDataNoCharge(string format)
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);

            List<string> alarmNumbers = new List<string>
            {
                alarmNumber,
                alarmNumber
            };
            #endregion

            #region Prequel
            CryWolfUtil.CreateFaImportFile(alarmNumbers, out string filepath, format);
            #endregion

            #region TestData
            ///Nothing
            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenProcessAlarms();

            //Start import
            ProcessAlarms processAlarms = new ProcessAlarms(session);
            processAlarms.SetImportModeFile(format, filepath);

            Library.TakeScreenShot(session, "Process Alarm screen after Set Import Mode", out path);

            processAlarms.StartManualImport();

            Library.TakeScreenShot(session, "Process Alarm screen before matching account", out path);

            processAlarms.MatchAccount(CryWolfUtil.GetNameOnAccount(alarmNumbers[0]));
            processAlarms.ProcessNormally();

            //Skip first Alarm 
            int faCountFA1 = processAlarms.GetFACount();
            Library.TakeScreenShot(session, "Process Alarm window before skipping first alarm", out  path);
            processAlarms.SkipFA();

            //Process second Alarm
            Library.TakeScreenShot(session, "Process Alarm window before matching account", out path);
            processAlarms.MatchAccount(CryWolfUtil.GetNameOnAccount(alarmNumbers[1]));
            processAlarms.ProcessNormally();

            int faCountFA2 = processAlarms.GetFACount();
            Library.TakeScreenShot(session, "Process Alarm window before charging second alarm", out  path);

            processAlarms.ChargeFA();

            processAlarms.DiscardSkippedRecords();

            ReportBuilder.ArrayBuilder($"FA Count after first Alarm (skipped): {faCountFA1}", true, processAlarms.categoryName);
            ReportBuilder.ArrayBuilder($"FA Count after second Alarm (charged): {faCountFA2}", true, processAlarms.categoryName);

            //Process first Alarm (again)
            processAlarms.SetImportModeFile(format, filepath);
            processAlarms.StartManualImport();
            processAlarms.ProcessNormally();
            processAlarms.MatchAccount(CryWolfUtil.GetNameOnAccount(alarmNumbers[0]));
            processAlarms.ProcessNormally();

            int faCountFA3 = processAlarms.GetFACount();
            #endregion

            #region Validation
            Library.TakeScreenShot(session, "Process Alarm window after processing first alarm again", out path);

            exitStatus = (faCountFA2 + 1 == faCountFA3);
            ReportBuilder.ArrayBuilder(
                $"FA Count after first Alarm (again): {faCountFA3}", 
                exitStatus, 
                processAlarms.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [TestCase("<Default>")]
        public void Scenario007_ValidateDriverLicenseField(string agency)
        {
            #region Test Data
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();
            Maintenance cryWolfMaintenance = new Maintenance(childSession);

            cryWolfMaintenance.OpenAccountRelatedSettings();

            AccountRelated accountRelated = new AccountRelated(childSession);

            accountRelated.SelectAgency(agency);
            accountRelated.ShowPersonalARFields();
            accountRelated.CommitAccountRelatedSettings();
            accountRelated.Close();
            cryWolfMaintenance.Close();

            childSession.Quit();

            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);

            List<string> actualStates = registrations.GetDLStates();
            #endregion

            #region Validation
            Library.TakeScreenShot(session, TestContext.CurrentContext.Test.MethodName, out path);
            //TestContext.AddTestAttachment(path);
            exitStatus = CryWolfUtil.ValidateStateList(actualStates);
            ReportBuilder.ArrayBuilder(
                $"State Driver's License field is a combo box and contains all states", 
                exitStatus, 
                registrations.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [TestCase("Issue Mnth/Day", "N/A", "365", "No","<Default>")]
        public void Scenario008_ValidatePermitTermAlert(string timePeriod, string month, string days, string dispatchCode, string agency)
        {
            #region Variable Assignment
            DateTime issueDate;
            DateTime newIssueDate;
            string alarmNumber;
            #endregion

            #region Test Data
            do
            {
                alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.CitizenWithOneFA);
                ReportBuilder.ArrayBuilder($"Account Number: [{alarmNumber}] found", true, "Test Data");

                issueDate = CryWolfUtil.GetIssueDate(alarmNumber);
                ReportBuilder.ArrayBuilder($"Initial Issue Date: [{issueDate}]", true, "Test Data");

                string incidentDate = CryWolfUtil.GetIncDateByAlarmNumberLetter(alarmNumber, "FA 1");
                ReportBuilder.ArrayBuilder($"Incident Date: [{incidentDate}]", true, "Test Data");

                newIssueDate = DateTime.Parse(incidentDate).AddDays(1);
                ReportBuilder.ArrayBuilder($"New Issue Date: [{newIssueDate}]", true, "Test Data");
            } while (newIssueDate.CompareTo(issueDate) < 1);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            ///MaintenanceHelper.SetIssueExpiration(timePeriod, month, days, dispatchCode);
            //TODO: Replace with 
            Maintenance cryWolfMaintenance = new Maintenance(childSession);

            cryWolfMaintenance.OpenAlarmCountingPeriod();

            AlarmCountingPeriod alarmCountingPeriod = new AlarmCountingPeriod(childSession);

            if (!alarmCountingPeriod.ValidateAlarmCountingPeriod(timePeriod, month, days, dispatchCode))
            {
                alarmCountingPeriod.SetAlarmCountingPeriod(timePeriod, month, days, dispatchCode);
            }

            alarmCountingPeriod.CloseAlarmCountingPeriod();

            cryWolfMaintenance.Close();
            childSession.Quit();
            #endregion

            #region Process
            main.OpenProcessRegistrations();
            
            Registrations registrations = new Registrations(session);

            registrations.OpenAccountRegistration(alarmNumber);
            registrations.EditAccountIssueDate(newIssueDate.ToShortDateString());
            registrations.UpdateAccount();

            //reporting
            exitStatus = registrations.ValidatePermitChangeDialog(); //<--takes screenshot
            ReportBuilder.ArrayBuilder(
                $"Confirm Permit Period Change dialog displays when false alarm counts are impacted by period changes", 
                exitStatus, 
                registrations.categoryName);
            Assert.IsTrue(exitStatus);

            //change date back validate the other way
            registrations.OpenAccountRegistration(alarmNumber);
            registrations.EditAccountIssueDate(issueDate.ToShortDateString());
            registrations.UpdateAccount();
            #endregion

            #region Validation
            Library.TakeScreenShot(session, TestContext.CurrentContext.Test.MethodName, out path);
            //TestContext.AddTestAttachment(path);
            exitStatus = registrations.ValidatePermitChangeDialog();
            ReportBuilder.ArrayBuilder(
                $"Confirm Permit Period Change dialog displays when false alarm counts are impacted by period changes", 
                exitStatus, 
                registrations.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [TestCase("Use Todays Date", "<Default>")]
        [TestCase("Use Todays And Last Day Of Month", "<Default>")]
        [TestCase("Use 1st Of Month", "<Default>")]
        [TestCase("Use Specific Date", "<Default>", 0, 0, 0, 0)]
        [TestCase("Use Specific Date", "<Default>", 0, 0, 11, 1)]
        [TestCase("Use Specific Date", "<Default>", 0, 0, 11, 15)]
        [TestCase("Use Specific Date", "<Default>", 0, 0, 11, 30)]
        [TestCase("Use Specific Date", "<Default>", 11, 15, 0, 0)]
        [TestCase("Use Specific Date", "<Default>", 11, 30, 0, 0)]
        [TestCase("First Issue Last Expire", "<Default>")]
        [Author("Paul Atkins")]
        public void Scenario009_ValidateIssueExpirationSetup(string issueExpSetting, string agency, int issueMonth = 0, int issueDay = 0, int expMonth = 0, int expDay = 0)
        {
            #region Variable Assignment
            #endregion

            #region Prequel
            DateTime issueDateToCheck = CryWolfUtil.CalculateNewIssueDate(issueExpSetting, out issueMonth, out issueDay);
            DateTime expirationDateToCheck = CryWolfUtil.CalulateNewExpDate(issueExpSetting, out expMonth, out expDay);

            MaintenanceHelper.SetIssueExpiration(issueExpSetting, agency);

            if (issueExpSetting.Replace(" ","").ToLower() == "usespecificdate")
            {
                MaintenanceHelper.SetARIssue(issueMonth, issueDay, agency);
                MaintenanceHelper.SetARExp(expMonth, expDay, agency);
            }

            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            //main.OpenCryWolfMaint();

            //childSession = CryWolfUtil.AttachToCryWolfMaint();
            //Maintenance cryWolfMaintenance = new Maintenance(childSession);

            //cryWolfMaintenance.OpenAccountRelatedSettings();

            //AccountRelated accountRelated = new AccountRelated(childSession);

            //exitStatus = accountRelated.SetIssueExpirationSettings(issueExpSetting, agency, issueMonth, issueDay, expirationMonth, expirationDay);//<--takes screenshot
            //ReportBuilder.ArrayBuilder($"Issue / Expiration Setting Chosen: {issueExpSetting}", exitStatus, cryWolfMaintenance.categoryName);
            //Assert.IsTrue(exitStatus);

            //accountRelated.CommitAccountRelatedSettings();
            //accountRelated.Close();
            //cryWolfMaintenance.Close();
            //childSession.Quit();
            #endregion

            #region Test Data
            #endregion

            #region Process
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            #endregion

            #region Validation
            exitStatus = registrations.CheckAccountIssueDate(issueDateToCheck, out string actualIssue);
            Library.TakeScreenShot(session, "Account Window for validation", out path);
            ReportBuilder.ArrayBuilder($"Validate the Account issue date is {issueDateToCheck}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = registrations.CheckAccountExpDate(expirationDateToCheck, out string actualExp);
            ReportBuilder.ArrayBuilder($"Validate the Account expiration date is {expirationDateToCheck}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Sliding", "N/A", 5, false, "<Default>")]
        public void Scenario010_ProcessAlarmSlidingAlarmCounting(string timePeriod, string month, int days, bool dispatchCode, string agency)
        {
            #region Variable Assignment
            //string timePeriod = (string)this.TestContext.DataRow["time_period"];
            //string month = (string)this.TestContext.DataRow["month"];
            //string days = (string)this.TestContext.DataRow["days"];
            //string dispatchCode = (string)this.TestContext.DataRow["dispatch_code"];

            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string streetAddress = CryWolfUtil.GetStreetAddress(alarmNumber);
            string nameOnAccount = CryWolfUtil.GetNameOnAccount(alarmNumber);

            MaintenanceHelper.SetAlarmCountingPeriod(agency, "<General>", timePeriod, month, days, dispatchCode);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            //get fa count
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);

            int faCount = registrations.GetFalseAlarmCount();

            registrations.CloseRegistrationsWindow();
            #endregion

            #region Test Data

            #endregion

            #region Process
            main.OpenProcessAlarms();

            //Process Alarm for first period
            ProcessAlarms cryWolfProcessAlarms = new ProcessAlarms(session);
            cryWolfProcessAlarms.EnterFalseAlarm(streetAddress, DateTime.Today.AddDays(-(days * 5)));
            cryWolfProcessAlarms.MatchAccount(nameOnAccount);

            int faCount1 = cryWolfProcessAlarms.GetFACount();

            Library.TakeScreenShot(session, "FA Count 1", out path);
            //TestContext.AddTestAttachment(path);

            cryWolfProcessAlarms.ChargeFA();

            //Process Alarm for second period
            cryWolfProcessAlarms.EnterFalseAlarm(streetAddress, DateTime.Today.AddDays(-(days * 3)));
            cryWolfProcessAlarms.MatchAccount(nameOnAccount);

            int faCount2 = cryWolfProcessAlarms.GetFACount();

            Library.TakeScreenShot(session, "FA Count 2", out path);
            //TestContext.AddTestAttachment(path);

            cryWolfProcessAlarms.ChargeFA();

            //Process Alarm for third period
            cryWolfProcessAlarms.EnterFalseAlarm(streetAddress, DateTime.Today);
            cryWolfProcessAlarms.MatchAccount(nameOnAccount);

            int faCount3 = cryWolfProcessAlarms.GetFACount();

            Library.TakeScreenShot(session, "FA Count 3", out path);
            //TestContext.AddTestAttachment(path);

            cryWolfProcessAlarms.ChargeFA();

            #endregion

            #region Validation
            exitStatus = ((faCount - faCount1) == (faCount - faCount2)) && ((faCount - faCount2) == (faCount - faCount3)); //subtract previous alarms (if existing to ensure comparison is valid)

            ReportBuilder.ArrayBuilder(
                $"Compare FA Counts for different Sliding periods FA Count 1: {faCount1}, FA Count 2: {faCount2}, FA Count 3: {faCount3}, Original FA Count: {faCount} ",
                exitStatus, cryWolfProcessAlarms.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [TestCase("10012017")]
        [Author("Paul Atkins")]
        //[TestCase("HP_OSSI")]
        public void Scenario011_ProcessAlarmSpaceInImportFilename(string format)
        {
            #region Variable Assignment
            List<string> alarmNumbers = new List<string>
            {
                CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen)
            };

            #endregion

            #region Prequel
            CryWolfUtil.CreateFaImportFileWithSpace(alarmNumbers, out string filePath, format);
            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenProcessAlarms();

            //Start import
            ProcessAlarms cryWolfProcessAlarms = new ProcessAlarms(session);
            cryWolfProcessAlarms.SetImportModeFile(format, filePath);

            Library.TakeScreenShot(session, "Process Alarm screen after Set Import Mode", out path);
            ReportBuilder.ArrayBuilder($"File Name for import: {filePath}", true, "Create False Alarm Import File");

            cryWolfProcessAlarms.StartManualImport();
            Library.TakeScreenShot(session, "Process Alarm screen before matching account", out path);
            cryWolfProcessAlarms.MatchAccount(CryWolfUtil.GetNameOnAccount(alarmNumbers[0]));
            #endregion

            #region Validation
            exitStatus = cryWolfProcessAlarms.GetFACount() > 0;
            Library.TakeScreenShot(session, "Process Alarm screen after matching account", out path);
            ReportBuilder.ArrayBuilder($"Validate that file processed correctly", exitStatus, cryWolfProcessAlarms.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Use Todays Date", "FUTURE", "<Default>")]
        public void Scenario012_CannotSetIssueAfterExpiration(string issueExpSetting, string issueDate, string agency)
        {
            #region Variable Assignment
            //string IssueExpSetting = (string)this.TestContext.DataRow["issue_exp_setting"];

            int issueMonth = 0;
            int issueDay = 0;
            int expirationMonth = 0;
            int expirationDay = 0;

            //string issueDate = (string)this.TestContext.DataRow["issue_date"];
            //string expirationDate = (string)this.TestContext.DataRow["expiration_date"];
            DateTime issueDateToSet = new DateTime();
            DateTime expirationDateToSet = new DateTime();
            MaintenanceHelper.SetIssueExpiration(issueExpSetting, agency);
            #endregion

            #region Prequel
            DateTime issueDateToCheck = CryWolfUtil.CalculateNewIssueDate(issueExpSetting, out issueMonth, out issueDay);
            DateTime expirationDateToCheck = CryWolfUtil.CalulateNewExpDate(issueExpSetting, out expirationMonth, out expirationDay);

            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            //main.OpenCryWolfMaint();

            //childSession = CryWolfUtil.AttachToCryWolfMaint();
            //Maintenance maintenance = new Maintenance(childSession);

            //maintenance.OpenAccountRelatedSettings();

            //AccountRelated accountRelated = new AccountRelated(childSession);

            //exitStatus = accountRelated.SetIssueExpirationSettings(issueExpSetting, agency, issueMonth, issueDay, expirationMonth, expirationDay);
            //ReportBuilder.ArrayBuilder($"Issue / Expiration Setting Chosen: {issueExpSetting}", exitStatus, maintenance.categoryName);
            //Assert.IsTrue(exitStatus);

            //accountRelated.CommitAccountRelatedSettings();
            //accountRelated.Close();
            //maintenance.Close();

            //childSession.Quit();
            #endregion

            #region Test Data
            switch (issueDate.ToUpper())
            {
                case "FUTURE":
                    issueDateToSet = DateTime.Today.AddYears(3);
                    expirationDateToSet = issueDateToSet.AddYears(-1);
                    break;
                case "":
                    issueDate = "";
                    break;
                default:
                    break;
            }


            #endregion

            #region Process
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);

            registrations.OpenAccountRegistration(CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen));
            registrations.EditAccountIssueDate(issueDateToSet.ToShortDateString());
            registrations.EditAccountExpirationDate(expirationDateToSet.ToShortDateString());
            registrations.UpdateAccount();
            #endregion

            #region Validation
            switch (issueDate.ToUpper())
            {
                case "FUTURE":
                    exitStatus = registrations.ValidateExpirationDialog();
                    ReportBuilder.ArrayBuilder("Validate Expiration Date Error Dialog", exitStatus, registrations.categoryName);
                    break;
                default:
                    string alarmNumber = registrations.GetAlarmNumber();
                    exitStatus = alarmNumber != "";
                    Library.TakeScreenShot(session, TestContext.CurrentContext.Test.MethodName, out path);
                    //TestContext.AddTestAttachment(path);
                    ReportBuilder.ArrayBuilder($"Validate New Account Received Alarm Number: {alarmNumber}", exitStatus, registrations.categoryName);
                    break;
            }

            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("18.5_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Floating", "N/A", 365, false, "<Default>")]
        public void Scenario013_ProcessAlarmFloatingAlarmCounting(string timePeriod, string month, int days, bool dispatchCode, string agency)
        {
            #region Variable Assignment
            //string timePeriod = (string)this.TestContext.DataRow["time_period"];
            //string month = (string)this.TestContext.DataRow["month"];
            //string days = (string)this.TestContext.DataRow["days"];
            //string dispatchCode = (string)this.TestContext.DataRow["dispatch_code"];

            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string streetAddress = CryWolfUtil.GetStreetAddress(alarmNumber);
            string nameOnAccount = CryWolfUtil.GetNameOnAccount(alarmNumber);

            MaintenanceHelper.SetAlarmCountingPeriod(agency, "<General>", timePeriod, month, days, dispatchCode);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            //main.OpenCryWolfMaint();

            //childSession = CryWolfUtil.AttachToCryWolfMaint();

            //Maintenance maintenance = new Maintenance(childSession);

            //maintenance.OpenAlarmCountingPeriod();

            //AlarmCountingPeriod alarmCountingPeriod = new AlarmCountingPeriod(childSession);

            //if (!alarmCountingPeriod.ValidateAlarmCountingPeriod(timePeriod, month, days, dispatchCode))
            //{
            //    alarmCountingPeriod.SetAlarmCountingPeriod(timePeriod, month, days, dispatchCode);

            //    exitStatus = alarmCountingPeriod.ValidateAlarmCountingPeriod(timePeriod, month, days, dispatchCode);
            //}
            //else
            //{
            //    exitStatus = true;
            //}

            //alarmCountingPeriod.CloseAlarmCountingPeriod();

            //ReportBuilder.ArrayBuilder($"Alarm Counting Period Chosen: {timePeriod}", exitStatus, maintenance.categoryName);
            //Assert.IsTrue(exitStatus);

            //maintenance.Close();
            #endregion

            #region Test Data
            #endregion

            #region Process
            main.OpenProcessAlarms();

            //Process Alarm for first period
            ProcessAlarms processAlarms = new ProcessAlarms(session);
            processAlarms.EnterFalseAlarm(streetAddress, DateTime.Today);
            processAlarms.MatchAccount(nameOnAccount);

            //int faCount1 = processAlarms.GetFACount();

            Library.TakeScreenShot(session, "FA Count", out path);
            //TestContext.AddTestAttachment(path);
            #endregion

            #region Validation
            exitStatus = processAlarms.ValidateCountingPeriod(timePeriod, days.ToString(), out DateTime from, out DateTime inc);
            ReportBuilder.ArrayBuilder(
                $"Validate Counting Period: {timePeriod}, with Days: {days}, From date: {from}, Incident date: {inc}",
                exitStatus,
                processAlarms.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("19.1_Regression")]
        [Author("Paul Atkins")]
        [Test]
        public void Scenario023_ProcessAlarmNoIncTime()
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string streetAddress = CryWolfUtil.GetStreetAddress(alarmNumber);
            string nameOnAccount = CryWolfUtil.GetNameOnAccount(alarmNumber);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            //get fa count
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);

            int faCount = registrations.GetFalseAlarmCount();

            registrations.CloseRegistrationsWindow();
            #endregion

            #region Test Data

            #endregion

            #region Process
            main.OpenProcessAlarms();

            //Process Alarm for first period
            ProcessAlarms cryWolfProcessAlarms = new ProcessAlarms(session);
            cryWolfProcessAlarms.EnterFalseAlarmNoIncTime(streetAddress);
            cryWolfProcessAlarms.MatchAccount(nameOnAccount);

            int faCount1 = cryWolfProcessAlarms.GetFACount();

            cryWolfProcessAlarms.ChargeFA();

            Library.TakeScreenShot(session, "FA Count", out path);
            //TestContext.AddTestAttachment(path);
            #endregion

            #region Validation
            exitStatus = faCount1 == faCount + 1;
            ReportBuilder.ArrayBuilder(
                $"Validate False Alarm Count increments. Original FA Count: {faCount} FA Count after Alarm Processing: {faCount1}", 
                exitStatus, 
                cryWolfProcessAlarms.categoryName);
            Assert.IsTrue(exitStatus);

            main.OpenProcessRegistrations();

            registrations.OpenAccountRegistration(alarmNumber);

            int faCount2 = registrations.GetFalseAlarmCount();

            registrations.CloseRegistrationsWindow();

            exitStatus = faCount2 == faCount + 1;
            ReportBuilder.ArrayBuilder(
                $"Validate False Alarm Count increments. Original FA Count: {faCount} FA Count after Alarm Processing: {faCount1}", 
                exitStatus, 
                registrations.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("19.1_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Residential","<Default>"), Order(1)]
        public void Scenario024_ArchiveLocationType(string locationType, string agency)
        {
            #region Variable Assignment
            #endregion

            #region Prequel
            #endregion

            #region Test Data
            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            Maintenance cryWolfMaintenance = new Maintenance(childSession);
            cryWolfMaintenance.OpenLocationTypesSettings();

            LocationTypes locationTypes = new LocationTypes(childSession);
            exitStatus = locationTypes.ArchiveLocationType(locationType, agency);
            ReportBuilder.ArrayBuilder($"Check that {locationType} was archived successfully", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);

            locationTypes.CommitLocationTypes();
            locationTypes.Close();

            cryWolfMaintenance.Close();
            childSession.Quit();
            #endregion

            #region Validation

            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            exitStatus = registrations.ValidateLocationTypeArchive(locationType);
            ReportBuilder.ArrayBuilder($"Check that {locationType} is not included in the Location Type combobox", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);
            registrations.CloseRegistrationsWindow();

            #endregion

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            cryWolfMaintenance = new Maintenance(childSession);
            cryWolfMaintenance.OpenLocationTypesSettings();

            locationTypes = new LocationTypes(childSession);
            exitStatus = locationTypes.UnArchiveLocationType(locationType, agency);
            ReportBuilder.ArrayBuilder($"Check that {locationType} was archived successfully", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);

            locationTypes.CommitLocationTypes();

            session.Quit();
        }

        [Category("19.1_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Residential", "<Default>"), Order(2)]
        public void Scenario025_UnArchiveLocationType(string locationType,string agency)
        {
            #region Variable Assignment
            //string locationType = (string)this.TestContext.DataRow["location_type"];

            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string streetAddress = CryWolfUtil.GetStreetAddress(alarmNumber);
            string nameOnAccount = CryWolfUtil.GetNameOnAccount(alarmNumber);
            #endregion

            #region Prequel

            #endregion

            #region Test Data

            #endregion
            //try
            //{
            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            Maintenance maintenance = new Maintenance(childSession);
            maintenance.OpenLocationTypesSettings();

            LocationTypes locationTypes = new LocationTypes(childSession);
            exitStatus = locationTypes.UnArchiveLocationType(locationType, agency);
            ReportBuilder.ArrayBuilder($"Check that {locationType} was unarchived successfully", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);

            locationTypes.CommitLocationTypes();
            locationTypes.Close();

            maintenance.Close();
            childSession.Quit();
            #endregion

            #region Validation
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            exitStatus = registrations.ValidateLocationTypeActive(locationType);
            ReportBuilder.ArrayBuilder($"Check that {locationType} is included in the Location Type combobox", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);

            #endregion

            session.Quit();
            //}
            //catch (Exception e1)
            //{
            //    Utility.HandleException(e1, windowsDriver, TestContext.CurrentContext.Test.MethodName, report);
            //}
        }

        [Category("19.1_Regression")]
        [Author("Paul Atkins")]
        [TestCase("amingione")]
        public void Scenario026_ArchiveLocationTypeNonAdmin(string userName)
        {
            #region Variable Assignment
            //string userName = (string)this.TestContext.DataRow["user_name"];
            string password = CommonTestSettings.DesktopPw;
            #endregion

            #region Prequel

            #endregion

            #region Test Data

            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.Login(userName, password.ToString());
            ReportOnLogin(userName);

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();
            #endregion

            #region Validation
            Maintenance maintenance = new Maintenance(childSession);
            exitStatus = maintenance.LocationTypesNonAdmin();
            ReportBuilder.ArrayBuilder($"Check that {userName} cannot access Location Types menu item", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);

            maintenance.Close();
            childSession.Quit();
            #endregion

            session.Quit();
        }

        [Category("19.1_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Residential","amingione","Commercial","<Default>")]
        public void Scenario027_EditArchivedLocationType(string locationType, string userName, string newLocationType,string agency)
        {
            #region Variable Assignment
            string password = CommonTestSettings.DesktopPw;
            #endregion

            #region Prequel
            //Archive Residential
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            Maintenance maintenance = new Maintenance(childSession);
            maintenance.OpenLocationTypesSettings();

            LocationTypes locationTypes = new LocationTypes(childSession);
            exitStatus = locationTypes.ArchiveLocationType(locationType,agency);
            ReportBuilder.ArrayBuilder($"Check that {locationType} was archived successfully", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);

            locationTypes.CommitLocationTypes();
            locationTypes.Close();

            maintenance.Close();
            childSession.Quit();

            main.Close();
            session.Quit();
            #endregion

            #region Test Data
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveResident);
            #endregion
            
            #region Process
            //Login as non admin user
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);

            main = new Main(session);

            main.ChooseDatabase();

            main.Login(userName, password);
            //Change random Residential to other 'active' location type
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);
            registrations.EditLocationType(newLocationType);
            registrations.UpdateAccount();
            #endregion

            #region Validation
            //Account saved successfully
            registrations.OpenAccountRegistration(alarmNumber);
            exitStatus = registrations.ValidateLocationType(newLocationType);
            ReportBuilder.ArrayBuilder($"Check that {newLocationType} is now Location Type displayed in combobox", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);

            main.Close();
            #endregion

            #region Cleanup
            //Re activate Residential
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            maintenance = new Maintenance(childSession);
            maintenance.OpenLocationTypesSettings();

            locationTypes = new LocationTypes(childSession);
            exitStatus = locationTypes.UnArchiveLocationType(locationType,agency);
            ReportBuilder.ArrayBuilder($"Check that {locationType} was unarchived successfully", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);

            locationTypes.CommitLocationTypes();
            locationTypes.Close();

            maintenance.Close();
            childSession.Quit();

            main.Close();
            session.Quit();
            #endregion
        }

        [Category("19.1_Regression")]
        [Author("Paul Atkins")]
        [Test]
        public void Scenario028_ConfigureCopyToRP()
        {
            #region Variable Assignment
            //string userName = (string)this.TestContext.DataRow["user_name"];
            //string password = (string)this.TestContext.DataRow["password"];
            //string locationType = (string)this.TestContext.DataRow["location_type"];
            //string newLocationType = (string)this.TestContext.DataRow["new_location_type"];
            #endregion

            #region Prequel         
            #endregion

            #region Test Data
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveResident);
            List<string> expectedBehaviors = new List<string>
            {
                "Copies name and address (default)",
                "Copies address only, never name",
                "Always copies address; name only for residential",
                "Hidden"
            };

            #endregion

            #region Process
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            Maintenance maintenance = new Maintenance(childSession);
            maintenance.OpenAccountRelatedSettings();

            AccountRelated accountRelated = new AccountRelated(childSession);
            #endregion

            #region Validation
            //Account saved successfully
            exitStatus = accountRelated.ValidateCopyToRpOptions(expectedBehaviors);
            ReportBuilder.ArrayBuilder($"Check that expected options: {expectedBehaviors} match actual options.", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);
            #endregion

            #region Cleanup
            accountRelated.Close();
            maintenance.Close();
            childSession.Quit();

            main.Close();
            #endregion

            session.Quit();
        }
        
        [Category("19.1_Regression")]
        [Author("Paul Atkins")]
        [Test, Combinatorial]
        public void Scenario029_CopyToRpEditAccount(
            [Values("Default","Address Only","Name Residential","Hidden")] string copyBehavior,
            [Values("Residential","Commercial")] string accountType,
            [Values("<Default>")] string agency)
        {
            #region Variable Assignment
            string alarmNumber;
            Person person = new Person();
            Address address = new Address();
            #endregion

            #region Prequel 
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            MaintenanceHelper.SetCopyToRP(copyBehavior, agency);

            //main.OpenCryWolfMaint();

            //childSession = CryWolfUtil.AttachToCryWolfMaint();

            //Maintenance maintenance = new Maintenance(childSession);
            //maintenance.OpenAccountRelatedSettings();

            //AccountRelated accountRelated = new AccountRelated(childSession);

            //exitStatus = accountRelated.ConfigureCopyToRpOptions(copyBehavior,agency);
            //ReportBuilder.ArrayBuilder($"Check that expected option: {copyBehavior} is selected.", exitStatus, "Validation");
            //Library.TakeScreenShot(childSession, TestContext.CurrentContext.Test.MethodName, out path);

            //Assert.IsTrue(exitStatus);

            //accountRelated.Close();
            //maintenance.Close();
            //childSession.Quit();
            #endregion

            #region Test Data
            if (accountType.ToLower() == "residential")
            {
                alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveResident);
            }
            else
            {
                alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCommercial);
                person.LastName = CryWolfUtil.GetUniqueCaseNumber();
            }

            ReportBuilder.ArrayBuilder($"Copy Behavior: {copyBehavior}", exitStatus, "Test Data");
            ReportBuilder.ArrayBuilder($"Account Type: {accountType}", exitStatus, "Test Data");
            ReportBuilder.ArrayBuilder($"Alarm Number: {alarmNumber}", exitStatus, "Test Data");
            #endregion

            #region Process -- Edit Alarm
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);

            registrations.OpenAccountRegistration(alarmNumber);
            #endregion Process -- Edit Alarm

            #region Validation --  Edit Alarm
            //Account saved successfully
            exitStatus = registrations.ValidateCopyToRP(copyBehavior);
            Library.TakeScreenShot(session, $"{TestContext.CurrentContext.Test.MethodName} - Edit Alarm License", out string filePath);
            ReportBuilder.ArrayBuilder($"Copy Behavior {copyBehavior} for existing account", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);
            #endregion Validation --  Edit Alarm

            #region Process -- New Alarm
            registrations.EnterNewAccountInfo(person, address, accountType, true);
            #endregion Process -- New Alarm

            #region Validation --  Edit Alarm
            exitStatus = registrations.ValidateCopyToRP(copyBehavior);
            Library.TakeScreenShot(session, $"{TestContext.CurrentContext.Test.MethodName} - New Alarm License", out filePath);
            ReportBuilder.ArrayBuilder($"Copy Behavior {copyBehavior} for new account", exitStatus, "Validation");
            Assert.IsTrue(exitStatus);
            #endregion

            #region Cleanup

            main.Close();
            #endregion

            session.Quit();
        }

        [Category("19.1_Regression")]
        [Author("Paul Atkins")]
        [TestCase("Residential")]
        public void Scenario030_ProcessAlarmViewLocationTypes(string locationType)
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string streetAddress = CryWolfUtil.GetStreetAddress(alarmNumber);
            //string nameOnAccount = CryWolfUtil.GetNameOnAccount(alarmNumber);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Test Data

            #endregion

            #region Process
            main.OpenProcessAlarms();

            //Process Alarm for first period
            ProcessAlarms processAlarms = new ProcessAlarms(session);

            processAlarms.EnterFalseAlarm(streetAddress);
            processAlarms.AddAccount();
            #endregion

            #region Validation
            exitStatus = processAlarms.ValidateLocationType(locationType);
            Library.TakeScreenShot(session, TestContext.CurrentContext.Test.MethodName, out string filePath);
            ReportBuilder.ArrayBuilder($"Validate Location Types available", exitStatus, processAlarms.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            session.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [Test]
        public void Scenario068_ClearAlarmCompaniesFromAlarmLicense()
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Process
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);

            registrations.NavigateToRelatedAC();
            registrations.ClearRelatedAlarmCompanies();
            registrations.UpdateAccount();

            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            #endregion

            #region Validation
            exitStatus = registrations.ValidateAlarmCompaniesBlank();
            Library.TakeScreenShot(session, "Account Window after clearing ALL Alarm Companies", out string filePath);
            ReportBuilder.ArrayBuilder($"Validate Alarm Companies blank", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus, $"Validate Alarm Companies blank");
            #endregion

            session.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [Test, Combinatorial]
        public void Scenario069_ClearAlarmCompanyFromAlarmLicense(
            [Values("Monitored By", "Sold By", "Serviced By", "Installed By")] string acType)
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Process
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            registrations.ClearAlarmCompany(acType);
            registrations.UpdateAccount();

            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            #endregion

            #region Validation
            exitStatus = registrations.ValidateAlarmCompanyBlank(acType);
            Library.TakeScreenShot(session, "Account Window after clearing Alarm Company", out string filePath);
            ReportBuilder.ArrayBuilder($"Validate Alarm Company blank", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus, $"Validate Alarm Company blank");
            #endregion

            session.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [Test, Combinatorial]
        public void Scenario070_AssignAlarmCompanyToAlarmLicense(
            [Values("Monitored By", "Sold By", "Serviced By", "Installed By")] string acType)
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string acNumber = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Process
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            registrations.AssignAlarmCompany(acType, acNumber);
            registrations.UpdateAccount();

            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            #endregion

            #region Validation
            exitStatus = registrations.ValidateAlarmCompany(acType, acNumber);
            Library.TakeScreenShot(session, "Account Window after Assigning Alarm Company", out string filePath);
            ReportBuilder.ArrayBuilder($"Validate Alarm Company assigned is: {acNumber}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus, $"Validate Alarm Company assigned is: {acNumber}");
            #endregion

            session.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [Test, Combinatorial]
        public void Scenario071_CopyMonitoringCompanyToAll()
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string acNumber = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo);
            string acType = "Monitored By";
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Process
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            registrations.AssignAlarmCompany(acType, acNumber); //Assign Monitoring Company
            registrations.CopyMonitoringACToOthers();
            registrations.UpdateAccount();

            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            #endregion

            #region Validation
            exitStatus = registrations.ValidateAlarmCompany(acType, acNumber);
            Library.TakeScreenShot(session, "Account Window after Assigning Alarm Company", out string filePath);
            ReportBuilder.ArrayBuilder($"Validate Alarm Company assigned is: {acNumber}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus, $"Validate Alarm Company assigned is: {acNumber}");
            #endregion

            session.Quit();
        }

        [TestCase("<Default>", "10012017", "<General>", "Floating", 365, false)]
        [TestCase("<Default>", "10012017", "<General>", "Issue Mnth/Day", 0, false)]
        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        public void Scenario085_Process_Alarm_ChargeMatrix(string agency, string format, string locationType, string countingMethod, int days, bool dispatchCode)
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            ReportBuilder.ArrayBuilder($"Alarm Number [{alarmNumber}] found to put in Alarm Processing file", true, "Test Data");

            List<string> alarmNumbers = new List<string>
            {
                alarmNumber
            };
            #endregion

            #region Prequel
            CryWolfUtil.CreateFaImportMansfieldDispatch(alarmNumbers, out string filepath, format, DateTime.Today.AddDays(-1), "BREAK");
            #endregion

            #region Process
            MaintenanceHelper.SetAlarmCountingPeriod(agency, locationType, countingMethod, "N/A", days, dispatchCode);
            MaintenanceHelper.DeleteDispatchGroups(agency);

            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);

            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            //Get FA Count
            exitStatus = main.OpenProcessRegistrations();
            Assert.IsTrue(exitStatus, "Process Registrations window did not open quickly enough");

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);
            int FaCount = registrations.GetFalseAlarmCount();
            registrations.CloseRegistrationsWindow();

            main.OpenProcessAlarms();

            //Start import
            ProcessAlarms processAlarms = new ProcessAlarms(session);
            processAlarms.SetImportModeFile(format, filepath);

            Library.TakeScreenShot(session, "Process Alarm screen after Set Import Mode", out path);

            processAlarms.StartManualImport();

            Library.TakeScreenShot(session, "Process Alarm screen before matching account", out path);

            processAlarms.MatchAccount(CryWolfUtil.GetNameOnAccount(alarmNumbers[0]));
            processAlarms.ProcessNormally();

            //Skip first Alarm 
            int faCountFA1 = processAlarms.GetFACount();
            Library.TakeScreenShot(session, "Process Alarm window before charging", out path);

            processAlarms.ChargeFA();

            exitStatus = (FaCount + 1 == faCountFA1);
            ReportBuilder.ArrayBuilder(
                $"FA Count after first Processed Alarm: {faCountFA1}",
                exitStatus,
                processAlarms.categoryName);
            Assert.IsTrue(exitStatus);

            exitStatus = processAlarms.AcceptAllRecordsProcessed();
            Assert.IsTrue(exitStatus, "All Records Processed Dialog failed");

            CryWolfUtil.CreateFaImportMansfieldDispatch(alarmNumbers, out filepath, format, DateTime.Today, "FALARM");

            processAlarms.SetImportModeFile(format, filepath);

            Library.TakeScreenShot(session, "Process Alarm screen after Set Import Mode", out path);

            processAlarms.StartManualImport();

            processAlarms.ProcessNormally();

            Library.TakeScreenShot(session, "Process Alarm screen before matching account", out path);

            processAlarms.MatchAccount(CryWolfUtil.GetNameOnAccount(alarmNumbers[0]));
            processAlarms.ProcessNormally();

            //Skip first Alarm 
            faCountFA1 = processAlarms.GetFACount();
            Library.TakeScreenShot(session, "Process Alarm window before charging", out path);

            processAlarms.ChargeFA();

            exitStatus = processAlarms.AcceptAllRecordsProcessed();
            Assert.IsTrue(exitStatus, "All Records Processed Dialog failed");
            #endregion

            #region Validation
            exitStatus = (FaCount + 2 == faCountFA1);
            ReportBuilder.ArrayBuilder(
                $"FA Count after second Processed Alarm: {faCountFA1}",
                exitStatus,
                processAlarms.categoryName);
            Assert.IsTrue(exitStatus);
            #endregion

            MaintenanceHelper.SetDallasDispatchGroups();
            session.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [Test, Combinatorial]
        public void Scenario086_AssignNAToAlarmLicense(
            [Values("Monitored By", "Sold By", "Serviced By", "Installed By")] string acType)
        {
            #region Variable Assignment
            string alarmNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizen);
            string acNumber = "N/A";
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Process
            main.OpenProcessRegistrations();

            Registrations registrations = new Registrations(session);
            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            registrations.AssignAlarmCompany(acType, acNumber);
            registrations.UpdateAccount();

            registrations.OpenAccountRegistration(alarmNumber);
            registrations.NavigateToRelatedAC();
            #endregion

            #region Validation
            exitStatus = registrations.ValidateAlarmCompany(acType, acNumber);
            Library.TakeScreenShot(session, "Account Window after Assigning Alarm Company", out string filePath);
            ReportBuilder.ArrayBuilder($"Validate Alarm Company assigned is: {acNumber}", exitStatus, registrations.categoryName);
            Assert.IsTrue(exitStatus, $"Validate Alarm Company assigned is: {acNumber}");
            #endregion

            session.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [Test]
        public void Scenario087_Admin_CreatesNewUser()
        {
            #region Variable Assignment
            string agency = "<Default>";
            Person person = new Person();
            string fullName = person.getFullName();
            string signon = $"{person.getFirstName().Substring(0, 1).ToLower()}{person.getLastName().ToLower()}";
            string password = CommonTestSettings.DesktopPw;
            string email = person.primaryEmail().GetEmail();
            #endregion

            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Process
            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            Maintenance maintenance = new Maintenance(childSession);

            maintenance.OpenUserSecurity();

            UserSecurity userSecurity = new UserSecurity(childSession);

            exitStatus = userSecurity.AddUserWithNoPermissions(agency, fullName, signon, password, email);
            Library.TakeScreenShot(childSession, "User Entry window after adding user", out string filePath);
            ReportBuilder.ArrayBuilder($"Validate user: {fullName} was added", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, $"Validate user: {fullName} was added");

            userSecurity.Close();

            maintenance.Close();

            childSession.Quit();

            main.Close();
            session.Quit();

            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.Login(signon, password, true);
            ReportOnLogin(signon);

            main.Close();
            session.Quit();

            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            #region Process
            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            maintenance = new Maintenance(childSession);

            maintenance.OpenUserSecurity();

            userSecurity = new UserSecurity(childSession);
            exitStatus = userSecurity.DeleteUserByFullName(fullName);
            Library.TakeScreenShot(childSession, "User Entry window after deleting user", out filePath);
            ReportBuilder.ArrayBuilder($"Validate user: {fullName} was deleted", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus, $"Validate user: {fullName} was deleted");
            #endregion
            userSecurity.Close();

            maintenance.Close();

            childSession.Quit();
        }

        [Category("19.2_Regression")]
        [Author("Paul Atkins")]
        [TestCase("amingione")]
        public void Scenario089_Admin_ChangesUserPassword(string signon)
        {
            string newPassword = "Passw0rd!";
            #region Prequel
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();
            #endregion

            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            Maintenance maintenance = new Maintenance(childSession);

            maintenance.OpenUserSecurity();

            UserSecurity userSecurity = new UserSecurity(childSession);
            userSecurity.EditUserPassword(signon, newPassword);

            userSecurity.Close();

            maintenance.Close();
            childSession.Quit();

            main.Close();

            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.Login(signon, newPassword);
            ReportOnLogin(signon);

            main.Close();

            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            session.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();


            main.OpenCryWolfMaint();

            childSession = CryWolfUtil.AttachToCryWolfMaint();

            maintenance = new Maintenance(childSession);

            maintenance.OpenUserSecurity();

            userSecurity = new UserSecurity(childSession);
            userSecurity.EditUserPassword(signon, CommonTestSettings.DesktopPw);

            userSecurity.Close();

            maintenance.Close();
            childSession.Quit();

            main.Close();
        }

        [Category("19.3_Regression")]
        [Author("Paul Atkins")]
        [Test]
        public void Scenario100_Payments_CountedNextBusinessDay()
        {
            string alarmNo = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveCitizenOutstandingInvoice);
            DateTime actionDate = CryWolfUtil.GetNextBusinessDay();
            DateTime cutoffTime = DateTime.Now.AddHours(-1);

            MaintenanceHelper.EnableNextBusinessDayPayment(cutoffTime);
            session = CryWolfUtil.TestInitialize(arguments, CommonTestSettings.RunLocation);
            exitStatus = session != null;

            Library.TakeScreenShot(session, "Start CryWolf application", out string path);
            ReportBuilder.ArrayBuilder("Start CryWolf application", exitStatus, "Test Initialize/Setup");
            Assert.IsTrue(exitStatus);
            //
            Main main = new Main(session);

            main.ChooseDatabase();

            exitStatus = main.LoginAsAdmin();
            ReportOnAdminLogin();

            main.OpenProcessPayments();

            Payments payments = new Payments(session);

            payments.ChooseAllInvoices(alarmNo);
            exitStatus = payments.ValidateActionDate(actionDate);
            Library.TakeScreenShot(session, "Validate Action Date is next Business Day", out path);
            ReportBuilder.ArrayBuilder("Validate Action Date is next Business Day", exitStatus, "UI Validation");

            MaintenanceHelper.DisableNextBusinessDayPayment();

            payments.CancelPayment();
            payments.Close();

            main.Close();

            session.Quit();
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

            if (CommonTestSettings.RunLocation == "local")
            {
                pWinAppDriver = Utility.StartWinAppDriver();
                exitStatus = (pWinAppDriver != null);
                Assert.IsTrue(exitStatus);
            }
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            Utility.KillExcelProcesses();

            if (CommonTestSettings.RunLocation == "local")
            {
                Utility.StopWinAppDriver();
            }
        }

        public static void ReportOnAdminLogin(bool _exitStatus = false, WindowsDriver<WindowsElement> _session = null)
        {
            if (_session != null)
            {
                session = _session;
                exitStatus = _exitStatus;
            }

            Library.TakeScreenShot(session, "Login to CryWolf as Admin", out string path);
            ReportBuilder.ArrayBuilder("Login to CryWolf as Admin", exitStatus, "CryWolf Login");
            Assert.IsTrue(exitStatus);
        }
        public void ReportOnLogin(string username)
        {
            Library.TakeScreenShot(session, $"Login to CryWolf as {username}", out string path);
            ReportBuilder.ArrayBuilder($"Login to CryWolf as {username}", exitStatus, "CryWolf Login");
            Assert.IsTrue(exitStatus);
        }
    }
}