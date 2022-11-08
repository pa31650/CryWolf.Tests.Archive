using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Utils;

namespace Desktop.PageObjects.AutoProcess
{
    partial class AutoProcess
    {
        private readonly WindowsDriver<WindowsElement> session;
        //ReportBuilder report = new ReportBuilder();
        private string reportAdd;
        private string categoryAdd;
        private bool status = false;

        public AutoProcess(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification
        private WindowsElement grdExpiringAR => session.FindElementByAccessibilityId("grdExpiringAR");
        private WindowsElement tabExpiringAccounts => session.FindElementByName("Expiring Accounts");
        private WindowsElement btnProcessCount => session.FindElementByAccessibilityId("btnProcess");
        private WindowsElement frmReady => session.FindElementByAccessibilityId("frmReady");
        private WindowsElement btnReadyOK => frmReady.FindElementByAccessibilityId("btnOK") as WindowsElement;
        private WindowsElement btnReadyCancel => frmReady.FindElementByAccessibilityId("btnCancel") as WindowsElement;
        private WindowsElement pbReady => frmReady.FindElementByAccessibilityId("pb1") as WindowsElement;
        private WindowsElement dialog => session.FindElementByTagName("dialog");
        private WindowsElement dialogOK => dialog.FindElementByAccessibilityId("2") as WindowsElement;
        //Basic interactions
        private WindowsElement getCurrentStatusByName(string name)
        {
            return grdExpiringAR.FindElementByName(name) as WindowsElement;
        }
        private WindowsElement getNewStatus(string row)
        {
            return grdExpiringAR.FindElementByName($"LetterSent Row {row}") as WindowsElement;
        }
        private int getExpiringArRowCount()
        {
            var dataItems = grdExpiringAR.FindElementsByTagName("DataItem");
            int rowCount = 0;
            for (int i = 0; i < dataItems.Count; i++)
            {
                if (grdExpiringAR.FindElementsByName($"Row {i} Column 0").Count == 0)
                {
                    rowCount = i;
                    break;
                }
            }
            return rowCount;
        }
        private void ClickExpiringAccounts()
        {
            tabExpiringAccounts.Click();
        }
        private void ClickProcessCountOnly()
        {
            btnProcessCount.Click();
        }
        private void ClickReadyOK()
        {
            if (session.Title != "AutoProcess Accounts")
            {
                Utility.SwitchWindow("AutoProcess Accounts", session);
            }
            btnReadyOK.Click();
        }
        private void ClickReadyCancel()
        {
            if (session.Title != "AutoProcess Accounts")
            {
                Utility.SwitchWindow("AutoProcess Accounts", session);
            }

            btnReadyCancel.Click();
        }
        //Short functional methods
        public bool ValidateAlarmedLocationRenewals(List<KeyValuePair<string, string>> expectedStatusPairs)
        {
            string failedStatus = null;
            List<KeyValuePair<string, string>> actualPairs = new List<KeyValuePair<string, string>>();
            ClickExpiringAccounts();

            int i = 1;

            //Get Current Status values
            do
            {
                if (!Regex.IsMatch(grdExpiringAR.FindElementByName($"CurrentStatus Row {i}").Text, "CurrentStatus Row [1-9]"))
                {
                    actualPairs.Add(new KeyValuePair<string, string>(
                    grdExpiringAR.FindElementByName($"CurrentStatus Row {i}").Text,
                    grdExpiringAR.FindElementByName($"LetterSent Row {i}").Text));
                }

                i++;
            } while (grdExpiringAR.FindElementsByName($"CurrentStatus Row {i}").Count != 0);

            if (expectedStatusPairs.Count != actualPairs.Count)
            {
                failedStatus = "Count mismatch";
            }

            foreach (var expectedPair in expectedStatusPairs)
            {
                foreach (var actualPair in actualPairs)
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
                    Console.WriteLine($"The expected pair of {expectedPair.Key} and {expectedPair.Value} was not found.");
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
        public void CloseAutoProcessAlerts()
        {
            Root root = new Root(CryWolfUtil.GetDesktopSession());

            do
            {
                if (root.GetAlertText() == "AutoProcess Now Completed")
                {
                    root.AcceptAlert();
                    break;
                }

                Console.WriteLine(root.GetAlertText());
                root.AcceptAlert();
            } while (root.IsAlertDisplayed());
        }
        public void ExecuteAutoProcess()
        {
            ClickProcessCountOnly();
            Utility.SetImplicitWait(session, 30);
            
            //Wait for AutoProcessAccounts window
            ClickReadyOK();

            Utility.SetImplicitWait(session, 10);

            CloseAutoProcessAlerts();
        }
    }
}
