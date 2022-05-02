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
    public class GradeConversionController : SuperClassController
    {

        public GradeConversionController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetGradeConversions")]
        public async Task<IActionResult> GetGradesConversions()
        {
            List<GradeConversion> lst = await _context.GradeConversions.OrderBy(x => x.SchoolId).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetGradeConversions/{pSchoolId}/{pLetterGrade}")]
        public async Task<IActionResult> GetGradeConversions(int pSchoolId, string pLetterGrade)
        {
            GradeConversion itm = await _context.GradeConversions.Where(x => x.SchoolId == pSchoolId && x.LetterGrade == pLetterGrade).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpDelete]
        [Route("DeleteGradeConversion/{pSchoolId}/{pLetterGrade}")]
        public async Task<IActionResult> DeleteGradeConversions(int pSchoolId, string pLetterGrade)
        {
            return await base.DeleteGradeConversion(pSchoolId, pLetterGrade);
        }

        [HttpPost]
        [Route("PostGradeConversions")]
        public async Task<IActionResult> PostGrade([FromBody] GradeConversionDTO gradeConversionDTO)
        {
            return await ModifyOrInsertGradeConversion(gradeConversionDTO, null);
        }

        [HttpPut]
        [Route("PutGradeConversions")]
        public async Task<IActionResult> PutGrade([FromBody] GradeConversionDTO gradeConversionDTO)
        {
            return await ModifyOrInsertGradeConversion(gradeConversionDTO, gradeConversionDTO);
        }


        private async Task<IActionResult> ModifyOrInsertGradeConversion(GradeConversionDTO gradeConversionDTO, GradeConversionDTO gradeConversionDTOOld){
            if (gradeConversionDTOOld == null){
                gradeConversionDTOOld = new GradeConversionDTO();
            }
            var trans = _context.Database.BeginTransaction();
            try
            {

                GradeConversion existGradeConversion = await _context.GradeConversions.Where(x => x.SchoolId == gradeConversionDTOOld.SchoolId && x.LetterGrade == gradeConversionDTOOld.LetterGrade).FirstOrDefaultAsync();

                bool newC = false;

                if (existGradeConversion == null){
                    existGradeConversion = new GradeConversion();
                    newC = true;
                }

                existGradeConversion.LetterGrade = gradeConversionDTO.LetterGrade;
                existGradeConversion.GradePoint = gradeConversionDTO.GradePoint;
                existGradeConversion.MinGrade = gradeConversionDTO.MinGrade;
                existGradeConversion.MaxGrade = gradeConversionDTO.MaxGrade;
                existGradeConversion.CreatedBy = gradeConversionDTO.CreatedBy;
                existGradeConversion.CreatedDate = gradeConversionDTO.CreatedDate;
                existGradeConversion.ModifiedBy = gradeConversionDTO.ModifiedBy;
                existGradeConversion.ModifiedDate = gradeConversionDTO.ModifiedDate;
                existGradeConversion.SchoolId = gradeConversionDTO.SchoolId;



                if (newC){
                    _context.GradeConversions.Add(existGradeConversion);
                }
                else{
                    _context.Update(existGradeConversion);
                }

                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(gradeConversionDTO.LetterGrade);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



    }
}