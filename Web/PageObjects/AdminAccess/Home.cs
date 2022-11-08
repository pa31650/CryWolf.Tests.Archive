using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.PageObjects.AdminAccess
{
    public class Home : CommonElements
    {
        private readonly IWebDriver driver;

        string reportAdd;
        string categoryAdd;
        bool status = true;

        public string categoryName = "Admin Access Home/Clock Page";

        public Home(IWebDriver _driver)
        {
            this.driver = _driver;
        }

        //Object Identification
        private IWebElement lnkLogIn => driver.FindElement(By.Id("lognow"));

        //Basic Interactions
        private void ClickLogIn()
        {
            lnkLogIn.Click();
        }

        //Short Functional Methods
        public bool NavigateToLogin()
        {
            ClickLogIn();

            return driver.FindElement(By.Id("cd")).Enabled;
        }
    }
}
