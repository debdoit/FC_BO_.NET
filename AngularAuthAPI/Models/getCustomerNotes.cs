using System;
using System.ComponentModel.DataAnnotations;

namespace AngularAuthAPI.Models
{
    public class getCustomerNotes
    {
        [Key]
        public string? CUSTOMER_ID { get; set; }
        public string? NOTES { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string? TYPE { get; set; }
        public string? FEEDBACK { get; set; }
        public string? UPDATED_BY { get; set; }
        public int? PRACTIONER_ID { get; set; }
        public int? TRANSACTION_ID { get; set; }
        
    }
}
