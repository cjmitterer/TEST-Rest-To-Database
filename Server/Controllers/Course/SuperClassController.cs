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

namespace SWARM.Server.Controllers
{
    public abstract class SuperClassController : Controller
    {

        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public SuperClassController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> DeleteCourse(int pCourseNo)
        {
            Course itmCourse = await _context.Courses.Where(x => x.CourseNo == pCourseNo).FirstOrDefaultAsync();

            List<Section> sections = await _context.Sections.Where(x => x.CourseNo == pCourseNo).ToListAsync();

            foreach (Section s in sections)
            {
                await DeleteSection(s.SectionId);
            }

            List<Course> referencing = await _context.Courses.Where(x => x.Prerequisite == pCourseNo).ToListAsync();

            foreach (Course c in referencing)
            {
                await DeleteCourse(c.CourseNo);
            }


            _context.Remove(itmCourse);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> DeleteSection(int pSectionId)
        {
            Section itmSection = await _context.Sections.Where(x => x.SectionId == pSectionId).FirstOrDefaultAsync();

            List<Enrollment> enrollments = await _context.Enrollments.Where(x => x.SectionId == pSectionId).ToListAsync();

            foreach (Enrollment e in enrollments)
            {
                await DeleteEnrollment(e.StudentId, e.SectionId);
            }

            List<GradeTypeWeight> gradeTypeWeights = await _context.GradeTypeWeights.Where(x => x.SectionId == pSectionId).ToListAsync();

            foreach (GradeTypeWeight g in gradeTypeWeights)
            {
                await DeleteGradeTypeWeight(g);
            }

            _context.Remove(itmSection);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> DeleteEnrollment(int studentId, int sectionId)
        {
            Enrollment itmEnrollment = await _context.Enrollments.Where(x => x.StudentId == studentId && x.SectionId == sectionId).FirstOrDefaultAsync();

            List<Grade> grades = await _context.Grades.Where(x => x.SectionId == sectionId && x.StudentId == studentId).ToListAsync();

            foreach (Grade g in grades)
            {
                await DeleteGrade(g);
            }

            _context.Remove(itmEnrollment);
            await _context.SaveChangesAsync();

            return Ok();
        }


        public async Task<IActionResult> DeleteGradeTypeWeight(int sectionId, string gradeTypeCode)
        {
            GradeTypeWeight g = await _context.GradeTypeWeights.Where(x => x.SectionId == sectionId && x.GradeTypeCode == gradeTypeCode).FirstOrDefaultAsync();

            return await DeleteGradeTypeWeight(g);
        }
        public async Task<IActionResult> DeleteGradeTypeWeight(GradeTypeWeight g)
        {
            List<Grade> grades = await _context.Grades.Where(x => x.GradeTypeCode == g.GradeTypeCode && g.SectionId == x.SectionId).ToListAsync();

            foreach (Grade gr in grades)
            {
                await DeleteGrade(gr);
            }

            _context.Remove(g);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> DeleteGrade(int studentId, int sectionId)
        {
            Grade g = await _context.Grades.Where(x => x.SectionId == sectionId && x.StudentId == studentId).FirstOrDefaultAsync();

            return await DeleteGrade(g);
        }

        public async Task<IActionResult> DeleteGrade(Grade g)
        {
            _context.Remove(g);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> DeleteGradeConversion(int schoolId, string letterGrade)
        {
            GradeConversion gc = await _context.GradeConversions.Where(x => x.SchoolId == schoolId && x.LetterGrade == letterGrade).FirstOrDefaultAsync();

            _context.Remove(gc);
            await _context.SaveChangesAsync();


            return Ok();
        }

        public async Task<IActionResult> DeleteGradeType(int schoolId, string gradeTypeCode)
        {
            GradeType gt = await _context.GradeTypes.Where(x => x.SchoolId == schoolId && x.GradeTypeCode == gradeTypeCode).FirstOrDefaultAsync();

            List<GradeTypeWeight> gradeTypeWeights = await _context.GradeTypeWeights.Where(x => x.SchoolId == schoolId && x.GradeTypeCode == gradeTypeCode).ToListAsync();

            foreach (GradeTypeWeight gtw in gradeTypeWeights)
            {
                await DeleteGradeTypeWeight(gtw);
            }

            _context.Remove(gt);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> DeleteInstructor(int instructorId)
        {
            Instructor instructor = await _context.Instructors.Where(x => x.InstructorId == instructorId).FirstOrDefaultAsync();

            List<Section> sections = await _context.Sections.Where(x => x.InstructorId == instructorId).ToListAsync();

            foreach (Section s in sections)
            {
                await DeleteSection(s.SectionId);
            }

            _context.Remove(instructor);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> DeleteSchool(int schoolId){
            School school = await _context.Schools.Where(x => x.SchoolId == schoolId).FirstOrDefaultAsync();

            List<Course> courses = await _context.Courses.Where(x => x.SchoolId == schoolId).ToListAsync();

            foreach (Course c in courses){
                await DeleteCourse(c.CourseNo);
            }

            List<Student> students = await _context.Students.Where(x => x.SchoolId == schoolId).ToListAsync();

            foreach (Student s in students){
                await DeleteStudent(s.StudentId);
            }

            List<Instructor> instructors = await _context.Instructors.Where(x => x.SchoolId == schoolId).ToListAsync();

            foreach (Instructor i in instructors)
            {
                await DeleteInstructor(i.InstructorId);
            }

            List<GradeType> gt = await _context.GradeTypes.Where(x => x.SchoolId == schoolId).ToListAsync();

            foreach (GradeType g in gt)
            {
                await DeleteGradeType(g.SchoolId, g.GradeTypeCode);
            }

            List<GradeConversion> gc = await _context.GradeConversions.Where(x => x.SchoolId == schoolId).ToListAsync();

            foreach (GradeConversion g in gc){
                await DeleteGradeConversion(g.SchoolId, g.LetterGrade);
            }

            _context.Remove(school);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            Student student = await _context.Students.Where(x => x.StudentId == studentId).FirstOrDefaultAsync();

            List<Enrollment> enrollments = await _context.Enrollments.Where(x => x.StudentId == studentId).ToListAsync();

            foreach (Enrollment e in enrollments)
            {
                await DeleteEnrollment(e.StudentId, e.SectionId);
            }

            _context.Remove(student);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> DeleteZipcode(string zip)
        {
            Zipcode zipcode = await _context.Zipcodes.Where(x => x.Zip == zip).FirstOrDefaultAsync();

            List<Instructor> instructors = await _context.Instructors.Where(x => x.Zip == zip).ToListAsync();

            foreach (Instructor i in instructors)
            {
                await DeleteInstructor(i.InstructorId);
            }

            _context.Remove(zipcode);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}