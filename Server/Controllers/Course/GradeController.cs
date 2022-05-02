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
    public class GradeController : SuperClassController
    {

        public GradeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetGrades")]
        public async Task<IActionResult> GetGrades()
        {
            List<Grade> lst = await _context.Grades.OrderBy(x => x.SectionId).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetGrades/{pSectionId}/{pStudentId}")]
        public async Task<IActionResult> GetGrades(int pSectionId, int pStudentId)
        {
            List<Grade> lst = await _context.Grades.Where(x => x.StudentId == pStudentId && x.SectionId == pSectionId).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetGrades/{pSectionId}/{pStudentId}/{pCode}/{pOccurrence}")]
        public async Task<IActionResult> GetGrades(int pSectionId, int pStudentId, string pCode, byte pOccurrence)
        {
            Grade itm = await _context.Grades.Where(x => x.StudentId == pStudentId && x.SectionId == pSectionId && x.GradeTypeCode == pCode && x.GradeCodeOccurrence == pOccurrence).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpDelete]
        [Route("DeleteGrade/{pSectionId}/{pStudentId}/{pCode}/{pOccurrence}")]
        public async Task<IActionResult> DeleteGrade(int pSectionId, int pStudentId, string pCode, byte pOccurrence)
        {
            Grade itm = await _context.Grades.Where(x => x.StudentId == pStudentId && x.SectionId == pSectionId && x.GradeTypeCode == pCode && x.GradeCodeOccurrence == pOccurrence).FirstOrDefaultAsync();
            return await base.DeleteGrade(itm);
        }

        [HttpPost]
        [Route("PostGrade")]
        public async Task<IActionResult> PostGrade([FromBody] GradeDTO gradeDTO)
        {
            return await ModifyOrInsertGrade(gradeDTO, null);
        }

        [HttpPut]
        [Route("PutGrade")]
        public async Task<IActionResult> PutGrade([FromBody] GradeDTO gradeDTO)
        {
            return await ModifyOrInsertGrade(gradeDTO, gradeDTO);
        }

        private async Task<IActionResult> ModifyOrInsertGrade(GradeDTO gradeDTO, GradeDTO gradeDTOOld)
        {
            if (gradeDTOOld == null)
            {
                gradeDTOOld = new GradeDTO();
            }
            var trans = _context.Database.BeginTransaction();
            try
            {

                Grade existGrade = await _context.Grades.Where(x => x.SectionId == gradeDTOOld.SectionId && x.StudentId == gradeDTOOld.StudentId && x.GradeTypeCode == gradeDTOOld.GradeTypeCode && x.GradeCodeOccurrence == gradeDTOOld.GradeCodeOccurrence).FirstOrDefaultAsync();

                bool newC = false;

                if (existGrade == null)
                {
                    existGrade = new Grade();
                    newC = true;
                }

                existGrade.SectionId = gradeDTO.SectionId;
                existGrade.StudentId = gradeDTO.StudentId;
                existGrade.GradeTypeCode = gradeDTO.GradeTypeCode;
                existGrade.GradeCodeOccurrence = gradeDTO.GradeCodeOccurrence;
                existGrade.NumericGrade = gradeDTO.NumericGrade;
                existGrade.Comments = gradeDTO.Comments;
                existGrade.CreatedBy = gradeDTO.CreatedBy;
                existGrade.CreatedDate = gradeDTO.CreatedDate;
                existGrade.ModifiedBy = gradeDTO.ModifiedBy;
                existGrade.ModifiedDate = gradeDTO.ModifiedDate;
                existGrade.SchoolId = gradeDTO.SchoolId;


                if (newC)
                {
                    _context.Grades.Add(existGrade);
                }
                else
                {
                    _context.Update(existGrade);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(gradeDTO.SectionId);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}