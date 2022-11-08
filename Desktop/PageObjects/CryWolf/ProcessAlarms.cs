using Desktop.Libraries;
using NUnit.Framework;
using OpenQA.Selenium;
//using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Utils;
using Utils.PersonFactory;
//using static Utils.CommonTestSettings;

namespace Desktop.PageObjects.CryWolf
{
    public class ProcessAlarms
    {
        private readonly WindowsDriver<WindowsElement> session;
        public string categoryName = "Process Alarms";
        //Library library = new Library();
        public bool status;

        public ProcessAlarms(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification
        private WindowsElement frmFAs => session.FindElementByAccessibilityId("frmFAs");
        private WindowsElement btnClose => frmFAs.FindElementByName("Close") as WindowsElement;

        #region General Fields
        private WindowsElement txtCaseNo => frmFAs.FindElementByAccessibilityId("txtCaseNo") as WindowsElement;
        private WindowsElement btnSearch => frmFAs.FindElementByAccessibilityId("btnSearch") as WindowsElement;
        private WindowsElement lblFrom => frmFAs.FindElementByAccessibilityId("lblFrom") as WindowsElement;
        private WindowsElement txtIncDate => frmFAs.FindElementByAccessibilityId("dtIncDate") as WindowsElement;
        private WindowsElement txtTmReceived => frmFAs.FindElementByAccessibilityId("txtTmReceived") as WindowsElement;
        private WindowsElement txtTmDispatched => frmFAs.FindElementByAccessibilityId("txtTmDispatched") as WindowsElement;
        private WindowsElement txtTmOnScn => frmFAs.FindElementByAccessibilityId("txtTmOnScene") as WindowsElement;
        private WindowsElement txtTmCleared => frmFAs.FindElementByAccessibilityId("txtTmCleared") as WindowsElement;
        private WindowsElement lblAlarmNo => frmFAs.FindElementByAccessibilityId("lblAlarmNo") as WindowsElement;
        private WindowsElement lblFalseAlarmCount => frmFAs.FindElementByAccessibilityId("lblCnt") as WindowsElement;
        private WindowsElement txtStrNum => frmFAs.FindElementByAccessibilityId("txtStrNum") as WindowsElement;
        private WindowsElement txtStreet => frmFAs.FindElementByAccessibilityId("txtStreet") as WindowsElement;
        private WindowsElement txtApt => frmFAs.FindElementByAccessibilityId("txtApt") as WindowsElement;
        private WindowsElement txtDispatchCode => frmFAs.FindElementByAccessibilityId("txtDispatchCode") as WindowsElement;
        private WindowsElement txtClearanceCode => frmFAs.FindElementByAccessibilityId("txtClearanceCode") as WindowsElement;
        private WindowsElement txtDispatcherInfo => frmFAs.FindElementByAccessibilityId("txtDispatcherInfo") as WindowsElement;
        private WindowsElement txtCallTakerInfo => frmFAs.FindElementByAccessibilityId("txtCallTakerInfo") as WindowsElement;
        private WindowsElement txtUnits => frmFAs.FindElementByAccessibilityId("txtUnits") as WindowsElement;
        private WindowsElement txtOfficerID => frmFAs.FindElementByAccessibilityId("txtOfficerID") as WindowsElement;
        private WindowsElement txtAlarmType => frmFAs.FindElementByAccessibilityId("txtAlarmType") as WindowsElement;
        private WindowsElement txtArea => frmFAs.FindElementByAccessibilityId("txtArea") as WindowsElement;
        private WindowsElement txtDispatcherComments => frmFAs.FindElementByAccessibilityId("txtDispatcherComments") as WindowsElement;
        private WindowsElement txtOfficerComments => frmFAs.FindElementByAccessibilityId("txtOfficerComments") as WindowsElement;
        private WindowsElement btnCharge => frmFAs.FindElementByAccessibilityId("btnCharge") as WindowsElement;
        private WindowsElement btnIgnore => frmFAs.FindElementByAccessibilityId("btnIgnore") as WindowsElement;
        private WindowsElement btnStart => frmFAs.FindElementByAccessibilityId("btnStart") as WindowsElement;
        private WindowsElement btnNext => frmFAs.FindElementByName("Next") as WindowsElement;
        private WindowsElement lblMode => frmFAs.FindElementByAccessibilityId("lblMode") as WindowsElement;
        private WindowsElement btnOptions => frmFAs.FindElementByAccessibilityId("btnOptions") as WindowsElement;
        #endregion

        #region File Selection
        private WindowsElement frmFAOptions => session.FindElementByAccessibilityId("frmFAOptions");
        private WindowsElement cbFileFormat => frmFAOptions.FindElementByAccessibilityId("cbFileFormat") as WindowsElement;
        private WindowsElement GetFileFormatList()
        {
            return cbFileFormat.FindElementByTagName("List") as WindowsElement;
        }
        private WindowsElement cbMode => frmFAOptions.FindElementByAccessibilityId("cbMode") as WindowsElement;
        private WindowsElement btnSelectFile => frmFAOptions.FindElementByAccessibilityId("btnSelectFile") as WindowsElement;
        private WindowsElement btnOK => frmFAOptions.FindElementByAccessibilityId("btnOK") as WindowsElement;
        #endregion

        #region Open Dialog
        private WindowsElement dlgOpen => frmFAOptions.FindElementByName("Open") as WindowsElement;
        #endregion

        #region Records Processed Dialog
        private WindowsElement dlgProcessedRecords => session.FindElementByTagName("Dialog");
        private WindowsElement btnProcessedRecordsOk => session.FindElementByName("OK") as WindowsElement;
        #endregion

        #region Account Selection
        private WindowsElement frmFAMatches => session.FindElementByAccessibilityId("frmFAMatches");
        private WindowsElement btnAddAccount => frmFAMatches.FindElementByAccessibilityId("btnAdd") as WindowsElement;
        #endregion

        #region Records Processed
        private WindowsElement dlgRecordsProcessed => session.FindElementByName("Records Processed");
        private WindowsElement btnYes => dlgRecordsProcessed.FindElementByAccessibilityId("6") as WindowsElement;
        private WindowsElement btnNo => dlgRecordsProcessed.FindElementByAccessibilityId("7") as WindowsElement;
        #endregion

        #region Multiple Incidents Warning
        private WindowsElement frmMultipleCallWarning => session.FindElementByAccessibilityId("frmMultipleCallWarning");
        private WindowsElement btnProcessNormal => frmMultipleCallWarning.FindElementByAccessibilityId("btnProcessNormal") as WindowsElement;
        #endregion

        #region Add Account
        private WindowsElement frmFARegAdd => frmFAMatches.FindElementByAccessibilityId("frmFARegAdd") as WindowsElement;
        private WindowsElement cbLocationType => frmFARegAdd.FindElementByAccessibilityId("cbLocationType") as WindowsElement;
        private WindowsElement txtLast => frmFARegAdd.FindElementByAccessibilityId("txtLast") as WindowsElement;
        private WindowsElement txtFirst => frmFARegAdd.FindElementByAccessibilityId("txtFirst") as WindowsElement;
        private WindowsElement txtPh1 => frmFARegAdd.FindElementByAccessibilityId("txtPh1") as WindowsElement;
        private WindowsElement txtRPLast => frmFARegAdd.FindElementByAccessibilityId("txtRPLast") as WindowsElement;
        private WindowsElement txtRPFirst => frmFARegAdd.FindElementByAccessibilityId("txtRPFirst") as WindowsElement;
        private WindowsElement txtRPStrNum => frmFARegAdd.FindElementByAccessibilityId("txtRPStrNum") as WindowsElement;
        private WindowsElement txtRPStreet => frmFARegAdd.FindElementByAccessibilityId("txtRPStreet") as WindowsElement;
        private WindowsElement txtRPCity => frmFARegAdd.FindElementByAccessibilityId("txtRPCity") as WindowsElement;
        private WindowsElement txtRPState => frmFARegAdd.FindElementByAccessibilityId("txtRPState") as WindowsElement;
        private WindowsElement txtRPZip => frmFARegAdd.FindElementByAccessibilityId("txtRPZip") as WindowsElement;
        private WindowsElement txtRPPhone1 => frmFARegAdd.FindElementByAccessibilityId("txtRPPhone1") as WindowsElement;
        private WindowsElement btnAdd => frmFARegAdd.FindElementByAccessibilityId("btnAdd") as WindowsElement;
        #endregion

        //Basic interactions
        private void ClickCloseButton()
        {
            frmFAs.Click();
            btnClose.Click();
        }

        #region General Fields
        private string GetAlarmNumber()
        {
            return lblAlarmNo.Text;
        }
        private void SetCaseNumber(string caseNo)
        {
            txtCaseNo.SendKeys(caseNo);
        }
        private void ClickSearch()
        {
            btnSearch.Click();
        }
        private void ClickAddAccount()
        {
            btnAddAccount.Click();
        }
        private void SetIncidentDate(string date)
        {
            txtIncDate.SendKeys(date);
        }
        private void SetTimeReceived(string time)
        {
            txtTmReceived.SendKeys(time);
        }
        private void SetTimeDispatched(string time)
        {
            txtTmDispatched.SendKeys(time);
        }
        private void SetTimeOnScene(string time)
        {
            txtTmOnScn.SendKeys(time);
        }
        private void SetTimeCleared(string time)
        {
            txtTmCleared.SendKeys(time);
        }
        private int getFACount()
        {
            return int.Parse(lblFalseAlarmCount.Text);
        }
        private void SetStreetNumber(string streetNum)
        {
            txtStrNum.SendKeys(streetNum);
        }
        private void SetStreetName(string streetName)
        {
            txtStreet.SendKeys(streetName);
        }
        private void SetAptSuite(string aptSuite)
        {
            txtApt.SendKeys(aptSuite);
        }
        private void SetDispatch(string dispatch = "POLICE")
        {
            txtDispatchCode.SendKeys(dispatch);
        }

        private void SetClearance(string clearance)
        {
            txtClearanceCode.SendKeys(clearance);
        }
        private void SetDispatcher(string dispatcher = "D5")
        {
            txtDispatcherInfo.SendKeys(dispatcher);
        }
        private void SetCallTaker(string callTaker = "CT 5")
        {
            txtCallTakerInfo.SendKeys(callTaker);
        }
        private void SetUnits(string units = "3322")
        {
            txtUnits.SendKeys(units);
        }
        private void SetOfficer(string officer = "9912")
        {
            txtOfficerID.SendKeys(officer);
        }
        private void SetAlarmType(string alarmType = "SILENT")
        {
            txtAlarmType.SendKeys(alarmType);
        }
        private void SetArea(string area = "BEAT 5")
        {
            txtArea.SendKeys(area);
        }
        private void SetDispatcherComments(string dispatcherComments = "HOLDUP")
        {
            txtDispatcherComments.SendKeys(dispatcherComments);
        }
        private void SetOfficerComments(string officerComments = "FALSE NO ONE THERE")
        {
            txtOfficerComments.SendKeys(officerComments);
        }
        private void ClickIgnore()
        {
            btnIgnore.Click();
        }
        private void ClickCharge()
        {
            btnCharge.Click();
        }
        private void ClickStart()
        {
            btnStart.Click();
        }
        private void ClickNext()
        {
            btnNext.Click();
        }
        private void ClickSelectModeFile()
        {
            btnOptions.Click();
        }
        #endregion

        #region File Selection
        private void ExpandFileFormat()
        {
            frmFAOptions.Click();
            cbFileFormat.Click();
        }
        private void SelectFileFormat(string fileFormat)
        {
            cbFileFormat.SelectListItem(fileFormat);
        }
        private void ClickSelectFile()
        {
            btnSelectFile.Click();
        }
        private void ClickOK()
        {
            //HandleWarningIncInDB("ClickOK");
            btnOK.Click();
        }
        #endregion

        #region Account Selection

        #endregion

        #region Records Processed
        private void ClickYes()
        {
            btnYes.Click();
        }
        private void ClickNo()
        {
            btnNo.Click();
        }
        #endregion

        #region Multiple Incidents Warning
        private bool ClickProcessNormally()
        {
            try
            {
                TestContext.WriteLine("Click Process Normally button");
                Actions actions = new Actions(session);
                actions.MoveToElement(frmMultipleCallWarning);
                actions.Click();
                actions.Perform();
                btnProcessNormal.Click();
                return true;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Process Normally button was not found as defined.");
                return false;
            }
        }
        #endregion

        #region Add Account
        private void SetLastName(string last)
        {
            txtLast.SendKeys(last);
        }
        private void SetFirstName(string first)
        {
            txtFirst.SendKeys(first);
        }
        private void SetPhone1(string phone)
        {
            txtPh1.SendKeys(phone);
        }
        private void SetRPFirst(string first)
        {
            txtRPFirst.SendKeys(first);
        }
        private void SetRPLast(string last)
        {
            txtRPLast.SendKeys(last);
        }
        private void SetRPStreetNumber(string strNum)
        {
            txtRPStrNum.SendKeys(strNum);
        }
        private void SetRPStreetName(string street)
        {
            txtRPStreet.SendKeys(street);
        }
        private void SetRPAddress(string streetAddress)
        {
            string[] streetSplit = streetAddress.Split(new Char[] { ',' });

            txtStrNum.SendKeys(streetSplit[0]);
            txtStreet.SendKeys(streetSplit[1]);
        }
        private void SetRPCity(string city)
        {
            txtRPCity.SendKeys(city);
        }
        private void SetRPState(string state)
        {
            txtRPState.SendKeys(state);
        }
        private void SetRPZip(string zip)
        {
            txtRPZip.SendKeys(zip);
        }
        private void ClickOkAddAccount()
        {
            btnAdd.Click();
        }
        #endregion

        #region Processed Records
        private void ClickProcessedRecordsOk()
        {
            if (session.FindElementByName("All records have now been processed").Displayed)
            {
                btnProcessedRecordsOk.Click();
            }
        }
        #endregion
        //Short functional methods
        #region General Fields
        public void EnterFalseAlarmNoIncTime(string streetAddress, string date = "")
        {
            if (date == "")
            {
                date = DateTime.Today.ToShortDateString();
            }
            string[] streetSplit = streetAddress.Split(new Char[] { ',' });
            SetManualMode();
            SetCaseNumber(CryWolfUtil.GetUniqueCaseNumber());
            SetIncidentDate(CryWolfUtil.GetDateStringNoFormat(DateTime.Parse(date)));
            SetTimeDispatched(CommonTestSettings.ProcessAlarmValues.TimeDispatched);
            SetTimeOnScene(CommonTestSettings.ProcessAlarmValues.TimeOnScene);
            SetTimeCleared(CommonTestSettings.ProcessAlarmValues.TimeCleared);
            SetStreetNumber(streetSplit[0]);
            SetStreetName(streetSplit[1]);

            if (streetSplit.Count() > 2)
            {
                SetAptSuite(streetSplit[2]);
            }

            SetDispatch(CryWolfUtil.GetValidDispatch());
            SetClearance("AF");
            SetDispatcher();
            SetCallTaker();
            SetUnits();
            SetOfficer();
            SetAlarmType();
            SetArea();
            SetDispatcherComments();
            SetOfficerComments();
        }
        public void EnterFalseAlarm(string streetAddress, DateTime date)
        {
            EnterFalseAlarm(streetAddress, date.ToShortDateString());
        }
        public void EnterFalseAlarm(string streetAddress, string date = "")
        {
            if (date == "")
            {
                date = DateTime.Today.ToShortDateString();
            }
            string[] streetSplit = streetAddress.Split(new Char[] { ',' });
            SetManualMode();
            SetCaseNumber(CryWolfUtil.GetUniqueCaseNumber());
            SetIncidentDate(CryWolfUtil.GetDateStringNoFormat(DateTime.Parse(date)));
            SetTimeReceived(CommonTestSettings.ProcessAlarmValues.IncidentTime);
            SetTimeDispatched(CommonTestSettings.ProcessAlarmValues.TimeDispatched);
            SetTimeOnScene(CommonTestSettings.ProcessAlarmValues.TimeOnScene);
            SetTimeCleared(CommonTestSettings.ProcessAlarmValues.TimeCleared);
            SetStreetNumber(streetSplit[0]);
            SetStreetName(streetSplit[1]);

            if (streetSplit.Count() > 2)
            {
                SetAptSuite(streetSplit[2]);
            }

            SetDispatch(CryWolfUtil.GetValidDispatch());
            SetClearance("AF");
            SetDispatcher();
            SetCallTaker();
            SetUnits();
            SetOfficer();
            SetAlarmType();
            SetArea();
            SetDispatcherComments();
            SetOfficerComments();
        }
        public void ProcessFaFileChargeAll(List<string> alarmNumbers)
        {
            //Click Start
            ClickStart();

            //Match Account
            foreach (var alarmNumber in alarmNumbers)
            {
                string name = CryWolfUtil.GetNameOnAccount(alarmNumber);
                MatchAccount(name);
                ChargeFA();
            }
        }
        public void StartManualImport()
        {
            ClickStart();
        }
        public void ProcessFAFileIgnoreFirst(List<string> alarmNumbers)
        {
            ClickStart();

            //Match Account
            foreach (var alarmNumber in alarmNumbers)
            {
                string name = CryWolfUtil.GetNameOnAccount(alarmNumber);
                MatchAccount(name);
                if (alarmNumber == alarmNumbers[0])
                {
                    ClickIgnore();
                }
                else
                {
                    ChargeFA();
                }
            }
        }

        public void MatchAccount(string nameOnAccount)
        {
            TestContext.WriteLine($"Searching for {nameOnAccount}");
            ClickSearch();
            Library.TakeScreenShot(session, "Process Alarm Account Match by Name", out string filePath);

            //Find Last, First or alarmno
            WindowsElement name = frmFAMatches.FindElementByName(nameOnAccount) as WindowsElement;
            name.Click();
            session.Mouse.DoubleClick(name.Coordinates);
            TestContext.WriteLine($"{nameOnAccount} was found");
        }
        public void AddAccount()
        {
            ClickSearch();
            ClickAddAccount();
        }
        public void ChargeFA()
        {
            ClickCharge();
        }
        public void CloseAlarmProcessing()
        {
            ClickCloseButton();
        }
        public int GetFACount()
        {
            return getFACount();
        }
        public void SkipFA()
        {
            ClickNext();
        }
        public bool ValidateAlarmNumber(out string AlarmNo)
        {
            AlarmNo = GetAlarmNumber();
            return AlarmNo.Length > 0;
        }
        #endregion

        #region File Selection
        public void SetManualMode()
        {
            if (lblMode.Text != "Manual")
            {
                ClickSelectModeFile();
                SelectFileFormat(@"[Manual Entry]");
                ClickOK();
            }
        }
        public void SetImportModeFile(string format, string path)
        {
            ClickSelectModeFile();
            SelectFileFormat(format);
            ClickSelectFile();
            ChooseImportFile(path);
            ClickOK();
        }
        public void ChooseImportFile(string path)
        {
            WebDriverWait wait = new WebDriverWait(session, TimeSpan.FromSeconds(60));
            wait.Until(ExpectedConditions.ElementToBeClickable(dlgOpen));

            WindowsElement fileName = dlgOpen.FindElementByClassName("Edit") as WindowsElement;
            fileName.SendKeys(path);
            var open = dlgOpen.FindElementByAccessibilityId("1");
            open.Click();

            HandleWarningIncInDB("ChooseImportFile");
            //temporary to find issue with popup handling
            Thread.Sleep(1000);
        }
        private void HandleWarningIncInDB(string method)
        {
            //WindowsElement windowsElement = frmFAOptions.FindElementByClassName(@"#32770") as WindowsElement;
            WebDriverWait wait = new WebDriverWait(session, TimeSpan.FromSeconds(5));
            wait.Until(session => session.FindElement(By.ClassName(@"#32770")));
            if (frmFAOptions.FindElementsByClassName(@"#32770").Count > 0)
            {
                WindowsElement OK = frmFAOptions.FindElementByClassName(@"#32770").FindElementByAccessibilityId("2") as WindowsElement;
                OK.Click();

                TestContext.WriteLine($"Warning for incidents already in database found and handled during method [{method}]");
            }
            else
            {
                TestContext.WriteLine($"Did not find Warning for incidents already in database during method [{method}]");
            }
        }
        #endregion

        #region Records Processed
        public void SaveSkippedRecords()
        {
            ClickYes();
        }
        public void DiscardSkippedRecords()
        {
            ClickNo();
        }
        public bool ValidateCountingPeriod(string countingMethod, string days, out DateTime from, out DateTime inc)
        {
            from = DateTime.Parse(lblFrom.Text);
            inc = DateTime.Parse(txtIncDate.Text);

            int iDays = int.Parse(days);
            bool status = false;

            switch (countingMethod.ToLower())
            {
                case "floating":
                    status = (inc.AddDays(-iDays) == from);
                    break;
                default:
                    break;
            }
            return status;
        }
        #endregion

        #region Multiple Incidents Warning
        public bool ProcessNormally()
        {
            //session.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));

            //Thread.Sleep(2000);
            try
            {
                TestContext.WriteLine("Waiting for Multiple Incidents Warning");
                WebDriverWait wait = new WebDriverWait(session, TimeSpan.FromSeconds(3));
                wait.Until(ExpectedConditions.ElementExists(By.Name("Multiple Incidents Warning")));

                TestContext.WriteLine("Multiple Incidents Warning dialog was found");
                status = ClickProcessNormally();

            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine("Multiple Incidents Warning dialog was not found after waiting for 3 seconds");
                status = true;
            }
            return status;
        }
        #endregion

        #region Add Account
        public bool ValidateLocationType(string locationType)
        {
            return cbLocationType.Text.Contains(locationType);
        }
        public void CompleteAddAccount(Person person, Address address)
        {
            //Account info
            SetLastName(person.LastName);
            SetFirstName(person.FirstName);
            SetPhone1(person.primaryPhone().Number());

            //Responsible Party
            SetRPLast(person.LastName);
            SetRPFirst(person.FirstName);
            SetRPStreetNumber(address.StreetNumber);
            SetRPStreetName(address.StreetName);
            SetRPCity(address.City);
            SetRPState(address.State);
            SetRPZip(address.ZipCode);

            ClickOkAddAccount();
        }
        #endregion

        #region Processed Records
        public bool AcceptAllRecordsProcessed()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(session, TimeSpan.FromSeconds(2));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Name("All records have now been processed")));
                ClickProcessedRecordsOk();
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine(ErrorStrings.TimeoutException);
                return false;
            }
        }
        #endregion
    }
}
