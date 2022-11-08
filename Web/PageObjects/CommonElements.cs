using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using Utils;
using Web.Libraries;

namespace Web.PageObjects
{
    public class CommonElements //: Library
    {
        //ReportBuilder report = ReportBuilder;
        string reportAdd;
        bool status = true;
        string categoryAdd;

        #region Page Element Locators
        public By parent = By.XPath("..");
        public static By parentLocator = By.XPath("..");
        public By grandParent = By.XPath("../..");
        public static By grandParentLocator = By.XPath("../..");
        public By greatGrandParent = By.XPath("../../..");
        public static By greatGrandParentLocator = By.XPath("../../..");
        public By disabledElement = By.XPath("./*[contains(@class, 'disabled')]");
        #endregion

        #region Methods
        public void SetImplicitWait(IWebDriver driver, int waitTime)
        {
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(waitTime));
        }

        /// <summary>
        /// Takes an IWebElement path and returns its value (which is its text if it's an input box)
        /// </summary>
        public string GetInputBoxText(IWebDriver driver, By by)
        {
            IWebElement inputBox = driver.FindElement(by);
            return inputBox.GetAttribute("value");
        }

        /// <summary>
        /// Iterates through all passed fields and fills them with all passed values
        /// </summary>
        public void FillMultipleTextFields(IWebDriver driver, Dictionary<By, string> elems)
        {
            foreach (KeyValuePair<By, string> pair in elems)
            {
                string input = pair.Value;
                if (!string.IsNullOrWhiteSpace(input))
                {
                    IWebElement textField = driver.FindElement(pair.Key);
                    textField.Clear();
                    SendKeyByKey(textField, input);
                    textField.SendKeys(Keys.Enter);
                    Thread.Sleep(50);
                }
            }
        }

        public void FillMultipleTextFields(IWebDriver driver, List<Tuple<By, string, bool>> tuples)
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
        public Dictionary<By, string> ReadMultipleTextFields(IWebDriver driver, Dictionary<By, string> elems)
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
                    reportAdd = "Could not find element with path: \"" + fieldPath + "\"";
                    ReportBuilder.ArrayBuilder(reportAdd, false, "Read Text Field");
                }
            }
            SetImplicitWait(driver, 30);
            return foundTextBoxes;
        }

        /// <summary>
        /// Builds a report on the equivalence of two dictionaries (built from textboxes and their values)
        /// </summary>
        /// <param name="expected">Dictionary built from excel sheet values</param>
        /// <param name="found">Dictionary built from getting values from input boxes</param>
        public void VerifyDictionariesAreEqual(IWebDriver driver, Dictionary<By, string> expected, Dictionary<By, string> found)
        {
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

                categoryAdd = "Verify Input Field Unchanged";
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

        /// <summary>
        /// Finds the textfield element path passed and fills it with the string passed
        /// </summary>
        public void FillTextField(IWebDriver driver, By by, string input)
        {
            IWebElement textField = driver.FindElement(by);
            textField.Clear();
            textField.SendKeys(input);
            textField.SendKeys(Keys.Tab);
        }

        public void FillPicklist(IWebDriver driver, By by, string input)
        {
            IWebElement textField = driver.FindElement(by);
            textField.Clear();
            SendKeyByKey(textField, input);
            textField.SendKeys(Keys.Tab);
        }

        /// <summary>
        /// Sends keys more slowly so that picklists don't clear input
        /// </summary>
        public void SendKeyByKey(IWebElement textField, string input, IWebDriver driver = null)
        {
            textField.Clear();

            // Simulates user clicking into field before entering information
            try { textField.Click(); }
            catch (InvalidOperationException) // Popup is likely in the way
            {
                if (!driver.Equals(null))
                {
                    try
                    {
                        Actions actions = new Actions(driver);
                        int width = textField.Size.Width;
                        int height = textField.Size.Height;

                        actions.MoveToElement(textField, width / 2, height / 2);
                    }
                    catch (InvalidOperationException)
                    {
                        ReportBuilder.ArrayBuilder("Cannot click on center of element with text \"" + input + "\"", false, "Element Not Clickable");
                    }
                }
                else
                {
                    ReportBuilder.ArrayBuilder("Cannot click on element with text \"" + input + "\"", false, "Element Not Clickable");
                }

            }

            Char[] inputArray = input.ToCharArray();
            foreach (Char letter in inputArray)
            {
                textField.SendKeys(letter.ToString());
                Thread.Sleep(25);
            }
        }

        public void AssertNavigation(IWebDriver driver, By webTitle, string pageTitle)
        {
            IWebElement header = driver.FindElement(webTitle);
            if (header.Displayed)
                ReportBuilder.ArrayBuilder($"Found and navigated to the \"{pageTitle}\" page", true, "Page Navigation");
            else
                ReportBuilder.ArrayBuilder($"Failed to navigate to the \"{pageTitle}\" page", false, "Page Navigation");
        }

        /// <summary>
        /// Repeatedly attempts to find an element until its specified timeout
        /// </summary>
        public void WaitFor(IWebDriver driver, By expectedElementPath)
        {

            Thread.Sleep(2000); // so it doesn't go too quickly
            int waitTime = 10;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitTime));

            try { wait.Until(d => d.FindElement(expectedElementPath)); }
            catch (WebDriverTimeoutException)
            {
                reportAdd = "Waited for " + waitTime + " seconds, failed to find element " + expectedElementPath;
                categoryAdd = "Wait Exception";
                ReportBuilder.ArrayBuilder(reportAdd, false, categoryAdd);
            }
        }

        /// <summary>
        /// Waits for object
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        public void WaitFor(IWebDriver driver, IWebElement webElement)
        {
            Thread.Sleep(2000); // so it doesn't go too quickly
            int waitTime = 10;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitTime));

            try { wait.Until(d => webElement.Enabled); }
            catch (WebDriverTimeoutException)
            {
                reportAdd = $"Waited for {waitTime} seconds, failed to find element {webElement.Text}";
                categoryAdd = "Wait Exception";
                ReportBuilder.ArrayBuilder(reportAdd, false, categoryAdd);
            }

            reportAdd = $"Found element {webElement.Text} after waiting for {waitTime} seconds.";
        }     

        public void SelectPicklistItemByIndex(IWebDriver driver, By elementPath, int index)
        {
            IWebElement picklist = driver.FindElement(elementPath);
            SelectElement item = new SelectElement(picklist);

            item.SelectByIndex(index);
        }     

        public static void VerifyFieldDisabled(IWebDriver driver, By fieldPath, string fieldName)
        {
            if (DriverOperations.IsElementDisabled(driver, fieldPath))
            {
                ReportBuilder.ArrayBuilder("Verified " + fieldName + " field is disabled", true, "Verify Field Disabled");
            }
            else
            {
                ReportBuilder.ArrayBuilder(fieldName + " field is not disabled", false, "Verify Field Disabled");
            }
        }

        public static void VerifyFieldPopulated(IWebDriver driver, By fieldPath, string fieldName)
        {
            if (DriverOperations.IsFieldPopulated(driver, fieldPath))
            {
                ReportBuilder.ArrayBuilder("Verified " + fieldName + " field is populated", true, "Verify Field Populated");
            }
            else
            {
                ReportBuilder.ArrayBuilder(fieldName + " field is not populated", false, "Verify FIeld Populated");
            }
        }

        public static void ClickView(IWebDriver driver)
        {
            DriverOperations.ClickButton(driver, By.Id("btn_view"));
            DriverOperations.WaitForPageLoad(driver);
        }

        public static void ClickSave(IWebDriver driver)
        {
            DriverOperations.ClickButton(driver, By.Id("btn_save"));
            DriverOperations.WaitForPageLoad(driver);
        }

        public static void ClickExit(IWebDriver driver)
        {
            DriverOperations.ClickButton(driver, By.Id("btn_exit"));
            DriverOperations.WaitForPageLoad(driver);
        }

        public static void ClickExitFooter(IWebDriver driver)
        {
            DriverOperations.ClickButton(driver, By.Id("footerButtonGroup_exit"));
            DriverOperations.WaitForPageLoad(driver);
        }

        public static void CloseAllFooters(IWebDriver driver)
        {
            while (DriverOperations.IsElementPresent(driver, By.Id("footerButtonGroup_exit")))
            {
                DriverOperations.ClickButton(driver, By.Id("footerButtonGroup_exit"));
                DriverOperations.WaitForPageLoad(driver);
            }
        }

        public static void ClickAdd(IWebDriver driver)
        {
            DriverOperations.ClickButton(driver, By.Id("btn_add"));
            DriverOperations.WaitForPageLoad(driver);
        }

        public static void ClickAddIfEnabled(IWebDriver driver)
        {
            if (!DriverOperations.IsElementDisabled(driver, By.Id("btn_add")))
            {
                DriverOperations.ClickButton(driver, By.Id("btn_add"));
            }
        }

        public static void ClickDuplicate(IWebDriver driver)
        {
            DriverOperations.ClickButton(driver, By.Id("footerFlyout_1"));
            DriverOperations.ClickButton(driver, By.Id("btn_duplicate"));
        }

        public static void ClickSaveIfEnabled(IWebDriver driver)
        {
            if (!DriverOperations.IsElementDisabled(driver, By.Id("btn_save")))
            {
                DriverOperations.ClickButton(driver, By.Id("btn_save"));
            }
        }

        public static void ClickDelete(IWebDriver driver)
        {
            DriverOperations.ClickButton(driver, By.Id("btn_delete"));
            DriverOperations.WaitForPageLoad(driver);
        }

        public static void ClickUse(IWebDriver driver)
        {
            DriverOperations.ClickButton(driver, By.Id("btn_use"));
            DriverOperations.WaitForPageLoad(driver);
        }

        public static void OpenPicklistAltDown(IWebDriver driver, By picklistLocator)
        {
            IWebElement picklist = driver.FindElement(picklistLocator);

            new Actions(driver).KeyDown(Keys.LeftAlt);
            new Actions(driver).SendKeys(picklist, Keys.Down);
            new Actions(driver).KeyUp(Keys.LeftAlt);
        }

        public static void OpenPicklistAltUp(IWebDriver driver, By picklistLocator)
        {
            IWebElement picklist = driver.FindElement(picklistLocator);

            new Actions(driver).KeyDown(Keys.LeftAlt).Perform();
            new Actions(driver).SendKeys(picklist, Keys.Up).Perform();
            new Actions(driver).KeyUp(Keys.LeftAlt).Perform();
        }

        public static void SelectHighlightedRecord(IWebDriver driver)
        {
            DriverOperations.SendEnterKey(driver);
            DriverOperations.WaitForPageLoad(driver);
        }
        #endregion
    }
}
