using System;
using StudyPortal.API.Models;

namespace StudyPortal.Test.Fixtures
{
    public static class SubjectsFixture
    {
        private static string[] authors = { "Dummy Author1", "Dummy Author2" };

        public static List<Course> GetTestCourses()
        {
            return new List<Course>()
            {
                new ( "Dummy Id1",
                    "Dummy Title1",
                    "Dummy Desc1",
                    "Dummy Summary1",
                    authors),

                new ("Dummy Id2",
                    "Dummy Title2",
                    "Dummy Desc2",
                    "Dummy Summary2",
                     authors),

                new ("Dummy Id3",
                     "Dummy Title3",
                     "Dummy Desc3",
                     "Dummy Summary3",
                      authors)
            };
            
        }

        public static List<Article> GetTestArticles()
        {

            return new List<Article>()
            {
                new ( "Dummy Id1",
                    "Dummy Title1",
                    "Dummy Desc1",
                    "Dummy Summary1",
                     authors),
               
                new ("Dummy Id2",
                    "Dummy Title2",
                    "Dummy Desc2",
                    "Dummy Summary2",
                     authors),
               
                new ("Dummy Id3",
                     "Dummy Title3",
                     "Dummy Desc3",
                     "Dummy Summary3",
                      authors)
                
            };

        }
    }
}

