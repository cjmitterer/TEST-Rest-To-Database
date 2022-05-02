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
    public class StudentController : SuperClassController
    {

        public StudentController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetStudents")]
        public async Task<IActionResult> GetStudents()
        {
            List<Instructor> lst = await _context.Instructors.OrderBy(x => x.InstructorId).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetStudents/{pStudentId}")]
        public async Task<IActionResult> GetStudents(int pStudentId)
        {
            Student itm = await _context.Students.Where(x => x.SchoolId == pStudentId).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpDelete]
        [Route("DeleteStudent/{pStudentId}")]
        public async Task<IActionResult> DeleteStudent(int pStudentId)
        {
            return await base.DeleteStudent(pStudentId);
        }

        [HttpPost]
        [Route("PostStudent")]
        public async Task<IActionResult> PostStudent([FromBody] StudentDTO studentDTO)
        {
            return await ModifyOrInsertStudent(studentDTO, null);
        }

        [HttpPut]
        [Route("PutStudent")]
        public async Task<IActionResult> PutStudent([FromBody] StudentDTO studentDTO)
        {
            return await ModifyOrInsertStudent(studentDTO, studentDTO);
        }

        private async Task<IActionResult> ModifyOrInsertStudent(StudentDTO studentDTO, StudentDTO studentDTOOld)
        {
            if (studentDTOOld == null){
                studentDTOOld = new StudentDTO();
            }
            var trans = _context.Database.BeginTransaction();
            try
            {

                Student existStudent = await _context.Students.Where(x => x.StudentId == studentDTOOld.StudentId).FirstOrDefaultAsync();

                bool newC = false;

                if (existStudent == null){
                    existStudent = new Student();
                    newC = true;
                }

                existStudent.StudentId = studentDTO.StudentId;
                existStudent.Salutation = studentDTO.Salutation;
                existStudent.FirstName = studentDTO.FirstName;
                existStudent.LastName = studentDTO.LastName;
                existStudent.StreetAddress = studentDTO.StreetAddress;
                existStudent.Zip = studentDTO.Zip;
                existStudent.Phone = studentDTO.Phone;
                existStudent.Employer = studentDTO.Employer;
                existStudent.RegistrationDate = studentDTO.RegistrationDate;
                existStudent.CreatedBy = studentDTO.CreatedBy;
                existStudent.CreatedDate = studentDTO.CreatedDate;
                existStudent.ModifiedBy = studentDTO.ModifiedBy;
                existStudent.ModifiedDate = studentDTO.ModifiedDate;
                existStudent.SchoolId = studentDTO.SchoolId;



                if (newC){
                    _context.Students.Add(existStudent);
                }
                else{
                    _context.Update(existStudent);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(studentDTO.SchoolId);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}