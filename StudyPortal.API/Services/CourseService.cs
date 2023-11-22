using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudyPortal.API.Configs;
using StudyPortal.API.Models;

namespace StudyPortal.API.Services;

public class CourseService : AbstractService, ISubjectsService<Course>
{
    private readonly IMongoCollection<Course> _courseCollection;

    public CourseService(IOptions<StudyPortalDatabaseSettings> studyPortalDatabaseSettings) : base(
        studyPortalDatabaseSettings)
    {
        _courseCollection =
            mongoDatabase.GetCollection<Course>(studyPortalDatabaseSettings.Value.CoursesCollectionName);
    }
    


    public async Task<List<Course>> GetAsync()
    {
        return await _courseCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Course?> GetAsync(string id)
    {
        return await _courseCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Course newCourse)
    {
        await _courseCollection.InsertOneAsync(newCourse);
    }


    public async Task UpdateAsync(string id, Course updatedCourse)
    {
        await _courseCollection.ReplaceOneAsync(x => x.Id == id, updatedCourse);
    }

    public async Task DeleteAsync(string id)
    {
        await _courseCollection.DeleteOneAsync(x => x.Id == id);
    }
}