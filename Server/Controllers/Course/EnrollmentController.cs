using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWARM.EF.Data;
using SWARM.EF.Models;
using SWARM.Server.Models;
using SWARM.Shared;
using SWARM.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;

namespace SWARM.Server.Controllers.Application
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : SuperClassController
    {

        public EnrollmentController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetEnrollments")]
        public async Task<IActionResult> GetEnrollments()
        {
            List<Enrollment> lst = await _context.Enrollments.OrderBy(x => x.EnrollDate).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetEnrollments/{pSectionId}/{pStudentId}")]
        public async Task<IActionResult> GetEnrollment(int pSectionId, int pStudentId)
        {
            Enrollment itm = await _context.Enrollments.Where(x => x.StudentId == pStudentId && x.SectionId == pSectionId).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpGet]
        [Route("GetEnrollments/{pSectionId}")]
        public async Task<IActionResult> GetEnrollment(int pSectionId)
        {
            List<Enrollment> lst = await _context.Enrollments.Where(x => x.SectionId == pSectionId).ToListAsync();
            return Ok(lst);
        }

        [HttpDelete]
        [Route("DeleteEnrollment/{pSectionId}/{pStudentId}")]
        public async Task<IActionResult> DeleteEnrollment(int pSectionId, int pStudentId)
        {

            return await base.DeleteEnrollment(pStudentId, pSectionId);
        }

        [HttpPost]
        [Route("PostEnrollment")]
        public async Task<IActionResult> PostEnrollment([FromBody] EnrollmentDTO enrollmentDTO)
        {
            return await ModifyOrInsertEnrollment(enrollmentDTO, null);
        }

        [HttpPut]
        [Route("PutEnrollment")]
        public async Task<IActionResult> PutEnrollment([FromBody] EnrollmentDTO enrollmentDTO)
        {
            return await ModifyOrInsertEnrollment(enrollmentDTO, enrollmentDTO);
        }

        private async Task<IActionResult> ModifyOrInsertEnrollment(EnrollmentDTO enrollmentDTO, EnrollmentDTO enrollmentDTOOld)
        {
            if (enrollmentDTOOld == null)
            {
                enrollmentDTOOld = new EnrollmentDTO();
            }

            var trans = _context.Database.BeginTransaction();
            try
            {
                Enrollment existEnrollment = await _context.Enrollments.Where(x => x.SectionId == enrollmentDTOOld.SectionId && x.StudentId == enrollmentDTOOld.StudentId).FirstOrDefaultAsync();

                bool newC = false;

                if (existEnrollment == null)
                {
                    existEnrollment = new Enrollment();
                    newC = true;
                }

                existEnrollment.SectionId = enrollmentDTO.SectionId;
                existEnrollment.StudentId = enrollmentDTO.StudentId;
                existEnrollment.EnrollDate = enrollmentDTO.EnrollDate;
                existEnrollment.FinalGrade = enrollmentDTO.FinalGrade;
                existEnrollment.CreatedBy = enrollmentDTO.CreatedBy;
                existEnrollment.CreatedDate = enrollmentDTO.CreatedDate;

                existEnrollment.ModifiedBy = enrollmentDTO.ModifiedBy;
                existEnrollment.ModifiedDate = enrollmentDTO.ModifiedDate;

                existEnrollment.SchoolId = enrollmentDTO.SchoolId;



                if (newC)
                {
                    _context.Enrollments.Add(existEnrollment);
                }
                else
                {
                    _context.Update(existEnrollment);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(enrollmentDTO.SectionId);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}