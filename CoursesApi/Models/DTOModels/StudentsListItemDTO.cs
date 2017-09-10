using System;

namespace CoursesApi.Models.DTOModels
{
   /// <summary>
   /// Student model which is exposed to the user
   /// </summary>
   public class StudentListItemDTO
   {
       /// <summary>
       /// The name of the student
       /// </summary>
       /// <returns></returns>
       public string Name { get; set; }
   }
}