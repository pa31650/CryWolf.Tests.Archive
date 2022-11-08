using Desktop.Libraries;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Utils;

namespace Desktop.PageObjects.Maintenance
{
    public class SpecialActions
    {
        private readonly WindowsDriver<WindowsElement> windowsDriver;
        private bool status = false;
        public string categoryName = "Special Actions";

        public SpecialActions(WindowsDriver<WindowsElement> driver)
        {
            this.windowsDriver = driver;
        }

        //Object Identification
        #region Main Page Objects
        private WindowsElement frmSpecialActions => windowsDriver.FindElementByAccessibilityId("frmSpecialActions");
        private WindowsElement btnClose => frmSpecialActions.FindElementByName("Close") as WindowsElement;
        private WindowsElement cbActionType => frmSpecialActions.FindElementByAccessibilityId("cbActionType") as WindowsElement;
        private WindowsElement grdSpecialActions => frmSpecialActions.FindElementByAccessibilityId("gd1") as WindowsElement;
        private WindowsElement btnSpecialActionsOK => frmSpecialActions.FindElementByAccessibilityId("btnOK") as WindowsElement;
        private WindowsElement dlgUpdateWarning => windowsDriver.FindElementByName("UPDATE WARNING");
        private WindowsElement btnUpdateWarningOK => dlgUpdateWarning.FindElementByName("OK") as WindowsElement;
        private WindowsElement cbAgency => frmSpecialActions.FindElementByAccessibilityId("cbAgency") as WindowsElement;
        #endregion

        //Basic Interactions
        public void SelectAgency(string agency)
        {
            if (cbAgency.Text != agency)
            {
                cbAgency.SelectListItem(agency);
            }
        }
        private void SetActionType(string actionType)
        {
            cbActionType.SelectListItem(actionType);
        }

        private void ClickSpecialActionsOk()
        {
            btnSpecialActionsOK.Click();
        }

        private void ClickUpdateWarningOK()
        {
            btnUpdateWarningOK.Click();
        }

        //Short functional methods
        public void Close()
        {
            frmSpecialActions.Click();
            btnClose.Click();
        }
        public void AddSpecialAction(string current, string paidLtr, string prepareLetter, string options)
        {
            string currentStatusRow = "gdCurrent Row[0 - 9]";
            string paidLtrTypeRow = "gdAndThey Row[0 - 9]";
            string prepareLetterRow = "gdPrepareLetter Row[0 - 9]";
            string optionsRow = "gdOptions Row[0 - 9]";

            var dataItems = grdSpecialActions.FindElementsByTagName("DataItem");

            //Add item to first blank line
            foreach (var dataItem in dataItems)
            {
                switch (dataItem.Text.ToString())
                {
                    case var statusMatch when new Regex(currentStatusRow).IsMatch(dataItem.Text.ToString()):
                        WindowsElement newCurrent = grdSpecialActions.FindElementByName(dataItem.Text) as WindowsElement;
                        newCurrent.ActivateGridDropdown();

                        windowsDriver.FindElementByName(current).Click();
                        break;
                    case var statusMatch when new Regex(paidLtrTypeRow).IsMatch(dataItem.Text.ToString()):
                        WindowsElement newPaidLtrType = grdSpecialActions.FindElementByName(dataItem.Text) as WindowsElement;
                        newPaidLtrType.ActivateGridDropdown();

                        windowsDriver.FindElementByName(paidLtr).Click();
                        break;
                    case var statusMatch when new Regex(prepareLetterRow).IsMatch(dataItem.Text.ToString()):
                        WindowsElement newPrepareLetter = grdSpecialActions.FindElementByName(dataItem.Text) as WindowsElement;
                        newPrepareLetter.ActivateGridDropdown();

                        windowsDriver.FindElementByName(prepareLetter).Click();
                        break;
                    case var statusMatch when new Regex(optionsRow).IsMatch(dataItem.Text.ToString()):
                        WindowsElement newOptions = grdSpecialActions.FindElementByName(dataItem.Text) as WindowsElement;
                        newOptions.ActivateGridDropdown();

                        windowsDriver.FindElementByName(options).Click();
                        break;
                }
            }
        }
        public void CommitSpecialActions()
        {
            ClickSpecialActionsOk();
            ClickUpdateWarningOK();
        }
        public void SetSpecialAction(string actionType, string currentStatus, string paidLetterType, string prepareLetter, string options)
        {
            SetActionType(actionType);
            AddSpecialAction(currentStatus, paidLetterType, prepareLetter, options);
            CommitSpecialActions();
        }
        public bool ValidateSpecialAction(string actionType, string currentStatus, string paidLetterType, string prepareLetter, string options, [CallerMemberName] string caller = null)
        {
            string[] expected = new string[] { currentStatus, paidLetterType, prepareLetter, options };

            SetActionType(actionType);

            var dataItems = grdSpecialActions.FindElementsByTagName("DataItem");

            for (int i = 0; i < dataItems.Count; i++)
            {
                if (Regex.IsMatch(dataItems[i].Text.ToString(), "Row [1-9] Column [0-9]"))
                {
                    if (dataItems[i + 1].Text.ToString() == currentStatus)
                    {
                        string[] vs = new string[] {
                            dataItems[i + 1].Text,
                            dataItems[i + 2].Text,
                            dataItems[i + 3].Text,
                            dataItems[i + 4].Text
                        };

                        status = vs.SequenceEqual(expected);
                    }
                    else if (dataItems[i + 1].Text.ToString() == "")
                    {
                        break;
                    }
                }
            }

            Library.TakeScreenShot(windowsDriver, caller);

            return status;
        }

        public void DeleteSpecialAction(string actionType, string currentStatus, string paidLetterType, string prepareLetter, string options)
        {
            string[] expected = new string[] { currentStatus, paidLetterType, prepareLetter, options };
            
            SetActionType(actionType);
            var dataItems = grdSpecialActions.FindElementsByTagName("DataItem");

            for (int i = 0; i < dataItems.Count; i++)
            {
                if (Regex.IsMatch(dataItems[i].Text.ToString(), "Row [1-9] Column [0-9]"))
                {
                    if (dataItems[i + 1].Text.ToString() == currentStatus)
                    {
                        string[] vs = new string[] {
                            dataItems[i + 1].Text,
                            dataItems[i + 2].Text,
                            dataItems[i + 3].Text,
                            dataItems[i + 4].Text
                        };

                        if (vs.SequenceEqual(expected))
                        {
                            dataItems[i].Click();
                            windowsDriver.Keyboard.SendKeys(Keys.Delete);
                        }
                    }
                    else if (dataItems[i + 1].Text.ToString() == "")
                    {
                        break;
                    }
                }
            }

            CommitSpecialActions();
        }
    }
}
