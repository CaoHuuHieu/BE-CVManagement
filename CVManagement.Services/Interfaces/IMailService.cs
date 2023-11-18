using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;

namespace CVManagement.Services.Interfaces
{
    public interface IMailService
    {
        public Task SendOTPAsync(string email);
        public Task<bool> StoreAndSendCV(long senderId, MailRequest email);
        public Task ResendCV();
        public Task SendReminderEmail(long senderId, long[] customerId);
    }
}
