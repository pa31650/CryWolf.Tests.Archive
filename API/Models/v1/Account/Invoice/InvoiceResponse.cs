using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.v1
{ 
    public partial class Invoice : CryWolf
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("relatedInvoiceId", NullValueHandling = NullValueHandling.Ignore)]
        public long? RelatedInvoiceId { get; set; }

        [JsonProperty("agency", NullValueHandling = NullValueHandling.Ignore)]
        public string Agency { get; set; }

        [JsonProperty("accountType", NullValueHandling = NullValueHandling.Ignore)]
        public AccountType AccountType { get; set; }
        
        [JsonProperty("accountNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountNumber { get; set; }

        [JsonProperty("letterName", NullValueHandling = NullValueHandling.Ignore)]
        public string LetterName { get; set; }

        [JsonProperty("caseNumber")]
        public object CaseNumber { get; set; }

        [JsonProperty("relatedCaseNumber")]
        public object RelatedCaseNumber { get; set; }

        [JsonProperty("comments")]
        public object Comments { get; set; }

        [JsonProperty("itemizedStatement")]
        public string ItemizedStatement { get; set; }

        [JsonProperty("isOutstanding", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsOutstanding { get; set; }

        [JsonProperty("amountDue", NullValueHandling = NullValueHandling.Ignore)]
        public long? AmountDue { get; set; }

        [JsonProperty("principleDue", NullValueHandling = NullValueHandling.Ignore)]
        public long? PrincipleDue { get; set; }

        [JsonProperty("principleAmount", NullValueHandling = NullValueHandling.Ignore)]
        public long? PrincipleAmount { get; set; }

        [JsonProperty("amountPaid", NullValueHandling = NullValueHandling.Ignore)]
        public long? AmountPaid { get; set; }

        [JsonProperty("refundedAmount", NullValueHandling = NullValueHandling.Ignore)]
        public long? RefundedAmount { get; set; }

        [JsonProperty("adjudicatedAmount", NullValueHandling = NullValueHandling.Ignore)]
        public long? AdjudicatedAmount { get; set; }

        [JsonProperty("taxesDue", NullValueHandling = NullValueHandling.Ignore)]
        public long? TaxesDue { get; set; }

        [JsonProperty("taxesPaid", NullValueHandling = NullValueHandling.Ignore)]
        public long? TaxesPaid { get; set; }

        [JsonProperty("actionDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? ActionDate { get; set; }

        [JsonProperty("createdOn")]
        public object CreatedOn { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("lastUpdatedOn")]
        public DateTimeOffset? LastUpdatedOn { get; set; }

        [JsonProperty("lastUpdatedBy")]
        public string LastUpdatedBy { get; set; }

        [JsonProperty("account", NullValueHandling = NullValueHandling.Ignore)]
        public Account Account { get; set; }
    }
    public partial class AccountType
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
    //public class Invoices
    //{
    //    public static List<Invoice> FromJson(string json) => JsonConvert.DeserializeObject<List<Invoice>>(json, API.Models.v1.Converter.Settings);
    //}
    public partial class Invoice
    {
        public static Invoice FromJson(string json) => JsonConvert.DeserializeObject<Invoice>(json, API.Models.v1.Converter.Settings);
    }
    //public static class Serialize
    //{
    //    public static string ToJson(this Account self) => JsonConvert.SerializeObject(self, API.Models.v1.Converter.Settings);
    //}

    //internal static class Converter
    //{
    //    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //    {
    //        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //        DateParseHandling = DateParseHandling.None,
    //        Converters =
    //        {
    //            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
    //        },
    //    };
    //}
}
