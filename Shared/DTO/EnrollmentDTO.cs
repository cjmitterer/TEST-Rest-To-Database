using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWARM.Shared.DTO
{
    public class EnrollmentDTO
    {
        public int StudentId { get; set; }
        public int SectionId { get; set; }
        public DateTime EnrollDate { get; set; }
        public byte? FinalGrade { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int SchoolId { get; set; }
    }
}
