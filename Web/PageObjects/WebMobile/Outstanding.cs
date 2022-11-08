using OpenQA.Selenium;
using System.Collections.Generic;

namespace Web.PageObjects.WebMobile
{
    public class Outstanding : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        string reportAdd;

        string categoryAdd;
        bool status = true;

        public Outstanding(IWebDriver driver)
        {
            this.driver = driver;
        }

        #region Page Elements
        private IWebElement outstandingPage => driver.FindElement(By.Id("page3"));
        private IWebElement tblOwed => outstandingPage.FindElement(By.Id("tblOwed"));
        private IWebElement btnPaySelected => outstandingPage.FindElement(By.Id("paySelected3")) as IWebElement;
        #endregion

        #region Basic UI Interactions
        private void SelectOpenInvoice(string invoiceNo)
        {
            
        }
        private void ClickPaySelected()
        {
            btnPaySelected.Click();
        }
        #endregion

        #region Short Business Processes
        public void PayInvoice(string invoiceNo)
        {
            chkOpenInvoice(invoiceNo).Click();
            
            ClickPaySelected();
        }
        private IWebElement chkOpenInvoice(string InvoiceNum)
        {
            IWebElement OpenInvoice = GetRowWithText(InvoiceNum).FindElement(By.ClassName("ui-checkbox"));
            return OpenInvoice;
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
        private List<IWebElement> GetOwedRows()
        {
            List<IWebElement> list = new List<IWebElement>(tblOwed.FindElements(By.TagName("tr")));
            return list;
        }
        #endregion
    }
}
