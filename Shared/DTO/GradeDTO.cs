using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWARM.Shared.DTO
{
    public class GradeDTO
    {
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public int SectionId { get; set; }
        public string GradeTypeCode { get; set; }
        public byte GradeCodeOccurrence { get; set; }
        public decimal NumericGrade { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
