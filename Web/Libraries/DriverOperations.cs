using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Utils;
using Web.PageObjects;



namespace Web.Libraries
{
    /// <summary>
    /// Handles IWebDriver and IWebElement interactions such as waiting for a page to load and clicking an element
    /// </summary>
    public static class DriverOperations
    {

        /// <summary>
        /// Sets implicit wait to given amount of seconds
        /// </summary>
        public static void SetImplicitWait(IWebDriver driver, int secondsToWait)
        {
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(secondsToWait));
        }

        /// <summary>
        /// Sets implicit wait to given milliseconds or seconds depending on given boolean
        /// </summary>
        public static void SetImplicitWait(IWebDriver driver, bool isMillseconds, int timeToWait)
        {
            if (isMillseconds)
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(timeToWait));
            }
            else
            {
                SetImplicitWait(driver, timeToWait);
            }
        }

        /// <summary>
        /// Checks that the expected element locator (such as the module header is present, reports success/failure
        /// </summary>
        public static void AssertNavigation(IWebDriver driver, By pageLocator, string pageTitle)
        {
            WaitForPageLoad(driver);
            IWebElement header = driver.FindElement(pageLocator);
            if (header.Displayed)
            {
                ReportBuilder.ArrayBuilder("Found and navigated to the " + pageTitle + " page", true, "Page Navigation");
            }
            else
            {
                ReportBuilder.ArrayBuilder("Failed to navigate to the " + pageTitle + " page", false, "Page Navigation");
            }
        }

        /// <summary>
        /// Clicks a button after it's clickable and the page is loaded.
        /// Waits and re-attempts click if an exception is through due to ajax-overlay
        /// </summary>
        public static void ClickButton(IWebDriver driver, By elementPath)
        {
            SetImplicitWait(driver, 0);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementToBeClickable(elementPath));

            try { driver.FindElement(elementPath).Click(); }
            catch (WebDriverException e) when (e.ToString().Contains("ajax-overlay"))
            {
                WaitForPageLoad(driver);
                driver.FindElement(elementPath).Click();
            }
            SetImplicitWait(driver, 30);
        }

        public static void ClickButton(IWebDriver driver, IWebElement button)
        {
            WaitForPageLoad(driver);

            try { button.Click(); }
            catch (InvalidOperationException exception)
            {
                if (exception.ToString().Contains("ajax-overlay"))
                {
                    WaitForPageLoad(driver);
                    button.Click();
                }
                else { throw exception; }
            }
        }

        /// <summary>
        /// Click Element and wait a standard time for settings to update (ex: help text that appears after clicking a field)
        /// </summary>
        public static void ClickElementAndWait(IWebDriver driver, By elementPath)
        {
            IWebElement element = driver.FindElement(elementPath);
            element.Click();
            Thread.Sleep(500);
        }
        /// <summary>
        /// Click Element and wait a standard time for settings to update (ex: dropdown picklist to extend)
        /// </summary>
        public static void ClickElementAndWait(IWebElement element)
        {
            element.Click();
            Thread.Sleep(1000);
        }
        /// <summary>
        /// Wait a standard time for sent input to be processed
        /// </summary>
        public static void WaitForInputLoad()
        {
            Thread.Sleep(500);
        }
        public static void WaitForActionLoad()
        {
            Thread.Sleep(200);
        }

        /// <summary>
        /// Waits for the absence of ajax-overlay, which gets in the way of clicking an element
        /// </summary>
        public static void WaitForPageLoad(IWebDriver driver)
        {
            By ajaxOverlay = By.Id("ajax-overlay");

            // There is sometimes a delay before the overlay appears
            // So just checking for its absence might proceed too soon
            SetImplicitWait(driver, 0);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(750));
            try { wait.Until(ExpectedConditions.ElementIsVisible(ajaxOverlay)); }
            catch (WebDriverTimeoutException) { /*page is already loaded*/ }

            wait.Timeout = TimeSpan.FromSeconds(30);
            wait.Until(d => d.FindElements(ajaxOverlay).Count == 0);
            SetImplicitWait(driver, 30);
        }
        public static void Refresh(IWebDriver driver)
        {
            driver.Navigate().Refresh();
            WaitForPageLoad(driver);
        }
        public static void RefreshAndWait(IWebDriver driver, TimeSpan millisecondsToWait)
        {
            driver.Navigate().Refresh();
            WaitForPageLoad(driver);
            Thread.Sleep(millisecondsToWait);
        }
        public static void ShortWaitOnOverlay(IWebDriver driver)
        {
            By ajaxOverlay = By.Id("ajax-overlay");

            SetImplicitWait(driver, 0);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(200))
            {
                PollingInterval = TimeSpan.FromMilliseconds(50),           
            };

            try { wait.Until(ExpectedConditions.ElementIsVisible(ajaxOverlay)); }
            catch (WebDriverTimeoutException) { /* page is already loaded*/ }

            wait.Timeout = TimeSpan.FromSeconds(10);
            wait.Until(d => d.FindElements(ajaxOverlay).Count == 0);
            SetImplicitWait(driver, 30);
        }
        public static void WaitForTimeSpan(TimeSpan ts)
        {
            DateTime dt = DateTime.Now;
            while (DateTime.Now < dt.Add(ts)) { }
        }

        /// <summary>
        /// Takes an IWebElement path and returns its value (which is its text if it's an input box)
        /// </summary>
        public static string GetInputBoxText(IWebDriver driver, By by)
        {
            IWebElement inputBox = driver.FindElement(by);
            return inputBox.GetAttribute("value");
        }

        /// <summary>
        /// Returns the maxlength attribute of the field. If the max length is 0, returns 1
        /// </summary>
        public static int GetInputBoxMaxLength(IWebDriver driver, By by)
        {
            IWebElement inputBox = driver.FindElement(by);
            int maxLength = Convert.ToInt32(inputBox.GetAttribute("maxlength"));
            return maxLength > 0 ? maxLength : 1;
        }

        public static bool IsElementDisabled(IWebDriver driver, By button)
        {
            CommonElements commonElements = new CommonElements();
            SetImplicitWait(driver, 0);
            bool isDisabled = driver.FindElement(button).FindElement(commonElements.parent).FindElements(commonElements.disabledElement).Count > 0 ? true : false;
            SetImplicitWait(driver, 30);
            return isDisabled;
        }
        public static bool IsElementClassNameDisabled(IWebDriver driver, By elementPath)
        {
            return driver.FindElement(elementPath).GetAttribute("class").Contains("disabled") ? true : false;
        }
        public static bool IsElementAttributeDisabled(IWebDriver driver, By elementPath)
        {
            return driver.FindElement(elementPath).GetAttribute("disabled") == "true" ? true : false;
        }
        public static void VerifyElementDisabled(IWebDriver driver, By elementPath, string elementName)
        {
            if (IsElementDisabled(driver, elementPath) || IsElementClassNameDisabled(driver, elementPath) || IsElementAttributeDisabled(driver, elementPath))
                ReportBuilder.ArrayBuilder($"Verified {elementName} is disabled", true, "Verify Element Disabled");
            else
                ReportBuilder.ArrayBuilder($"{elementName} is not disabled", false, "Verify Element Disabled");
        }
        public static void VerifyElementDisabled(IWebDriver driver, By elementPath, string elementName, bool isExpectedDisabled)
        {
            string elementState = isExpectedDisabled ? "Disabled" : "Enabled";

            if (IsElementDisabled(driver, elementPath) || IsElementClassNameDisabled(driver, elementPath) || IsElementAttributeDisabled(driver, elementPath))
                ReportBuilder.ArrayBuilder($"{elementName} is disabled", isExpectedDisabled, $"Verify Element {elementState}");
            else
                ReportBuilder.ArrayBuilder($"{elementName} is not disabled", !isExpectedDisabled, $"Verify Element {elementState}");
        }

        public static void VerifyTabOrder(IWebDriver driver, Dictionary<By, string> tabs, string formName)
        {
            bool isTabOrderCorrect = true;

            foreach (KeyValuePair<By, string> tab in tabs)
            {
                By expectedActiveElementLocator = tab.Key;
                string expectedActiveElementName = tab.Value;

                if (!IsActiveElement(driver, expectedActiveElementLocator))
                {
                    string report = string.Format("Expected next tab to put user in the {0} field but it is not the active field", expectedActiveElementName);
                    ReportBuilder.ArrayBuilder(report, false, "Verify Tab Order");
                    isTabOrderCorrect = false;
                }
                SendTab(driver);
            }

            if (isTabOrderCorrect)
            {
                ReportBuilder.ArrayBuilder("Verified that the tab order is correct for the " + formName + " form", true, "Verify Tab Order");
            }
        }

        public static bool IsRadioButtonChecked(IWebDriver driver, By by)
        {
            SetImplicitWait(driver, 0);
            string className = driver.FindElement(by).GetAttribute("class");
            bool isChecked = className.Contains("ui-icon-checked") ? true : false;
            SetImplicitWait(driver, 30);
            return isChecked;
        }

        /// <summary>
        /// Radio Button value is stored in database as "True" if button is checked, "False" if not
        /// </summary>
        public static string GetRadioButtonValue(IWebDriver driver, By by)
        {
            return IsRadioButtonChecked(driver, by).ToString();
        }

        /// <summary>
        /// Toggle Button value is stored in database "1" if button is checked, "0" if not
        /// </summary>
        public static string GetToggleButtonValue(IWebDriver driver, By by)
        {
            return IsRadioButtonChecked(driver, by) ? "1" : "0";
        }

        public static string GetYesNoButtonValue(IWebDriver driver, By by)
        {
            return IsRadioButtonChecked(driver, by) ? "Y" : "N";
        }

        public static bool IsElementPresent(IWebDriver driver, By elementPath)
        {
            SetImplicitWait(driver, 0);
            bool isPresent = driver.FindElements(elementPath).Count > 0 ? true : false;
            SetImplicitWait(driver, 30);
            return isPresent;
        }
        public static bool IsElementPresent(IWebDriver driver, By elementPath, int implicitWaitMillseconds)
        {
            SetImplicitWait(driver, true, implicitWaitMillseconds);
            bool isPresent = driver.FindElements(elementPath).Count > 0 ? true : false;
            SetImplicitWait(driver, 30);
            return isPresent;
        }
        public static bool IsElementPresent(IWebDriver driver, IWebElement parent, By elementPath)
        {
            SetImplicitWait(driver, 0);
            bool isPresent = parent.FindElements(elementPath).Count > 0 ? true : false;
            SetImplicitWait(driver, 30);
            return isPresent;
        }

        public static bool IsElementClickable(IWebDriver driver, By elementPath)
        {
            SetImplicitWait(driver, 0);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            bool isClickable = false;

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(elementPath));
                isClickable = true;
            }
            catch (WebDriverTimeoutException) { /* element is not clickable */}

            SetImplicitWait(driver, 30);
            return isClickable;
        }

        /// <summary>
        /// Performs hotkey action with passed key and modifier key (which is set to Alt by default). 
        /// For example: Hotkey(driver, "S") performs Alt + S. HotKey(driver, "S", Keys.Control) performs Ctrl + S
        /// </summary>
        public static void HotKey(IWebDriver driver, string key, string modifierKey = null)
        {
            WaitForPageLoad(driver);

            if (modifierKey is null) { modifierKey = Keys.Alt; }

            new Actions(driver)
                .KeyDown(modifierKey)
                .SendKeys(key)
                .KeyUp(modifierKey)
                .Perform();

            WaitForPageLoad(driver);
        }

        public static void HotKeyC(IWebDriver driver) { HotKey(driver, "C"); }
        public static void HotKeyD(IWebDriver driver) { HotKey(driver, "D"); }
        public static void HotKeyH(IWebDriver driver) { HotKey(driver, "H"); }
        public static void HotKeyI(IWebDriver driver) { HotKey(driver, "I"); }
        public static void HotKeyK(IWebDriver driver) { HotKey(driver, "K", Keys.Control); }
        public static void HotKeyN(IWebDriver driver) { HotKey(driver, "N"); }
        public static void HotKeyO(IWebDriver driver) { HotKey(driver, "O"); }
        public static void HotKeyS(IWebDriver driver) { HotKey(driver, "S"); }
        public static void HotKeyU(IWebDriver driver) { HotKey(driver, "U"); }
        public static void HotKeyV(IWebDriver driver) { HotKey(driver, "V"); }
        public static void HotKeyX(IWebDriver driver) { HotKey(driver, "X"); }
        public static void HotKeyY(IWebDriver driver) { HotKey(driver, "Y"); }

        public static void HotKeyUp(IWebDriver driver)
        {
            WaitForPageLoad(driver);

            new Actions(driver)
                .KeyDown(Keys.Alt)
                .SendKeys(Keys.Up)
                .KeyUp(Keys.Alt)
                .Perform();

            WaitForPageLoad(driver);
        }

        public static void SendEnterKey(IWebDriver driver)
        {
            new Actions(driver).SendKeys(Keys.Enter).Perform();
        }

        public static void HotKeyCtrlSpace(IWebDriver driver)
        {
            new Actions(driver)
                .KeyDown(Keys.Control)
                .SendKeys(Keys.Space)
                .KeyUp(Keys.Control)
                .Perform();
        }


        public static void VerifyActiveElement(IWebDriver driver, By elementPath, string elementName)
        {
            IWebElement expectedActiveElement = driver.FindElement(elementPath);
            IWebElement actualActiveElement = driver.SwitchTo().ActiveElement();

            if (expectedActiveElement.Equals(actualActiveElement))
            {
                ReportBuilder.ArrayBuilder($"Verified active element is: {elementName}", true, "Check Active Element");
            }
            else
            {
                ReportBuilder.ArrayBuilder($"Active element is not: {elementName}", false, "Check Active Element");
                Library.TakeScreenShot(driver, "ActiveElement");
            }
        }
        public static bool IsActiveElement(IWebDriver driver, By elementPath)
        {
            IWebElement expectedActiveElement = driver.FindElement(elementPath);
            IWebElement actualActiveElement = driver.SwitchTo().ActiveElement();

            return expectedActiveElement.Equals(actualActiveElement);
        }

        public static void VerifyElementEnabled(IWebDriver driver, By elementPath, string elementName)
        {
            if (driver.FindElement(elementPath).Enabled && !IsElementClassNameDisabled(driver, elementPath))
            {
                ReportBuilder.ArrayBuilder("Verified " + elementName + " is enabled", true, "Verify Element Enabled");
            }
            else
            {
                ReportBuilder.ArrayBuilder(elementName + " is not enabled", false, "Verify Element Enabled");
            }
        }

        public static void VerifyElementPresence(IWebDriver driver, By elementPath, string elementName)
        {
            SetImplicitWait(driver, 0);
            int elementCount = driver.FindElements(elementPath).Count;
            if (elementCount == 1)
            {
                ReportBuilder.ArrayBuilder("Verified presence of element: " + elementName, true, "Verify Element Presence");
            }
            else
            {
                ReportBuilder.ArrayBuilder("Expected 1 " + elementName + " element but found " + elementCount, false, "Verify Element Presence");
            }
            SetImplicitWait(driver, 30);
        }

        public static void VerifyElementPresence(IWebDriver driver, By elementPath, string elementName, bool isExpectedPresent)
        {
            if (IsElementPresent(driver, elementPath))
            {
                ReportBuilder.ArrayBuilder(elementName + " is present", isExpectedPresent, "Verify Element Presence");
            }
            else
            {
                ReportBuilder.ArrayBuilder(elementName + " is not present", !isExpectedPresent, "Verify Element Presence");
            }
        }


        public static void ResizeWindow(IWebDriver driver, int width, int height)
        {
            driver.Manage().Window.Size = new Size(width, height);
        }
        public static void ResizeWindow(IWebDriver driver, Size size)
        {
            driver.Manage().Window.Size = size;
        }
        public static int GetWindowWidth(IWebDriver driver)
        {
            return driver.Manage().Window.Size.Width;
        }
        public static int GetWindowHeight(IWebDriver driver)
        {
            return driver.Manage().Window.Size.Height;
        }

        public static Size GetElementSize(IWebDriver driver, By elementPath)
        {
            return driver.FindElement(elementPath).Size;
        }
        public static Point GetElementPosition(IWebDriver driver, By elementPath)
        {
            return driver.FindElement(elementPath).Location;
        }

        public static List<IWebElement> GetListItems(IWebDriver driver, By pageLocator, By listLocator)
        {
            IWebElement list = driver.FindElement(pageLocator).FindElement(listLocator);
            SetImplicitWait(driver, 0);
            List<IWebElement> items = list.FindElements(By.TagName("li")).ToList();
            SetImplicitWait(driver, 30);
            return items;
        }

        public static IEnumerable<string> GetListItemsText(IWebDriver driver, By pageLocator, By listLocator)
        {
            List<IWebElement> items = GetListItems(driver, pageLocator, listLocator);
            
            foreach (IWebElement item in items)
            {
                yield return item.Text;
            }
        }


        public static void SelectListItemWithText(IWebDriver driver, string tableXPath, string tableName, int columnIndex, string text)
        {
            int tableElementNumber = 1;
            IWebElement table = driver.FindElement(By.XPath(tableXPath));
            ReadOnlyCollection<IWebElement> allRows = table.FindElements(By.TagName("tr"));
            foreach (IWebElement row in allRows)
            {
                string tableItemXPath = tableXPath + "/tr[" + tableElementNumber + "]/td[" + columnIndex + "]";
                IWebElement tableItem = driver.FindElement(By.XPath(tableItemXPath));
                string tableItemText = tableItem.Text;

                if (tableItemText.Equals(text))
                {
                    ClickButton(driver, tableItem);
                    DriverOperations.WaitForPageLoad(driver);
                    ReportBuilder.ArrayBuilder("Selected item from " + tableName + " table with text: \"" + text + "\"", true, "Select Item From List");
                    return;
                }

                tableElementNumber++;
            }

            ReportBuilder.ArrayBuilder("Failed to select item from " + tableName + " table with text: \"" + text + "\"", false, "Select Item From List");
        }

        public static bool IsFieldPopulated(IWebDriver driver, By fieldPath)
        {
            return GetInputBoxText(driver, fieldPath).Length > 0 ? true : false;
        }

        public static int GetRowCount(IWebDriver driver, By list)
        {
            SetImplicitWait(driver, 0);
            int rowCount = driver.FindElement(list).FindElements(By.TagName("tr")).Count;
            SetImplicitWait(driver, 30);
            return rowCount;
        }
        public static IReadOnlyCollection<IWebElement> GetRows(IWebDriver driver, By list)
        {
            SetImplicitWait(driver, 0);
            IReadOnlyCollection<IWebElement> rows = driver.FindElement(list).FindElements(By.TagName("tr"));
            SetImplicitWait(driver, 30);
            return rows;
        }
        public static int GetColumnIndex(IWebDriver driver, By headerTextLocator, string tableHeadersXPath, string columnName)
        {
            IWebElement tableHeaders = driver.FindElement(By.XPath(tableHeadersXPath));
            ReadOnlyCollection<IWebElement> headerRow = tableHeaders.FindElements(By.TagName("th"));
            int currColumn = 1, expectedColumn = -1;
            foreach (IWebElement cell in headerRow)
            {
                if (cell.FindElement(headerTextLocator).Text.Equals(columnName)) { expectedColumn = currColumn; break; }
                currColumn++;
            }

            if (expectedColumn == -1)
            {
                ReportBuilder.ArrayBuilder("Failed to find column by the name: " + columnName, false, "Find Column Index");
            }

            return expectedColumn;
        }


        public static bool IsPopupPresent(IWebDriver driver, By popupPath)
        {
            SetImplicitWait(driver, 1);
            bool popupExists = driver.FindElements(popupPath).Count > 0 ? true : false;
            SetImplicitWait(driver, 30);
            return popupExists;
        }

        public static void ClearField(IWebDriver driver, By fieldPath)
        {
            IWebElement field = driver.FindElement(fieldPath);
            field.Clear();

            if (GetInputBoxText(driver, fieldPath).Length > 0)
            {
                ReportBuilder.ArrayBuilder("Failed to clear field with selector: " + fieldPath.ToString(), false, "Clear Field");
            }
        }

        /// <summary>
        /// Modify then clear field to enforce change-of-state
        /// </summary>
        public static void ModifyThenClearField(IWebDriver driver, By fieldPath)
        {
            FillTextField(driver, fieldPath, " ", false);
            ClearField(driver, fieldPath);
        }
        public static void EnforceClearField(IWebDriver driver, By fieldPath)
        {
            IWebElement element = driver.FindElement(fieldPath);

            new Actions(driver).Click(element).Perform();
            new Actions(driver).SendKeys(Keys.End).Perform();
            new Actions(driver).KeyDown(Keys.LeftShift).SendKeys(Keys.Home).KeyUp(Keys.LeftShift).Perform();
            new Actions(driver).SendKeys(Keys.Backspace).Perform();
            new Actions(driver).SendKeys(Keys.Tab).Perform();

            string fieldValue = GetInputBoxText(driver, fieldPath);
            if (fieldValue != "")
            {
                ReportBuilder.ArrayBuilder("Could not clear field " + fieldPath + ",  found value: \"" + fieldValue + "\"", false, "Clear Field");
            }
        }


        public static void SendShiftTab(IWebDriver driver)
        {
            new Actions(driver).KeyDown(Keys.Shift).SendKeys(Keys.Tab).KeyUp(Keys.Shift).Perform();
        }
        public static void SendTab(IWebDriver driver)
        {
            new Actions(driver).SendKeys(Keys.Tab).Perform();
        }
        public static void TabField(IWebDriver driver, By fieldPath)
        {
            IWebElement field = driver.FindElement(fieldPath);
            field.SendKeys(Keys.Tab);
        }

        private static void OpenPicklistDropdownViaSpace(IWebDriver driver, By fieldPath)
        {
            EnforceClearField(driver, fieldPath);
            driver.FindElement(fieldPath).Click();
            new Actions(driver).SendKeys(Keys.Space).Perform();
        }
        
        /// <summary>
        /// With a dropdown picklist already open, send Down to highlight the next item and send Enter to select it
        /// </summary>
        public static void SelectNextItemFromDropdown(IWebDriver driver)
        {
            new Actions(driver).SendKeys(Keys.Down).Perform();
            WaitForInputLoad();
            new Actions(driver).SendKeys(Keys.Enter).Perform();
            WaitForInputLoad();
        }

        

        public static void SelectSpecificItemFromSingleMoverPicklist(IWebDriver driver, string itemText, By picklistPageLocator, string picklistName)
        {
            // Get a list of items in the available list, save the number of items in the selected list
            IWebElement singleMoverPicklist = driver.FindElement(picklistPageLocator);
            IWebElement availableList = singleMoverPicklist.FindElement(By.Id("available"));
            IList<IWebElement> availableItems = availableList.FindElements(By.TagName("li"));
            int selectedItemCount = GetSingleMoverPicklistSelectItemsCount(driver, singleMoverPicklist);

            // Move the specified item from available to selected, save the updated number of items in the selected list
            IWebElement expectedItem = availableItems.Single(item => item.Text == itemText);
            expectedItem.Click();
            ClickButton(driver, singleMoverPicklist.FindElement(By.Id("btn_select")));
            WaitForActionLoad();
            int updatedSelectedItemCount = GetSingleMoverPicklistSelectItemsCount(driver, singleMoverPicklist);
            
            // Verify the number of items in the selected list has increased by 1
            if (updatedSelectedItemCount == selectedItemCount + 1)
            {
                string report = string.Format("Moved \"{0}\" from available to selected in the \"{1}\" picklist", itemText, picklistName);
                ReportBuilder.ArrayBuilder(report, true, "Select Item");
            }
            else
            {
                string report = string.Format("Failed to move \"{0}\" from available to selected in the \"{1}\" picklist", itemText, picklistName);
                ReportBuilder.ArrayBuilder(report, false, "Select Item");
            }
        }
        private static int GetSingleMoverPicklistSelectItemsCount(IWebDriver driver, IWebElement picklist)
        {
            DriverOperations.SetImplicitWait(driver, 0);
            int count = picklist.FindElement(By.Id("selected")).FindElements(By.TagName("li")).Count;
            DriverOperations.SetImplicitWait(driver, 30);
            return count;
        }
                
        public static void SelectRandomItemFromPicklist(IWebDriver driver, By picklistLocator, int numSteps)
        {
            EnforceClearField(driver, picklistLocator);

            try { driver.FindElement(picklistLocator).Click(); }
            catch { HandleClickException(driver, picklistLocator); }

            new Actions(driver).SendKeys(Keys.Space).Perform();
            WaitForActionLoad();
      
            for (int i = 1; i <= numSteps; i++)
            {
                new Actions(driver).SendKeys(Keys.Down).Perform();
            }
            WaitForActionLoad();
            new Actions(driver).SendKeys(Keys.Enter).Perform();

            WaitForInputLoad();
            if (!IsFieldPopulated(driver, picklistLocator))
            {
                ReportBuilder.ArrayBuilder("Failed to populate picklist " + picklistLocator + " via dropdown", false, "Populate Picklist");
            }
        }

        public static string GetElementID(IWebDriver driver, By elementPath)
        {
            return driver.FindElement(elementPath).GetAttribute("id");
        }
        public static string GetFieldCode(IWebDriver driver, string fieldPathID)
        {
            return GetInputBoxText(driver, By.CssSelector("[name='" + fieldPathID + "']"));
        }
        /// <summary>
        /// Gets the value of the element that's stored in the database. (Ex: DANVILLE is stored as DANV in the database)
        /// </summary>
        public static string GetFieldCode(IWebDriver driver, By elementPath)
        {
            string fieldPathID = GetElementID(driver, elementPath);
            return GetFieldCode(driver, fieldPathID);
        }

        public static void PopulateFieldWithReport(IWebDriver driver, By fieldPath, string fieldName, string scenario)
        {
            string input = "Populating " + fieldName + " field for " + scenario;
            FillTextField(driver, fieldPath, input, false);
            driver.SwitchTo().ActiveElement();

            if (GetInputBoxText(driver, fieldPath).Equals(input))
            {
                ReportBuilder.ArrayBuilder("Verified " + fieldName + " field was populated", true, "Populate Field");
            }
            else
            {
                ReportBuilder.ArrayBuilder(fieldName + " field was not populated", false, "Populate Field");
            }
        }

        /// <summary>
        /// Iterates through a collection of elements and returns true if one is successfully clicked.
        /// This circumvents issue where multiple of the same element are overlayed over one another and one in the background is clicked and throws an exception.
        /// </summary>
        public static bool ClickFrontElement(IWebDriver driver, IReadOnlyCollection<IWebElement> overlayedElements)
        {
            bool isElementClicked = false;
            foreach (IWebElement element in overlayedElements)
            {
                try
                {
                    element.Click();
                    isElementClicked = true;
                    break;
                }
                catch (ElementNotVisibleException) { }
                //catch (ElementClickInterceptedException) { }
            }
      
            return isElementClicked;
        }

        /// <summary>
        /// Clears, sends input to text field, and tabs to save changes.
        /// Sends keys slowly if the field is a picklist
        /// </summary>
        public static void FillTextField(IWebDriver driver, By elementPath, string input, bool isPicklist = true)
        {
            //EnforceClearField(driver, elementPath); // note: tabs out of field which causes page to change if it's the last field on that tab
            ClearField(driver, elementPath);

            IWebElement textField = driver.FindElement(elementPath);
            if (isPicklist) { SendKeyByKey(textField, input ?? "", driver); }
            else { textField.SendKeys(input ?? ""); }

            textField.SendKeys(Keys.Tab);
        }

        

        /// <summary>
        /// Iterates through all passed fields and fills them with all passed values.
        /// Sends keys more slowly to fields that are picklists for reliability
        /// </summary>
        public static void FillMultipleTextFields(IWebDriver driver, List<Tuple<By, string, bool>> tuples)
        {
            foreach (Tuple<By, string, bool> tuple in tuples)
            {
                string input = tuple.Item2;
                if (!string.IsNullOrWhiteSpace(input))
                {
                    IWebElement textField = driver.FindElement(tuple.Item1);
                    textField.Clear();
                    if (tuple.Item3) { SendKeyByKey(textField, input, driver); }
                    else { textField.SendKeys(input); }

                    textField.SendKeys(Keys.Enter);
                    Thread.Sleep(25);
                }
            }
        }
        /// <summary>
        ///  Replaces passed dictionary of elements and values with new values found in each element
        /// </summary>
        public static Dictionary<By, string> ReadMultipleTextFields(IWebDriver driver, Dictionary<By, string> elems)
        {
            SetImplicitWait(driver, 3);
            Dictionary<By, string> foundTextBoxes = new Dictionary<By, string>();
            foreach (KeyValuePair<By, string> pair in elems)
            {
                By fieldPath = pair.Key;
                if (driver.FindElements(fieldPath).Count > 0)
                {
                    string newValue = GetInputBoxText(driver, fieldPath).TrimEnd('\t', '\n');
                    foundTextBoxes.Add(fieldPath, newValue);
                }
                else
                {
                    //ReportBuilder report = new ReportBuilder();
                    string reportAdd = "Could not find element with path: \"" + fieldPath + "\"";
                    ReportBuilder.ArrayBuilder(reportAdd, false, "Read Text Field");
                }
            }
            SetImplicitWait(driver, 30);
            return foundTextBoxes;
        }
        public static bool CheckFieldsPopulated(IWebDriver driver, List<By> fieldPaths)
        {
            bool allFieldsPopulated = true;
            foreach (By path in fieldPaths)
            {
                if (GetInputBoxText(driver, path).Length == 0)
                {
                    ReportBuilder.ArrayBuilder("Field found with locator: " + path + " is not populated", false, "Check Field Populated");
                    allFieldsPopulated = false;
                }
            }

            return allFieldsPopulated;
        }
        /// <summary>
        /// Builds a report on the equivalence of two dictionaries (built from textboxes and their values)
        /// </summary>
        /// <param name="expected">Dictionary built from excel sheet values</param>
        /// <param name="found">Dictionary built from getting values from input boxes</param>
        public static void VerifyDictionariesAreEqual(Dictionary<By, string> expected, Dictionary<By, string> found)
        {
            //ReportBuilder report = new ReportBuilder();
            string reportAdd = "";
            bool status = true;

            if (expected.Count != found.Count)
            {
                ReportBuilder.ArrayBuilder("Dictionaries are not the same size", false, "Compare Dictionaries");
                return;
            }

            foreach (By key in expected.Keys)
            {
                if (!string.IsNullOrWhiteSpace(expected[key]))
                {
                    if (!expected[key].Equals(found[key].Trim())) { status = false; }
                }

                string categoryAdd = "Verify Input Field Unchanged";
                if (!status)
                {
                    if (found[key].Equals(""))
                    {
                        reportAdd = "Entered Value: \"" + expected[key] + "\" but input box is empty. (Check bug ID: 55782)";
                    }
                    else
                    {
                        reportAdd = "Entered Value: \"" + expected[key] + "\" DOES NOT match found value: \"" + found[key] + "\"";
                    }
                }
                else { reportAdd = "Entered Value: \"" + expected[key] + "\" matches found value"; }
                ReportBuilder.ArrayBuilder(reportAdd, status, categoryAdd);

                if (!status) { status = true; }
            }
        }


        public static bool VerifyDictionariesAreEqual(Dictionary<string, string> expected, Dictionary<string, string> found)
        {
            if (expected.Count != found.Count)
            {
                ReportBuilder.ArrayBuilder("Dictionaries are not the same size", false, "Compare Dictionaries");
                return false;
            }

            foreach (string key in expected.Keys)
            {
                if (expected[key] != found[key])
                {
                    return false;
                }
            }

            return true;
        }

        public static Dictionary<By, string> ClearEmptyDictionaryValues(Dictionary<By, string> dictionary)
        {
            Dictionary<By, string> textFieldInputs = new Dictionary<By, string>();
            foreach (KeyValuePair<By, string> pair in dictionary)
            {
                if (!string.IsNullOrWhiteSpace(pair.Value))
                {
                    textFieldInputs.Add(pair.Key, pair.Value);
                }
            }

            return textFieldInputs;
        }

        /// <summary>
        /// Sends keys more slowly so that picklists don't clear input
        /// </summary>
        public static void SendKeyByKey(IWebElement textField, string input, IWebDriver driver)
        {
            textField.Clear();        
            try { textField.Click(); } // Simulates user clicking into field before entering information
            catch (InvalidOperationException) { HandleClickException(driver, textField); }
            catch (WebDriverException) { HandleClickException(driver, textField); }

            Char[] inputArray = input.ToCharArray();
            foreach (Char letter in inputArray)
            {
                textField.SendKeys(letter.ToString());
                Thread.Sleep(25);
            }
        }
        /// <summary>
        /// Attempts to click the element using actions class, reports failure if unsuccessful
        /// </summary>
        private static void HandleClickException(IWebDriver driver, IWebElement element)
        {
            try
            {
                // Click the center of the element is clicking the top-left corner throws an exception
                int width = element.Size.Width;
                int height = element.Size.Height;
                new Actions(driver).MoveToElement(element, width / 2, height / 2).Perform();
            }
            catch (InvalidOperationException)
            {
                ReportBuilder.ArrayBuilder("Cannot click on element \"" + element.GetAttribute("id") + "\"", false, "Element Not Clickable");
            }
        }
        private static void HandleClickException(IWebDriver driver, By elementPath)
        {
            IWebElement element = driver.FindElement(elementPath);

            try
            {
                // Click the center of the element is clicking the top-left corner throws an exception
                int width = element.Size.Width;
                int height = element.Size.Height;
                new Actions(driver).MoveToElement(element, width / 2, height / 2).Click().Perform();
            }
            catch (InvalidOperationException)
            {
                ReportBuilder.ArrayBuilder("Cannot click on element \"" + element.GetAttribute("id") + "\"", false, "Element Not Clickable");
            }
        }


        

        public static void VerifyHelpText(IWebDriver driver, string scenario, string foundText, string fieldName)
        {
            string expectedText = Library.GetValueFromExcel("MCTMessages", scenario, "Help Text");
            if (foundText.Equals(expectedText))
            {
                ReportBuilder.ArrayBuilder("Verified \"" + fieldName + "\" displayed expected help text", true, "Verify Help Text");
            }
            else
            {
                string report = "Expected \"" + fieldName + "\" to display help text: \"" + expectedText + "\" but found \"" + foundText + "\" instead";
                ReportBuilder.ArrayBuilder(report, false, "Verify Help Text");
            }
        }
        public static void VerifyFieldValue(IWebDriver driver, string expectedValue, By fieldPath, string fieldName)
        {
            string foundValue = GetInputBoxText(driver, fieldPath);

            if (foundValue == expectedValue)
            {
                ReportBuilder.ArrayBuilder("Verified " + fieldName + " has expected value: " + expectedValue, true, "Verify Field Value");
            }
            else
            {
                string report = string.Format("Expected \"{0}\" to be: \"{1}\" but found \"{2}\" instead", fieldName, expectedValue, foundValue);
                ReportBuilder.ArrayBuilder(report, false, "Verify Field Value");
            }
        }

        
        public static void SaveImage(IWebDriver driver, string scenario, By imagePath)
        {
            IWebElement image = driver.FindElement(imagePath);
            SaveImage(driver, scenario, image);
        }
        /// <summary>
        /// Saves the image of the size and location of the IWebElement to the current directory.
        /// </summary>
        public static void SaveImage(IWebDriver driver, string scenario, IWebElement image)
        {
            string path = Library.GetPath("MCTImages");

            Library.LoadVariables(path, scenario);

            string saveLocation = Directory.GetCurrentDirectory();
            string uncroppedFilePath = $"{saveLocation}\\{Library.dict["Target Image Temp"]}";
            string filePath = $"{saveLocation}\\{Library.dict["Target Image"]}";

            Library.ClearVariables();

            Point imageLocation = image.Location;

            Bitmap uncroppedBitmap = new Bitmap(uncroppedFilePath);
            Rectangle cropArea = new Rectangle(imageLocation, image.Size);
            Bitmap croppedBitmap = CropBitmap(uncroppedBitmap, cropArea);
            croppedBitmap.Save(filePath);
        }

        /// <param name="bitmap">Bitmap of screenshot that contains desired image</param>
        /// <param name="cropArea">Location and Size of desired image</param>
        /// <returns>bitmap of specified section of original bitmap</returns>
        public static Bitmap CropBitmap(Bitmap bitmap, Rectangle cropArea)
        {
            return bitmap.Clone(cropArea, bitmap.PixelFormat);
        }


        public static void SaveChildModuleIcon(IWebDriver driver, By childModuleButton, string scenario, string moduleName, bool isOriginalImage)
        {
            IWebElement childModuleButtonParent = driver.FindElement(childModuleButton).FindElement(new CommonElements().parent);

            if (isOriginalImage) { SaveImage(driver, scenario + moduleName + "Original", childModuleButtonParent); }
            else { SaveImage(driver, scenario + moduleName + "Updated", childModuleButtonParent); }
        }


        public static void VerifyIconImageUpdated(IWebDriver driver, string scenario, string iconName)
        {
            if (!Library.CompareImages(driver, scenario + "Original", scenario + "Updated"))
            {
                ReportBuilder.ArrayBuilder("Verified " + iconName + " icon was successfully updated", true, "Update Icon");
            }
            else
            {
                ReportBuilder.ArrayBuilder(iconName + " icon was not updated", false, "Update Icon");
                Library.TakeScreenShot(driver, scenario);
            }
        }
        //#endregion


        public static void SetSupportIndexedDBFalse(IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("Model.SupportIndexedDB=false");
        }

        /// <summary>
        /// Executes Javascript to display a tooltip of x-y coordinates in (x, y) form when cursor stops moving
        /// </summary>
        public static void EnableMouseoverCoordinates(IWebDriver driver)
        {
            string script =
            "document.onmousemove = function(e)" +
                    "{ " +
                        "var x = e.pageX;" +
                        "var y = e.pageY;" +
                        "e.target.title = \"(\" + x + \", \" + y + \")\";" +
                    "};";

            ((IJavaScriptExecutor)driver).ExecuteScript(script);
        }

        //used for debugging a By to check if the correct element was found
        /// <summary>
        /// Finds an element using <paramref name="locator"/>, outlines it in red, and returns it.
        /// </summary>
        public static IWebElement HighlightElement(IWebDriver driver, By locator)
        {
            IWebElement target = driver.FindElement(locator);
            IWebElement found = (IWebElement)((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.border='5px dotted red'; return arguments[0];", target);
            return found;
        }
        //used for debugging to check if the correct element was found
        /// <summary>
        /// Finds an element using <paramref name="locator"/>, outlines it in red, and returns it.
        /// </summary>
        public static IWebElement HighlightElement(IWebDriver driver, IWebElement element)
        {
            IWebElement found = (IWebElement)((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.border='5px dotted red'; return arguments[0];", element);
            return found;
        }



    }
}
