using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWARM.Shared.DTO
{
    public class GradeConversionDTO
    {
        public int SchoolId { get; set; }
        public string LetterGrade { get; set; }
        public decimal GradePoint { get; set; }
        public byte MaxGrade { get; set; }
        public byte MinGrade { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
