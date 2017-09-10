using System;
using System.Linq;
using System.Collections.Generic;
using CoursesApi.Models.DTOModels;
using CoursesApi.Models.EntityModels;
using AutoMapper;
using CoursesApi.Models.ViewModels;
using CoursesApi.Models.Exceptions;



namespace CoursesApi.Repositories
{
    public class CoursesRepository : ICoursesRepository
    {
        private AppDataContext _db;
        
        public CoursesRepository(AppDataContext db)
        {
            _db = db;
        }

        public IEnumerable<CoursesListItemDTO> GetCourses(string semsester)
        {
            var courses = (from c in _db.Courses
                           join t in _db.CourseTemplates on c.CourseTemplate equals t.Template 
                           where c.Semester == semsester
                           select new CoursesListItemDTO 
                           {
                               Id = c.Id,
                               Name = t.CourseName,
                               NumberOfStudents = (_db.Enrollments.Count(s => s.CourseId == c.Id))
                           }).ToList();

            return courses;
        }

        public CourseDetailsDTO GetCourseById(int courseId)
        {            
            var course = _db.Courses.SingleOrDefault(c => c.Id == courseId);

            if (course == null) 
            {
                return null;
            }

            var result = new CourseDetailsDTO
            {
                Id = course.Id,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                Name = _db.CourseTemplates.Where(t => t.Template == course.CourseTemplate)
                                                         .Select(c => c.CourseName).FirstOrDefault(),
                Students = (from sr in _db.Enrollments
                           where sr.CourseId == course.Id
                           join s in _db.Students on sr.StudentSSN equals s.SSN
                           select new StudentDTO
                           {
                               SSN = s.SSN,
                               Name = s.Name
                           }).ToList()
            };

            return result;

        }
        public CourseDetailsDTO UpdateCourse(int courseId, CourseViewModel updatedCourse)
        {
            var course = _db.Courses.SingleOrDefault(c => c.Id == courseId);

            if (course == null) 
            {
                return null;
            }

            course.StartDate = updatedCourse.StartDate;
            course.EndDate = updatedCourse.EndDate;

            _db.SaveChanges();

            return GetCourseById(courseId);
        }

        public IEnumerable<StudentDTO> GetStudentsByCourseId(int courseId)
        {
            var course = _db.Courses.SingleOrDefault(c => c.Id == courseId);

            if (course == null) 
            {
                return null;
            }

            var students = (from sr in _db.Enrollments
                            where sr.CourseId == courseId
                            join s in _db.Students on sr.StudentSSN equals s.SSN
                            select new StudentDTO
                            {
                                SSN = s.SSN,
                                Name = s.Name
                            }).ToList();

            return students;
        }

        public StudentDTO AddStudentToCourse(int courseId, StudentViewModel newStudent)
        {
            // Student needs to be a registered user
            // Check if student exists
            var studentExistsInDatabase = (from c in _db.Students
                                            where c.SSN == newStudent.SSN
                                            select c ).SingleOrDefault();
            
            if(studentExistsInDatabase == null)
            {
                throw new StudentNotExistsException();
            }

            // Check if course exists in database
            var courseExistsInDatabase = (from c in _db.Courses
                          where c.Id == courseId
                          select c).SingleOrDefault();
            
            if(courseExistsInDatabase == null)
            {
                throw new CourseNotExistsException();
            }

            // Check course max capacity
            var studentCapacity =   (from c in _db.Courses
                                                where c.Id == courseId
                                                select c ).SingleOrDefault();

            // Check course capacity status
            int studentCapacityStatus = (from c in _db.Enrollments
                                            where c.CourseId == courseId &&
                                            c.Status == "Enrolled"
                                            select c).Count();

            if(studentCapacity.MaxStudents == studentCapacityStatus)
            {
                throw new MaxCapacityException();
            }      

            // Check if student is already enrolled in course
            var studentIsEnrolled = (from c in _db.Enrollments
                                    where c.StudentSSN == newStudent.SSN &&
                                    c.CourseId == courseId &&
                                    c.Status == "Enrolled"
                                    select c ).SingleOrDefault();

            if(studentIsEnrolled != null)
            {
                throw new DublicateEnrolledException();
            }

            // Check if student is already on the waiting list
            // before enrolling him
            var studentIsWaiting = (from c in _db.Enrollments
                                    where c.StudentSSN == newStudent.SSN &&
                                    c.CourseId == courseId &&
                                    c.Status == "Waiting"
                                    select c ).SingleOrDefault();

            // Check if student is deleted
            // before enrolling him
            var studentIsDeleted = (from c in _db.Enrollments
                                    where c.StudentSSN == newStudent.SSN &&
                                    c.CourseId == courseId &&
                                    c.Status == "Deleted"
                                    select c ).SingleOrDefault();
            
            // Removing deleted status                                    
            if(studentIsDeleted != null)
            {
            studentIsWaiting.Status = "Enrolled";
             _db.Enrollments.Update(studentIsWaiting);
            // Removing waiting status
            }else if(studentIsWaiting != null)
            {
            studentIsWaiting.Status = "Enrolled";
             _db.Enrollments.Update(studentIsWaiting);
            }else
            {
                _db.Enrollments.Add( new Enrollment {CourseId = courseId, StudentSSN = newStudent.SSN, Status = "Enrolled"});
            }

            _db.SaveChanges();

            return new StudentDTO
            {
                SSN = newStudent.SSN,
                Name = (from st in _db.Students
                       where st.SSN == newStudent.SSN
                       select st).SingleOrDefault().Name
            };
        }

        public bool DeleteCourseById(int courseId)
        {
            var course = (from c in _db.Courses
                            where c.Id == courseId
                            select c).SingleOrDefault();
            
            if (course == null)
            {
                return false;
            }
            _db.Courses.Remove(course);
            _db.SaveChanges();

            return true;
        }

        public CourseDetailsDTO AddCourse(CourseViewModel newCourse)
        {
            var entity = new Course { CourseTemplate = newCourse.CourseID, Semester = newCourse.Semester, StartDate = newCourse.StartDate, EndDate = newCourse.EndDate };

            _db.Courses.Add(entity);
            _db.SaveChanges();

            return new CourseDetailsDTO 
            {
                Id = entity.Id,
                Name = _db.CourseTemplates.FirstOrDefault(ct => ct.Template == newCourse.CourseID).CourseName,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Students = _db.Enrollments
                    .Where(e => e.CourseId == entity.Id)
                    .Join(_db.Students, enroll => enroll.StudentSSN, stud => stud.SSN, (e, s) => s)
                    .Select(s => new StudentDTO {
                        SSN = s.SSN,
                        Name = s.Name
                    }).ToList()
            };
        }
       
       public StudentDTO AddToWaitingList(int courseId, StudentViewModel waiting)
       {
            // Student needs to be a registered user
            // Check if student exists
            var studentExistsInDatabase = (from c in _db.Students
                                            where c.SSN == waiting.SSN
                                            select c ).SingleOrDefault();

            // If the student does not exist in database
            // Throw exception
            if(studentExistsInDatabase == null)
            {
                throw new StudentNotExistsException();
            }

            // Student cannot already be a registered user in course
            // Checking if student in already in course either
            // "Enrolled" or "Waiting"
            var courseDublicate = (from c in _db.Enrollments
                                            where c.StudentSSN == waiting.SSN &&
                                            c.Id == courseId &&
                                            c.Status == "Enrolled"
                                            select c ).SingleOrDefault();
           
            // If the student is already registered in
            // the specified course
            // Throw exception
            if(courseDublicate != null)
            {
                throw new DublicateEnrolledException();
            }

            // Student cannot already exist on the waiting list
            // Check if student exists on waiting list
            var studentDuplicate = (from c in _db.Enrollments
                         where c.Id == courseId &&
                         c.StudentSSN == waiting.SSN &&
                         c.Status == "Waiting"
                          select c).SingleOrDefault();

            // If the student is already registered in
            // the waiting list for course
            // Throw exception
            if(studentDuplicate != null)
            {
                throw new DuplicateWaitingException();
            }
            
            _db.Enrollments.Add( 
                new Enrollment {CourseId = courseId , StudentSSN = waiting.SSN, Status = "Waiting"}
            );
            _db.SaveChanges();

            return new StudentDTO
            {
                SSN = waiting.SSN,
                Name = (from st in _db.Students
                       where st.SSN == waiting.SSN
                       select st).SingleOrDefault().Name
            };
       }

       public bool DeleteStudentFromCourseById(int courseId, StudentViewModel deleteStudent)
        {
            // Student needs to be a registered user
            // Check if student exists
            var studentExistsInDatabase = (from c in _db.Students
                                            where c.SSN == deleteStudent.SSN
                                            select c ).SingleOrDefault();
            
            if(studentExistsInDatabase == null)
            {
                throw new StudentNotExistsException();
            }

            // Check if course exists in database
            var courseExistsInDatabase = (from c in _db.Courses
                          where c.Id == courseId
                          select c).SingleOrDefault();
            
            if(courseExistsInDatabase == null)
            {
                throw new CourseNotExistsException();
            }

            // Check if student is already deleted in course
            var studentIsDeleted = (from c in _db.Enrollments
                                    where c.StudentSSN == deleteStudent.SSN &&
                                    c.CourseId == courseId &&
                                    c.Status == "Deleted"
                                    select c ).SingleOrDefault();

            if(studentIsDeleted != null)
            {
                throw new StudentDeletedException();
            }

            // Check if student in enrolled
            var studentEnrolled = (from c in _db.Enrollments
                                    where c.StudentSSN == deleteStudent.SSN &&
                                    c.CourseId == courseId &&
                                    c.Status == "Enrolled"
                                    select c ).SingleOrDefault();
                                    
            if(studentEnrolled == null)
            {
                throw new StudentNotEnrolled();
            }

            studentEnrolled.Status = "Deleted";
            _db.Enrollments.Update(studentEnrolled);
            
            _db.SaveChanges();

            return true;
        }

    }
} 
           
