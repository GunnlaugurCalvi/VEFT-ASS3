using System.Collections.Generic;
using CoursesApi.Models.DTOModels;
using CoursesApi.Repositories;
using System;
using CoursesApi.Models.ViewModels;

namespace CoursesApi.Services
{
    public class CoursesService : ICoursesService
    {
        private ICoursesRepository _repo;
        public CoursesService(ICoursesRepository repo)
        {
            _repo = repo;
        }
        
        public IEnumerable<CoursesListItemDTO> GetCourses(string semester)
        {            
            var courses = _repo.GetCourses(semester);
            
            return courses;
        }

        public CourseDetailsDTO GetCourseById(int courseId)
        {
            var course = _repo.GetCourseById(courseId);
        
            return course;
        }

        public CourseDetailsDTO UpdateCourse(int courseId, CourseViewModel updatedCourse)
        {
            var course = _repo.UpdateCourse(courseId, updatedCourse);

            return course;
        }

        public IEnumerable<StudentDTO> GetStudentsByCourseId(int courseId)
        {
            var students = _repo.GetStudentsByCourseId(courseId);

            return students;
        }

        public StudentDTO AddStudentToCourse(int courseId,  StudentViewModel newStudent)
        {
            var student = _repo.AddStudentToCourse(courseId, newStudent);

            return student;
        }

        public bool DeleteCourseById(int courseId)
        {
            var result = _repo.DeleteCourseById(courseId);
            
            return result;
        }

        public bool DeleteStudentFromCourseById(int courseId, string ssn)
        {
            var result = _repo.DeleteStudentFromCourseById(courseId, ssn);
            
            return result;
        }

        public CourseDetailsDTO AddCourse(CourseViewModel newCourse)
        {
            return _repo.AddCourse(newCourse);
        }
       
        public StudentDTO AddToWaitingList(int courseId, StudentViewModel waiting)
        {
            return _repo.AddToWaitingList(courseId, waiting);
        }

        public IEnumerable<StudentDTO> GetWaitingList(int courseId)
       {
           return _repo.GetWaitingList(courseId);
       }
    }
}