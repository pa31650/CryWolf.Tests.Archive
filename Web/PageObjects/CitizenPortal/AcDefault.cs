using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
//using SeleniumExtras.WaitHelpers;
using System;
using Utils;
//using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Web.PageObjects.CitizenPortal
{
    public class AcDefault : CommonElements
    {
        private readonly IWebDriver driver;

        string reportAdd;
        string categoryAdd;
        bool status = true;

        public string categoryName = "Alarm Company Citizen Registration Page";

        public AcDefault(IWebDriver _driver)
        {
            this.driver = _driver;
        }

        //Object Identification
        private IWebElement txtAccountNum => driver.FindElement(By.Id("sn"));
        private IWebElement txtPassword => driver.FindElement(By.Id("pd"));
        private IWebElement btnSubmit => driver.FindElement(By.Id("btnLogOn"));
        private IWebElement lblNotice => driver.FindElement(By.Id("lblNotice"));

        //Basic Interactions
        private string GetNoticeText()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("lblNotice")));
                return lblNotice.Text;
            }
            catch (TimeoutException)
            {
                ReportBuilder.ArrayBuilder(ErrorStrings.TimeoutException, false, "UI Validation");
                return "NotFound";
            }
        }
        private void SetAccountNumber(string AcNum)
        {
            try
            {
                txtAccountNum.SendKeys(AcNum);
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Account Number input was not found as defined");

            }

            TestContext.WriteLine($"Account Number {AcNum} was entered into field");
        }
        private void SetPassword(string password)
        {
            try
            {
                txtPassword.SendKeys(password);
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Password input was not found as defined");
            }

        }
        private void ClickSubmit()
        {
            btnSubmit.Click();
        }
        //Short Functional Methods
        public bool ValidateLoginError(string field)
        {
            switch (field)
            {
                case "Account Number":
                    return GetNoticeText().Equals("Invalid user submitted");
                case "Password":
                    return GetNoticeText().Equals("Invalid value for item: pd");
                default:
                    TestContext.WriteLine($"{field} was not recognized. Use Account Number or Password");
                    return false;
            }
        }
        public bool Login(string AcNum, string password)
        {
            SetAccountNumber(AcNum);

            Actions builder = new Actions(driver);
            builder.SendKeys(Keys.Tab).Perform();
            WaitFor(driver, txtPassword);

            SetPassword(password);

            ClickSubmit();

            try
            {
                string message = driver.SwitchTo().Alert().Text;
                driver.SwitchTo().Alert().Accept();
                TestContext.WriteLine($"Alert encountered with message: [{message}]");
                return false;
            }
            catch (Exception)
            {

            }

            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnChangePd")));
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine($"User: [{AcNum}], Password: [{password}] failed to login");
                return false;
            }

            reportAdd = $"User: [{AcNum}] was logged in successfully";
            return true;

        }
    }
}
