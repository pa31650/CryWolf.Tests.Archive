using OpenQA.Selenium;
using System.Collections.Generic;

namespace Web.PageObjects.Web
{
    public class PayOnline : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;
        //CommonElements commonElements = new CommonElements();

        #region Constructor
        public PayOnline(IWebDriver driver)
        {
            this.driver = driver;
        }
        #endregion

        #region Page Elements
        private IWebElement tblOwed => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_grdOwed"));
        private IWebElement btnContinue => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_btnSubmit"));
        #endregion

        #region Basic UI Interactions
        private List<IWebElement> GetOwedRows()
        {
            List<IWebElement> list = new List<IWebElement>(tblOwed.FindElements(By.TagName("tr")));
            return list;
        }
        private IWebElement GetRowWithText(string search)
        {
            IWebElement rowFound;
            List<IWebElement> webElements = GetOwedRows();
            foreach (var element in webElements)
            {
                if (element.Text.Contains(search))
                {
                    return element;
                }
            }
            return null;
        }
        private void ClickContinue()
        {
            btnContinue.Click();
        }
        private IWebElement chkOpenInvoice(string InvoiceNum)
        {
            IWebElement OpenInvoice = GetRowWithText(InvoiceNum).FindElement(By.TagName("input"));
            return OpenInvoice;
        }
        
        #endregion

        #region Short Business Processes
        public void SelectOpenInvoice(string InvoiceNumber)
        {
            chkOpenInvoice(InvoiceNumber).Click();
        }
        public void SubmitInvoices()
        {
            ClickContinue();
        }
        #endregion

    }
}
