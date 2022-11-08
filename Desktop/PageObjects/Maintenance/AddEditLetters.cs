using Desktop.Libraries;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Desktop.PageObjects.Maintenance
{
    public class AddEditLetters
    {
        private readonly WindowsDriver<WindowsElement> windowsDriver;
        private bool status = false;
        public string categoryName = "Add/Edit Letters";

        public AddEditLetters(WindowsDriver<WindowsElement> driver)
        {
            this.windowsDriver = driver;
        }

        //Object Identification
        #region Main Page
        private WindowsElement frmLetter2 => windowsDriver.FindElementByAccessibilityId("frmLetter2");
        private WindowsElement cbLetterName => frmLetter2.FindElementByAccessibilityId("cbLetterTitle") as WindowsElement;
        private WindowsElement cbUse => frmLetter2.FindElementByAccessibilityId("cbLetterUse") as WindowsElement;
        private WindowsElement cbSendTo => frmLetter2.FindElementByAccessibilityId("cbSendTo") as WindowsElement;
        private WindowsElement cbLetterTemplate => frmLetter2.FindElementByAccessibilityId("cbLetterTemplate") as WindowsElement;
        private WindowsElement tabLetterBody => frmLetter2.FindElementByName("Letter Body") as WindowsElement;
        private WindowsElement tabGeneralSettings => frmLetter2.FindElementByName("General Settings") as WindowsElement;
        private WindowsElement grdStatus => frmLetter2.FindElementByAccessibilityId("grdStatus") as WindowsElement;
        private WindowsElement btnSaveLetter => frmLetter2.FindElementByAccessibilityId("btnSave") as WindowsElement;
        private WindowsElement rbRenewPermit => frmLetter2.FindElementByAccessibilityId("rbRenewPermit") as WindowsElement;
        private WindowsElement cbAgency => frmLetter2.FindElementByAccessibilityId("cbAgency") as WindowsElement;
        #endregion

        #region Form Letters
        private WindowsElement frmInputBox => windowsDriver.FindElementByAccessibilityId("frmInputBox");
        private WindowsElement txtLetterName => frmInputBox.FindElementByAccessibilityId("txtLetterName") as WindowsElement;
        private WindowsElement btnInputBoxOK => frmInputBox.FindElementByName("OK") as WindowsElement;
        #endregion

        //Basic Interactions
        public void SelectAgency(string agency)
        {
            if (!cbAgency.Text.Equals(agency))
            {
                cbAgency.SelectListItem(agency);
            }
        }
        private WindowsElement getTabUse()
        {
            WindowsElement tabUse = null;
            var uses = windowsDriver.FindElementsByName("Use");

            foreach (var use in uses)
            {
                if (use.GetAttribute("ControlType") == "ControlType.Tab")
                {
                    tabUse = use;
                }
            }
            return tabUse;

        }
        private WindowsElement getTabStatus()
        {
            return getTabUse().FindElementByName("Status") as WindowsElement;
        }
        private void SetUseForForms(string useName)
        {
            cbUse.Click();
            cbUse.FindElementByName(useName).Click();
        }
        private void SetSendToForForms(string sendTo)
        {
            cbSendTo.Click();
            cbSendTo.FindElementByName(sendTo).Click();
        }
        private void SetLetterTemplateForForms(string letterTemplate)
        {
            cbLetterTemplate.Click();
            cbLetterTemplate.FindElementByName(letterTemplate).Click();
        }
        private void SelectActionsTab()
        {
            WindowsElement tabActions = getTabUse().FindElementByName("Actns") as WindowsElement;
            tabActions.Click();
        }
        private void SelectStatusTab()
        {
            WindowsElement tabStatus = getTabStatus() as WindowsElement;
            tabStatus.Click();
        }
        private void SetLetterName(string letterName)
        {
            txtLetterName.SendKeys(letterName);
        }
        private void ClickLetterNameOK()
        {
            btnInputBoxOK.Click();
        }

        //Short functional methods
        public void SelectLetterName(string letterName)
        {
            cbLetterName.Click();
            cbLetterName.FindElementByName(letterName).Click();
        }
        public bool IsLetterPresent(string ltrName)
        {
            cbLetterName.Click();
            bool isLetterPresent = (cbLetterName.FindElementsByName(ltrName).Count > 0);
            cbLetterName.Click();
            return isLetterPresent;

        }
        public bool ValidateLetterStatus(List<KeyValuePair<string, string>> expectedStatusPairs)
        {
            string failedStatus = null;

            SelectStatusTab();

            var dataItems = grdStatus.FindElementsByTagName("DataItem");

            List<KeyValuePair<string, string>> actualStatusPairs = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < dataItems.Count; i++)
            {
                if (Regex.IsMatch(dataItems[i].Text.ToString(), "Row [0-9] Column [0-9]"))
                {
                    if (!Regex.IsMatch(dataItems[i + 1].Text.ToString(), "CurrentStatus Row [0-9]"))
                    {
                        actualStatusPairs.Add(new KeyValuePair<string, string>(dataItems[i + 1].Text, dataItems[i + 2].Text));
                    }
                }
            }

            foreach (KeyValuePair<string, string> expectedPair in expectedStatusPairs)
            {
                foreach (KeyValuePair<string, string> actualPair in actualStatusPairs)
                {
                    if ((actualPair.Key == expectedPair.Key) && actualPair.Value == expectedPair.Value)
                    {
                        status = true;
                        break;
                    }
                    status = false;
                }

                if (!status)
                {
                    Console.WriteLine($"The Status pair of {expectedPair.Key} and {expectedPair.Value} was not found.");
                    failedStatus = $"{expectedPair.Key},{expectedPair.Value}";
                }
            }

            if (failedStatus == null)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }
        public bool ValidateLetterActions(string ltrIssueExpSetting)
        {
            SelectActionsTab();

            switch (ltrIssueExpSetting.Replace(" ",string.Empty).ToLower())
            {
                case "renewpermit":
                    return rbRenewPermit.Selected;
                default:
                    TestContext.WriteLine($"{ltrIssueExpSetting} is not recognized.");
                    return false;
            }
        }
        public void AddStatusPair(string current, string changeTo)
        {
            var dataItems = grdStatus.FindElementsByTagName("DataItem");

            //Add item to first blank line
            foreach (var dataItem in dataItems)
            {
                if (Regex.IsMatch(dataItem.Text.ToString(), "Current Row [0-9]"))
                {
                    //var newCurrent = windowsDriver.FindElementByAccessibilityId("grdStatus").FindElementByName(dataItem.Text);
                    var newCurrent = grdStatus.FindElementByName(dataItem.Text);
                    newCurrent.Click();
                    newCurrent.SendKeys(Keys.Enter);
                    newCurrent.Click();
                    windowsDriver.FindElementByName(current).Click();
                }
                else if (Regex.IsMatch(dataItem.Text.ToString(), "lvChangeTo Row [0-9]"))
                {
                    var newChangeTo = grdStatus.FindElementByName(dataItem.Text);
                    newChangeTo.Click();
                    newChangeTo.SendKeys(Keys.Enter);
                    newChangeTo.Click();
                    windowsDriver.FindElementByName(changeTo).Click();
                }
            }

        }
        public void CreateNewLetter(string ltrAgency, string ltrName, string ltrUse, string ltrSendTo, string ltrLetterTemplate, string ltrIssueExpSetting, List<KeyValuePair<string, string>> keyValuePairs)
        {
            #region Complete TX Text Control load
            tabLetterBody.Click();
            WebDriverWait wait = new WebDriverWait(windowsDriver, TimeSpan.FromSeconds(10));
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.Name("buttonBar1")));
            }
            catch (InvalidOperationException)
            {
                tabLetterBody.Click();
                wait.Until(ExpectedConditions.ElementExists(By.Name("buttonBar1")));
            }

            tabGeneralSettings.Click();
            #endregion

            SetUseForForms(ltrUse);

            SetSendToForForms(ltrSendTo);

            SetLetterTemplateForForms(ltrLetterTemplate);

            SelectStatusTab();

            #region Add Status info
            foreach (var kvp in keyValuePairs)
            {
                AddStatusPair(kvp.Key, kvp.Value);
            }
            #endregion

            SelectActionsTab();

            SetActionsOptions(ltrIssueExpSetting);

            #region Save New Letter
            btnSaveLetter.Click();
            SetLetterName(ltrName);
            //txtLetterName.SendKeys(ltrName);
            ClickLetterNameOK();
            //btnOK.Click();
            #endregion
        }
        public void SetActionsOptions(string ltrIssueExpSetting)
        {
            switch (ltrIssueExpSetting.ToLower())
            {
                case "renewpermit":
                    rbRenewPermit.Click();
                    break;
                default:
                    Console.WriteLine($"{ltrIssueExpSetting} is not coded for.");
                    break;
            }
        }
        public void DeleteLetter(string ltrName)
        {
            #region Select Name/Title
            windowsDriver.FindElementByAccessibilityId("cbLetterTitle").Click();
            windowsDriver.FindElementByAccessibilityId("cbLetterTitle").FindElementByName("New Letter").Click();
            #endregion

            #region Delete Selected Letter
            windowsDriver.FindElementByAccessibilityId("btnDelete").Click();
            windowsDriver.FindElementByName("Yes").Click();
            #endregion
        }


    }
}
