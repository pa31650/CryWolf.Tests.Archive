using Desktop.Libraries;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Utils;
using Utils.PersonFactory;

namespace Desktop.PageObjects.CryWolf
{
    public class Registrations
    {
        private readonly WindowsDriver<WindowsElement> session;

        string reportAdd;
        string categoryAdd;
        bool status = false;

        public string categoryName = "Account Registrations";

        public Registrations(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification
        #region Main Form
        private WindowsElement frmArs => session.FindElementByAccessibilityId("frmARs");
        private WindowsElement btnClearLinkedRP => frmArs.FindElementByAccessibilityId("btnClearLinkedRP") as WindowsElement;
        private WindowsElement DownPageButton => frmArs.FindElementByAccessibilityId("DownPageButton") as WindowsElement;
        private WindowsElement txtAlarmNo => frmArs.FindElementByAccessibilityId("txtAlarmNo") as WindowsElement;
        private WindowsElement cbLocationType => frmArs.FindElementByAccessibilityId("cbLocationType") as WindowsElement;
        private WindowsElement txtLast => frmArs.FindElementByAccessibilityId("txtLast") as WindowsElement;
        private WindowsElement txtFirst => frmArs.FindElementByAccessibilityId("txtFirst") as WindowsElement;
        private WindowsElement txtBusiness => frmArs.FindElementByAccessibilityId("txtBusiness") as WindowsElement;
        private WindowsElement txtStrNum => frmArs.FindElementByAccessibilityId("txtStrNum") as WindowsElement;
        private WindowsElement txtStreet => frmArs.FindElementByAccessibilityId("txtStreet") as WindowsElement;
        private WindowsElement txtSuite => frmArs.FindElementByAccessibilityId("txtSuite") as WindowsElement;
        private WindowsElement txtCity => frmArs.FindElementByAccessibilityId("txtCity") as WindowsElement;
        private WindowsElement txtState => frmArs.FindElementByAccessibilityId("txtState") as WindowsElement;
        private WindowsElement txtZip => frmArs.FindElementByAccessibilityId("1001") as WindowsElement;
        private WindowsElement txtPh1 => frmArs.FindElementByAccessibilityId("txtPh1") as WindowsElement;
        private WindowsElement txtPh2 => frmArs.FindElementByAccessibilityId("txtPh2") as WindowsElement;
        private WindowsElement txtEmail => frmArs.FindElementByAccessibilityId("txtEmail") as WindowsElement;
        private WindowsElement cbCurrentStatus => frmArs.FindElementByAccessibilityId("cbCurrentStatus") as WindowsElement;
        private WindowsElement dtIssued => frmArs.FindElementByAccessibilityId("dtIssued") as WindowsElement;
        private WindowsElement dtExpiration => frmArs.FindElementByAccessibilityId("dtExpiration") as WindowsElement;
        private WindowsElement lblCounts => frmArs.FindElementByAccessibilityId("lblCounts") as WindowsElement;
        private WindowsElement cbDLState_Person1 => frmArs.FindElementsByAccessibilityId("cbDLState")[0] as WindowsElement;
        private WindowsElement btnCopyDown => frmArs.FindElementByAccessibilityId("btnCopyDown") as WindowsElement;
        private WindowsElement btnUpdate => frmArs.FindElementByAccessibilityId("btnAdd") as WindowsElement;
        private WindowsElement btnClear => frmArs.FindElementByAccessibilityId("btnClear") as WindowsElement;
        private WindowsElement btnCloseWindow => frmArs.FindElementByName("Close") as WindowsElement;
        private WindowsElement lblAlarmNo => frmArs.FindElementByAccessibilityId("lblAlarmNo") as WindowsElement;
        private WindowsElement contextMenu => session.FindElementByName("DropDown");
        private WindowsElement frmSetPassword => session.FindElementByAccessibilityId("frmSetPassword");
        private WindowsElement txtPassword => frmSetPassword.FindElementByAccessibilityId("txtPassword") as WindowsElement;
        private WindowsElement txtVerifyPassword => frmSetPassword.FindElementByAccessibilityId("txtVerifyPassword") as WindowsElement;
        private WindowsElement btnSetPassword => frmSetPassword.FindElementByAccessibilityId("btnOK") as WindowsElement;
        private WindowsElement dialog => frmSetPassword.FindElementByClassName(@"#32770") as WindowsElement;
        private WindowsElement txtMessage => dialog.FindElementByAccessibilityId("65535") as WindowsElement;
        private WindowsElement btnOk => dialog.FindElementByAccessibilityId("2") as WindowsElement;
        #endregion

        #region Person 1
        private WindowsElement fm1 => session.FindElementByAccessibilityId("fm1");
        private WindowsElement person1Last => fm1.FindElementByAccessibilityId("txtLast") as WindowsElement;
        private WindowsElement person1First => fm1.FindElementByAccessibilityId("txtFirst") as WindowsElement;
        private WindowsElement person1StrNum => fm1.FindElementByAccessibilityId("txtStrNum") as WindowsElement;
        private WindowsElement person1Street => fm1.FindElementByAccessibilityId("txtStreet") as WindowsElement;
        private WindowsElement person1Suite => fm1.FindElementByAccessibilityId("txtSuite") as WindowsElement;
        private WindowsElement person1City => fm1.FindElementByAccessibilityId("txtCity") as WindowsElement;
        private WindowsElement person1State => fm1.FindElementByAccessibilityId("txtState") as WindowsElement;
        private WindowsElement person1Zip => fm1.FindElementByAccessibilityId("txtZip") as WindowsElement;
        private WindowsElement person1Email => fm1.FindElementByAccessibilityId("txtEmail") as WindowsElement;
        private WindowsElement person1Ph1 => fm1.FindElementByAccessibilityId("txtPh1") as WindowsElement;
        private WindowsElement person1Ph2 => fm1.FindElementByAccessibilityId("txtPh2") as WindowsElement;
        #endregion

        #region Related Alarm Companies
        private WindowsElement tabAlarmCompanies => session.FindElementByName("Related Alarm Companies") as WindowsElement;
        private WindowsElement paneAlarmCompanies => session.FindElementByAccessibilityId("tabPage2") as WindowsElement;
        private WindowsElement btnClearACs => paneAlarmCompanies.FindElementByAccessibilityId("btnClearACs") as WindowsElement;
        private WindowsElement btnClearSelectedAlarmCompany => paneAlarmCompanies.FindElementByAccessibilityId("btnClearSelectedAlarmCompany") as WindowsElement;
        private WindowsElement btnCopyMonitoringToOthers => paneAlarmCompanies.FindElementByAccessibilityId("btnCopyMonitoringToOthers") as WindowsElement;
        private WindowsElement ACGrid => paneAlarmCompanies.FindElementByAccessibilityId("ACGrid") as WindowsElement;
        private WindowsElement btnMonitoredBy => paneAlarmCompanies.FindElementByAccessibilityId("btnMonitoredBy") as WindowsElement;
        private WindowsElement btnSoldBy => paneAlarmCompanies.FindElementByAccessibilityId("btnSoldBy") as WindowsElement;
        private WindowsElement btnServicedBy => paneAlarmCompanies.FindElementByAccessibilityId("btnServicedBy") as WindowsElement;
        private WindowsElement btnInstalledBy => paneAlarmCompanies.FindElementByAccessibilityId("btnInstalledBy") as WindowsElement;
        private WindowsElement frmACLists => session.FindElementByAccessibilityId("frmACLists") as WindowsElement;
        #endregion

        #region Expiration Dialog
        private WindowsElement dlgExpirationMessage => session.FindElementByClassName(@"#32770");
        private WindowsElement txtExpirationMessage => dlgExpirationMessage.FindElementByName("The expiration date cannot be before the issue date./n/rPlease adjust the expiration date before saving.") as WindowsElement;
        private WindowsElement btnExpirationOK => dlgExpirationMessage.FindElementByAccessibilityId("2") as WindowsElement;
        #endregion

        # region Confirm Permit Period Change
        private WindowsElement dlgPermitChangePeriod => session.FindElementByName("Confirm Permit Period Change");
        private WindowsElement btnChangePermitPeriodYes => dlgPermitChangePeriod.FindElementByName("Yes") as WindowsElement;
        private WindowsElement btnChangePermitPeriodNo => dlgPermitChangePeriod.FindElementByName("No") as WindowsElement;
        #endregion

        # region GeoCode Results
        private WindowsElement frmGeoCodeResults => session.FindElementByAccessibilityId("frmGeoCodeResults");
        private WindowsElement ckToggleAll => frmGeoCodeResults.FindElementByAccessibilityId("ckToggleAll") as WindowsElement;
        private WindowsElement btnGeoCodeSuccessOk => frmGeoCodeResults.FindElementByAccessibilityId("btnOK") as WindowsElement;
        #endregion

        //Basic Interactions
        private void DismissPasswordDialog()
        {
            btnOk.Click();
        }
        private string GetPasswordMessage()
        {
            return txtMessage.Text;
        }
        private void ClickSetPassword()
        {
            btnSetPassword.Click();
        }
        private void SetPassword(string password)
        {
            txtPassword.SendKeys(password);
        }
        private void SetVerifyPassword(string password)
        {
            txtVerifyPassword.SendKeys(password);
        }
        private void SelectContextMenuItem(string menuItem)
        {
            RightClickAccount();
            Actions action = new Actions(session);
            action.MoveToElement(contextMenu.FindElementByName(menuItem));
            action.Click();
            action.Perform();
        }
        private void RightClickAccount()
        {
            Actions action = new Actions(session);
            action.MoveToElement(lblAlarmNo);
            action.ContextClick();
            action.Perform();

        }
        private void ClickCopyMonitoringToOthers()
        {
            btnCopyMonitoringToOthers.Click();
        }
        private void ClickAssignMonitoredBy()
        {
            btnMonitoredBy.Click();
        }

        private void ClickAssignSoldBy()
        {
            btnSoldBy.Click();
        }

        private void ClickAssignServicedBy()
        {
            btnServicedBy.Click();
        }

        private void ClickAssignInstalledBy()
        {
            btnInstalledBy.Click();
        }

        private void ClickClearAllACs()
        {
            btnClearACs.Click();
        }

        private void ClickClearSelectedAC()
        {
            btnClearSelectedAlarmCompany.Click();
        }

        private void ClickRelatedAlarmCompaniesTab()
        {
            tabAlarmCompanies.Click();
        }

        private void SetAlarmNo(string alarmNo)
        {
            txtAlarmNo.SendKeys(alarmNo);
        }

        private void SetLocationType(string locationType)
        {
            TestContext.WriteLine($"Setting Location Type to: [{locationType}]");
            cbLocationType.SelectListItem(locationType);
        }

        private List<string> GetLocationTypes()
        {
            List<string> vs = new List<string>();
            cbLocationType.GetListItems(out vs);
            return vs;
        }

        private void SetLastName(string lastName)
        {
            TestContext.WriteLine($"Setting Last Name to: [{lastName}]");
            txtLast.SendKeys(lastName);
        }

        private void SetFirstName(string firstName)
        {
            TestContext.WriteLine($"Setting Last Name to: [{firstName}]");
            txtFirst.SendKeys(firstName);
        }

        private void SetStreetNumber(string streetNumber)
        {
            TestContext.WriteLine($"Setting Street Number to: [{streetNumber}]");
            txtStrNum.SendKeys(streetNumber);
        }

        private void SetStreet(string street)
        {
            TestContext.WriteLine($"Setting Street to: [{street}]");
            txtStreet.SendKeys(street);
        }

        private void SetSuite(string suite)
        {
            txtSuite.SendKeys(suite);
        }

        private void SetCity(string city)
        {
            txtCity.SendKeys(city);
        }

        private void SetState(string state)
        {
            txtState.SendKeys(state);
        }

        private void SetZipCode(string zip)
        {
            txtZip.SendKeys(zip);
        }

        private void SetHomePhone(string homePhone)
        {
            txtPh1.SendKeys(homePhone);
        }

        private void SetWorkPhone(string workPhone)
        {
            txtPh2.SendKeys(workPhone);
        }

        private void SetEmail(string email)
        {
            txtEmail.SendKeys(email);
        }

        private void ClickCopyDownButton()
        {
            btnCopyDown.Click();
        }

        private void ClickUpdateButton()
        {
            btnUpdate.Click();
        }

        private void ClickClearButton()
        {
            btnClear.Click();
        }

        private void ClickCloseButton()
        {
            btnCloseWindow.Click();
        }

        private void ClickDownPageButton()
        {
            DownPageButton.Click();
        }

        private void AcceptPermitPeriodChange()
        {
            btnChangePermitPeriodYes.Click();
        }

        private void DiscardPermitPeriodChange()
        {
            btnChangePermitPeriodNo.Click();
        }

        private void ToggleLoadValues()
        {
            TestContext.WriteLine("Clicking Toggle All checkbox");
            ckToggleAll.Click();
        }

        private void ClickGeoCodeSuccessOK()
        {
            btnGeoCodeSuccessOk.Click();
        }

        private void CloseExpirationMessageDialog()
        {
            btnExpirationOK.Click();
        }

        private void ClickClearLinkedRPButton()
        {
            btnClearLinkedRP.Click();
        }

        //Short Functional methods
        #region Change Password
        public bool ResetPassword(string newPassword)
        {
            SetPassword(newPassword);
            SetVerifyPassword(newPassword);
            ClickSetPassword();

            string message = GetPasswordMessage();
            TestContext.WriteLine(message);

            Library.TakeScreenShot(session, "Password Reset Message", out string filePath);

            DismissPasswordDialog();
            return message.Contains("password was changed successfully");
        }

        public void OpenResetPassword()
        {
            SelectContextMenuItem("Reset Password");
        }
        #endregion

        #region Alarm Companies
        public void AssignAlarmCompany(string acType, string acNumber)
        {
            //Click button
            switch (acType.Replace(" ", "").ToLower())
            {
                case "monitoredby":
                    ClickAssignMonitoredBy();
                    break;
                case "servicedby":
                    ClickAssignServicedBy();
                    break;
                case "soldby":
                    ClickAssignSoldBy();
                    break;
                case "installedby":
                    ClickAssignInstalledBy();
                    break;
                default:
                    break;
            }

            //Double click alarm Account#
            WindowsElement name = frmACLists.FindElementByName(acNumber) as WindowsElement;
            name.Click();
            session.Mouse.DoubleClick(name.Coordinates);

            TestContext.WriteLine($"{acNumber} was found");
        }

        public void CopyMonitoringACToOthers()
        {
            ClickCopyMonitoringToOthers();
        }
        public void ClearAlarmCompany(string acType)
        {
            SelectAlarmCompanyType(acType);

            ClickClearSelectedAC();
        }
        public bool ValidateAlarmCompaniesBlank()
        {
            return ValidateAlarmCompanies("");
        }
        public bool ValidateAlarmCompanies(string acNumber)
        {
            bool status;

            status = ValidateMonitoredBy(acNumber) &&
            ValidateSoldBy(acNumber) &&
            ValidateServicedBy(acNumber) &&
            ValidateInstalledBy(acNumber);

            return status;
        }
        public bool ValidateAlarmCompanyBlank(string acType)
        {
            return ValidateAlarmCompany(acType, "");
        }
        public bool ValidateMonitoredBy(string expectedAcNumber)
        {
            return ValidateAlarmCompany("Monitored By", expectedAcNumber);
        }
        public bool ValidateSoldBy(string expectedAcNumber)
        {
            return ValidateAlarmCompany("Sold By", expectedAcNumber);
        }
        public bool ValidateServicedBy(string expectedAcNumber)
        {
            return ValidateAlarmCompany("Serviced By", expectedAcNumber);
        }
        public bool ValidateInstalledBy(string expectedAcNumber)
        {
            return ValidateAlarmCompany("Installed By", expectedAcNumber);
        }
        public bool ValidateAlarmCompany(string acType, string expectedAcNumber)
        {
            //Compare ACNumber
            string actualACNumber = GetAlarmCompanyByType(acType, out string acName);
            bool status = expectedAcNumber.Equals(actualACNumber);

            TestContext.WriteLine($"{acType} Company info found was {actualACNumber}:{acName}");

            return status;
        }
        public void ClearRelatedAlarmCompanies()
        {
            ClickClearAllACs();
        }
        public string GetAlarmCompanyByType(string acType, out string acName)
        {
            int row = ACGrid.GetRowWithCellText(acType);
            int column = ACGrid.GetColumnWithCellText(@"Cmpny #");
            string acNumber = ACGrid.GetCellData(row, column);
            acName = ACGrid.GetCellData(row, column + 1);
            return acNumber;
        }
        public void SelectAlarmCompanyType(string acType)
        {
            int row = ACGrid.GetRowWithCellText(acType);
            int column = ACGrid.GetColumnWithCellText(@"Cmpny #");

            WindowsElement cell = ACGrid.GetCell(row, column);

            cell.Click();
        }

        public void NavigateToRelatedAC()
        {
            ClickRelatedAlarmCompaniesTab();
        }
        #endregion

        public void EditLocationType(string locationType)
        {
            SetLocationType(locationType);
        }

        public void OpenAccountRegistration(string alarmNo)
        {
            if (!txtAlarmNo.Enabled)
            {
                ClickClearButton();
            }

            SetAlarmNo(alarmNo);

            session.Keyboard.SendKeys(Keys.Enter);

            ReportBuilder.ArrayBuilder($"Loading Account Registration window for [{alarmNo}]", true, "UI Navigation");

            var dlgWindows = session.FindElementsByClassName(@"#32770");
            if (dlgWindows.Count != 0)
            {
                foreach (var dialog in dlgWindows)
                {
                    Utility.SwitchWindow("Validation Error", session);
                    string txtMessage = dialog.FindElementByTagName("Text").Text;
                    Console.WriteLine(txtMessage);
                    dialog.FindElementByName("OK").Click();
                }

            }
        }

        public bool ValidateLocationType(string locationType)
        {
            return cbLocationType.Text.Contains(locationType);
        }

        public bool ValidateLocationTypeArchive(string locationType)
        {
            return !GetLocationTypes().Contains(locationType);
        }

        public bool ValidateLocationTypeActive(string locationType)
        {
            return GetLocationTypes().Contains(locationType);
        }

        public bool CheckAccountStatus(string statusToCheck, out string actualStatus)
        {
            actualStatus = cbCurrentStatus.Text;
            return actualStatus == statusToCheck;
        }

        public bool CheckAccountIssueDate(DateTime issueToCheck, out string actualIssue)
        {
            actualIssue = dtIssued.Text;
            return DateTime.Parse(actualIssue) == issueToCheck;
        }

        public void EditAccountIssueDate(string issue)
        {
            dtIssued.SendKeys(CryWolfUtil.GetDateStringNoFormat(DateTime.Parse(issue)));
            ReportBuilder.ArrayBuilder($"Updating Account Issue Date to [{issue}]", true, "Edit Account");
        }

        public void EditAccountExpirationDate(string expiration = "")
        {
            if (expiration != "")
            {
                dtExpiration.SendKeys(CryWolfUtil.GetDateStringNoFormat(DateTime.Parse(expiration)));
            }
        }

        public bool CheckAccountExpDate(DateTime expToCheck, out string actualExp)
        {
            actualExp = dtExpiration.Text;
            return DateTime.Parse(actualExp) == expToCheck;
        }

        public int GetFalseAlarmCount()
        {
            return Convert.ToInt32(lblCounts.Text.Split()[0]);
        }

        public string GetAlarmNumber()
        {
            return txtAlarmNo.Text;
        }

        public string GetNameOnAccount()
        {
            string firstName = txtFirst.Text;
            string lastName = txtLast.Text;

            //if (cbLocationType.Text == "Commercial")
            //{
            //    SetLocationType("Residential");
            //}

            if (cbLocationType.Text == "Commercial")
            {
                TestContext.WriteLine($"Name on Account was: Last - {lastName}");
                return lastName;
            }
            else
            {
                TestContext.WriteLine($"Name on Account was: First - {firstName} Last - {lastName}");
                return $"{lastName}, {firstName}";
            }

            //if (firstName == "")
            //{
            //    return lastName;
            //}
            //else
            //{
            //    return $"{lastName}, {firstName}";
            //}
        }

        public string GetStreetAddressOnAccount()
        {
            string address;
            string suite = txtSuite.Text;
            string strNum = txtStrNum.Text;
            string street = txtStreet.Text;

            if (suite == "")
            {
                address = $"{strNum},{street}";
            }
            address = $"{strNum},{street},{suite}";

            Console.WriteLine($"Address found was: {address}");

            return address;
        }

        public void CloseRegistrationsWindow()
        {
            frmArs.Click();
            ClickCloseButton();
        }

        public void ChooseDLState(string state)
        {
            cbDLState_Person1.SelectListItem(state);
        }

        public List<string> GetDLStates()
        {
            List<string> vs = new List<string>();
            for (int i = 0; i < 25; i++)
            {
                cbDLState_Person1.GetListItems(out vs);
                while (vs.Count < 1)
                {
                    ClickDownPageButton();
                    cbDLState_Person1.GetListItems(out vs);
                }
                break;
            }
            return vs;
        }

        public void UpdateAccount()
        {
            ClickUpdateButton();
            ReportBuilder.ArrayBuilder("Updating Account with changes made", true, "Account Update");
        }

        public bool ValidatePermitChangeDialog([CallerMemberName] string caller = null)
        {
            WebDriverWait driverWait = new WebDriverWait(session, TimeSpan.FromSeconds(5));

            bool status = dlgPermitChangePeriod.Displayed;

            Library.TakeScreenShot(session, caller, out string filePath);

            if (status)
            {
                AcceptPermitPeriodChange();
            }

            return status;
        }

        public void EnterNewAccountInfo(Person person, Address address, string alarmType, bool autoGeoCode)
        {
            if (!txtAlarmNo.Enabled)
            {
                ClickClearButton();
            }

            if (!person.isNew())
            {
                person.getNewFullName(person);
            }

            if (!address.isNew())
            {
                address.GetNewAddress();
            }

            if (address.City == string.Empty)
            {
                address.SetCityAsDefault();
            }

            if (address.ZipCode == string.Empty)
            {
                address.SetZipAsDefault();
            }

            if (address.StreetNumber == string.Empty)
            {
                address.GetNewAddress();
            }

            //location type
            SetLocationType(alarmType);

            //name
            SetLastName(person.LastName);
            SetFirstName(person.FirstName);

            //address
            SetStreetNumber(address.StreetNumber);
            SetStreet(address.StreetName);

            if (autoGeoCode)
            {
                txtCity.Click();

                WebDriverWait wait = new WebDriverWait(session, TimeSpan.FromSeconds(2));
                wait.Until(ExpectedConditions.ElementToBeClickable(frmGeoCodeResults));

                if (frmGeoCodeResults.Displayed)
                {
                    ToggleLoadValues();
                    ClickGeoCodeSuccessOK();
                }
                else
                {
                    throw new Exception("GeoCode Results Not Successful");
                }

                var geoWindows = session.FindElementsByAccessibilityId("frmGeoCodeResults");

                if (geoWindows.Count > 1)
                {
                    TestContext.WriteLine($"Multiple GeoCode Windows found. Count: [{geoWindows.Count}]");

                    foreach (var geoWindow in geoWindows)
                    {
                        geoWindow.FindElementByName("OK").Click();
                    }
                }
            }
            else
            {
                SetCity(address.City);
                SetState(address.State);
                SetZipCode(address.ZipCode);
            }

            SetHomePhone(person.primaryPhone().Number());
            SetWorkPhone(person.primaryPhone().Number());
            SetEmail(person.primaryEmail().GetEmail());

        }

        public void CopyToRP()
        {
            ClickCopyDownButton();
        }

        public void ClearLinkedRP()
        {
            ClickClearLinkedRPButton();
        }

        public bool ValidateCopyToRP(string behavior)
        {
            bool status = false;
            string locationType = cbLocationType.Text;

            if (behavior.ToLower().Contains("hidden"))
            {

                return frmArs.FindElementsByAccessibilityId("btnCopyDown").Count == 0;
            }

            //Click Clear
            ClearLinkedRP();

            //Click Copy
            CopyToRP();

            switch (behavior.Replace(" ", string.Empty).ToLower())
            {
                case "addressonly":
                    if (!CompareName())
                    {
                        status = true;
                    }
                    else
                    {
                        //TestContext.WriteLine("Name compare failed");
                        status = false;
                        break;
                    }

                    if (CompareAddress())
                    {
                        status = true;
                    }
                    else
                    {
                        //TestContext.WriteLine("Address compare failed");
                        status = false;
                        break;
                    }

                    if (CompareContactInfo())
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                        break;
                    }

                    break;

                case "nameresidential":
                    if (locationType.ToLower().Contains("residential"))
                    {
                        if (CompareName())
                        {
                            status = true;
                        }
                        else
                        {
                            status = false;
                            break;
                        }
                        if (CompareAddress())
                        {
                            status = true;
                        }
                        else
                        {
                            status = false;
                            break;
                        }
                        if (CompareContactInfo())
                        {
                            status = true;
                        }
                        else
                        {
                            status = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!CompareName())
                        {
                            status = true;
                        }
                        else
                        {
                            status = false;
                            break;
                        }
                        if (CompareAddress())
                        {
                            status = true;
                        }
                        else
                        {
                            status = false;
                            break;
                        }
                        if (CompareContactInfo())
                        {
                            status = true;
                        }
                        else
                        {
                            status = false;
                            break;
                        }
                    }
                    break;

                case "default":
                    if (CompareName())
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                        break;
                    }

                    if (CompareAddress())
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                        break;
                    }

                    if (CompareContactInfo())
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                        break;
                    }

                    break;

                default:
                    break;
            }

            return status;
        }

        private bool CompareContactInfo()
        {
            string phone1 = txtPh1.Text.Trim();
            string phone2 = txtPh2.Text.Trim();
            string email = txtEmail.Text.Trim();
            //
            string person1_phone1 = person1Ph1.Text.Trim();
            string person1_phone2 = person1Ph2.Text.Trim();
            string person1_email = person1Email.Text.Trim();

            bool status = (phone1.Equals(person1_phone1))
                && (phone2.Equals(person1_phone2))
                && (email.Equals(person1_email));

            if (status)
            {
                Console.WriteLine(
                            $"Account Contact Info: Phone1 {phone1}, Phone2 {phone2}, Email {email} " +
                            $"matches Person1 Phone1 {person1_phone1}, Phone 2 {person1_phone2}, Email {person1_email}");
            }
            else
            {
                Console.WriteLine(
                            $"Account Contact Info: Phone1 {phone1}, Phone2 {phone2}, Email {email} " +
                            $"does not match Person1 Phone1 {person1_phone1}, Phone 2 {person1_phone2}, Email {person1_email}");
            }
            return status;
        }

        private bool CompareAddress()
        {
            string strNum = txtStrNum.Text;
            string street = txtStreet.Text;
            string suite = txtSuite.Text;
            string city = txtCity.Text;
            string state = txtState.Text;
            string zip = txtZip.Text;
            //
            string person1_strNum = person1StrNum.Text;
            string person1_street = person1Street.Text;
            string person1_suite = person1Suite.Text;
            string person1_city = person1City.Text;
            string person1_state = person1State.Text;
            string person1_zip = person1Zip.Text;

            bool status = (strNum.Equals(person1_strNum))
                && (street.Equals(person1_street))
                && (suite.Equals(person1_suite))
                && (city.Equals(person1_city))
                && (state.Equals(person1_state))
                && (zip.Equals(person1_zip));

            if (status)
            {
                Console.WriteLine($"Account Address: {strNum} {street} {suite}{Environment.NewLine}" +
                    $"{city}, {state} {zip}{Environment.NewLine}" +
                    $"matches Person1 Address: {person1_strNum} {person1_street} {person1_suite}{Environment.NewLine}" +
                    $"{person1_city}, {person1_state} {person1_zip}");
            }
            else
            {
                Console.WriteLine($"Account Address: {strNum} {street} {suite}{Environment.NewLine}" +
                    $"{city}, {state} {zip}{Environment.NewLine}" +
                    $"doesn't match Person1 Address: {person1_strNum} {person1_street} {person1_suite}{Environment.NewLine}" +
                    $"{person1_city}, {person1_state} {person1_zip}");
            }

            return status;
        }

        private bool CompareName()
        {
            string lastName = txtLast.Text;
            string firstName = txtFirst.Text;
            //
            string person1_lastName = person1Last.Text;
            string person1_firstName = person1First.Text;

            bool status = (lastName.Equals(person1_lastName))
                && (firstName.Equals(person1_firstName));

            if (status)
            {
                Console.WriteLine($"Account Name {lastName}, {firstName} matches Person 1 {person1_lastName}, {person1_firstName}");
            }
            else
            {
                Console.WriteLine($"Account Name {lastName}, {firstName} doesn't match Person 1 {person1_lastName}, {person1_firstName}");
            }
            return status;
        }

        public void AddRelatedPerson(Person person = null)
        {
            if (person == null)
            {
                CopyToRP();
            }
        }

        public bool ValidateExpirationDialog()
        {
            status = dlgExpirationMessage.Displayed;
            Library.TakeScreenShot(session, "Validate Expiration Date Error Dialog", out string path);
            CloseExpirationMessageDialog();

            return status;
        }
    }
}
