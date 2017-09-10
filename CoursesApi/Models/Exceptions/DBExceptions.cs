using System;
using System.Runtime.Serialization;

namespace CoursesApi.Models.Exceptions
{
    public class StudentNotEnrolled : Exception
    {
        public StudentNotEnrolled() : base("Student is not enrolled -Borat") { }

        public StudentNotEnrolled(string message) : base(message)
        {
        }

        public StudentNotEnrolled(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StudentNotEnrolled(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class StudentDeletedException : Exception
    {
        public StudentDeletedException() : base("Student already deleted -Borat") { }

        public StudentDeletedException(string message) : base(message)
        {
        }

        public StudentDeletedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StudentDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class MaxCapacityException : Exception
    {
        public MaxCapacityException() : base("Course max capacity reached -Borat") { }

        public MaxCapacityException(string message) : base(message)
        {
        }

        public MaxCapacityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MaxCapacityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class CourseNotExistsException : Exception
    {
        public CourseNotExistsException() : base("Course does not exist in database -Borat") { }

        public CourseNotExistsException(string message) : base(message)
        {
        }

        public CourseNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CourseNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class StudentExistsException : Exception
    {
        public StudentExistsException() : base("Student already exists in database -Borat") { }

        public StudentExistsException(string message) : base(message)
        {
        }

        public StudentExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StudentExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class StudentNotExistsException : Exception
    {
        public StudentNotExistsException() : base(" Student name not found in database -Borat") { }

        public StudentNotExistsException(string message) : base(message)
        {
        }

        public StudentNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StudentNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class DuplicateWaitingException : Exception
    {
        public DuplicateWaitingException() : base("This student already on waiting list -Borat") { }

        public DuplicateWaitingException(string message) : base(message)
        {
        }

        public DuplicateWaitingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateWaitingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class DublicateEnrolledException : Exception
    {
        public DublicateEnrolledException() : base("This student already enrolled in course -Borat") { }

        public DublicateEnrolledException(string message) : base(message)
        {
        }

        public DublicateEnrolledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DublicateEnrolledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}