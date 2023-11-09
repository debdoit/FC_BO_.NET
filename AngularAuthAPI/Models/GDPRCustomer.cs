using System.ComponentModel.DataAnnotations;

namespace AngularAuthAPI.Models
{
    public class GDPRCustomer
    {
        [Key]
        public int? ID { get; set; }
        public string? CUSTOMER_ID { get; set; }
        public bool? DISPLAY { get; set; }
        public bool? HOLD { get; set; }
        public bool? DELETE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? DELETED_DATE { get; set; }
        public DateTime? EMAIL_REMINDER_DATE { get; set; }
        public string? BRAZE_EXTERNAL_ID { get; set; }
        public byte[]? C_Info { get; set; }
        public string? TWO_FA_CODE { get; set; }
        public int? AUTH_STATUS { get; set; }
        public bool? ORDER_EXIST { get; set; }
    }
}
