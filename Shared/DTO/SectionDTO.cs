using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWARM.Shared.DTO
{
    public class SectionDTO
    {
        public int SectionId { get; set; }
        public int CourseNo { get; set; }
        public byte SectionNo { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string Location { get; set; }
        public int InstructorId { get; set; }
        public byte? Capacity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int SchoolId { get; set; }
    }
}
