using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.PersonFactory;

namespace Desktop.PageObjects.CryWolf
{
    public class AlarmCompanies
    {
        private readonly WindowsDriver<WindowsElement> session;

        string reportAdd;
        string categoryAdd;
        bool status = false;

        public string categoryName = "Alarm Companies";

        public AlarmCompanies(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification

        #region Main Form
        WindowsElement frmACs => session.FindElementByAccessibilityId("frmACs");

        WindowsElement txtAlarmNo => frmACs.FindElementByAccessibilityId("txtAlarmNo") as WindowsElement;

        WindowsElement txtCompanyName => frmACs.FindElementByAccessibilityId("txtCompanyName") as WindowsElement;

        WindowsElement txtStrNum => frmACs.FindElementByAccessibilityId("txtStrNum") as WindowsElement;

        WindowsElement txtStreet => frmACs.FindElementByAccessibilityId("txtStreet") as WindowsElement;

        WindowsElement txtCity => frmACs.FindElementByAccessibilityId("txtCity") as WindowsElement;

        WindowsElement txtState => frmACs.FindElementByAccessibilityId("txtState") as WindowsElement;

        WindowsElement txtZipCode => frmACs.FindElementByAccessibilityId("1001") as WindowsElement;

        WindowsElement btnClose => frmACs.FindElementByName("Close") as WindowsElement;

        private WindowsElement lblAlarmNo => frmACs.FindElementByAccessibilityId("Label1") as WindowsElement;

        private WindowsElement contextMenu => session.FindElementByName("DropDown");
        #endregion

        #region Set Password
        private WindowsElement frmSetPassword => session.FindElementByAccessibilityId("frmSetPassword");

        private WindowsElement txtPassword => frmSetPassword.FindElementByAccessibilityId("txtPassword") as WindowsElement;

        private WindowsElement txtVerifyPassword => frmSetPassword.FindElementByAccessibilityId("txtVerifyPassword") as WindowsElement;

        private WindowsElement btnSetPassword => frmSetPassword.FindElementByAccessibilityId("btnOK") as WindowsElement;

        private WindowsElement dialog => frmSetPassword.FindElementByClassName(@"#32770") as WindowsElement;

        private WindowsElement txtMessage => dialog.FindElementByAccessibilityId("65535") as WindowsElement;

        private WindowsElement btnOk => dialog.FindElementByAccessibilityId("2") as WindowsElement;
        #endregion

        #region Related Persons
        WindowsElement paneRelatedPersons => frmACs.FindElementByAccessibilityId("tabPage1") as WindowsElement;

        WindowsElement txtP1Last => paneRelatedPersons.FindElementByAccessibilityId("txtLast") as WindowsElement;

        WindowsElement txtP1StrNum => paneRelatedPersons.FindElementByAccessibilityId("txtStrNum") as WindowsElement;

        WindowsElement txtP1Street => paneRelatedPersons.FindElementByAccessibilityId("txtStreet") as WindowsElement;

        WindowsElement txtP1City => paneRelatedPersons.FindElementByAccessibilityId("txtCity") as WindowsElement;

        WindowsElement txtP1State => paneRelatedPersons.FindElementByAccessibilityId("txtState") as WindowsElement;

        WindowsElement txtP1Zip => paneRelatedPersons.FindElementByAccessibilityId("txtZip") as WindowsElement;
        #endregion

        #region Toolbar
        WindowsElement btnAdd => session.FindElementByAccessibilityId("btnAdd");

        WindowsElement btnCopyDown => session.FindElementByAccessibilityId("btnCopyDown");
        #endregion

        #region New Account
        WindowsElement dlgNewAccount => session.FindElementByClassName(@"#32770");

        WindowsElement lblNewAccountMessage => dlgNewAccount.FindElementByAccessibilityId("65535") as WindowsElement;

        WindowsElement btnNewAccountOK => dlgNewAccount.FindElementByAccessibilityId("2") as WindowsElement;
        #endregion

        //Basic Interactions
        private void DismissPasswordDialog()
        {
            btnOk.Click();
        }
        private string GetPasswordMessage()
        {
            return txtMessage.Text;
        }
        private void ClickSetPassword()
        {
            btnSetPassword.Click();
        }
        private void SetPassword(string password)
        {
            txtPassword.SendKeys(password);
        }
        private void SetVerifyPassword(string password)
        {
            txtVerifyPassword.SendKeys(password);
        }
        private void SelectContextMenuItem(string menuItem)
        {
            RightClickAccount();
            Actions action = new Actions(session);
            action.MoveToElement(contextMenu.FindElementByName(menuItem));
            action.Click();
            action.Perform();
        }
        private void RightClickAccount()
        {
            Actions action = new Actions(session);
            action.MoveToElement(lblAlarmNo);
            action.ContextClick();
            action.Perform();

        }
        #region Main Form
        private void SetAlarmNumber(string alarmNo)
        {
            txtAlarmNo.SendKeys(alarmNo);
        }
        private void SetCompanyName(string companyName)
        {
            txtCompanyName.SendKeys(companyName);
        }

        private void SetStreetNum(string streetNum)
        {
            txtStrNum.SendKeys(streetNum);
        }

        private void SetStreetName(string streetName)
        {
            txtStreet.SendKeys(streetName);
        }

        private void SetCity(string city)
        {
            txtCity.SendKeys(city);
        }

        private void SetState(string state)
        {
            txtState.SendKeys(state);
        }

        private void SetZipCode(string zip)
        {
            txtZipCode.SendKeys(zip);
        }

        private void ClickClose()
        {
            btnClose.Click();
        }
        #endregion

        #region Related Persons
        #region Person 1
        private void SetP1LastName(string last)
        {

        }
        private void SetP1StreetNum(string streetNum)
        {

        }
        private void SetP1StreetName(string streetName)
        {

        }
        private void SetP1City(string city)
        {

        }
        private void SetP1State(string state)
        {

        }
        private void SetP1ZipCode(string zip)
        {

        }
        #endregion

        #region Person 2

        #endregion
        #endregion

        #region Toolbar
        private void ClickAdd()
        {
            btnAdd.Click();
        }

        private void ClickCopyToRP()
        {
            btnCopyDown.Click();
        }
        #endregion

        #region New Account
        private string GetAccountMessage()
        {
            return lblNewAccountMessage.Text;
        }

        private string GetNewAccountNumber()
        {
            string accountMessage = GetAccountMessage();
            string[] message = accountMessage.Split(' ');

            return message.Last();
        }

        private void DismissNewAccountMessage()
        {
            btnNewAccountOK.Click();
        }
        #endregion

        //Short Functional Methods
        #region Change Password
        public bool ResetPassword(string newPassword)
        {
            SetPassword(newPassword);
            SetVerifyPassword(newPassword);
            ClickSetPassword();

            string message = GetPasswordMessage();
            TestContext.WriteLine(message);

            Library.TakeScreenShot(session, "Password Reset Message", out string filePath);

            DismissPasswordDialog();
            return message.Contains("password was changed successfully");
        }

        public void OpenResetPassword()
        {
            SelectContextMenuItem("Reset Password");
        }
        #endregion

        #region Main Form
        public void OpenAlarmCompany(string alarmNumber)
        {
            SetAlarmNumber(alarmNumber);
            session.Keyboard.SendKeys(Keys.Enter);
        }
        public void EnterNewAccountInfo(Person person, Address address)
        {
            SetCompanyName(person.LastName);
            SetStreetNum(address.StreetNumber);
            SetStreetName(address.StreetName);
            SetCity(address.City);
            SetState(address.State);
            SetZipCode(address.ZipCode);
        }

        public void Close()
        {
            ClickClose();
        }
        #endregion

        #region Related Persons
        public void AddRelatedPerson(Person person = null)
        {
            if (person == null)
            {
                ClickCopyToRP();
            }
        }
        #endregion

        #region Toolbar
        public void UpdateAccount()
        {
            ClickAdd();
        }
        #endregion

        #region New Account
        public bool ValidateNewAccount(out string accountNum)
        {
            accountNum = GetNewAccountNumber();
            bool result = GetAccountMessage().Contains("New Alarm Co");

            Library.TakeScreenShot(session, "New Alarm Co Account", out string filePath);

            DismissNewAccountMessage();
            return result;
        }
        #endregion
    }
}
