using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Libraries
{
    static class Extensions
    {
        #region Menus
        public static WindowsElement SelectMenuItemByName(this WindowsElement windowsElement, string itemName, WindowsDriver<WindowsElement> windowsDriver)
        {
            windowsElement.Click();

            var list = windowsElement.FindElementsByName(itemName);

            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.Text == itemName)
                    {
                        windowsDriver.Mouse.Click(item.Coordinates);
                    }
                }
            }
            else
            {
                throw new NotFoundException($"{itemName} was not found.");
            }

            return windowsElement;
        }
        #endregion

        
        public static WindowsElement SelectListItem(this WindowsElement windowsElement, string itemName)
        {
            return SelectItem(windowsElement, itemName, "ListItem");
        }
        public static WindowsElement SelectListItemNoClick(this WindowsElement windowsElement, string itemName)
        {
            return SelectItemNoClick(windowsElement, itemName, "ListItem");
        }
        public static WindowsElement SelectItemNoClick(this WindowsElement windowsElement, string itemName,string itemType)
        {
            WindowsDriver<WindowsElement> windowsDriver = (WindowsDriver<WindowsElement>)windowsElement.WrappedDriver;

            var list = windowsElement.FindElementsByTagName(itemType);

            foreach (var item in list)
            {
                if (item.Text == itemName)
                {
                    windowsDriver.Mouse.MouseMove(item.Coordinates);
                    windowsDriver.Mouse.Click(item.Coordinates);
                }
            }
            return windowsElement;
        }
        private static WindowsElement SelectItem(this WindowsElement windowsElement, string itemName, string itemType)
        {
            WindowsDriver<WindowsElement> windowsDriver = (WindowsDriver<WindowsElement>)windowsElement.WrappedDriver;
            windowsElement.Click();
            
            var list = windowsElement.FindElementsByTagName(itemType);

            foreach (var item in list)
            {
                if (item.Text.ToLower() == itemName.ToLower())
                {
                    windowsDriver.Mouse.MouseMove(item.Coordinates);
                    windowsDriver.Mouse.Click(item.Coordinates);
                }
            }
            return windowsElement;
        }

        public static WindowsElement GetListItemsNoClick(this WindowsElement windowsElement, out List<string> vs)
        {
            vs = new List<string>();
            //windowsElement.Click();
            var list = windowsElement.FindElementsByTagName("ListItem");

            foreach (var item in list)
            {
                vs.Add(item.Text);
            }
            return windowsElement;
        }

        public static WindowsElement GetListItems(this WindowsElement windowsElement, out List<string> vs)
        {
            vs = new List<string>();
            windowsElement.Click();
            var list = windowsElement.FindElementsByTagName("ListItem");
            
            foreach (var item in list)
            {
                vs.Add(item.Text);
            }
            return windowsElement;
        }

        #region FlexGrid Extensions
        public static WindowsElement ActivateGridDropdown(this WindowsElement windowsElement)
        {
            windowsElement.Click();
            windowsElement.SendKeys(Keys.Enter);
            windowsElement.Click();
            return windowsElement;
        }
        [Obsolete]
        public static List<WindowsElement> GetRowCollection(this WindowsElement windowsElement)
        {
            var vs = windowsElement.FindElementsByXPath($"//Table/child::*").Count == 0 ?
                windowsElement.FindElementsByXPath($"//List/child::*") : windowsElement.FindElementsByXPath($"//Table/child::*");

            List<WindowsElement> rowCollection = new List<WindowsElement>();
            foreach (var item in vs)
            {
                rowCollection.Add((WindowsElement)item);
            }

            return rowCollection;
        }
        [Obsolete]
        public static int GetRowCount(this WindowsElement windowsElement)
        {
            int rows = GetRowCollection(windowsElement).Count();
            return rows;
        }
        [Obsolete]
        public static List<WindowsElement> GetColumnCollection(this WindowsElement windowsElement, WindowsElement row)
        {
            IReadOnlyCollection<AppiumWebElement> vs;
            if (windowsElement.TagName.Contains("List"))
            {
                vs = row.FindElementsByTagName("ListItem").Count == 0 ? row.FindElementsByTagName("HeaderItem") : row.FindElementsByTagName("ListItem");
            }
            else
            {
                vs = row.FindElementsByTagName("DataItem").Count == 0 ? row.FindElementsByTagName("Header") : row.FindElementsByTagName("DataItem");
            }

            //if (vs.Count == 0)
            //{
            //    vs = row.FindElementsByTagName("Header").Count == ;
            //}

            List<WindowsElement> columnCollection = new List<WindowsElement>();
            foreach (var item in vs)
            {
                columnCollection.Add((WindowsElement)item);
            }
            return columnCollection;
        }
        [Obsolete]
        public static int GetColumnCount(this WindowsElement windowsElement, int row)
        {
            int columns = GetColumnCollection(windowsElement, GetRowCollection(windowsElement)[row]).Count;
            return columns;
        }
        public static WindowsElement GetCell(this WindowsElement windowsElement, int row, int column)
        {
            WindowsElement cell;
            string parent = string.Empty;
            if (windowsElement.TagName.Contains("List"))
            {
                parent = "List";
            }
            else
            {
                parent = "Table";
            }
            cell = (WindowsElement)windowsElement.FindElementByXPath($"//{parent}/child::*[{row}]/child::*[{column}]");
            return cell;
        }
        [Obsolete]
        public static void ClickCell(this WindowsElement windowsElement, int row, int column)
        {
            GetCell(windowsElement, row, column).Click();
        }
        [Obsolete]
        public static string GetCellData(this WindowsElement windowsElement,int row, int column)
        {
            string data = GetCell(windowsElement, row, column).Text;
            return data;
        }
        public static int GetRowWithCellText(this WindowsElement windowsElement, string text)
        {
            int row = GetRowWithCellText(windowsElement, text, -1, 1, true);
            return row;
        }
        [Obsolete]
        public static int GetRowWithCellText(this WindowsElement windowsElement, string text, int columnPosition)
        {
            int row = GetRowWithCellText(windowsElement, text, columnPosition, 1, true);
            return row;
        }
        [Obsolete]
        public static int GetRowWithCellText(this WindowsElement windowsElement, string text, int columnPosition, int startRow)
        {
            int row = GetRowWithCellText(windowsElement, text, columnPosition, startRow, true);
            return row;
        }
        [Obsolete]
        public static int GetRowWithCellText(this WindowsElement windowsElement, string text, int columnPosition, int startRow, bool exact)
        {
            int currentRow = 1;
            int rowFound = 0;

            List<WindowsElement> rowCollection = GetRowCollection(windowsElement);
            foreach (var rowElement in rowCollection)
            {
                if (startRow > currentRow)
                {
                    currentRow++;
                }
                else if (currentRow <= rowCollection.Count)
                {
                    if (columnPosition == -1)
                    {
                        foreach (WindowsElement cell in GetColumnCollection(windowsElement, rowElement))
                        {
                            if (exact)
                            {
                                if (cell.Text.Trim().Equals(text))
                                {
                                    return currentRow;
                                }
                            }
                            else
                            {
                                if (cell.Text.ToLower().Trim().Contains(text.ToLower()))
                                {
                                    return currentRow;
                                }
                            
                            }
                        }
                    }
                    else
                    {
                        WindowsElement cell = GetCell(windowsElement, currentRow, columnPosition);
                        if (exact && cell.Text.Trim().Equals(text))
                        {
                            return currentRow;
                        }
                        else 
                        {
                            if (cell.Text.ToLower().Trim().Contains(text.ToLower()))
                            {
                                return currentRow;
                            }
                                
                        }
                    }
                }
                currentRow++;
            }
            return rowFound;
        }
        public static int GetColumnWithCellText(this WindowsElement windowsElement, string text)
        {
            int column = GetColumnWithCellText(windowsElement, text, 1);
            return column;
        }

        public static int GetColumnWithCellText(this WindowsElement windowsElement, string text, int rowPosition)
        {
            int currentColumn = 1;
            List<WindowsElement> columns = GetColumnCollection(windowsElement, GetRowCollection(windowsElement)[rowPosition - 1]);
            foreach (WindowsElement cell in columns)
            {
                if (currentColumn <= columns.Count)
                {
                    if (cell.Text.Trim().Equals(text))
                    {
                        return currentColumn;
                    }
                }
                currentColumn++;
            }
            return 0;
        }
        #endregion

    }
}
