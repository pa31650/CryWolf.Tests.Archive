using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Utils;

namespace Web.PageObjects.CitizenPortal
{
    public class ChangePassword : CommonElements
    {
        private IWebDriver driver = null;
        ///All encrypted passwords set in encrypted DB to 'Passw0rd!'

        #region Constructor
        public ChangePassword(IWebDriver driver)
        {
            this.driver = driver;
        }
        #endregion

        #region Page Elements
        private IWebElement dlgChange => driver.FindElement(By.Id("dialogChange"));
        private IWebElement txtCurrentPassword => driver.FindElement(By.Id("pd0"));
        private IWebElement txtNewPassword => driver.FindElement(By.Id("pd1"));
        private IWebElement txtVerifyPassword => driver.FindElement(By.Id("pd2"));
        private IWebElement btnSubmit => driver.FindElement(By.Id("btnSubmit"));
        private IWebElement lblWarning => driver.FindElement(By.Id("lblWarning"));
        #endregion

        #region Basic UI Interactions
        private void SetCurrentPassword(string currentPassword)
        {
            txtCurrentPassword.Clear();
            txtCurrentPassword.SendKeys(currentPassword);
        }
        private void SetNewPassword(string newPassword)
        {
            txtNewPassword.SendKeys(newPassword);
        }
        private void SetVerifyPassword(string verifyPassword)
        {
            txtVerifyPassword.SendKeys(verifyPassword);
        }
        private void ClickSubmit()
        {
            btnSubmit.Click();
        }
        private string GetWarningLabelText()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementToBeClickable(lblWarning));

            return lblWarning.Text;
        }
        private bool IsDialogDisplayed()
        {
            return dlgChange.Enabled;
        }
        #endregion

        #region Short Business Processes
        public bool ChangeUserPassword(string currentPassword, string newPassword = "Passw0rd!")
        {
            SetCurrentPassword(currentPassword);
            SetNewPassword(newPassword);
            SetVerifyPassword(newPassword);
            ClickSubmit();
            Library.TakeScreenShot(driver, "Change User Password result", out string filePath);
            return ValidatePasswordChange();
        }
        public bool ValidatePasswordChange()
        {
            return GetWarningLabelText().CompareTo("Password successfully changed") == 0;
        }
        public bool Exist()
        {
            return IsDialogDisplayed();
        }
        #endregion

    }
}
