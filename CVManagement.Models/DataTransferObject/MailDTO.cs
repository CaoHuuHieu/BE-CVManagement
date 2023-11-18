namespace CVManagement.Models.DataTransferObject
{
    public class MailRequest
    {
        public long[] UserIds {  get; set; }
        public long[] CVIds{  get; set; }
        public int ReminderInterval { get; set; }
    }
}
