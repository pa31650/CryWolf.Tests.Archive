using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using Utils;

namespace Web.PageObjects.CitizenPortal
{
    public class CommonFields : CommonElements
    {
        private IWebDriver driver = null;

        public string categoryName = "Citizen Registration Page";

        public CommonFields(IWebDriver driver)
        {
            this.driver = driver;
        }

        //Object Identification
        #region Page Elements

        //Alarmed Location
        private SelectElement ddAlarmedLocationState => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlState")));

        private IWebElement chkHasOutdoorCamera => driver.FindElement(By.Id("hasOutdoorCamera"));

        private IWebElement chkEnrollInNeighborhoodEyes => driver.FindElement(By.Id("enrollInNeighborhoodEyes"));

        //Mailing Information
        private SelectElement ddMailingState => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlRPState")));

        private IWebElement txtDateOfBirth => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtCustom1"));

        private IWebElement dtpDobCalendar => driver.FindElement(By.Id("ui-datepicker-div")).FindElement(By.ClassName("ui-datepicker-calendar"));

        private SelectElement ddDriversLisenseState => new SelectElement(driver.FindElement(By.Id("ddlDriversLisenseState")));

        private IWebElement txtDriversLicenseNumber => driver.FindElement(By.Id("txtDriversLicenseNumber"));

        //Contact 1
        private SelectElement ddContact1State => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlC1State")));

        //Contact 2
        private SelectElement ddContact2State => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlC2State")));

        //Alarm Company Information
        private SelectElement cbMonitoredBy => new SelectElement(driver.FindElement(By.XPath(@"//select[contains(@id,'cbMonitoredBy')]")));
        private SelectElement cbSoldBy => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_cbSoldBy")));
        private SelectElement cbServicedBy => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_cbServicedBy")));
        private IWebElement lblServicedBy => driver.FindElement(By.Id("spservy"));
        private SelectElement cbInstalledBy => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_cbInstalledBy")));
        private IWebElement lblInstalledBy => driver.FindElement(By.Id("spinstby"));


        private IWebElement txtPassword => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtPswrd1"));
        private IWebElement btnSubmit => driver.FindElement(By.XPath("//*[contains(@id,'btnSubmit')]"));

        //reCAPTCHA Objects
        private IWebElement ckReCaptchaAnchor => driver.FindElement(By.Id("recaptcha-anchor"));
        private IReadOnlyCollection<IWebElement> txtPhones => driver.FindElements(By.ClassName("key-telephone"));
        #endregion

        //Basic Interactions
        #region Basic UI Interactions
        private bool isEnrolledInNeighborhoodEyes()
        {
            return chkEnrollInNeighborhoodEyes.Selected;
        }

        private bool isHaveCameraSelected()
        {
            return chkHasOutdoorCamera.Selected;
        }
        private void ClickHaveACamera()
        {
            chkHasOutdoorCamera.Click();
        }

        private void ClickEnrollNeighborhood()
        {
            chkEnrollInNeighborhoodEyes.Click();
        }

        private void SelectMailingState(string state)
        {
            ddMailingState.SelectByText(state);
        }
        private void SetDOB(string dob)
        {
            txtDateOfBirth.SendKeys(dob);
        }
        private void SelectContact1State(string state)
        {
            ddContact1State.SelectByText(state);
        }
        private void SelectContact2State(string state)
        {
            ddContact2State.SelectByText(state);
        }
        private bool IsDateSelected()
        {
            List<IWebElement> selectedDate = new List<IWebElement>(dtpDobCalendar.FindElements(By.ClassName("  ui-datepicker-current-day")));

            return (selectedDate.Count == 0);
        }
        private void SelectDLState(string state)
        {
            ddDriversLisenseState.SelectByText(state);
        }
        private void SetDLNumber(string dlNum)
        {
            txtDriversLicenseNumber.Clear();
            txtDriversLicenseNumber.SendKeys(dlNum);
        }
        private void SelectMonitoringCompanyByValue(string monitoringCo)
        {
            TestContext.WriteLine($"Selecting Monitoring Company [{monitoringCo}]");
            cbMonitoredBy.SelectByValue(monitoringCo);
        }
        private void SelectSoldByCompanyByValue(string soldCo)
        {
            TestContext.WriteLine($"Selecting Sold By Company [{soldCo}]");
            cbSoldBy.SelectByValue(soldCo);
        }
        private void SelectServicedByCompanyByValue(string servicedCo)
        {
            TestContext.WriteLine($"Selecting Serviced By Company [{servicedCo}]");
            cbServicedBy.SelectByValue(servicedCo);
        }
        private void SelectInstalledBy(string installCo)
        {
            TestContext.WriteLine($"Selecting Serviced By Company [{installCo}]");
            cbInstalledBy.SelectByValue(installCo);
        }
        #endregion

        //Short Functional Methods
        #region Short Business Processes
        public bool ValidatePhoneInputs(string pageName)
        {
            List<Char> printableChars = new List<char>();
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                char c = Convert.ToChar(i);
                if ((!char.IsControl(c))&&(!char.IsNumber(c)))
                {
                    printableChars.Add(c);
                }
            }

            foreach (IWebElement phone in txtPhones)
            {
                string phoneID = phone.GetAttribute("id");

                for (int i = 0; i <= 81; i++)
                {
                    var key = printableChars[i].ToString();
                    phone.SendKeys(key);
                    //if (phone.Text.Contains(key))
                    //{
                    //    TestContext.WriteLine($"Non-numeric key detected: [{key}] in [{phoneID}]");
                    //    return false;
                    //}

                }

                if (!phone.Text.Equals(string.Empty))
                {
                    TestContext.WriteLine($"Non-numeric key detected: [{phone.Text}] in [{phoneID}]");
                    return false;
                }
                TestContext.WriteLine($"No Non-numeric characters found in {phoneID}");
                
            }
            Library.TakeScreenShot(driver, $"Validate [{pageName}] phone inputs",out string path);
            TestContext.WriteLine($"No Non-numeric characters found in phone fields on page [{pageName}]");
            return true;
        }

        public void EnrollInNeighborhoodEyes()
        {
            if (!isHaveCameraSelected())
            {
                TestContext.WriteLine("Citizen indicates they have a camera");
                ClickHaveACamera();
            }
            else
            {
                TestContext.WriteLine("Citizen already indicated they have a camera");
            }

            if (!isEnrolledInNeighborhoodEyes())
            {
                TestContext.WriteLine("Enrolling in Neighborhood Eyes");
                ClickEnrollNeighborhood();
            }
            else
            {
                TestContext.WriteLine("Citizen already enrolled in Neighborhood Eyes");
            }
        }

        public void UnenrollInNeighborhoodEyes()
        {
            if (isEnrolledInNeighborhoodEyes())
            {
                TestContext.WriteLine("Unenrolling in Neighborhood Eyes");
                ClickEnrollNeighborhood();
            }

            TestContext.WriteLine("Citizen is not enrolled in Neighborhood Eyes");
        }

        public void NotifyCamera()
        {
            if (!isHaveCameraSelected())
            {
                TestContext.WriteLine("Citizen indicates they have a camera");
                ClickHaveACamera();
            }
            else
            {
                TestContext.WriteLine("Citizen already indicated they have a camera");
            }
        }

        public void EnterDLNumber(string dlNumber)
        {
            TestContext.WriteLine($"Entering DL Number: [{dlNumber}]");
            SetDLNumber(dlNumber);
        }

        public void ChooseDLState(string dlState)
        {
            TestContext.WriteLine($"Entering DL State: [{dlState}]");
            SelectDLState(dlState);
        }

        private bool ValidateStateDropdown(SelectElement selectElement)
        {
            //WebElement element = driver.findElement(By.id("my-id"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(selectElement.WrappedElement);
            actions.Perform();

            List<IWebElement> states = new List<IWebElement>(selectElement.Options);
            List<string> actualStates = new List<string>();

            foreach (var state in states)
            {
                actualStates.Add(state.Text);
            }

            return (CryWolfUtil.ValidateStateList(actualStates));
        }

        private bool ValidateAlarmedLocationIsSingle(SelectElement stateDD)
        {
            try
            {
                return stateDD.Options.Count == 1;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Alarmed Location State was not found as defined");
                return false;
            }
        }

        private bool ValidateStateDDIsDefault(SelectElement stateDD, string defaultState)
        {
            try
            {
                return stateDD.SelectedOption.Text == defaultState;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Alarmed Location State was not found as defined");
                return false;
            }
        }

        public bool ValidateAlarmedLocationState(string defaultState)
        {
            try
            {
                return ValidateStateDDIsDefault(ddAlarmedLocationState, defaultState) && ValidateAlarmedLocationIsSingle(ddAlarmedLocationState);

            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Alarmed Location State was not found as defined");
                return false;
            }
        }

        public bool ValidateMailingState(string defaultState = "")
        {
            try
            {
                return ValidateStateDDIsDefault(ddMailingState, defaultState);
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Mailing State was not found as defined");
                return false;
            }
        }

        public bool ValidateContact1State()
        {
            try
            {
                return ValidateStateDropdown(ddContact1State);
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Contact 1 State was not found as defined");
                return false;
            }
        }

        public bool ValidateContact2State()
        {
            try
            {
                return ValidateStateDropdown(ddContact2State);
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Contact 2 State was not found as defined");
                return false;
            }
        }

        public void ChooseMailingState(string state)
        {
            if (state != "")
            {
                SelectMailingState(state);
            }
        }

        public void EnterDob(string dob)
        {
            if (dob != "")
            {
                SetDOB(dob);
            }
        }

        public bool ValidateDobField(string dobInput)
        {
            string expected = txtDateOfBirth.Text;
            EnterDob(dobInput);
            txtDateOfBirth.SendKeys(Keys.Tab);
            return IsDateSelected();
        }
        public bool SelectMonitoringCompany(string alarmCo)
        {
            IList<IWebElement> monitoringOptions = cbMonitoredBy.Options;

            if (SelectAlarmCompany(alarmCo, monitoringOptions, out string optionText))
            {
                SelectMonitoringCompanyByValue(optionText);
                return true;
            }

            //WriteLine($"Alarm Company containing {alarmCo} was not found");
            return false;
        }

        public bool SelectSoldByCompany(string alarmCo)
        {
            IList<IWebElement> soldByOptions = cbSoldBy.Options;

            if (SelectAlarmCompany(alarmCo, soldByOptions, out string optionText))
            {
                SelectSoldByCompanyByValue(optionText);
                return true;
            }

            return false;
        }

        public bool SelectServicedByCompany(string alarmCo)
        {
            IList<IWebElement> servicedByOptions = cbServicedBy.Options;

            if (SelectAlarmCompany(alarmCo, servicedByOptions, out string optionText))
            {
                SelectServicedByCompanyByValue(optionText);
                return true;
            }

            return false;
        }

        public bool SelectInstalledByCompany(string alarmCo)
        {
            IList<IWebElement> installedByOptions = cbInstalledBy.Options;

            if (SelectAlarmCompany(alarmCo, installedByOptions, out string optionText))
            {
                SelectServicedByCompanyByValue(optionText);
                return true;
            }

            return false;
        }

        public bool SelectAlarmCompany(string alarmCo, IList<IWebElement> alarmOptions, out string optionText)
        {
            optionText = "NotFound";

            foreach (var option in alarmOptions)
            {
                if (option.Text.Contains($"({alarmCo})"))
                {
                    optionText = option.Text;
                    return true;
                }
            }

            TestContext.WriteLine($"Alarm Company containing {alarmCo} was not found");
            return false;
        }

        public bool ValidateMonitoringCompany(string alarmCo)
        {
            return cbMonitoredBy.SelectedOption.Text.Contains(alarmCo);
        }

        public bool ValidateSoldByCompany(string alarmCo)
        {
            return cbSoldBy.SelectedOption.Text.Contains(alarmCo);
        }

        public bool ValidateServicedByCompany(string alarmCo)
        {
            return cbServicedBy.SelectedOption.Text.Contains(alarmCo);
        }

        public bool ValidateServicedByField(bool shouldExist)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(btnSubmit);
            actions.Perform();

            if (shouldExist)
            {
                return driver.FindElements(By.Id("spservy")).Count > 0;
            }
            else
            {
                return driver.FindElements(By.Id("spservy")).Count == 0;
            }
        }

        public bool ValidateInstalledByCompany(string alarmCo)
        {
            return cbInstalledBy.SelectedOption.Text.Contains(alarmCo);
        }

        public bool ValidateInstalledByField(bool shouldExist)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(btnSubmit);
            actions.Perform();

            if (shouldExist)
            {
                return driver.FindElements(By.Id("spinstby")).Count > 0;
            }
            else
            {
                return driver.FindElements(By.Id("spinstby")).Count == 0;
            }
        }

        public bool ValidateReCaptcha()
        {
            bool isDisplayed;
            try
            {
                isDisplayed = driver.FindElements(By.Id("recaptcha-anchor")).Count > 0;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
            return isDisplayed;
        }
        #endregion
    }
}
