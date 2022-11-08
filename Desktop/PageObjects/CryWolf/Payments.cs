using Desktop.Libraries;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using System;

namespace Desktop.PageObjects.CryWolf
{
    class Payments
    {
        private readonly WindowsDriver<WindowsElement> session;
        //ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = false;

        public Payments(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification
        #region Payments
        private WindowsElement winPayments => session.FindElementByAccessibilityId("frmPayments");
        private WindowsElement btnClose => winPayments.FindElementByName("Close") as WindowsElement;
        private WindowsElement txtAlarmNo => winPayments.FindElementByAccessibilityId("txtAlarmNo") as WindowsElement;
        private WindowsElement btnSearch => winPayments.FindElementByAccessibilityId("btnSearch") as WindowsElement;
        private WindowsElement btnReady => winPayments.FindElementByAccessibilityId("btnReady") as WindowsElement;
        private WindowsElement tableInvoices => winPayments.FindElementByAccessibilityId("lv2") as WindowsElement;
        private WindowsElement btnSelectAll => winPayments.FindElementByAccessibilityId("btnSelectAll") as WindowsElement;
        #endregion

        #region Payment sub window
        private WindowsElement frmPaymentWindow => session.FindElementByAccessibilityId("frmPayment");

        private WindowsElement btnOk => frmPaymentWindow.FindElementByAccessibilityId("btnOK") as WindowsElement;

        private WindowsElement cbOptLetter => frmPaymentWindow.FindElementByAccessibilityId("cbLetter") as WindowsElement;

        private WindowsElement ckPrintNow => frmPaymentWindow.FindElementByAccessibilityId("ckPrintNow") as WindowsElement;

        private WindowsElement GetLetterList()
        {
            return cbOptLetter.FindElementByTagName("List") as WindowsElement;
        }

        private WindowsElement dtPaid => frmPaymentWindow.FindElementByAccessibilityId("dtPaid") as WindowsElement;

        private WindowsElement btnCancel => frmPaymentWindow.FindElementByAccessibilityId("btnCancel") as WindowsElement;
        #endregion

        //Basic Interactions
        private void ClickCancel()
        {
            btnCancel.Click();
        }
        private DateTime GetActionDate()
        {
            return DateTime.Parse(dtPaid.Text);
        }
        private void ClickPrintNow()
        {
            ckPrintNow.Click();
        }
        private void EnterAlarmNo(string alarmNo)
        {
            txtAlarmNo.SendKeys(alarmNo);
        }
        private void ClickSearch()
        {
            btnSearch.Click();
        }
        private void ClickReady()
        {
            btnReady.Click();
        }
        private void ClickSelectAll()
        {
            btnSelectAll.Click();
        }
        private void ClickCloseButton()
        {
            btnClose.Click();
        }
        private void ClickOk()
        {
            btnOk.Click();
        }

        //Short Functional Methods
        public void CancelPayment()
        {
            ClickCancel();
        }
        public bool ValidateActionDate(DateTime date)
        {
            date.ToShortDateString();
            return date.CompareTo(GetActionDate()) == 0;
        }
        public void SearchAlarmNo(string alarmNo)
        {
            EnterAlarmNo(alarmNo);
            session.Keyboard.SendKeys(Keys.Enter);
        }

        public void ChooseAllInvoices(string alarmNo)
        {
            Console.WriteLine($"Choosing all invoices for Alarm No: {alarmNo}");
            SearchAlarmNo(alarmNo);
            ClickSelectAll();
            ClickReady();
        }

        public void SelectLetterToSend(string letterToSend = "N/A None")
        {
            cbOptLetter.SelectListItem(letterToSend);
        }

        public void CompletePaymentForm()
        {
            ClickOk();
        }

        public void CompletePaymentSendOptLetter(string letterToSend = "N/A None")
        {
            Console.WriteLine($"Completing payment and sending optional letter: {letterToSend}");
            SelectLetterToSend(letterToSend);
            ClickPrintNow();
            CompletePaymentForm();
        }
        public void Close()
        {
            Console.WriteLine($"Closing Payments Window");
            ClickCloseButton();
        }
        public void ValidatePaymentDate(DateTime date)
        {

        }
    }
}
