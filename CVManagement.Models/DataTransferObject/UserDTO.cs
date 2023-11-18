using System.ComponentModel.DataAnnotations;

namespace CVManagement.Models.DataTransferObject
{
    public class UserUpdate
    {
 
        //[Required]
        //public string OneTimePassword { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Avatar { get; set; }
        //public string Role { get; set; }

    }
    public class UserBasicInfor
    {
        public long Id { get; set; }
        public string Email { get; set; }  
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Role { get; set; }
        public int Status { get; set; }
        public string Company { get; set; }
    }
    public class UserLogin
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class Register
    {
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
    public class PasswordReset
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
    public class OTPResetPassword
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Otp { get; set; }
    }

    public class UserFromToken
    {
        public long Id { get; set; }
        public string Role { get; set; }
    }

    public class HumanResourceDatail {
        public ICollection<UserBasicInfor> Customers { get; set; }
        public ICollection<CurriculumVitaeBasicInfor> CurriculumVitaes {get; set; }

    }
    public class HumanResourceAndCustomers {
        public long[] HrIds { get; set; }
        public long[] CustomerIds { get; set; }
    }

}
