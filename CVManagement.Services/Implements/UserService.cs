using AutoMapper;
using CVManagement.Exceptions;
using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Implements;
using CVManagement.Repositories.Interfaces;
using CVManagement.Services.Interfaces;

using System.Security.Cryptography;


namespace CVManagement.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepos;
        public UserService(IUserRepository userRepos, IMapper mapper)
        {
            _userRepos = userRepos;
            _mapper = mapper;
        }
      
        public async Task<ICollection<UserBasicInfor>> GetAllCustomerForHrManager()
        {
            IEnumerable<User> users = await _userRepos.GetAll();
            return this._mapper.Map<List<UserBasicInfor>>(users);
        }
        public async Task<ICollection<UserBasicInfor>> GetAllCustomersByHrId(long hrId)
        {
            User user = await _userRepos.GetById(hrId);
            return this._mapper.Map<List<UserBasicInfor>>(user.Customers.Where(u => u.Status != (int)UserStatus.Lock));
        }
        public async Task<ICollection<UserBasicInfor>> GetAllHumanResource()
        {
            IEnumerable<User> users = await _userRepos.GetAll();
            return this._mapper.Map<List<UserBasicInfor>>(users.Where(u => u.Role.Equals(UserRole.Hr)));
        }
        public async Task<ICollection<UserBasicInfor>> GetAllValidHumanResource()
        {
            IEnumerable<User> users = await _userRepos.GetAll();
            return this._mapper.Map<List<UserBasicInfor>>(users.Where(u => u.Role.Equals(UserRole.Hr) && u.Status != (int)UserStatus.Lock));
        }
        public async Task<ICollection<UserBasicInfor>> GetAllCustomerForAssignToHr()
        {
            IEnumerable<User> users = await _userRepos.GetAll();
            return this._mapper.Map<List<UserBasicInfor>>(users.Where(u => u.Role.Equals(UserRole.Customer) && u.Status != (int)UserStatus.Lock));
        }
      

        public async Task<HumanResourceDatail> GetDetailOfHumanResource(long hrId)
        {
            User user = await _userRepos.GetById(hrId);
            var curriculumVitaes = this._mapper.Map < List <CurriculumVitaeBasicInfor >> (user.CVs);
            var customers = this._mapper.Map<HashSet<UserBasicInfor>>(user.Customers.Where(c => c.Status != (int)UserStatus.Lock));
            return new HumanResourceDatail()
            {
                CurriculumVitaes = curriculumVitaes,
                Customers = customers
            };
        }

        public async Task AssignCustomerToHumanResource(HumanResourceAndCustomers hrAndCustomers)
        {
            var users = await _userRepos.GetAll();
            var humanResources = users.Where(hr => hrAndCustomers.HrIds.Contains(hr.Id)).ToList();
            var customers = users.Where(customer => hrAndCustomers.CustomerIds.Contains(customer.Id)).ToList();
            foreach (var humanResource in humanResources)
            {
                foreach(var customer in customers)
                    humanResource.Customers.Add(customer);
                await _userRepos.Update(humanResource);
            }
        }

        public async Task<UserBasicInfor> GetById(long id)
        {
            var user = await _userRepos.GetById(id);
            return this._mapper.Map<UserBasicInfor>(user);
        }

        private string GeneratePassword() {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[8];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < 8; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
        public async Task<bool> Save(Register account)
        {
            HMACSHA512? hmac = new HMACSHA512();
            if(account.Password == "")
            {
                account.Password = GeneratePassword();
            }
            User user = new User()
            {
                Email = account.Email,
                OneTimePassword = account.Password,
                Avatar = account.FullName[0].ToString() + ".png",
                FullName = account.FullName,
                Role = account.Role,
                Company = account.Company,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(account.Password))
            };
            return await _userRepos.Save(user);
        }

        public  async Task<bool> Update(int id, UserUpdate account)
        {
            User user = await _userRepos.GetById(id);
            if (user != null)
            {
                user.Avatar = account.Avatar;
                user.FullName = account.FullName;
                return await _userRepos.Update(user);
            }
            else
                throw new EntityException("User is not found to update!");
         
        }

        public async Task<bool> Update(User user)
        {
            return await _userRepos.Update(user);
        }

        public async Task<User> FindByEmail(string email)
        {
            return await _userRepos.FindByEmail(email);
        }

        public async Task<bool> ValidateOTP(OTPResetPassword otpRequest)
        {
            var user = await _userRepos.FindByEmail(otpRequest.Email);
            if (user != null)
            {
                return user.OneTimePassword == otpRequest.Otp;
            }
            else
                throw new EntityException("Email is not valid!");
        }
        public async Task<bool> ResetPassword(PasswordReset passwordReset)
        {
            var user = await _userRepos.FindByEmail(passwordReset.Email);
            if (user != null)
            {
                using (HMACSHA512? hmac = new HMACSHA512())
                {
                    user.PasswordSalt = hmac.Key;
                    user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordReset.newPassword));
                }
                user.Status = (int)UserStatus.Active;
                return await _userRepos.Update(user);
            }
            throw new EntityException("User not found to reset password");
        }

        public bool CheckPassword(string password, User user)
        {
            using (HMACSHA512? hmac = new HMACSHA512(user.PasswordSalt))
            {
                var compute = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return  compute.SequenceEqual(user.PasswordHash);
            }
        }
        public async Task<bool> LockOrUnlockAccount(long id)
        {
            var user = await _userRepos.GetById(id);
            if (user != null)
            {
                if (user.Status == (int)UserStatus.Lock)
                    user.Status = (int) UserStatus.Active;
                else
                    user.Status = (int)UserStatus.Lock;
                return await _userRepos.Update(user);
            }
            else
                throw new EntityException("Entity is not found!");
        }
        public async Task<bool> DeleteCustomerOfHumanResource(long hrId, long customerId)
        {
            var hr = await _userRepos.GetById(hrId);
            if (hr != null)
            {
                ICollection<User> customersOfHr = hr.Customers;
                User customerDelete = customersOfHr.FirstOrDefault(u => u.Id == customerId);
                if (customerDelete != null)
                {
                    customersOfHr.Remove(customerDelete);
                    hr.Customers = customersOfHr;
                    return await _userRepos.Update(hr);
                }
                return false;
            }
            else
                throw new EntityException("Entity is not found!");
        }
    }
}
