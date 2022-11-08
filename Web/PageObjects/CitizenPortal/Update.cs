using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using Utils;

namespace Web.PageObjects.CitizenPortal
{
    public class Update : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();

        string reportAdd;
        string categoryAdd;
        bool status = true;

        public string categoryName = "Citizen Update Page";

        public Update(IWebDriver driver)
        {
            this.driver = driver;
        }

        //Object Identification
        #region Page Elements
        //Links
        private IWebElement lblUpdate => driver.FindElement(By.Id("lblUpdate"));

        //Alarmed Location
        private IWebElement lblAlarmedAddress => driver.FindElement(By.Id("spcsz"));
        private SelectElement ddAlarmedLocationState => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlState")));

        //Mailing Information
        private IWebElement lblMailingAddress => driver.FindElement(By.Id("spcsz2"));
        private SelectElement ddMailingState => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlRPState")));
        private IWebElement txtDateOfBirth => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtCustom1"));
        private IWebElement dtpDobCalendar => driver.FindElement(By.Id("ui-datepicker-div")).FindElement(By.ClassName("ui-datepicker-calendar"));

        //Contact 1
        private IWebElement lblContact1Address => driver.FindElement(By.Id("spcsz3"));
        private SelectElement ddContact1State => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlC1State")));

        //Contact 2
        private IWebElement lblContact2Address => driver.FindElement(By.Id("spcsz4"));
        private SelectElement ddContact2State => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlC2State")));

        //Update
        private IWebElement btnSubmit => driver.FindElement(By.Id("btnSubmit"));
        #endregion

        //Basic Interactions
        #region Basic UI Interactions
        private void ClickUpdateLink()
        {
            lblUpdate.Click();
        }
        private void ClickSubmit()
        {
            btnSubmit.Click();
            TestContext.WriteLine("Submit button clicked");
        }
        private void SelectMailingState(string state)
        {
            ddMailingState.SelectByText(state);
        }

        private string GetAlarmedAddressLabel()
        {
            return lblAlarmedAddress.Text;
        }

        private string GetMailingAddressLabel()
        {
            return lblMailingAddress.Text;
        }

        private string GetContact1AddressLabel()
        {
            return lblContact1Address.Text;
        }

        private string GetContact2AddressLabel()
        {
            return lblContact2Address.Text;
        }

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
            string actualAlarmedAddress = GetAlarmedAddressLabel();
            string actualMailingAddress = GetMailingAddressLabel();
            string actualContact1Address = GetContact1AddressLabel();
            string actualContract2Address = GetContact2AddressLabel();

            return actualAlarmedAddress.Equals("City Prov Post") &&
                actualMailingAddress.Equals("City Prov Post") &&
                actualContact1Address.Equals("City Prov Post") &&
                actualContract2Address.Equals("City Prov Post");
        }

        private bool ValidateAmericanAddress()
        {
            string actualAlarmedAddress = GetAlarmedAddressLabel();
            string actualMailingAddress = GetMailingAddressLabel();
            string actualContact1Address = GetContact1AddressLabel();
            string actualContract2Address = GetContact2AddressLabel();

            return actualAlarmedAddress.Equals("City State Zip") &&
                actualMailingAddress.Equals("City State Zip") &&
                actualContact1Address.Equals("City State Zip") &&
                actualContract2Address.Equals("City State Zip");
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

            //status = CryWolfUtil.ValidateStateList(actualStates);

            return (CryWolfUtil.ValidateStateList(actualStates));
        }

        public bool ValidateAlarmedLocationState()
        {
            return !ddAlarmedLocationState.WrappedElement.Enabled;
        }

        public bool ValidateMailingState()
        {
            return ValidateStateDropdown(ddMailingState);
        }

        public bool ValidatePage()
        {
            IReadOnlyCollection<IWebElement> inputs = driver.FindElements(By.TagName("input"));

            foreach (var input in inputs)
            {
                try
                {
                    string value = input.Text;

                    input.Clear();
                    input.SendKeys("'");
                    ClickSubmit();
                    status = lblUpdate.Enabled;

                    if (status)
                    {
                        TestContext.WriteLine("input allowed updates without error");
                    }
                    else
                    {
                        TestContext.WriteLine("input updates errored");
                    }

                    driver.Navigate().Back();
                    //ClickUpdateLink();

                    input.Clear();
                    input.SendKeys(value);

                    ClickSubmit();
                    ClickUpdateLink();
                }
                catch (ElementNotVisibleException)
                {

                }
                catch (InvalidElementStateException)
                {

                }

            }
            return false;
        }

        public bool ValidateEmailDefault()
        {
            return driver.FindElements(By.Id("ckEmailAll")).Count == 0;
        }

        public void SubmitRegistrationInformation()
        {
            ClickSubmit();
        }


        public bool ValidateServicedByField(bool shouldExist)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(btnSubmit);
            actions.Perform();

            if (shouldExist)
            {
                return driver.FindElements(By.Id("spsrvby")).Count > 0;
            }
            else
            {
                return driver.FindElements(By.Id("spsrvby")).Count == 0;
            }
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
        #endregion
    }
}
