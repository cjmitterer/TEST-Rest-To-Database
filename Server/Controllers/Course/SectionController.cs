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
    public class SectionController : SuperClassController
    {

        public SectionController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetSections")]
        public async Task<IActionResult> GetSectionss()
        {
            List<Section> lstsections = await _context.Sections.OrderBy(x => x.SectionId).ToListAsync();
            return Ok(lstsections);
        }

        [HttpGet]
        [Route("GetSections/{pSectionId}")]
        public async Task<IActionResult> GetSection(int pSectionId)
        {
            Section itmSection = await _context.Sections.Where(x => x.SectionId == pSectionId).FirstOrDefaultAsync();
            return Ok(itmSection);
        }

        [HttpDelete]
        [Route("DeleteSection/{pSectionId}")]
        public async Task<IActionResult> DeleteSection(int pSectionId){

            return await base.DeleteSection(pSectionId);
        }

        [HttpPost]
        [Route("PostSection")]
        public async Task<IActionResult> PostSection([FromBody] SectionDTO _SectionDTO)
        {
            return await ModifyOrInsertSection(_SectionDTO, null);
        }

        [HttpPut]
        [Route("PutSection")]
        public async Task<IActionResult> PutSection([FromBody] SectionDTO _SectionDTO)
        {
            return await ModifyOrInsertSection(_SectionDTO, _SectionDTO);
        }

        private async Task<IActionResult> ModifyOrInsertSection(SectionDTO _SectionDTO, SectionDTO _SectionDTOOld)
        {
            if (_SectionDTOOld == null){
                _SectionDTOOld = new SectionDTO();
            }

            var trans = _context.Database.BeginTransaction();
            try
            {
                Section existSection = await _context.Sections.Where(x => x.SectionId == _SectionDTOOld.SectionId).FirstOrDefaultAsync();

                bool newC = false;

                if (existSection == null){
                    existSection = new Section();
                    newC = true;
                }

                existSection.SectionId = _SectionDTO.SectionId;
                existSection.CourseNo = _SectionDTO.CourseNo;
                existSection.SectionNo = _SectionDTO.SectionNo;
                existSection.StartDateTime = _SectionDTO.StartDateTime;
                existSection.Location = _SectionDTO.Location;
                existSection.Capacity = _SectionDTO.Capacity;
                existSection.InstructorId = _SectionDTO.InstructorId;
                existSection.SchoolId = _SectionDTO.SchoolId;
                existSection.CreatedBy = _SectionDTO.CreatedBy;
                existSection.CreatedDate = _SectionDTO.CreatedDate;
                existSection.ModifiedBy = _SectionDTO.ModifiedBy;
                existSection.ModifiedDate = _SectionDTO.ModifiedDate;

                if (newC){
                    _context.Sections.Add(existSection);
                }
                else{
                    _context.Update(existSection);
                }

                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_SectionDTO.SectionId);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }






    }
}