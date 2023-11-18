using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;

namespace CVManagement.Services.Interfaces
{
    public interface IUserService
    {
        public Task<ICollection<UserBasicInfor>> GetAllCustomerForHrManager();
        public Task<ICollection<UserBasicInfor>> GetAllCustomersByHrId(long hrId);
        public Task<ICollection<UserBasicInfor>> GetAllHumanResource();
        public Task<ICollection<UserBasicInfor>> GetAllCustomerForAssignToHr();
        public Task<ICollection<UserBasicInfor>> GetAllValidHumanResource();
        public Task<HumanResourceDatail> GetDetailOfHumanResource(long hrId);
        public Task AssignCustomerToHumanResource(HumanResourceAndCustomers hrAndCustomers);
        public Task<UserBasicInfor> GetById(long id);
        public Task<bool> Update(int id, UserUpdate user);
        public Task<bool> Update(User user);
        public Task<bool> Save(Register user);
        public Task<bool> LockOrUnlockAccount(long id);
        public Task<User> FindByEmail(string email);
        public Task<bool> ValidateOTP(OTPResetPassword otp);
        public Task<bool> ResetPassword(PasswordReset passwordReset);
        public bool CheckPassword(string password, User user);
        public Task<bool> DeleteCustomerOfHumanResource(long hrId, long customerId);
    }
}
