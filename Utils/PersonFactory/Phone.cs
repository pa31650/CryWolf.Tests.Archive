using System;

namespace Utils.PersonFactory
{
    public class Phone
    {
        private bool isPrimary = false;
        private string locatorId { get; set; }
        private string country { get; set; }
        private string type { get; set; }
        private string number { get; set; }

        public Phone()
        {
            this.country = "United States";
            this.type = "Home";
            this.number = $"336{new Random().Next(1111111,9999999)}";
        }

        public string Number()
        {
            return this.number;
        }
        public String getFormattedNumber()
        {
            throw new NotImplementedException();
        }

        public void setPrimary(bool isPrimary)
        {
            this.isPrimary = isPrimary;
        }
        public bool IsPrimary()
        {
            return this.isPrimary;
        }
    }
}