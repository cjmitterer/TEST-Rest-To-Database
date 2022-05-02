using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWARM.Shared.DTO
{
    public class GradeTypeWeightDTO
    {
        public int SchoolId { get; set; }
        public int SectionId { get; set; }
        public string GradeTypeCode { get; set; }
        public byte NumberPerSection { get; set; }
        public byte PercentOfFinalGrade { get; set; }
        public bool DropLowest { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
