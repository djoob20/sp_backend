using System;
namespace StudyPortal.API.Configs
{
    public class StudyPortalDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string CoursesCollectionName { get; set; } = null!;

        public string ArticlesCollectionName { get; set; } = null!;

        public string UsersCollectionName { get; set; } = null!;

        public string GoogleSecret { get; set; } = null!;
        
        public string GoogleClientId { get; set; } = null!;

    }
}

