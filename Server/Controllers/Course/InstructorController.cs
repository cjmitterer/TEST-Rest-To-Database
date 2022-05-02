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
    public class InstructorController : SuperClassController
    {

        public InstructorController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetInstructors")]
        public async Task<IActionResult> GetInstructors()
        {
            List<Instructor> lst = await _context.Instructors.OrderBy(x => x.InstructorId).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetInstructors/{pInstructorId}")]
        public async Task<IActionResult> GetInstructors(int pInstructorId)
        {
            Instructor itm = await _context.Instructors.Where(x => x.InstructorId == pInstructorId).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpDelete]
        [Route("DeleteInstructor/{pInstructorId}")]
        public async Task<IActionResult> DeleteInstructor(int pInstructorId)
        {
            return await base.DeleteInstructor(pInstructorId);
        }

        [HttpPost]
        [Route("PostInstructor")]
        public async Task<IActionResult> PostInstructor([FromBody] InstructorDTO instructorDTO)
        {
            return await ModifyOrInsertInstructor(instructorDTO, null);
        }

        [HttpPut]
        [Route("PutInstructor")]
        public async Task<IActionResult> PutInstructor([FromBody] InstructorDTO instructorDTO)
        {
            return await ModifyOrInsertInstructor(instructorDTO, instructorDTO);
        }

        private async Task<IActionResult> ModifyOrInsertInstructor(InstructorDTO instructorDTO, InstructorDTO instructorDTOOld)
        {
            if (instructorDTOOld == null)
            {
                instructorDTOOld = new InstructorDTO();
            }
            var trans = _context.Database.BeginTransaction();
            try
            {

                Instructor existInstructor = await _context.Instructors.Where(x => x.InstructorId == instructorDTOOld.InstructorId).FirstOrDefaultAsync();

                bool newC = false;

                if (existInstructor == null)
                {
                    existInstructor = new Instructor();
                    newC = true;
                }

                existInstructor.InstructorId = instructorDTO.InstructorId;
                existInstructor.Salutation = instructorDTO.Salutation;
                existInstructor.FirstName = instructorDTO.FirstName;
                existInstructor.LastName = instructorDTO.LastName;
                existInstructor.StreetAddress = instructorDTO.StreetAddress;
                existInstructor.Zip = instructorDTO.Zip;
                existInstructor.Phone = instructorDTO.Phone;
                existInstructor.CreatedBy = instructorDTO.CreatedBy;
                existInstructor.CreatedDate = instructorDTO.CreatedDate;
                existInstructor.ModifiedBy = instructorDTO.ModifiedBy;
                existInstructor.ModifiedDate = instructorDTO.ModifiedDate;
                existInstructor.SchoolId = instructorDTO.SchoolId;

                if (newC)
                {
                    _context.Instructors.Add(existInstructor);
                }
                else
                {
                    _context.Update(existInstructor);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(instructorDTO.InstructorId);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}