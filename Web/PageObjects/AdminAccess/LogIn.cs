using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Web.PageObjects.AdminAccess
{
    public class LogIn : CommonElements
    {
        private readonly IWebDriver driver;

        string reportAdd;
        string categoryAdd;
        bool status = true;

        public string categoryName = "Login Page";

        public LogIn(IWebDriver _driver)
        {
            this.driver = _driver;
        }

        //Object Identification
        private IWebElement txtAgency => driver.FindElement(By.Id("cd"));
        private IWebElement txtSignon => driver.FindElement(By.Id("sn"));
        private IWebElement txtPassword => driver.FindElement(By.Id("pd"));
        private IWebElement btnSubmit => driver.FindElement(By.XPath("//button/span[contains(text(),'Submit')]"));

        //Basic Interactions
        private void SetAgency(string agency)
        {
            txtAgency.SendKeys(agency);
        }

        private void SetSignon(string signon)
        {
            txtSignon.SendKeys(signon);
            txtSignon.SendKeys(Keys.Tab);
        }

        private void SetPassword(string password)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(ExpectedConditions.ElementToBeClickable(txtPassword));

            txtPassword.SendKeys(password);
        }

        private void ClickSubmit()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(ExpectedConditions.ElementToBeClickable(btnSubmit));
            btnSubmit.Click();
        }

        //Short Functional Methods
        public bool Login(string agency, string signon, string password)
        {
            TestContext.WriteLine($"Setting agency code [{agency}]");
            SetAgency(agency);

            TestContext.WriteLine($"Setting signon [{signon}]");
            SetSignon(signon);

            TestContext.WriteLine($"Setting password");
            SetPassword(password);

            ClickSubmit();

            try
            {

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(ExpectedConditions.UrlContains("default1"));
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine(ErrorStrings.TimeoutException);
                return false;
            }
        }
    }
}
