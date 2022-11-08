using Desktop.Libraries;
using OpenQA.Selenium.Appium.Windows;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Utils;

namespace Desktop.PageObjects.Maintenance
{
    public class AlarmCountingPeriod
    {
        private readonly WindowsDriver<WindowsElement> session;
        private bool status = false;
        public string categoryName = "Account Related";

        public AlarmCountingPeriod(WindowsDriver<WindowsElement> driver)
        {
            this.session = driver;
        }

        //Object Identification
        #region Main Page
        private WindowsElement frmFACnting => session.FindElementByAccessibilityId("frmFACnting") as WindowsElement;
        private WindowsElement cbAgency => frmFACnting.FindElementByAccessibilityId("cbAgency") as WindowsElement;
        private WindowsElement grdAlarmCountingPeriod => frmFACnting.FindElementByAccessibilityId("grd") as WindowsElement;
        private WindowsElement btnAlarmCountingPeriodOK => frmFACnting.FindElementByAccessibilityId("btnOK") as WindowsElement;
        private WindowsElement btnCloseAlarmCountingPeriod => frmFACnting.FindElementByName("Close") as WindowsElement;
        #endregion

        //Basic Interactions
        private string GetAgency()
        {
            return cbAgency.Text;
        }
        private void SelectAgency(string agency = "<Default>")
        {
            cbAgency.SelectListItem(agency);
        }
        private void ClickAlarmCountingPeriodOK()
        {
            btnAlarmCountingPeriodOK.Click();
        }

        //Short functional methods
        public void CommitAlarmCountingPeriod()
        {
            ClickAlarmCountingPeriodOK();
        }
        public void CloseAlarmCountingPeriod()
        {
            btnCloseAlarmCountingPeriod.Click();
        }
        public void EditCountingPeriod(string timePeriod, string month, string days, string dispatchCode, string location)
        {
            var headers = grdAlarmCountingPeriod.FindElementsByTagName("Header");
            int columnCount = headers.Count();

            var dataItems = grdAlarmCountingPeriod.FindElementsByTagName("DataItem");
            int rowCount = (dataItems.Count / columnCount);
            int row = 0;

            foreach (var item in dataItems)
            {
                if (item.Text == location)
                {
                    row = dataItems.IndexOf(item) / rowCount;
                    if (row < 1)
                    {
                        row = 1;
                    }
                    break;
                }
            }

            foreach (var header in headers)
            {
                int index = (headers.IndexOf(header) + (columnCount * (row - 1)));
                WindowsElement temp = dataItems.ElementAt(index) as WindowsElement;

                switch (header.Text)
                {
                    case "Location/Dispatch Group":
                        temp.ActivateGridDropdown();

                        session.FindElementByName(location).Click();
                        break;

                    case "Time Period":
                        temp.ActivateGridDropdown();

                        session.FindElementByName(timePeriod).Click();
                        break;

                    case "Month":
                        temp.ActivateGridDropdown();

                        session.FindElementByName(month).Click();
                        break;

                    case "Days/Dt":
                        temp.Clear();
                        temp.SendKeys(days);
                        break;

                    case "Dispatch Code":
                        temp.ActivateGridDropdown();

                        session.FindElementByName(dispatchCode).Click();
                        break;

                    default:
                        break;
                }
            }
        }
        public void SetAlarmCountingPeriod(string timePeriod, string month, string days, string dispatchCode, string location = "<General>")
        {
            EditCountingPeriod(timePeriod, month, days, dispatchCode, location);
            CommitAlarmCountingPeriod();
        }
        public bool ValidateAlarmCountingPeriod(string timePeriod, string month, string days, string dispatchCode, string location = "<General>", [CallerMemberName] string caller = null)
        {
            string[] expected = new string[] { location, timePeriod, month, days, dispatchCode };

            if (!GetAgency().Equals("<Default>"))
            {
                SelectAgency();
            }
            
            Library.TakeScreenShot(session, "Alarm Counting Period window", out string filePath);

            var dataItems = grdAlarmCountingPeriod.FindElementsByTagName("DataItem");
            foreach (var item in dataItems)
            {
                if (Regex.IsMatch(item.Text.ToString(), "Row [1-9] Column [0-9]"))
                {
                    if (dataItems.ElementAt(dataItems.IndexOf(item) + 1).Text == location)
                    {
                        string[] vs = new string[]
                        {
                            dataItems.ElementAt(dataItems.IndexOf(item)+1).Text,
                            dataItems.ElementAt(dataItems.IndexOf(item)+2).Text,
                            dataItems.ElementAt(dataItems.IndexOf(item)+3).Text,
                            dataItems.ElementAt(dataItems.IndexOf(item)+4).Text,
                            dataItems.ElementAt(dataItems.IndexOf(item)+5).Text

                        };
                        status = vs.SequenceEqual(expected);
                    }
                    else if (dataItems.ElementAt(dataItems.IndexOf(item) + 1).Text == "")
                    {
                        break;
                    }
                }
            }

            //CloseAlarmCountingPeriod();
            return status;
        }

    }
}
