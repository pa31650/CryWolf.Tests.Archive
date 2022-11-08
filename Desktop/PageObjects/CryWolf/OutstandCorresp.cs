using OpenQA.Selenium.Appium.Windows;
using Utils;

namespace Desktop.PageObjects.CryWolf
{
    public class OutstandCorresp
    {
        private readonly WindowsDriver<WindowsElement> session;
        private readonly WindowsDriver<WindowsElement> desktopSession = CryWolfUtil.GetDesktopSession();

        public OutstandCorresp(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification
        #region Outstanding Correspondence Window
        private WindowsElement btnOutstandingLetters => session.FindElementByAccessibilityId("btnOutstanding");
        private WindowsElement frmLetters => session.FindElementByAccessibilityId("frmLetters");
        private WindowsElement btnCloseWindow => frmLetters.FindElementByName("Close") as WindowsElement;
        #endregion

        #region Email Warning
        private WindowsElement dlgEmailWarning => session.FindElementByName("Email Warning");
        private WindowsElement btnNoEmailWarning => dlgEmailWarning.FindElementByAccessibilityId("7") as WindowsElement;
        #endregion

        #region Accounts Skipped due to 'Bad Address' Flag
        private WindowsElement frmSkipped => session.FindElementByAccessibilityId("frmSkippedAccounts");
        private WindowsElement btnOkSkipped => frmSkipped.FindElementByAccessibilityId("btnOK") as WindowsElement;
        #endregion

        #region Outstanding Letter
        private WindowsElement frmOutstandingLetter => session.FindElementByAccessibilityId("frmOutstanding");
        private WindowsElement btnCloseLetterWindow => frmOutstandingLetter.FindElementByName("Close") as WindowsElement;
        private WindowsElement tlbFile => frmOutstandingLetter.FindElementsByTagName("ToolBar")[4] as WindowsElement; ///TODO Find better way to identify File Toolbar on Outstanding Letter window
        private WindowsElement btnPrint => tlbFile.FindElementsByTagName("Button")[5] as WindowsElement; ///TODO Find better way to identify Print button on Outstanding Letter window / File Toolbar
        #endregion

        #region System Print Dialog
        private WindowsElement dlgSystemPrint => desktopSession.FindElementByName("Print");
        private WindowsElement btnCancelSystemPrint => dlgSystemPrint.FindElementByName("Cancel") as WindowsElement;
        #endregion

        #region Print Completion Notification
        private WindowsElement frmMarkDone => session.FindElementByAccessibilityId("frmMarkDone");
        private WindowsElement btnYesMarkDone => frmMarkDone.FindElementByAccessibilityId("btnYes") as WindowsElement;
        #endregion

        #region Storarge Success Dialog
        private WindowsElement dlgStorageSuccess => session.FindElementByClassName(@"#32770");
        private WindowsElement btnOkStorageSuccess => dlgStorageSuccess.FindElementByAccessibilityId("2") as WindowsElement;
        #endregion

        //Basic interactions
        #region Outstanding Correspondence Window
        private void ClickOutstandingLettersButton()
        {
            btnOutstandingLetters.Click();
        }
        private void CreateLetters(string letterType)
        {
            WindowsElement txtLetterType = session.FindElementByName(letterType);

            txtLetterType.Click();
            session.Mouse.DoubleClick(txtLetterType.Coordinates);

        }
        private void CloseWindow()
        {
            frmLetters.Click();
            btnCloseWindow.Click();
        }
        #endregion

        #region Email Warning
        private void ClickNoButtonEmailWarning()
        {
            btnNoEmailWarning.Click();
        }
        #endregion

        #region Accounts Skipped due to 'Bad Address' Flag
        private void ClickOkSkipped()
        {
            btnOkSkipped.Click();
        }
        #endregion

        #region Outstanding Letter
        private void ClickPrintOutstandingLetter()
        {
            btnPrint.Click();
        }
        private void CloseOutstandingLetterWindow()
        {
            frmOutstandingLetter.Click();
            btnCloseLetterWindow.Click();
        }
        #endregion

        #region System Print Dialog
        private void ClickCancelSystemPrint()
        {
            btnCancelSystemPrint.Click();
        }
        #endregion

        #region Print Completion Notification
        private void ClickYesMarkDone()
        {
            btnYesMarkDone.Click();
        }
        #endregion

        #region Storage Success Dialog
        private void ClickOkStorageSuccess()
        {
            btnOkStorageSuccess.Click();
        }
        #endregion

        //Short functional methods
        public void ExecuteOustandingCorrespondence(string letterType)
        {
            //settings

            ClickOutstandingLettersButton();

            CreateLetters(letterType);

            ClickNoButtonEmailWarning();

            ClickOkSkipped();

            PrintCreatedLetters();

            CloseWindow();
        }

        public void PrintCreatedLetters()
        {
            ClickPrintOutstandingLetter();

            ClickCancelSystemPrint();

            ClickYesMarkDone();

            ClickOkStorageSuccess();

            CloseOutstandingLetterWindow();
        }
    }
}
