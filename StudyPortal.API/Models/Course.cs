using System;
namespace StudyPortal.API.Models
{
    public class Course : Subjects
    {
        public Course(string id, string title, string description, string summary, string [] authors) : base(id, title, description, summary, authors)
        {
        }
    }
}

