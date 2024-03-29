﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursesApi.Models.DTOModels;
using CoursesApi.Models.EntityModels;
using CoursesApi.Models.ViewModels;
using CoursesApi.Services;
using Microsoft.AspNetCore.Mvc;
using CoursesApi.Repositories;
using CoursesApi.Models.Exceptions;

namespace Api.Controllers
{
    /// <summary>
    /// A resource for courses
    /// </summary>
    [Route("api/[controller]")]
    public class CoursesController : Controller
    {
        private ICoursesService _coursesService;

        public CoursesController(ICoursesService coursesService)
        {
              _coursesService = coursesService;
        }

        /// <summary>
        /// A route which should receive all courses from the database
        /// </summary>
        /// <param name="semester">The semester which filters the courses</param>
        /// <returns>A list of CourseDTO's</returns>
        [HttpGet]
        public IActionResult GetCourses(string semester = "20173")
        {
            var courses = _coursesService.GetCourses(semester);
            
            return Ok(courses);
        }

        /// <summary>
        /// A route which should return a course by providing a valid id
        /// </summary>
        /// <param name="courseId">An integer id for a course</param>
        /// <returns>A single CourseDTO object</returns>
        [HttpGet]
        [Route("{courseId:int}", Name = "GetCourseById")]
        public IActionResult GetCourseById(int courseId)
        {
            var courses = _coursesService.GetCourseById(courseId);
            
            if (courses == null)
            {
                return NotFound();
            }

            return Ok(courses);
        }

        [HttpPost]
        [Route("", Name = "AddCourse")]
        public IActionResult AddCourse([FromBody] CourseViewModel course) 
        {
            if (course == null) { return BadRequest(); }
            if (!ModelState.IsValid) { return StatusCode(412); }

            var newCourse = _coursesService.AddCourse(course);

            return Created(newCourse.Name, newCourse);
        }

        /// <summary>
        /// Updates a course
        /// </summary>
        /// <param name="courseId">An integer id for a course</param>
        /// <param name="updatedCourse">The updated values for the course</param>
        /// <returns>The updated course</returns>
        [HttpPut]
        [Route("{courseId:int}")]
        public IActionResult UpdateCourse(int courseId, [FromBody] CourseViewModel updatedCourse)
        {
            if (updatedCourse == null) { return BadRequest(); }
            if (!ModelState.IsValid) { return StatusCode(412); }

            var course = _coursesService.UpdateCourse(courseId, updatedCourse);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        /// <summary>
        /// Retrieves a list of students which are enrolled in a given course
        /// </summary>
        /// <param name="courseId">The id of the course</param>
        /// <returns>A list of StudentDTO's</returns>
        [HttpGet]
        [Route("{courseId:int}/students")]
        public IActionResult GetStudentsByCourseId(int courseId)
        {
            var students = _coursesService.GetStudentsByCourseId(courseId);

            if (students == null)
            {
                return NotFound();
            }

            return Ok(students);
        }

        /// <summary>
        /// Adds a student to a course. Student must be already in the system
        /// </summary>
        /// <param name="courseId">The id of the course</param>
        /// <param name="newStudent">The new student to add</param>
        /// <returns>The newly created student</returns>
        [HttpPost]
        [Route("{courseId:int}/students")]
        public IActionResult AddStudentToCourse(int courseId, [FromBody] StudentViewModel newStudent)
        {
            if (newStudent == null) { return BadRequest(); }
            
            if (!ModelState.IsValid) { return StatusCode(412); }
            
            try
            {
                var response = _coursesService.AddStudentToCourse(courseId, newStudent);
                return Created(response.SSN, response);

            }catch(CoursesApi.Models.Exceptions.StudentNotExistsException e)
            {
                    return NotFound(e.Message);
            }catch(CoursesApi.Models.Exceptions.CourseNotExistsException e)
            {
                    return NotFound(e.Message);
            }catch(CoursesApi.Models.Exceptions.DublicateEnrolledException e)
            {
                    return StatusCode(412, e.Message);
            }catch(CoursesApi.Models.Exceptions.MaxCapacityException e)
            {
                    return StatusCode(412, e.Message);
            }              
        }

        /// <summary>
        /// Deletes a course
        /// </summary>
        /// <param name="courseId">The id of the course</param>
        /// <returns>A status code 204 (if successful)</returns>
        [HttpDelete]
        [Route("{courseId:int}")]
        public IActionResult DeleteCourse(int courseId)
        {
            var success = _coursesService.DeleteCourseById(courseId);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a student from course
        /// </summary>
        /// <param name="courseId">The id of the course</param>
        /// <param name="deleteStudent">Student SSN</param>
        /// <returns>A status code 204 (if successful)</returns>
        [HttpDelete]
        [Route("{courseId:int}/students/{ssn}")]
        public IActionResult DeleteStudentFromCourse(int courseId, string ssn)
        {
            if (ssn == null) { return BadRequest(); }
            
            if (!ModelState.IsValid) { return StatusCode(412); }

            try
            {
                    var success = _coursesService.DeleteStudentFromCourseById(courseId, ssn);
                    return NoContent();
                    
            }catch(CoursesApi.Models.Exceptions.StudentNotExistsException e)
            {
                    return NotFound(e.Message);
            }catch(CoursesApi.Models.Exceptions.CourseNotExistsException e)
            {
                    return NotFound(e.Message);
            }catch(CoursesApi.Models.Exceptions.StudentDeletedException e)
            {
                    return BadRequest(e.Message);
            }catch(CoursesApi.Models.Exceptions.StudentNotEnrolled e)
            {
                    return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Adds students to course waiting list
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="waiting"></param>
        /// <returns>Added student</returns>
        [HttpPost]
        [Route("{courseId:int}/waitinglist")]
        public IActionResult AddToWaitingList(int courseId, [FromBody] StudentViewModel waiting)
        {
            
            if (waiting == null) { return BadRequest(); }
            
            if (!ModelState.IsValid) { return StatusCode(412); }
            
            
            try
            {
                var success = _coursesService.AddToWaitingList(courseId, waiting);
                return Created(success.SSN, success);

            }catch(CoursesApi.Models.Exceptions.StudentNotExistsException e)
            {
                    return NotFound(e.Message);
            }catch(CoursesApi.Models.Exceptions.DuplicateWaitingException e)
            {
                    return StatusCode(412, e.Message);
            }catch(CoursesApi.Models.Exceptions.DublicateEnrolledException e)
            {
                    return StatusCode(412, e.Message);
            }catch(CoursesApi.Models.Exceptions.CourseNotExistsException e)
            {
                    return NotFound(e.Message);
            }
                        
        }

        /// <summary>
        /// Gets current waiting list for specific course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns>Name and SSN for waiting list</returns>
        [HttpGet]
        [Route("{courseId:int}/waitinglist")]
        public IActionResult GetWaitingList(int courseId)
        {
           if(!ModelState.IsValid){ return StatusCode(412); }
          
           try
           {
               var courses = _coursesService.GetWaitingList(courseId);
               return Ok(courses);   
           }catch(CoursesApi.Models.Exceptions.CourseNotExistsException e)
           {
               return NotFound(e.Message);
           }
        }
    }
}
