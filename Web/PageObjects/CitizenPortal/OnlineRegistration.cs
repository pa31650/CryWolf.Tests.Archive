using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using Utils;
using Utils.PersonFactory;

namespace Web.PageObjects.CitizenPortal
{
    public class OnlineRegistration : CommonElements
    {
        private IWebDriver driver = null;

        string reportAdd;
        string categoryAdd;
        bool status = true;
        string DLNumMissing = "Please enter your driver's license number.";
        string DLStateMissing = "Please select a state for your driver's license.";
        string MonitorContacts = "Please submit either the company that monitors this location or two keyholder contacts.";

        public string categoryName = "Citizen Registration Page";

        public OnlineRegistration(IWebDriver driver)
        {
            this.driver = driver;
        }

        //Object Identification
        #region Page Elements

        //Alarmed Location
        private SelectElement cbLocation => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_cbLocation")));
        private IWebElement txtLastName => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtLastName"));
        private IWebElement txtFirstName => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtFirstName"));
        private IWebElement txtStrNum => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtStrNum"));
        private IWebElement txtStreet => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtStreet"));
        private IWebElement txtCity => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtCity"));
        private IWebElement lblState => driver.FindElement(By.Id("spstate1"));
        private SelectElement ddAlarmedLocationState => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlState")));
        private IWebElement lblZip => driver.FindElement(By.Id("spzip1"));
        private IWebElement txtZip => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtZip"));
        private IWebElement txtPhone1 => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtPhone1"));
        private IWebElement txtEmail => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtEmail"));

        //Mailing Information
        private IWebElement ckUseLocation => driver.FindElement(By.Id("ckUseLocation"));
        private IWebElement lblMailState => driver.FindElement(By.Id("spstate2"));
        private SelectElement ddMailingState => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlRPState")));
        private IWebElement lblMailZip => driver.FindElement(By.Id("spzip2"));
        private IWebElement txtDateOfBirth => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtCustom1"));
        private IWebElement dtpDobCalendar => driver.FindElement(By.Id("ui-datepicker-div")).FindElement(By.ClassName("ui-datepicker-calendar"));

        //Contact 1
        private IWebElement txtC1LastName => driver.FindElement(By.Id("txtC1LastName"));
        private IWebElement txtC1FirstName => driver.FindElement(By.Id("txtC1FirstName"));
        private IWebElement lblContact1Address => driver.FindElement(By.Id("spcsz1"));
        private IWebElement txtC1Phone1 => driver.FindElement(By.Id("txtC1Phone1"));
        private SelectElement ddContact1State => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlC1State")));

        //Contact 2
        private IWebElement txtC2LastName => driver.FindElement(By.Id("txtC2LastName"));
        private IWebElement txtC2FirstName => driver.FindElement(By.Id("txtC2FirstName"));
        private IWebElement lblContact2Address => driver.FindElement(By.Id("spcsz2"));
        private IWebElement txtC2Phone1 => driver.FindElement(By.Id("txtC2Phone1"));
        private SelectElement ddContact2State => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlC2State")));

        //Alarm Company Information
        //private SelectElement cbMonitoredBy => new SelectElement(driver.FindElement(By.Id("cbMonitoredBy")));

        //Password
        private IWebElement txtPswrd1 => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtPswrd1"));
        private IWebElement txtPswrd2 => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtPswrd2"));
        private IWebElement ckspconfirm => driver.FindElement(By.Id("spconfirm"));

        //Submit
        private IWebElement btnSubmit => driver.FindElement(By.Id("btnSubmit"));

        //Error Messages
        private IWebElement lblRejected => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_lblRejected"));
        #endregion

        //Basic Interactions
        #region Basic UI Interactions

        #region Alarmed Location        
        private void SelectLocation(string location)
        {
            cbLocation.SelectByText(location);
        }
        private void EnterAlarmedLastName(string lastName)
        {
            txtLastName.SendKeys(lastName);
        }
        private void EnterAlarmedFirstName(string firstName)
        {
            txtFirstName.SendKeys(firstName);
        }
        private void EnterAlarmedStreetNum(string streetNum)
        {
            txtStrNum.SendKeys(streetNum);
        }
        private void EnterAlarmedStreetName(string streetName)
        {
            txtStreet.SendKeys(streetName);
        }
        private void EnterAlarmedCity(string city)
        {
            WaitFor(driver, txtCity);
            txtCity.Clear();
            txtCity.SendKeys(city);
            WaitFor(driver, txtCity);

            if (txtCity.Text != city)
            {
                txtCity.Clear();
                txtCity.SendKeys(city);
            }
        }
        private string GetAlarmedStateLabelText()
        {
            return lblState.Text;
        }
        private void SelectAlarmedState(string state)
        {
            ddAlarmedLocationState.SelectByText(state);
        }
        private string GetAlarmedZipLabelText()
        {
            return lblZip.Text;
        }
        private void EnterAlarmedZip(string zip)
        {
            txtZip.SendKeys(zip);
        }
        private void EnterMainPhone(string phone)
        {
            txtPhone1.SendKeys(phone);
        }
        private void EnterAlarmedEmail(string email)
        {
            txtEmail.SendKeys(email);
        }
        #endregion

        #region Mailing/Billing Information
        private void SetAlarmedLocationInformation(bool use)
        {
            if (use && !ckUseLocation.Selected)
            {
                ckUseLocation.Click();
            }
            else if (!use && ckUseLocation.Selected)
            {
                ckUseLocation.Click();
            }
        }
        private string GetMailingStateLabelText()
        {
            return lblMailState.Text;
        }
        private void SelectMailingState(string state)
        {

            ddMailingState.SelectByText(state);
        }
        private string GetMailingZipLabelText()
        {
            return lblMailZip.Text;
        }
        #endregion

        #region Contact1
        private void SetContact1LastName(string last)
        {
            txtC1LastName.SendKeys(last);
        }

        private void SetContact1FirstName(string first)
        {
            txtC1FirstName.SendKeys(first);
        }

        private void SetContact1Phone1(string phone)
        {
            txtC1Phone1.SendKeys(phone);
        }

        private void SelectContact1State(string state)
        {
            ddContact1State.SelectByText(state);
        }
        private string GetContact1AddressLabelText()
        {
            return lblContact1Address.Text;
        }
        #endregion

        #region Contact2
        private void SetContact2LastName(string last)
        {
            txtC2LastName.SendKeys(last);
        }

        private void SetContact2FirstName(string first)
        {
            txtC2FirstName.SendKeys(first);
        }

        private void SetContact2Phone1(string phone)
        {
            txtC2Phone1.SendKeys(phone);
        }
        private void SelectContact2State(string state)
        {
            ddContact2State.SelectByText(state);
        }
        private string GetContact2AddressLabelText()
        {
            return lblContact2Address.Text;
        }
        #endregion

        #region Alarm Company Information
        //private void SelectMonitoringCompanyByValue(string monitoringCo)
        //{
        //    cbMonitoredBy.SelectByValue(monitoringCo);

        //}
        #endregion

        private bool IsDateSelected()
        {
            List<IWebElement> selectedDate = new List<IWebElement>(dtpDobCalendar.FindElements(By.ClassName("  ui-datepicker-current-day")));

            return (selectedDate.Count == 0);
        }

        #region Password
        private void EnterPassword(string newPassword)
        {
            txtPswrd1.SendKeys(newPassword);
        }
        private void ConfirmPassword(string newPassword)
        {
            txtPswrd2.SendKeys(newPassword);
        }
        private void ClickAcceptTerms()
        {
            ckspconfirm.Click();
        }
        private bool AreTermsAccepted()
        {
            return ckspconfirm.Selected;
        }
        #endregion

        private void ClickSubmit()
        {
            btnSubmit.Click();
        }

        #region Rejected
        private bool IsDlStateReqDisplayed()
        {
            return lblRejected.Text == DLStateMissing;
        }
        private bool IsDlNumReqDisplayed()
        {
            return lblRejected.Text == DLNumMissing;
        }

        #endregion

        #endregion

        //Short Functional Methods
        #region Short Business Processes
        public bool ValidateAddressLabels(string defaultLocale)
        {
            switch (defaultLocale)
            {
                case "en-ca":
                    return ValidateCanadianAddress();
                case "en-us":
                    return ValidateAmericanAddress();
                default:
                    return false;
            }
        }
        private bool ValidateCanadianAddress()
        {
            string actualAlarmedState = GetAlarmedStateLabelText();
            string actualAlarmedZip = GetAlarmedZipLabelText();
            string actualMailingState = GetMailingStateLabelText();
            string actualMailingZip = GetMailingZipLabelText();
            string actualContact1Address = GetContact1AddressLabelText();
            string actualContact2Address = GetContact2AddressLabelText();

            return actualAlarmedState.Equals("Province") &&
                actualAlarmedZip.Equals("Postal Code") &&
                actualMailingState.Equals("Province") &&
                actualMailingZip.Equals("Postal Code") &&
                actualContact1Address.Equals("City Prov Post") &&
                actualContact2Address.Equals("City Prov Post");
        }
        private bool ValidateAmericanAddress()
        {
            string actualAlarmedState = GetAlarmedStateLabelText();
            string actualAlarmedZip = GetAlarmedZipLabelText();
            string actualMailingState = GetMailingStateLabelText();
            string actualMailingZip = GetMailingZipLabelText();
            string actualContact1Address = GetContact1AddressLabelText();
            string actualContact2Address = GetContact2AddressLabelText();

            return actualAlarmedState.Equals("State") &&
                actualAlarmedZip.Equals("Zip Code") &&
                actualMailingState.Equals("State") &&
                actualMailingZip.Equals("Zip Code") &&
                actualContact1Address.Equals("City State Zip") &&
                actualContact2Address.Equals("City State Zip");
        }
        //public bool SelectMonitoringCompany(string alarmCo)
        //{
        //    IList<IWebElement> monitoringOptions = cbMonitoredBy.Options;

        //    foreach (var option in monitoringOptions)
        //    {
        //        if (option.Text.Contains($"({alarmCo})"))
        //        {
        //            SelectMonitoringCompanyByValue(option.Text);
        //            return true;
        //        }
        //    }

        //    TestContext.WriteLine($"Alarm Company containing {alarmCo} was not found");
        //    return false;
        //}
        public bool ValidateDlStateReq()
        {
            return IsDlStateReqDisplayed();
        }
        public bool ValidateDlNumberReq()
        {
            return IsDlNumReqDisplayed();
        }
        public void SubmitRegistrationInformation()
        {
            ClickSubmit();
        }
        public void AcceptTermsAndConditions()
        {
            if (!AreTermsAccepted())
            {
                ClickAcceptTerms();
            }
        }
        public void CreatePassword(string newPassword)
        {
            EnterPassword(newPassword);
            ConfirmPassword(newPassword);
        }
        public void UseAlarmedLocationInformation(bool use)
        {
            SetAlarmedLocationInformation(use);
        }
        public void EnterResidentialLocation(Person person, Address address)
        {
            EnterAlarmedLocation(person, address, "Residential");
        }
        public void EnterCommercialLocation(Person person, Address address)
        {
            EnterAlarmedLocation(person, address, "Commercial");
        }
        public void EnterAlarmedLocation(Person person, Address address, string location)
        {
            if (address.City == string.Empty)
            {
                address.SetCityAsDefault();
            }

            TestContext.WriteLine($"Selecting Location: [{location}]");
            SelectLocation(location);

            TestContext.WriteLine($"Entering Alarmed Location information: [{person.FirstName}] [{person.LastName}]{Environment.NewLine}" +
                $"[{address.StreetNumber}] [{address.StreetName}]");
            EnterAlarmedLastName(person.LastName);
            EnterAlarmedFirstName(person.FirstName);
            EnterAlarmedStreetNum(address.StreetNumber);
            EnterAlarmedStreetName(address.StreetName);

            EnterAlarmedCity(address.City);
            SelectAlarmedState(address.State);
            EnterAlarmedZip(address.ZipCode);
            EnterMainPhone(person.primaryPhone().Number());
            EnterAlarmedEmail(person.primaryEmail().GetEmail());
        }
        public void EnterAlarmed_No_Location(Person person, Address address)
        {
            //SelectLocation(location);
            EnterAlarmedLastName(person.LastName);
            EnterAlarmedFirstName(person.FirstName);
            EnterAlarmedStreetNum(address.StreetNumber);
            EnterAlarmedStreetName(address.StreetName);
            EnterAlarmedCity(address.City);
            SelectAlarmedState(address.State);
            EnterAlarmedZip(address.ZipCode);
            EnterMainPhone(person.primaryPhone().Number());
            EnterAlarmedEmail(person.primaryEmail().GetEmail());
        }
        public void EnterContact1Information(Person person)
        {
            SetContact1LastName(person.LastName);
            SetContact1FirstName(person.FirstName);
            SetContact1Phone1(person.primaryPhone().Number());
        }
        public void EnterContact2Information(Person person)
        {
            SetContact2LastName(person.LastName);
            SetContact2FirstName(person.FirstName);
            SetContact2Phone1(person.primaryPhone().Number());
        }
        private bool ValidateStateDropdown(SelectElement selectElement)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(selectElement.WrappedElement);
            actions.Perform();

            List<IWebElement> states = new List<IWebElement>(selectElement.Options);
            List<string> actualStates = new List<string>();

            foreach (var state in states)
            {
                actualStates.Add(state.Text);
            }

            //status = CryWolfUtil.ValidateStateList(actualStates);

            return (CryWolfUtil.ValidateStateList(actualStates));
        }
        private bool ValidateAlarmedLocationIsSingle(SelectElement stateDD)
        {
            return stateDD.Options.Count == 1;
        }

        private bool ValidateStateDDIsDefault(SelectElement stateDD, string defaultState)
        {
            return stateDD.SelectedOption.Text == defaultState;
        }

        public bool ValidateAlarmedLocationState(string defaultState)
        {
            return ValidateStateDDIsDefault(ddAlarmedLocationState, defaultState) && ValidateAlarmedLocationIsSingle(ddAlarmedLocationState);
        }

        public bool ValidateMailingState(string defaultState)
        {
            return ValidateStateDDIsDefault(ddMailingState, defaultState);
        }

        public bool ValidateContact1State()
        {

            return ValidateStateDropdown(ddContact1State);
        }

        public bool ValidateContact2State()
        {
            return ValidateStateDropdown(ddContact2State);
        }

        public void ChooseMailingState(string state)
        {
            if (state != "")
            {
                SelectMailingState(state);
            }
        }

        public bool ValidateMonitorAndContacts()
        {
            bool status = driver.SwitchTo().Alert().Text.Contains(MonitorContacts);
            driver.SwitchTo().Alert().Accept();
            return status;
        }
        #endregion
    }
}
