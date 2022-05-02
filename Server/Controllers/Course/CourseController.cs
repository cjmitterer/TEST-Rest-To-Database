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
using SWARM.Server.Controllers;

namespace SWARM.Server.Controllers.Application
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : SuperClassController
    {


        public CourseController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }


        [HttpGet]
        [Route("GetCourses")]
        public async Task<IActionResult> GetCourses()
        {
            List<Course> lstCourses = await _context.Courses.OrderBy(x => x.CourseNo).ToListAsync();
            return Ok(lstCourses);
        }

        [HttpGet]
        [Route("GetCourses/{pCourseNo}")]
        public async Task<IActionResult> GetCourse(int pCourseNo)
        {
            Course itmCourse = await _context.Courses.Where(x => x.CourseNo == pCourseNo).FirstOrDefaultAsync();
            return Ok(itmCourse);
        }

        [HttpDelete]
        [Route("DeleteCourse/{pCourseNo}")]
        public async Task<IActionResult> DeleteCourse(int pCourseNo)
        {
            return await base.DeleteCourse(pCourseNo);
        }

        [HttpPost]
        [Route("PostCourse")]
        public async Task<IActionResult> PostCourse([FromBody] CourseDTO _CourseDTO)
        {
            return await ModifyOrInsertCourse(_CourseDTO, null);
        }

        [HttpPut]
        [Route("PutCourse")]
        public async Task<IActionResult> PutCourse([FromBody] CourseDTO _CourseDTO){
            return await ModifyOrInsertCourse(_CourseDTO, _CourseDTO);
        }


        private async Task<IActionResult> ModifyOrInsertCourse(CourseDTO _CourseDTO, CourseDTO _CourseDTOOld){
            if (_CourseDTOOld == null){
                _CourseDTOOld = new CourseDTO();
            }

            var trans = _context.Database.BeginTransaction();
            try
            {
                Course existCourse = await _context.Courses.Where(x => x.CourseNo == _CourseDTOOld.CourseNo).FirstOrDefaultAsync();

                bool newC = false;

                if (existCourse == null){
                    existCourse = new Course();
                    newC = true;
                }

                existCourse.Cost = _CourseDTO.Cost;
                existCourse.Description = _CourseDTO.Description;
                existCourse.Prerequisite = _CourseDTO.Prerequisite;
                existCourse.PrerequisiteSchoolId = _CourseDTO.PrerequisiteSchoolId;
                existCourse.SchoolId = _CourseDTO.SchoolId;
                existCourse.CreatedBy = _CourseDTO.CreatedBy;
                existCourse.CreatedDate = _CourseDTO.CreatedDate;
                existCourse.ModifiedBy = _CourseDTO.ModifiedBy;
                existCourse.ModifiedDate = _CourseDTO.ModifiedDate;


                if (newC)
                {
                    _context.Courses.Add(existCourse);
                }
                else
                {
                    _context.Update(existCourse);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(_CourseDTO.CourseNo);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpPost]
        [Route("GetCourses")]
        public async Task<DataEnvelope<CourseDTO>> GetCoursesPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<CourseDTO> dataToReturn = null;
            IQueryable<CourseDTO> queriableStates = _context.Courses
                    .Select(sp => new CourseDTO
                    {
                        Cost = sp.Cost,
                        CourseNo = sp.CourseNo,
                        CreatedBy = sp.CreatedBy,
                        CreatedDate = sp.CreatedDate,
                        Description = sp.Description,
                        ModifiedBy = sp.ModifiedBy,
                        ModifiedDate = sp.ModifiedDate,
                        Prerequisite = sp.Prerequisite,
                        PrerequisiteSchoolId = sp.PrerequisiteSchoolId,
                        SchoolId = sp.SchoolId
                    });

            // use the Telerik DataSource Extensions to perform the query on the data
            // the Telerik extension methods can also work on "regular" collections like List<T> and IQueriable<T>
            try
            {

                DataSourceResult processedData = await queriableStates.ToDataSourceResultAsync(gridRequest);

                if (gridRequest.Groups != null && gridRequest.Groups.Count > 0)
                {
                    // If there is grouping, use the field for grouped data
                    // The app must be able to serialize and deserialize it
                    // Example helper methods for this are available in this project
                    // See the GroupDataHelper.DeserializeGroups and JsonExtensions.Deserialize methods
                    dataToReturn = new DataEnvelope<CourseDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<CourseDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<CourseDTO>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
            }
            catch (Exception e)
            {
                //fixme add decent exception handling
            }
            return dataToReturn;
        }

    }
}