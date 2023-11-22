using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudyPortal.API.Configs;
using StudyPortal.API.Models;

namespace StudyPortal.API.Services;

public interface IUserService
{
    public Task<List<User>> GetAsync();

    public Task<User?> GetAsync(string id);

    public Task CreateAsync(User newUser);

    public Task UpdateAsync(string id, User updatedUser);

    public Task DeleteAsync(string id);
}

public class UserService : AbstractService, IUserService
{
    private readonly IOptions<StudyPortalDatabaseSettings> _studyPortalSettings;
    private readonly IMongoCollection<User> _userCollection;

    public UserService(IOptions<StudyPortalDatabaseSettings> studyPortalSettings) : base(studyPortalSettings)
    {
        _userCollection = mongoDatabase.GetCollection<User>(studyPortalSettings.Value.UsersCollectionName);
        _studyPortalSettings = studyPortalSettings;
    }
    

    public async Task<List<User>> GetAsync()
    {
        return await _userCollection.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetAsync(string id)
    {
        return await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }


    public async Task CreateAsync(User newUser)
    {
        await _userCollection.InsertOneAsync(newUser);
    }

    public async Task UpdateAsync(string id, User updatedUser)
    {
        await _userCollection.ReplaceOneAsync(u => u.Id == id, updatedUser);
    }

    public async Task DeleteAsync(string id)
    {
        await _userCollection.DeleteOneAsync(u => u.Id == id);
    }
}