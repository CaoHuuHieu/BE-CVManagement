using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVManagement.Models.DataTransferObject
{
    public class CurriculumVitaeForCustomer
    {
        public CurriculumVitaeBasicInfor CurriculumVitae { get; set; }
        public int Views { get; set; }
        public DateTime? LastView { get; set; }  
    }
    public class CurriculumVitaeBasicInfor
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime UploadDate { get; set; }
        public UserBasicInfor Poster { get; set; }
        public string FileUrl { get; set; }
        public int Status { get; set; }
    }

    public class CurriculumnVitaeFile
    {
        public byte[] FileByte { get; set; }
        public string ContentType { get; set; }
        public string fileUrl { get; set; }
    }
}
