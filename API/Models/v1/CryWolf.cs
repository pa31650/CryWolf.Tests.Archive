using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace API.Models.v1
{
    public class CryWolf
    {
        public static string URL = CommonTestSettings.CryWolfAPI;
        public static string URLv1 = $"{URL}v1.0/";

        public static Account Account()
        {
            return new Account();
        }
        public static AccountTypes AccountTypes()
        {
            return new AccountTypes();
        }
        public static Settings Settings()
        {
            return new Settings();
        }

        public static Auth Auth()
        {
            return new Auth();
        }
        public static Refresh Refresh()
        {
            return new Refresh();
        }
        
        public static Search Search()
        {
            return new Search();
        }
        public static Invoice Invoice()
        {
            return new Invoice();
        }
        public static Payments Payments()
        {
            return new Payments();
        }
    }
}
