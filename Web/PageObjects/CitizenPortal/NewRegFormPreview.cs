using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using Utils;

namespace Web.PageObjects.CitizenPortal
{
    public class NewRegFormPreview : CommonElements
    {
        private IWebDriver driver = null;

        ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;

        public NewRegFormPreview(IWebDriver driver)
        {
            this.driver = driver;
        }

        #region Page Elements
        IWebElement btnLoad => driver.FindElement(By.Id("btnLoad"));
        #endregion

        #region Basic UI Interactions
        private void ClickSignOnToPay()
        {
            btnLoad.Click();
        }
        #endregion

        #region Short Business Processes
        public void SignOnToPay()
        {
            ClickSignOnToPay();
        }
        #endregion
    }
}