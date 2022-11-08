using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.PageObjects.CitizenPortal
{
    public class AcAddArAccount : CommonElements
    {
        private readonly IWebDriver driver;

        string reportAdd;
        string categoryAdd;
        bool status = true;
        string DLNumMissing = "Please enter your driver's license number.";
        string DLStateMissing = "Please select a state for your driver's license.";
        string MonitorContacts = "Please submit either the company that monitors this location or two keyholder contacts.";

        public string categoryName = "Alarm Company Citizen Registration Page";

        public AcAddArAccount(IWebDriver _driver)
        {
            this.driver = _driver;
        }

        //Object Identification
        #region Page Elements

        //Alarmed Location
        private IWebElement lblState => driver.FindElement(By.Id("spstate1"));
        private SelectElement ddAlarmedLocationState => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlState")));
        private IWebElement lblZip => driver.FindElement(By.Id("spzip1"));

        //Mailing Information
        private IWebElement lblMailState => driver.FindElement(By.Id("spstate2"));
        private SelectElement ddMailingState => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ddlRPState")));
        private IWebElement lblMailZip => driver.FindElement(By.Id("spzip2"));

        //Contact 1
        private IWebElement lblContact1Address => driver.FindElement(By.Id("spcsz1"));

        //Contact 2
        private IWebElement lblContact2Address => driver.FindElement(By.Id("spcsz2"));

        //Alarm Company
        private SelectElement ddMonitoredBy => new SelectElement(driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_cbMonitoredBy")));

        private IWebElement btnSubmit => driver.FindElement(By.XPath("//*[contains(@id,'btnSubmit')]"));
        #endregion

        //Basic Interactions
        private string GetAlarmedStateLabel()
        {
            try
            {
                return lblState.Text;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("State Label was not found as currently defined");
                return "NotFound";
            }
        }
        private string GetAlarmedZipLabel()
        {
            try
            {
                return lblZip.Text;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Zip Label was not found as currently defined");
                return "NotFound";
            }

        }
        private string GetMailingStateLabel()
        {
            try
            {
                return lblMailState.Text;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Mailing State Label was not found as currently defined");
                return "NotFound";
            }

        }
        private string GetMailingZipLabel()
        {
            try
            {
                return lblMailZip.Text;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Mailing Zip Label was not found as currently defined");
                return "NotFound";
            }

        }
        private string GetContact1AddressLabel()
        {
            try
            {
                return lblContact1Address.Text;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Contact 1 Address Label was not found as currently defined");
                return "NotFound";
            }

        }
        private string GetContact2AddressLabel()
        {
            try
            {
                return lblContact2Address.Text;
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Contact 2 Address Label was not found as currently defined");
                return "NotFound";
            }

        }

        private void SelectMonitoringCompanyByValue(string monitoringCo)
        {
            ddMonitoredBy.SelectByValue(monitoringCo);

        }

        //Short Functional Methods
        public bool SelectMonitoringCompany(string alarmCo)
        {
            IList<IWebElement> monitoringOptions = ddMonitoredBy.Options;

            foreach (var option in monitoringOptions)
            {
                if (option.Text.Contains($"({alarmCo})"))
                {
                    SelectMonitoringCompanyByValue(option.Text);
                    return true;
                }
            }

            TestContext.WriteLine($"Alarm Company containing {alarmCo} was not found");
            return false;
        }
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
            string actualAlarmedState = GetAlarmedStateLabel();
            string actualAlarmedZip = GetAlarmedZipLabel();
            string actualMailingState = GetMailingStateLabel();
            string actualMailingZip = GetMailingZipLabel();
            string actualContact1Address = GetContact1AddressLabel();
            string actualContact2Address = GetContact2AddressLabel();

            return actualAlarmedState.Equals("Province") &&
                actualAlarmedZip.Equals("Postal Code") &&
                actualMailingState.Equals("Province") &&
                actualMailingZip.Equals("Postal Code") &&
                actualContact1Address.Equals("City Prov Post") &&
                actualContact2Address.Equals("City Prov Post");
        }
        private bool ValidateAmericanAddress()
        {
            string actualAlarmedState = GetAlarmedStateLabel();
            string actualAlarmedZip = GetAlarmedZipLabel();
            string actualMailingState = GetMailingStateLabel();
            string actualMailingZip = GetMailingZipLabel();
            string actualContact1Address = GetContact1AddressLabel();
            string actualContact2Address = GetContact2AddressLabel();

            return actualAlarmedState.Equals("State") &&
                actualAlarmedZip.Equals("Zip Code") &&
                actualMailingState.Equals("State") &&
                actualMailingZip.Equals("Zip Code") &&
                actualContact1Address.Equals("City State Zip") &&
                actualContact2Address.Equals("City State Zip");
        }

        public bool ValidateServicedByField(bool shouldExist)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(btnSubmit);
            actions.Perform();

            if (shouldExist)
            {
                return driver.FindElements(By.Id("ctl00_ContentPlaceHolder1_tdServicedBy")).Count > 0;
            }
            else
            {
                return driver.FindElements(By.Id("ctl00_ContentPlaceHolder1_tdServicedBy")).Count == 0;
            }
        }
        
        public bool ValidateInstalledByField(bool shouldExist)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(btnSubmit);
            actions.Perform();

            if (shouldExist)
            {
                return driver.FindElements(By.Id("ctl00_ContentPlaceHolder1_tdInstalledBy")).Count > 0;
            }
            else
            {
                return driver.FindElements(By.Id("ctl00_ContentPlaceHolder1_tdInstalledBy")).Count == 0;
            }
        }
    }
}
