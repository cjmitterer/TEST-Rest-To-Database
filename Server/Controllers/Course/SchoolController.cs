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
    public class SchoolController : SuperClassController
    {

        public SchoolController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetSchools")]
        public async Task<IActionResult> GetSchools()
        {
            List<School> lst = await _context.Schools.OrderBy(x => x.SchoolId).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetSchools/{pSchoolId}")]
        public async Task<IActionResult> GetSchools(int pSchoolId)
        {
            School itm = await _context.Schools.Where(x => x.SchoolId == pSchoolId).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpDelete]
        [Route("DeleteSchool/{pSchoolId}")]
        public async Task<IActionResult> DeleteSchool(int pSchoolId)
        {
            return await base.DeleteSchool(pSchoolId);
        }

        [HttpPost]
        [Route("PostSchool")]
        public async Task<IActionResult> PostSchool([FromBody] SchoolDTO schoolDTO)
        {
            return await ModifyOrInsertSchool(schoolDTO, null);
        }

        [HttpPut]
        [Route("PutSchool")]
        public async Task<IActionResult> PutSchool([FromBody] SchoolDTO schoolDTO)
        {
            return await ModifyOrInsertSchool(schoolDTO, schoolDTO);
        }

        private async Task<IActionResult> ModifyOrInsertSchool(SchoolDTO schoolDTO, SchoolDTO schoolDTOOld)
        {
            if (schoolDTOOld == null){
                schoolDTOOld = new SchoolDTO();
            }
            var trans = _context.Database.BeginTransaction();
            try
            {

                School existSchool = await _context.Schools.Where(x => x.SchoolId == schoolDTOOld.SchoolId).FirstOrDefaultAsync();

                bool newC = false;

                if (existSchool == null)
                {
                    existSchool = new School();
                    newC = true;
                }

                existSchool.SchoolName = schoolDTO.SchoolName;
                existSchool.CreatedBy = schoolDTO.CreatedBy;
                existSchool.CreatedDate = schoolDTO.CreatedDate;
                existSchool.ModifiedBy = schoolDTO.ModifiedBy;
                existSchool.ModifiedDate = schoolDTO.ModifiedDate;
                existSchool.SchoolId = schoolDTO.SchoolId;

                if (newC){
                    _context.Schools.Add(existSchool);
                }
                else{
                    _context.Update(existSchool);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(schoolDTO.SchoolId);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}