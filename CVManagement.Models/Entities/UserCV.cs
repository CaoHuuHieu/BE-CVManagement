using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CVManagement.Models.Entities
{
    public class UserCV
    {
        public long CVId { get; set; }
        public long CustomerId { get; set; }
        public long SenderId { get; set; }
        public int Views { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastView { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public int ReminderInterval {  get; set; }
        public DateTime SendDate { get; set; } = DateTime.Now;
        [ForeignKey(nameof(CustomerId))]
        [JsonIgnore]
        public User? Customer { get; set; }
        [ForeignKey(nameof(CVId))]
        [JsonIgnore]
        public CV? CV { get; set; }
        [ForeignKey(nameof(SenderId))]
        [JsonIgnore]
        public User? Sender { get; set; }
    }
}
