using System;
namespace StudyPortal.API.Models
{
    public class Article: Subjects
    {
        public Article(string id,
                       string title,
                       string description,
                       string summary,
                       string [] authors
                       ) : base( id,
                                title,
                                description,
                                summary,
                                authors )
        {
        }

    }
}

