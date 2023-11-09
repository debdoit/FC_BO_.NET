namespace AngularAuthAPI.Models
{
    public class GDPRCustomerUpdateRequest
    {
        public int DISPLAY { get; set; } // 0 or 1
        public int HOLD { get; set; }    // 0 or 1
        public int DELETE { get; set; }  // 0 or 1
        public string UpdateType { get; set; }
        public int UpdateValue { get; set; }
    }
}
