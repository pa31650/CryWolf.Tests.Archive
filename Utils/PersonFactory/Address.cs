using System;

namespace Utils.PersonFactory
{
    public class Address
    {
        private string state;

        private bool isPrimary = false;
        private String type = null;
        private String country = "";
        private String countryAbbv = "";
        private String address1 = "";
        private String address2 = "";

        Random random = new Random();

        public string AutoCount { get; set; }
        public string ZipCode { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public Address(bool isNew = false)
        {
            this.AutoCount = CryWolfUtil.GetGeoCodeAutoCount();
            this.ZipCode = SQLHandler.GetDatabaseValue($"select Zip from LU2_xGEOCODE where AutoCount1 = {AutoCount};", "Zip", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            this.StreetNumber = GetValidStreetNumber();

            this.StreetName = GetValidStreetName();
            this.City = SQLHandler.GetDatabaseValue($"select City from LU2_xGEOCODE where AutoCount1 = {AutoCount}", "City", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            this.State = SQLHandler.GetDatabaseValue($"select State from LU2_xGEOCODE where AutoCount1 = {AutoCount}", "State", CommonTestSettings.dbHost, CommonTestSettings.dbName);

            if ((this.StreetName.Contains("-")) || (isNew = true))
            {
                this.GetNewAddress();
            }
        }

        public bool isNew()
        {
            return SQLHandler.GetDatabaseInt(
                $@"SELECT COUNT( * ) as ""Count"" FROM ALARM_LICENSES WHERE strNum = '{StreetNumber}' and Street = '{StreetName}';", "Count", CommonTestSettings.dbHost, CommonTestSettings.dbName) == 0;
        }

        public void GetNewAddress()
        {
            do
            {
                this.StreetNumber = GetValidStreetNumber();
                this.StreetName = GetValidStreetName(); //SQLHandler.GetDatabaseValue($"select FullStreet from LU2_xGEOCODE where AutoCount1 = {AutoCount};", "FullStreet", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            } while (!this.isNew());
        }
        public Address RefreshAddress(Address address)
        {
            address.StreetName = GetValidStreetName(); //SQLHandler.GetDatabaseValue($"select FullStreet from LU2_xGEOCODE where AutoCount1 = {AutoCount};", "FullStreet", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            address.StreetNumber = GetValidStreetNumber();
            //random.Next(
            //SQLHandler.GetDatabaseInt($"select sFrom from LU2_xGEOCODE where AutoCount1 = {AutoCount}", "sFrom", CommonTestSettings.dbHost, CommonTestSettings.dbName),
            //SQLHandler.GetDatabaseInt($"select sThru from LU2_xGEOCODE where AutoCount1 = {AutoCount}", "sThru", CommonTestSettings.dbHost, CommonTestSettings.dbName)).ToString();

            return address;
        }

        public void SetCityAsDefault()
        {
            this.City = CryWolfUtil.GetDefaultCity();
            //return address;
        }
        public void SetZipAsDefault()
        {
            this.ZipCode = SQLHandler.GetDatabaseValue("select returnZip from LU2_DEFAULT;", "returnZip", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public string GetValidStreetNumber()
        {
            int isEven = SQLHandler.GetDatabaseInt($"select sEven from LU2_xGEOCODE where AutoCount1 = {AutoCount}", "sEven", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            int streetNumber;

            if (isEven == 1) //street number is even
            {

                do
                {
                    streetNumber = GetStreetNumber();
                } while ((streetNumber % 2) != 0);

            }
            else //street number is odd
            {
                do
                {
                    streetNumber = GetStreetNumber();
                } while ((streetNumber % 2) == 0);
            }

            return streetNumber.ToString();
        }
        public string GetValidStreetName()
        {
            string streetName = SQLHandler.GetDatabaseValue($"select FullStreet from LU2_xGEOCODE where AutoCount1 = {AutoCount};", "FullStreet", CommonTestSettings.dbHost, CommonTestSettings.dbName);

            return streetName;

        }
        private int GetStreetNumber()
        {
            return random.Next(
                        SQLHandler.GetDatabaseInt($"select sFrom from LU2_xGEOCODE where AutoCount1 = {AutoCount}", "sFrom", CommonTestSettings.dbHost, CommonTestSettings.dbName),
                        SQLHandler.GetDatabaseInt($"select sThru from LU2_xGEOCODE where AutoCount1 = {AutoCount}", "sThru", CommonTestSettings.dbHost, CommonTestSettings.dbName));
        }
    }
}
