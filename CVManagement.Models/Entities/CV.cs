using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CVManagement.Models.Entities
{
    public class CV
    {
        public CV() {
            UserCVs = new HashSet<UserCV>();
        }
        [Key]
        public long Id { get; set; }
        [StringLength(200)]   
        public string FileName { get; set; }
        [StringLength(200)]
        public string UploadName { get; set; }
        [StringLength(200)]
        public string FileUrl { get; set; }
        [StringLength(10)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public long PosterId { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
        [ForeignKey(nameof(PosterId))]
        public User Poster { get; set; }
        [JsonIgnore]
        public ICollection<UserCV>? UserCVs { get; set;}

    }
}
