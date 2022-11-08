
namespace Utils.PersonFactory
{
    public class Email
    {
        private bool isPrimary { get; set; }
        private string LocatorId { get; set; }
        private string Country { get; set; }
        private string Type { get; set; }
        private string email { get; set; }
        private bool OptIn { get; set; }

        public Email()
        {
            this.Country = "USA";
            this.Type = "Personal";
            this.email = $"{Utility.getRandomString(1).Substring(0, 1).ToUpper()}{Utility.getRandomString(11).Substring(1).ToLower()}@test.com";
            this.OptIn = false;
        }
        public void setPrimary(bool isPrimary)
        {
            this.isPrimary = isPrimary;
        }
        public void setEmail(string email)
        {
            this.email = email;
        }
        public bool IsPrimary()
        {
            return this.isPrimary;
        }
        public string GetEmail()
        {
            return email;
        }
        
    }
}