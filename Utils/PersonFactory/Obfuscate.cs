using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;

namespace Utils.PersonFactory
{
    class Obfuscate
    {
        public string dbName = "CW-QA-TX-DA";
        public string dbHost = "CRYWOLF-PREPROD";

        [Test,Explicit]
        public void ObfuscateAlarm_Licenses()
        {
            #region Residential
            string sqlstatement = "select AlarmNo from ALARM_LICENSES where locationType = 'Residential' and not updatedBy = 'testdata' order by AlarmNo;";
            //string sqlstatement = "select AlarmNo from ALARM_LICENSES where locationType = 'Residential' and not phone1 like '336%' and email not like '%@AutomatedTesting.com' and not updatedBy = 'testdata' order by AlarmNo;";
            List<string> columnNames = new List<string>
            {
                "lastName",
                "firstName",
                "phone1",
                "phone2",
                "email"
            };
            
            List<string> alarmNumbers = SQLHandler.GetStringResults(sqlstatement, dbHost, dbName);

            //Update lastName,firstName,phone1 (if not blank or NULL),phone2 (if not blank or NULL),email (if not blank or NULL)
            foreach (var alarmno in alarmNumbers)
            {
                Person person = new Person();
                sqlstatement = $"select lastName,firstName,phone1,phone2,email from ALARM_LICENSES where AlarmNo = '{alarmno}' and locationType = 'Residential';";
                List<string> dbValues = SQLHandler.GetDatabaseValues(sqlstatement, columnNames, dbHost, dbName);
                sqlstatement = $"update ALARM_LICENSES set";
                for (int i = 0; i < columnNames.Count; i++)
                {

                    switch (columnNames[i])
                    {
                        case "lastName":
                            //Update last name
                            string newLast = person.getLastName().ToUpper();
                            sqlstatement = sqlstatement + $" {columnNames[i]} = '{newLast}',";
                            break;
                        case "firstName":
                            //Update first name
                            string newFirst = person.getFirstName().ToUpper();
                            sqlstatement = sqlstatement + $"{columnNames[i]} = '{newFirst}',";
                            break;
                        case "phone1":
                        case "phone2":
                            string newPhone;
                            if ((dbValues[i] != "") && (dbValues[i] != null))
                            {
                                newPhone = person.primaryPhone().Number().ToString();
                            }
                            else
                            {
                                newPhone = "";
                            }
                            sqlstatement = sqlstatement + $"{columnNames[i]} = '{newPhone}',";
                            break;
                        case "email":
                            string newEmail;
                            if ((dbValues[i] != "") && (dbValues[i] != null))
                            {
                                newEmail = person.primaryEmail().GetEmail();
                            }
                            else
                            {
                                newEmail = "";
                            }
                            sqlstatement = sqlstatement + $"{columnNames[i]} = '{newEmail}'";
                            break;
                        default:
                            break;
                    }
                }
                sqlstatement += $",updatedBy = 'testdata' where AlarmNo = '{alarmno}';";
                SQLHandler.UpdateDatabaseValue(sqlstatement, dbHost, dbName);
                person = null;
                Thread.Sleep(10);
            }
            #endregion

            #region Commercial
            sqlstatement = "SELECT AlarmNo " +
                "FROM ALARM_LICENSES " +
                "WHERE locationType = 'Commercial' " +
                "AND NOT updatedBy = 'testdata' " +
                "ORDER BY AlarmNo;";

            alarmNumbers = SQLHandler.GetStringResults(sqlstatement, dbHost, dbName);

            foreach (var alarmno in alarmNumbers)
            {
                Person person = new Person();

                sqlstatement = $"select lastName,firstName,phone1,phone2,email from ALARM_LICENSES where AlarmNo = '{alarmno}' and locationType = 'Commercial';";
                List<string> dbValues = SQLHandler.GetDatabaseValues(sqlstatement, columnNames, dbHost, dbName);

                sqlstatement = $"update ALARM_LICENSES set ";

                for (int i = 0; i < columnNames.Count; i++)
                {

                    switch (columnNames[i])
                    {
                        case "firstName":
                            //Update first name
                            string newFirst = string.Empty;
                            sqlstatement += $"{columnNames[i]} = '{newFirst}',";
                            break;
                        case "phone1":
                        case "phone2":
                            string newPhone;
                            if ((dbValues[i] != "") && (dbValues[i] != null))
                            {
                                newPhone = person.primaryPhone().Number().ToString();
                            }
                            else
                            {
                                newPhone = "";
                            }
                            sqlstatement += $"{columnNames[i]} = '{newPhone}',";
                            break;
                        case "email":
                            string newEmail;
                            if ((dbValues[i] != "") && (dbValues[i] != null))
                            {
                                newEmail = person.primaryEmail().GetEmail();
                            }
                            else
                            {
                                newEmail = "";
                            }
                            sqlstatement += $"{columnNames[i]} = '{newEmail}'";
                            break;
                        default:
                            break;
                    }
                }

                sqlstatement += $",updatedBy = 'testdata' where AlarmNo = '{alarmno}';";
                SQLHandler.UpdateDatabaseValue(sqlstatement, dbHost, dbName);
                person = null;
                Thread.Sleep(10);
            }
            #endregion

            #region All Others
            sqlstatement = "SELECT AlarmNo " +
                "FROM ALARM_LICENSES " +
                "WHERE NOT updatedBy = 'testdata' " +
                "ORDER BY AlarmNo;";

            alarmNumbers = SQLHandler.GetStringResults(sqlstatement, dbHost, dbName);

            foreach (var alarmno in alarmNumbers)
            {
                Person person = new Person();

                sqlstatement = $"select lastName,firstName,phone1,phone2,email from ALARM_LICENSES where AlarmNo = '{alarmno}';";
                
                List<string> dbValues = SQLHandler.GetDatabaseValues(sqlstatement, columnNames, dbHost, dbName);

                sqlstatement = $"update ALARM_LICENSES set ";

                for (int i = 0; i < columnNames.Count; i++)
                {

                    switch (columnNames[i])
                    {
                        
                        case "firstName":
                            //Update first name
                            string newFirst = string.Empty;
                            sqlstatement += $"{columnNames[i]} = '{newFirst}',";
                            break;
                        case "phone1":
                        case "phone2":
                            string newPhone;
                            if ((dbValues[i] != "") && (dbValues[i] != null))
                            {
                                newPhone = person.primaryPhone().Number().ToString();
                            }
                            else
                            {
                                newPhone = "";
                            }
                            sqlstatement += $"{columnNames[i]} = '{newPhone}',";
                            break;
                        case "email":
                            string newEmail;
                            if ((dbValues[i] != "") && (dbValues[i] != null))
                            {
                                newEmail = person.primaryEmail().GetEmail();
                            }
                            else
                            {
                                newEmail = "";
                            }
                            sqlstatement += $"{columnNames[i]} = '{newEmail}'";
                            break;
                        default:
                            break;
                    }
                }

                sqlstatement += $",updatedBy = 'testdata' where AlarmNo = '{alarmno}';";
                SQLHandler.UpdateDatabaseValue(sqlstatement, dbHost, dbName);
                person = null;
                Thread.Sleep(10);
            }
            #endregion
        }

        [Test,Explicit]
        public void ObfuscateAlarm_RP()
        {
            string sqlstatement = "select person_id from ALARM_RELATED_NAMES where not updatedBy = 'testdata';";
            List<string> columnNames = new List<string>
            {
                "lastName",
                "firstName",
                "phone1",
                "phone2",
                "phone3",
                "phone4",
                "email"
            };
            
            List<int> personNumbers = SQLHandler.GetIntResults(sqlstatement, "person_id", dbHost, dbName);

            //Update lastName,firstName,phone1 (if not blank or NULL),phone2 (if not blank or NULL),email (if not blank or NULL)
            foreach (var personNumber in personNumbers)
            {
                Person person = new Person();
                sqlstatement = $"select lastName,firstName,phone1,phone2,phone3,phone4,email from ALARM_RELATED_NAMES where person_id = '{personNumber}';";
                List<string> dbValues = SQLHandler.GetDatabaseValues(sqlstatement, columnNames, dbHost, dbName);
                sqlstatement = $"update ALARM_RELATED_NAMES set";
                for (int i = 0; i < columnNames.Count; i++)
                {
                    switch (columnNames[i])
                    {
                        case "lastName":
                            //Update last name
                            string newLast = person.getLastName().ToUpper();
                            sqlstatement += $" {columnNames[i]} = '{newLast}',";
                            break;
                        case "firstName":
                            //Update first name
                            string newFirst = person.getFirstName().ToUpper();
                            sqlstatement += $" {columnNames[i]} = '{newFirst}',";
                            break;
                        case "phone1":
                        case "phone2":
                        case "phone3":
                        case "phone4":
                            string newPhone;
                            if ((dbValues[i] != "") && (dbValues[i] != null))
                            {
                                newPhone = person.primaryPhone().Number().ToString();
                            }
                            else
                            {
                                newPhone = "";
                            }
                            sqlstatement += $" {columnNames[i]} = '{newPhone}',";
                            break;
                        case "email":
                            string newEmail;
                            if ((dbValues[i] != "") && (dbValues[i] != null))
                            {
                                newEmail = person.primaryEmail().GetEmail();
                            }
                            else
                            {
                                newEmail = "";
                            }
                            sqlstatement += $" {columnNames[i]} = '{newEmail}'";
                            break;
                        default:
                            break;
                    }
                }

                sqlstatement += $",updatedBy = 'testdata' where person_id = '{personNumber}';";
                SQLHandler.UpdateDatabaseValue(sqlstatement, dbHost, dbName);
                person = null;
                Thread.Sleep(15);
            }
        }
    }
}
