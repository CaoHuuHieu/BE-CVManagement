using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CVManagement.Models.Entities
{
    public enum UserStatus
    {
        Lock = 0,
        Pending = 1,
        Active = 2,
    }
    public static class UserRole
    {
        public static readonly string Hrmanager = "HrManager";
        public static readonly string Hr = "Hr";
        public static readonly string Customer = "Customer";
    }
    public class User
    {
        public User()
        {
            ReceiveCVs = new HashSet<UserCV>();
            Customers = new HashSet<User>();
            SendedCVs = new HashSet<UserCV>();

        }
        [Key]
        public long Id { get; set; }
        [StringLength(50)]
        public string FullName { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(50)]
        public string Company { get; set; }
        [StringLength(50)]
        public string Avatar { get; set; }
        [StringLength(30)]
        public string OneTimePassword { get; set; }   
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [StringLength(600)]
        public string? Token { get; set; }
        [StringLength(20)]
        public string Role { get; set; } = UserRole.Customer;
        public int Status { get; set; } = (int)UserStatus.Pending;
        public ICollection<User> HumanResources { get; set; }
        public ICollection<User> Customers { get; set; }
        [InverseProperty("Customer")]
        public ICollection<UserCV>? ReceiveCVs { get; set; }
        [InverseProperty("Sender")]
        [JsonIgnore]
        public ICollection<UserCV>? SendedCVs { get; set; }
        [JsonIgnore]
        public IEnumerable<CV>? CVs { get; set; }
        public IEnumerable<Notification>? Notifications { get; set; }
    }
}
