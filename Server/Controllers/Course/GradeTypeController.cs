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
    public class GradeTypeController : SuperClassController
    {

        public GradeTypeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetGradeTypes")]
        public async Task<IActionResult> GetGradesTypes()
        {
            List<GradeType> lst = await _context.GradeTypes.OrderBy(x => x.SchoolId).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetGradeTypes/{pSchoolId}/{pGradeTypeCode}")]
        public async Task<IActionResult> GetGradeTypes(int pSchoolId, string pGradeTypeCode)
        {
            GradeType itm = await _context.GradeTypes.Where(x => x.SchoolId == pSchoolId && x.GradeTypeCode == pGradeTypeCode).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpDelete]
        [Route("DeleteGradeType/{pSchoolId}/{pGradeTypeCode}")]
        public async Task<IActionResult> DeleteGradeType(int pSchoolId, string pGradeTypeCode)
        {
            return await base.DeleteGradeType(pSchoolId, pGradeTypeCode);
        }

        [HttpPost]
        [Route("PostGradeType")]
        public async Task<IActionResult> PostGradeType([FromBody] GradeTypeDTO gradeTypeDTO)
        {
            return await ModifyOrInsertGradeType(gradeTypeDTO, null);
        }

        [HttpPut]
        [Route("PutGradeType")]
        public async Task<IActionResult> PutGradeType([FromBody] GradeTypeDTO gradeTypeDTO)
        {
            return await ModifyOrInsertGradeType(gradeTypeDTO, gradeTypeDTO);
        }

        private async Task<IActionResult> ModifyOrInsertGradeType(GradeTypeDTO gradeTypeDTO, GradeTypeDTO gradeTypeDTOOld)
        {
            if (gradeTypeDTOOld == null)
            {
                gradeTypeDTOOld = new GradeTypeDTO();
            }
            var trans = _context.Database.BeginTransaction();
            try
            {

                GradeType existGradeType = await _context.GradeTypes.Where(x => x.SchoolId == gradeTypeDTOOld.SchoolId && x.GradeTypeCode == gradeTypeDTOOld.GradeTypeCode).FirstOrDefaultAsync();

                bool newC = false;

                if (existGradeType == null)
                {
                    existGradeType = new GradeType();
                    newC = true;
                }

                existGradeType.GradeTypeCode = gradeTypeDTO.GradeTypeCode;
                existGradeType.Description = gradeTypeDTO.Description;
                existGradeType.CreatedBy = gradeTypeDTO.CreatedBy;
                existGradeType.CreatedDate = gradeTypeDTO.CreatedDate;

                existGradeType.ModifiedBy = gradeTypeDTO.ModifiedBy;
                existGradeType.ModifiedDate = gradeTypeDTO.ModifiedDate;

                existGradeType.SchoolId = gradeTypeDTO.SchoolId;



                if (newC)
                {
                    _context.GradeTypes.Add(existGradeType);
                }
                else
                {
                    _context.Update(existGradeType);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(gradeTypeDTO.GradeTypeCode);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}