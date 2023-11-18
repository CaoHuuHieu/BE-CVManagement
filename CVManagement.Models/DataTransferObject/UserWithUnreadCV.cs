using CVManagement.Models.Entities;

namespace CVManagement.Models.DataTransferObject
{
    public class UserWithUnreadCV
    {
        public UserBasicInfor User  {  get; set; } 
        public IEnumerable<UserCV> UnreadCV { get; set; }
    }

    public class UserWithUnreadCV1
    {
        public UserBasicInfor Sender { get; set; }
        public UserBasicInfor Customer { get; set; }
        public IEnumerable<CV> UnreadCVs { get; set; }
    }
}
