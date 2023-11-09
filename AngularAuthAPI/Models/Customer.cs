using System;
using System.ComponentModel.DataAnnotations;

namespace AngularAuthAPI.Models
{
    public class Customer
    {
        [Key]
        public Guid? ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileColor { get; set; }
        public bool? IsEnabled { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool? LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public string? ZipCode { get; set; }
        public int? DateOfBirthDay { get; set; }
        public int? DateOfBirthMonth { get; set; }
        public int? DateOfBirthYear { get; set; }
        public string? CoverPhoto { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? LocationName { get; set; }
        public string? ACSIdentity { get; set; }
        public bool? ReceiveNewsLetters { get; set; }
        public bool? ReceivePromotions { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Background { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LoginTime { get; set; }
        public Guid? ReferalId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? AppleSid { get; set; }
        public int? LoginsCount { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyWebSiteAddress { get; set; }
        public int? FcAdvisorType { get; set; }
        public int? CustomerId { get; set; }
    }
}
