using System;
using System.Collections.Generic;
using System.Linq;
using Utils.PersonFactory.Seeds;

namespace Utils.PersonFactory
{
    public class Person
    {
        Random random = new Random();
        private List<Address> addresses = new List<Address>();
        private List<Phone> phones = new List<Phone>();
        private List<Email> emails = new List<Email>();
        private bool isPrimary = false;
        private string title { get; set; }
        private string firstName;
        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value.First().ToString().ToUpper() + value.Substring(1).ToLower();
            }
        }
        private String middleName { get; set; }
        private string lastName;
        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value.First().ToString().ToUpper() + value.Substring(1).ToLower();
            }
        }
        private String suffix { get; set; }
        private String nickname { get; set; }
        private String birthDate { get; set; }
        private string age;
        public string Age
        {
            get
            {
                if (int.Parse(this.age) < 0)
                {
                    return "0";
                }
                return this.age;
            }
            set
            {
                age = value;
            }
        }
        private string dlnumber;
        public string DlNumber
        {
            get
            {
                return dlnumber;
            }
            set
            {
                dlnumber = value;
            }
        }
        private String username { get; set; }
        private String password { get; set; }
        private bool deceased = false;
        /**
         * Upon instantiation, generate name guest data, along with
         * address, phone, and email data
         *
         * @author Justin Phlegar
         * @version 11/28/2015 Justin Phlegar - original
         */
        public Person()
        {
            populateSeededData();
            if (int.Parse(this.age) <= 18)
            {
                this.age = "45";
                //setAge("45");
                this.birthDate = "1970-01-14";
                //setBirthDate("1970-01-14");
            }
        }
        public String getFullName()
        {
            return $"{firstName} {lastName}";
        }
        public string getLastName()
        {
            return lastName;
        }
        public string getFirstName()
        {
            return firstName;
        }
        public void setFirstName()
        {
            this.firstName = firstName.First().ToString().ToUpper() + firstName.Substring(1).ToLower();
        }
        public bool isChild()
        {
            return (int.Parse(this.age) <= 18);
        }
        /**
         * Associate a new Email to the guest using preset data
         *
         * @author Justin Phlegar
         * @version 11/28/2015 Justin Phlegar - original
         */
        public void addEmail(Email email)
        {
            emails.Add(email);
        }
        public void addEmail()
        {
            emails.Add(new Email());
        }
        /**
         * Associate a new Phone to the guest using random data. Will be
         * marked not a primary
         *
         * @author Justin Phlegar
         * @version 11/28/2015 Justin Phlegar - original
         */
        public void addPhone()
        {
            phones.Add(new Phone());
        }

        /**
         * Associate a new Phone to the guest using preset data
         *
         * @author Justin Phlegar
         * @version 11/28/2015 Justin Phlegar - original
         */
        public void addPhone(Phone phone)
        {
            phones.Add(phone);
        }
        /**
         * Return the phone number marked as Primary
         *
         * @author Justin Phlegar
         * @version 11/28/2015 Justin Phlegar - original
         * @return the Guest's primary Phone Number
         */
        public void AddAddress(Address address)
        {
            addresses.Add(address);
        }
        public Phone primaryPhone()
        {
            Phone primaryPhone = null;

            foreach (Phone phone in phones)
            {
                if (phone.IsPrimary())
                {
                    primaryPhone = phone;
                }
            }

            return primaryPhone;
        }
        public Email primaryEmail()
        {
            Email primaryEmail = null;

            foreach (Email email in emails)
            {
                if (email.IsPrimary())
                {
                    primaryEmail = email;
                }
            }
            return primaryEmail;
        }

        protected void populateSeededData()
        {
            bool isMale = (random.Next(0, 1) < .5);

            if (isMale)
            {
                this.title = "Mr.";
                firstName = (MaleFirstNames.getFirstName()); ;
            }
            else
            {
                this.title = "Ms.";
                firstName = (FemaleFirstNames.getFirstName());
            }

            this.middleName = "Automation";

            lastName = (LastNames.getLastName());

            //SimpleDateFormat format = new SimpleDateFormat(
            //    "yyyy-MM-dd'T'hh:mm:ss'Z'", Locale.US);
            //Date date = Randomness.randomDate();
            //String dateOfBirth = format.format(date);

            DateTime dob = RandomDate();
            //            dob.setTime(date);
            DateTime today = DateTime.Now;
            int convertedAge = int.Parse(DateTime.Now.Year.ToString()) - int.Parse(dob.Year.ToString());
            if (today.DayOfYear <= dob.DayOfYear)
            {
                convertedAge--;
            }

            if (convertedAge < 0)
            {
                convertedAge = 0;
            }
            this.age = convertedAge.ToString();
            this.birthDate = dob.ToShortDateString();
            //DateTimeConversion.convert(dateOfBirth,yyyy-MM-dd'T'hh:mm:ss'Z'", "yyyy-MM-dd");
            dlnumber = new Random().Next(11111111, 99999999).ToString();
            addPhone(new Phone());
            addEmail(new Email());
            emails[0].setPrimary(true);
            emails[0].setEmail($"{firstName}.{lastName}@AutomatedTesting.com");

            phones[0].setPrimary(true);
            //AddAddress(new Address());
        }
        private Random gen = new Random();
        DateTime RandomDate()
        {
            DateTime date = DateTime.Now.AddYears(-85);
            int startyear = int.Parse(date.Year.ToString());
            DateTime start = new DateTime(startyear, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }
        public bool isNew()
        {
            return SQLHandler.GetDatabaseInt(
                $@"SELECT COUNT( * ) as ""Count"" FROM ALARM_LICENSES WHERE lastName = '{lastName}' and firstName = '{firstName}';", "Count", CommonTestSettings.dbHost, CommonTestSettings.dbName) == 0;
        }
        public Person getNewFullName(Person person)
        {
            do
            {
                person.firstName = MaleFirstNames.getFirstName();
                person.lastName = LastNames.getLastName();
            } while (!person.isNew());
            return person;
        }

    }
}
