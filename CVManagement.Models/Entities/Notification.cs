using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVManagement.Models.Entities
{
    public class NotificationStatus
    {
        public static int Unread = 0;
        public static int Read = 1;
    }
    public class Notification
    {
        [Key]
        public long Id { get; set; }    
        public long UserId { get; set; }
        [StringLength(200)]
        public string Message { get; set; }
        [StringLength(100)]
        public string Link {  get; set; }
        public int Status { get; set; } = NotificationStatus.Unread;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
