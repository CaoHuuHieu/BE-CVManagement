using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using CVManagement.Services.Interfaces;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace CVManagement.Services.Implements
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepos;
        private readonly IUserCVRepository _userCVRepos;
        private readonly ICVRepository _cvRepos;
        private readonly string server, emailFrom, passwordEmail, urlViewFile;
        private readonly int port;
        public MailService(IConfiguration configuration, IUserRepository userRepos, ICVRepository cvRepos, IUserCVRepository userCVRepos)
        {
            _configuration = configuration;
            _userRepos = userRepos;
            _cvRepos = cvRepos;
            server = _configuration["MailSettings:Server"];
            emailFrom = _configuration["MailSettings:SenderEmail"];
            passwordEmail = _configuration["MailSettings:Password"];
            port = Convert.ToInt32(_configuration["MailSettings:Port"]);
            urlViewFile = _configuration["Link:ViewFile"];
            _userCVRepos = userCVRepos;
        }
        public async Task SendOTPAsync(string email)
        {
            var user = await _userRepos.FindByEmail(email);
            if (user != null)
            {
                string otp = GenerateOTP();
                user.OneTimePassword = otp;
                bool isAddedOtp = await _userRepos.Update(user);
                if (isAddedOtp == true)
                {
                    string subject = "[SMARTDEV] Your OTP for Account Verification";
                    string body = $"Dear {user.FullName},<br>" +
                        $"You have requested access to your account. Below is the One-Time Password (OTP) for completing the verification process:<br>" +
                        $"OTP:<b>{otp} </b><br>" +
                        $"Please enter this code on the verification page to proceed with the login or transaction. Do not share this code with anyone, including our staff. This code is valid for a short period to ensure the security of your account.<br>" +
                        $"If you did not request this code, please disregard this email and consider changing your password immediately to secure your account.<br>" +
                        $"If you encounter any issues, please contact us at our support email address: [Your support email address].<br>" +
                        $"Best regards,<br>" +
                        $"SMARTDEV";
                    MimeMessage mail = CreateMail(email, subject, body);
                    await SendMailAsync(mail);
                }
            }
            else
            {
                throw new Exception("User not found");
            }
        }
        public async Task<bool> StoreAndSendCV(long senderId, MailRequest email)
        {
            var sender = await _userRepos.GetById(senderId);
            var customers = await _userRepos.GetAll();
            customers = customers.Where(u => email.UserIds.Contains(u.Id));
            var cvs = await _cvRepos.GetAll();
            cvs = cvs.Where(cv => email.CVIds.Contains(cv.Id));
            foreach (var customer in customers)
            {
                string linkCVs = "Here is your link <br>";
                var userCVsOfCustomer = await _userCVRepos.GetByUserId(customer.Id);
                foreach (var cv in cvs)
                {
                    var usercv = userCVsOfCustomer.Where(uc => uc.CustomerId == customer.Id && uc.CVId == cv.Id).FirstOrDefault();
                    if (usercv != null)
                        userCVsOfCustomer.Remove(usercv);
                    userCVsOfCustomer.Add(new UserCV { CVId = cv.Id, CustomerId = customer.Id, Sender = sender, ReminderInterval = email.ReminderInterval });
                    linkCVs += $"<li><a href='{urlViewFile}?id={cv.Id}'>{cv.FileName}</a></li>";
                    Console.Write(linkCVs);
                }

                customer.ReceiveCVs = userCVsOfCustomer;
                bool isAddUserCv = await _userRepos.Update(customer);
                if (isAddUserCv)
                {
                    string subject = "[SMARTDEV] Review of Available Curriculum Vitae";
                    string body = GenerateEmailSendCvBody(customer, linkCVs);
                    MimeMessage mail = CreateMail(customer.Email, subject, body);
                    await SendMailAsync(mail);
                }
            }
            return true;
        }
        public async Task ResendCV()
        {
            var customerWithUnreadCVs = await _userRepos.GetUserWithUnreadCV();
            ICollection<MimeMessage> mails = await GenerateMailSendCurriculumVitae(customerWithUnreadCVs);
            foreach (MimeMessage mail in mails)
            {
                await SendMailAsync(mail);
            }
        }

        public async Task SendReminderEmail(long senderId, long[] customerIds)
        {
            var customerWithUnreadCVs = await _userRepos.GetUserWithUnreadCV(senderId, customerIds);
            ICollection<MimeMessage> mails = await GenerateMailSendCurriculumVitae(customerWithUnreadCVs);
            foreach (MimeMessage mail in mails)
            {
                await SendMailAsync(mail);
            }
        }
        private MimeMessage CreateMail(string mailTo, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailFrom));
            email.To.Add(MailboxAddress.Parse(mailTo));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            return email;
        }
        private async Task SendMailAsync(MimeMessage mail)
        {
            var smtp = new SmtpClient();
            smtp.Connect(server, port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailFrom, passwordEmail);
            await smtp.SendAsync(mail);
        }
        
        private string GenerateEmailSendCvBody(User customer, string linkCVs)
        {
            string mailBody = $"<div style='color: black; font-size: 16px'>Dear {customer.FullName},<br>" +
                $" We hope this message finds you well. We would like to inform you that several new curriculum vitae (CVs) have been uploaded to your account." +
                $"<ol>{linkCVs}</ol>" +
                $"Please log in your account to review these CVs at your earliest convenience.<br>" +
                $"Your account: <br>" +
                $"Username: {customer.Email}<br>" +
                $"Password: {customer.OneTimePassword}<br>" +
                $"If you have any questions or require further assistance, please feel free to reach out to us.<br>" +
                $"Thank you for your continued partnership with us. We look forward to assisting you with your recruitment endeavors.";
            mailBody += CreateMailFooter();
            return mailBody;
        }

        private async Task<ICollection<MimeMessage>> GenerateMailSendCurriculumVitae(ICollection<UserWithUnreadCV> customerWithUnreadCVs)
        {
            string subject = "[SMARTDEV] Reminder Unread CV";
            List<MimeMessage> mails = new List<MimeMessage>();
            foreach (var customerWithUnreadCV in customerWithUnreadCVs)
            {
                string body = $"<div style = 'color: #000 ; font-size: 16px'>Dear {customerWithUnreadCV.User.FullName}, <br>" +
                    $"This is a friendly reminder that you have some unread CVs. " +
                    $"Please take a moment to review them: <ol>";
                foreach (var unreadCV in customerWithUnreadCV.UnreadCV)
                {
                    var cvInfor = await _cvRepos.GetById(unreadCV.CVId);
                    unreadCV.SendDate = DateTime.Now;
                    await _userCVRepos.Update(unreadCV);
                    body += $"<li><a href='{urlViewFile}/{cvInfor.Id}'>{cvInfor.FileName}</a> </li>";
                }
                body += "</ol> <br> ";
                body += CreateMailFooter();
                mails.Add(CreateMail(customerWithUnreadCV.User.Email, subject, body));
            }
            return mails;
        }
        private string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 1000000);
            return otp.ToString();
        }
        private string CreateMailFooter()
        {
            string result = $"<br>----------------------<br>" +
                $"Best regards,<br> </div>" +
                $"<div style='color: blue; font-weight: bold; font-size: 20px'>SmartDev</div>" +
                $" <div style='color: #000><span style='font-weight: bold;'>Email:</span> smartdevcompany@smartdev.com <br>" +
                $"<span style='font-weight: bold;'>Address:</span> 81 Quang Trung, Da Nang, Viet Nam" +
                $"<div> <span style='color: black; font-weight: bold; font-size: 24px'> SmartDev</span> - Outcome Driven Software</div>" +
                $"<div style='color: red'>Winner of SME100® Fast Moving Companies </div>" +
                $"-----------------------------------------------------------------<br>" +
                $"This message and any attachments may contain confidential or privileged information and are only for the use of the intended recipient of this message. If you are not the intended recipient, please notify the sender by return email immediately. Then, please delete or destroy this and all copies of this message and all attachments. Any unauthorised disclosure, use, distribution, or reproduction of this message or any attachments is prohibited and may be unlawful." +
                $"<br>-----------------------------------------------------------------  </div>";
            return result;
        }
    }
}
