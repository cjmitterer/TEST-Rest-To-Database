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
    public class GradeTypeWeightController : SuperClassController
    {

        public GradeTypeWeightController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetGradeTypeWeights")]
        public async Task<IActionResult> GetGradesTypeWeights()
        {
            List<GradeTypeWeight> lst = await _context.GradeTypeWeights.OrderBy(x => x.SectionId).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetGradeTypeWeights/{pSectionId}/{pGradeTypeCode}")]
        public async Task<IActionResult> GetGradeTypeWeights(int pSectionId, string pGradeTypeCode)
        {
            GradeTypeWeight itm = await _context.GradeTypeWeights.Where(x => x.SectionId == pSectionId && x.GradeTypeCode == pGradeTypeCode).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpDelete]
        [Route("DeleteGradeTypeWeight/{pSectionId}/{pGradeTypeCode}")]
        public async Task<IActionResult> DeleteGradeTypeWeight(int pSectionId, string pGradeTypeCode)
        {
            return await base.DeleteGradeTypeWeight(pSectionId, pGradeTypeCode);
        }

        [HttpPost]
        [Route("PostGradeTypeWeight")]
        public async Task<IActionResult> PostGradeTypeWeight([FromBody] GradeTypeWeightDTO gradeTypeWeightDTO)
        {
            return await ModifyOrInsertGradeTypeWeight(gradeTypeWeightDTO, null);
        }

        [HttpPut]
        [Route("PutGradeTypeWeight")]
        public async Task<IActionResult> PutGradeTypeWeight([FromBody] GradeTypeWeightDTO gradeTypeWeightDTO)
        {
            return await ModifyOrInsertGradeTypeWeight(gradeTypeWeightDTO, gradeTypeWeightDTO);
        }


        private async Task<IActionResult> ModifyOrInsertGradeTypeWeight(GradeTypeWeightDTO gradeTypeWeightDTO, GradeTypeWeightDTO gradeTypeWeightDTOOld)
        {
            if (gradeTypeWeightDTOOld == null){
                gradeTypeWeightDTOOld = new GradeTypeWeightDTO();
            }
            var trans = _context.Database.BeginTransaction();
            try
            {

                GradeTypeWeight existGradeTypeWeight = await _context.GradeTypeWeights.Where(x => x.SectionId == gradeTypeWeightDTOOld.SectionId && x.GradeTypeCode == gradeTypeWeightDTOOld.GradeTypeCode).FirstOrDefaultAsync();

                bool newC = false;

                if (existGradeTypeWeight == null)
                {
                    existGradeTypeWeight = new GradeTypeWeight();
                    newC = true;
                }

                existGradeTypeWeight.GradeTypeCode = gradeTypeWeightDTO.GradeTypeCode;
                existGradeTypeWeight.SectionId = gradeTypeWeightDTO.SectionId;
                existGradeTypeWeight.NumberPerSection = gradeTypeWeightDTO.NumberPerSection;
                existGradeTypeWeight.PercentOfFinalGrade = gradeTypeWeightDTO.PercentOfFinalGrade;
                existGradeTypeWeight.DropLowest = gradeTypeWeightDTO.DropLowest;
                existGradeTypeWeight.CreatedBy = gradeTypeWeightDTO.CreatedBy;
                existGradeTypeWeight.CreatedDate = gradeTypeWeightDTO.CreatedDate;
                existGradeTypeWeight.ModifiedBy = gradeTypeWeightDTO.ModifiedBy;
                existGradeTypeWeight.ModifiedDate = gradeTypeWeightDTO.ModifiedDate;
                existGradeTypeWeight.SchoolId = gradeTypeWeightDTO.SchoolId;



                if (newC)
                {
                    _context.GradeTypeWeights.Add(existGradeTypeWeight);
                }
                else
                {
                    _context.Update(existGradeTypeWeight);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(gradeTypeWeightDTO.GradeTypeCode);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}