using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1.AlarmCompany
{
    partial class AlarmCompany : CryWolf
    {
        public string id { get; set; }
        public string status { get; set; }
        public string agency { get; set; }
        public string name { get; set; }
        public string doingBusinessAs { get; set; }
        public int amountOutstanding { get; set; }
        public Permit permit { get; set; }
        public Address address { get; set; }
        public Phone phone { get; set; }
        public SecondaryPhone secondaryPhone { get; set; }
        public List<OtherPhone> otherPhones { get; set; }
        public DateTime noResponseFlag { get; set; }
        public DateTime revokedFlag { get; set; }
        public DateTime suspendedFlag { get; set; }
        public string comments { get; set; }
        public Contact contact { get; set; }
        public List<OtherContact> otherContacts { get; set; }
        
        public class Permit
        {
            public DateTime issued { get; set; }
            public DateTime expires { get; set; }
        }

        public class Address
        {
            public string streetNumber { get; set; }
            public string streetName { get; set; }
            public string suite { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postalCode { get; set; }
        }

        public class Phone
        {
            public string type { get; set; }
            public string number { get; set; }
            public string extension { get; set; }
        }

        public class SecondaryPhone
        {
            public string type { get; set; }
            public string number { get; set; }
            public string extension { get; set; }
        }

        public class OtherPhone
        {
            public string type { get; set; }
            public string number { get; set; }
            public string extension { get; set; }
        }

        public class Phone2
        {
            public string type { get; set; }
            public string number { get; set; }
            public string extension { get; set; }
        }

        public class OtherPhoneNumber
        {
            public string type { get; set; }
            public string number { get; set; }
            public string extension { get; set; }
        }

        public class Address2
        {
            public string streetNumber { get; set; }
            public string streetName { get; set; }
            public string suite { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postalCode { get; set; }
        }

        public class OtherAddress
        {
            public string streetNumber { get; set; }
            public string streetName { get; set; }
            public string suite { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postalCode { get; set; }
        }

        public class Contact
        {
            public string type { get; set; }
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string middleInitial { get; set; }
            public string email { get; set; }
            public Phone2 phone { get; set; }
            public List<OtherPhoneNumber> otherPhoneNumbers { get; set; }
            public Address2 address { get; set; }
            public List<OtherAddress> otherAddresses { get; set; }
        }

        public class Phone3
        {
            public string type { get; set; }
            public string number { get; set; }
            public string extension { get; set; }
        }

        public class OtherPhoneNumber2
        {
            public string type { get; set; }
            public string number { get; set; }
            public string extension { get; set; }
        }

        public class Address3
        {
            public string streetNumber { get; set; }
            public string streetName { get; set; }
            public string suite { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postalCode { get; set; }
        }

        public class OtherAddress2
        {
            public string streetNumber { get; set; }
            public string streetName { get; set; }
            public string suite { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postalCode { get; set; }
        }

        public class OtherContact
        {
            public string type { get; set; }
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string middleInitial { get; set; }
            public string email { get; set; }
            public Phone3 phone { get; set; }
            public List<OtherPhoneNumber2> otherPhoneNumbers { get; set; }
            public Address3 address { get; set; }
            public List<OtherAddress2> otherAddresses { get; set; }
        }

        
    }
}
